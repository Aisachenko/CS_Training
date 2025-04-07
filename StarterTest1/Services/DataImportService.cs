using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Model;
using WindowsFormsApp1.Repositories;
using System.IO;

namespace WindowsFormsApp1.Services
{
    public class DataImportService
    {
        private readonly IPeople _peopleRepository;

        public DataImportService(IPeople peopleRepository)
        {
            _peopleRepository = peopleRepository;
        }

        public async Task ImportFromCsvAsync(string filePath, IProgress<int> progress)
        {
            var peopleList = new List<People>();
            int count = 0;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    int totalLines = File.ReadLines(filePath).Count();

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var values = line.Split(';');
                        if (values.Length == 6)
                        {
                            try
                            {
                                var person = new People
                                {
                                    Date = DateTime.Parse(values[0]),
                                    FirstName = values[1],
                                    LastName = values[2],
                                    SurName = values[3],
                                    City = values[4],
                                    Country = values[5]
                                };
                                peopleList.Add(person);
                            }
                            catch (FormatException ex)
                            {
                                MessageBox.Show($"Date format error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            if (peopleList.Count >= 1000)
                            {
                                await BatchInsertAsync(peopleList);
                                count += peopleList.Count;
                                int progressPercentage = (int)((double)count / totalLines * 100);
                                progress.Report(progressPercentage);
                                peopleList.Clear();
                            }
                        }
                    }

                    if (peopleList.Count > 0)
                    {
                        await BatchInsertAsync(peopleList);
                        count += peopleList.Count;
                    }
                    MessageBox.Show($"{count} records successfully imported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Import canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BatchInsertAsync(List<People> people)
        {
            using (var context = new DataContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                using (var connection = new SqlConnection("Data Source=HLEB\\SQLEXPRESS;Initial Catalog=dbCS;Integrated Security=True;"))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                            {
                                bulkCopy.DestinationTableName = "People";

                                bulkCopy.ColumnMappings.Add("Date", "Date");
                                bulkCopy.ColumnMappings.Add("FirstName", "FirstName");
                                bulkCopy.ColumnMappings.Add("LastName", "LastName");
                                bulkCopy.ColumnMappings.Add("SurName", "SurName");
                                bulkCopy.ColumnMappings.Add("City", "City");
                                bulkCopy.ColumnMappings.Add("Country", "Country");

                                var dataTable = new DataTable();
                                dataTable.Columns.Add("Date", typeof(DateTime));
                                dataTable.Columns.Add("FirstName", typeof(string));
                                dataTable.Columns.Add("LastName", typeof(string));
                                dataTable.Columns.Add("SurName", typeof(string));
                                dataTable.Columns.Add("City", typeof(string));
                                dataTable.Columns.Add("Country", typeof(string));

                                foreach (var person in people)
                                {
                                    dataTable.Rows.Add(person.Date, person.FirstName, person.LastName, person.SurName, person.City, person.Country);
                                }
                                await bulkCopy.WriteToServerAsync(dataTable);
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Error during batch insert: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            context.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }
                }
            }
        }

        public async Task BatchDeleteAsync(List<int> ids, IProgress<int> progress)
        {
            using (var context = new DataContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                const int batchSize = 1000;
                for (int i = 0; i < ids.Count; i += batchSize)
                {
                    var batchIds = ids.Skip(i).Take(batchSize).ToList();
                    var recordsToDelete = context.people.Where(p => batchIds.Contains(p.IdPeople)).ToList();

                    context.people.RemoveRange(recordsToDelete);
                    await context.SaveChangesAsync();

                    int progressPercentage = (int)((double)(i + batchIds.Count) / ids.Count * 100);
                    progress.Report(progressPercentage);
                }
            }
        }

    }
}
