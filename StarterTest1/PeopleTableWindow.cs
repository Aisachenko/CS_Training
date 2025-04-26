using WindowsFormsApp1.Repositories;
using WindowsFormsApp1.Model;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.IO;
using OfficeOpenXml;
using System.Collections.Generic;
using Guna.UI2.WinForms;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using WindowsFormsApp1.Services;
using System.Data.Entity;

namespace WindowsFormsApp1
{
    public partial class PeopleTableWindow : Form
    {
        private readonly IPeople _peopleRepository;
        private readonly DataImportService _dataImportService;
        private readonly DataExportService _dataExportService;

        public PeopleTableWindow()
        {
            InitializeComponent();
            _dataImportService = new DataImportService(_peopleRepository);
            _dataExportService = new DataExportService(_peopleRepository);
            StartPosition = FormStartPosition.CenterScreen;
            var serviceProvider = ServiceProviderFactory.ServiceProvider;
            _peopleRepository = serviceProvider.GetService<IPeople>();
            LoadPeople();
            Guna2DateTimePicker[] dates = { DateAdd, DateUpd, DateDel, DateExp };
            foreach (var date in dates)
            {
                date.Format = DateTimePickerFormat.Custom;
                date.CustomFormat = "dd/MM/yyyy";
            }

        }

        private void LoadPeople()
        {
            var people = _peopleRepository.GetAll();
            guna2DataGridView1.DataSource = people.ToList();
        }

        private void AddBtn_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameAdd.Text) ||
                string.IsNullOrWhiteSpace(LastNameAdd.Text) ||
                string.IsNullOrWhiteSpace(SurNameAdd.Text) ||
                string.IsNullOrWhiteSpace(CityAdd.Text) ||
                string.IsNullOrWhiteSpace(CountryAdd.Text) ||
                DateAdd.Value == null || DateAdd.Value == DateTime.MinValue)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var person = new People
            {
                Date = DateAdd.Value,
                FirstName = FirstNameAdd.Text,
                LastName = LastNameAdd.Text,
                SurName = SurNameAdd.Text,
                City = CityAdd.Text,
                Country = CountryAdd.Text
            };
            _peopleRepository.Add(person);
            LoadPeople();
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            if (int.TryParse(IdUpd.Text, out int selectedId))
            {
                if (selectedId > 0 && selectedId <= guna2DataGridView1.RowCount)
                {
                    var person = (People)guna2DataGridView1.Rows[selectedId - 1].DataBoundItem;
                    person.Date = DateUpd.Value;
                    person.FirstName = FirstNameUpd.Text;
                    person.LastName = LastNameUpd.Text;
                    person.SurName = SurNameUpd.Text;
                    person.City = CityUpd.Text;
                    person.Country = CountryUpd.Text;
                    _peopleRepository.Update(person);
                    LoadPeople();
                    DateUpd.Visible = false;
                    LastNameUpd.Visible = false;
                    FirstNameUpd.Visible = false;
                    SurNameUpd.Visible = false;
                    CityUpd.Visible = false;
                    CountryUpd.Visible = false;
                    UpdateBtn.Visible = false;
                }
                else
                {
                    MessageBox.Show("Identifier out of range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FindButton_Click(object sender, EventArgs e, string idInput, Control[] fieldsToUpdate, Guna2Button actionButton, Guna2DateTimePicker datePicker)
        {
            if (int.TryParse(idInput, out int selectedId))
            {
                var allPeople = _peopleRepository.GetAll();

                var selectedPerson = allPeople.FirstOrDefault(person => person.IdPeople == selectedId);

                if (selectedPerson != null)
                {
                    if (DateTime.TryParse(selectedPerson.Date.ToString(), out DateTime dateValue))
                    {
                        datePicker.Value = dateValue;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    fieldsToUpdate[0].Text = selectedPerson.FirstName;
                    fieldsToUpdate[1].Text = selectedPerson.LastName;
                    fieldsToUpdate[2].Text = selectedPerson.SurName;
                    fieldsToUpdate[3].Text = selectedPerson.City;
                    fieldsToUpdate[4].Text = selectedPerson.Country;

                    foreach (var field in fieldsToUpdate)
                    {
                        field.Visible = true;
                    }
                    actionButton.Visible = true;
                }
                else
                {
                    MessageBox.Show("Record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FindUpdBtn_Click(object sender, EventArgs e)
        {
            Control[] fieldsToUpdate = { FirstNameUpd, LastNameUpd, SurNameUpd, CityUpd, CountryUpd };
            FindButton_Click(sender, e, IdUpd.Text, fieldsToUpdate, UpdateBtn, DateUpd);
        }

        private void FindDelBtn_Click(object sender, EventArgs e)
        {
            DeleteAllBtn.Visible = false;
            Control[] fieldsToDelete = { FirstNameDel, LastNameDel, SurNameDel, CityDel, CountryDel };
            FindButton_Click(sender, e, IdDel.Text, fieldsToDelete, DeleteBtn, DateDel);
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (int.TryParse(IdDel.Text, out int selectedId))
            {
                if (selectedId > 0 && selectedId <= guna2DataGridView1.RowCount)
                {
                    var person = (People)guna2DataGridView1.Rows[selectedId - 1].DataBoundItem;
                    _peopleRepository.Delete(person.IdPeople);
                    LoadPeople();
                    DateDel.Visible = false;
                    LastNameDel.Visible = false;
                    FirstNameDel.Visible = false;
                    SurNameDel.Visible = false;
                    CityDel.Visible = false;
                    CountryDel.Visible = false;
                    DeleteBtn.Visible = false;
                    DeleteAllBtn.Visible = true;
                }
                else
                {
                    MessageBox.Show("Identifier out of range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid identifier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ExportAllToExcel_Click(object sender, EventArgs e)
        {
            var people = _peopleRepository.GetAll().ToList();
            await _dataExportService.ExportToExcelAsync(people);
        }

        private async void ExportCombToExcel_Click(object sender, EventArgs e)
        {
            var filteredData = await FilterData();
            if (filteredData != null)
            {
                await _dataExportService.ExportToExcelAsync(filteredData);
            }
        }

        private async void ExportAllToXml_Click(object sender, EventArgs e)
        {
            var people = _peopleRepository.GetAll().ToList();
            await _dataExportService.ExportToXmlAsync(people);
        }

        private async void ExportCombToXml_Click(object sender, EventArgs e)
        {
            var filteredData = await FilterData();
            if (filteredData != null)
            {
                await _dataExportService.ExportToXmlAsync(filteredData);
            }
        }

        private async void ExportAllToCsv_Click(object sender, EventArgs e)
        {
            var people = _peopleRepository.GetAll().ToList();
            await _dataExportService.ExportToCsvAsync(people);
        }

        private async void ImportCsvBtn_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Choose CSV file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var progressForm = new ProgressForm())
                {
                    progressForm.Show();
                    var progress = new Progress<int>(value => progressForm.UpdateProgress(value));

                    try
                    {
                        await _dataImportService.ImportFromCsvAsync(openFileDialog.FileName, progress);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        progressForm.Close();
                    }
                }
                LoadPeople();
            }
        }

        private async void DeleteAllBtn_Click(object sender, EventArgs e)
        {
            using (var progressForm = new ProgressForm())
            {
                progressForm.Show();
                try
                {
                    await Task.Run(() => _peopleRepository.TruncateTable());
                    MessageBox.Show("All records have been deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during deletion: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                progressForm.Close();
            }
            LoadPeople();
        }

        public async Task<List<People>> FilterData()
        {
            var date = DateExp.Value.Date;
            var firstName = FirstNameExp.Text;
            var lastName = LastNameExp.Text;
            var surName = SurNameExp.Text;
            var city = CityExp.Text;
            var country = CountryExp.Text;

            IQueryable<People> query = _peopleRepository.GetAll();

            if (date != null)
            {
                query = query.Where(p => p.Date == null || DbFunctions.TruncateTime(p.Date) == date);
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query.Where(p => p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query.Where(p => p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(surName))
            {
                query = query.Where(p => p.SurName.Equals(surName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.Equals(city, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(p => p.Country.Equals(country, StringComparison.OrdinalIgnoreCase));
            }

            List<People> filteredData = await query.ToListAsync();

            if (filteredData.Count == 0)
            {
                MessageBox.Show("There is no data to export for the given criteria.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            return filteredData;
        }
    }
}