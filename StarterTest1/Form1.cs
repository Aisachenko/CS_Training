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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly IPeople _peopleRepository;

        public Form1()
        {
            InitializeComponent();
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
                    var person = (People)guna2DataGridView1.Rows[selectedId-1].DataBoundItem;
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
            Control[] fieldsToDelete = { FirstNameDel, LastNameDel, SurNameDel, CityDel, CountryDel };
            FindButton_Click(sender, e, IdDel.Text, fieldsToDelete, DeleteBtn, DateDel);
        }
        
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (int.TryParse(IdDel.Text, out int selectedId))
            {
                if (selectedId > 0 && selectedId <= guna2DataGridView1.RowCount)
                {
                    var person = (People)guna2DataGridView1.Rows[selectedId-1].DataBoundItem;
                    _peopleRepository.Delete(person.IdPeople);
                    LoadPeople();
                    DateDel.Visible = false;
                    LastNameDel.Visible = false;
                    FirstNameDel.Visible = false;
                    SurNameDel.Visible = false;
                    CityDel.Visible = false;
                    CountryDel.Visible = false;
                    DeleteBtn.Visible = false;
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

        public void ExportToExcel(List<People> data)
        {
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("ExportedData");

                workSheet.Cells[1, 1].Value = "Date";
                workSheet.Cells[1, 2].Value = "First Name";
                workSheet.Cells[1, 3].Value = "Last Name";
                workSheet.Cells[1, 4].Value = "Surname";
                workSheet.Cells[1, 5].Value = "City";
                workSheet.Cells[1, 6].Value = "Country";

                for (int i = 0; i < data.Count; i++)
                {
                    var person = data[i];
                    workSheet.Cells[i + 2, 1].Value = person.Date;
                    workSheet.Cells[i + 2, 2].Value = person.FirstName;
                    workSheet.Cells[i + 2, 3].Value = person.LastName;
                    workSheet.Cells[i + 2, 4].Value = person.SurName;
                    workSheet.Cells[i + 2, 5].Value = person.City;
                    workSheet.Cells[i + 2, 6].Value = person.Country;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Save Excel file"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                    excel.SaveAs(excelFile);
                    MessageBox.Show("Data successfully exported to Excel!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public List<People> FilteredData()
        {
            List<People> data = (List<People>)_peopleRepository.GetAll();
            var date = DateExp.Value;
            var firstName = FirstNameExp.Text;
            var lastName = LastNameExp.Text;
            var surName = SurNameExp.Text;
            var city = CityExp.Text;
            var country = CountryExp.Text;
            var filteredData = data.Where(p => (p.Date.Date == date.Date || p.Date == null)
                                                && (p.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(firstName))
                                                && (p.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(lastName))
                                                && (p.SurName.Equals(surName, StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(firstName))
                                                && (p.City.Equals(city, StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(city))
                                                && (p.Country.Equals(country, StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(country))).ToList();

            if (filteredData.Count == 0)
            {
                MessageBox.Show("There is no data to export for the given criteria.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            return filteredData;
        }

        private void ExportAllToExcel_Click(object sender, EventArgs e)
        { 
            ExportToExcel((List<People>)_peopleRepository.GetAll());
        }

        private void ExportCombToExcel_Click(object sender, EventArgs e)
        {
            var filteredData = FilteredData();
            ExportToExcel(filteredData);
        }

        public void ExportToXml(List<People> data)
        {
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "XML Files|*.xml",
                Title = "Save XML file"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                XElement xml = new XElement("TestProgram",
                    from person in data
                    select new XElement("Record",
                        new XAttribute("id", person.IdPeople),
                        new XElement("Date", person.Date),
                        new XElement("FirstName", person.FirstName),
                        new XElement("LastName", person.LastName),
                        new XElement("SurName", person.SurName),
                        new XElement("City", person.City),
                        new XElement("Country", person.Country)
                    )
                );
                xml.Save(saveFileDialog.FileName);
                MessageBox.Show("Data successfully exported to XML!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportAllToXml_Click(object sender, EventArgs e)
        {
            ExportToXml((List<People>)_peopleRepository.GetAll());
        }

        private void ExportCombToXml_Click(object sender, EventArgs e)
        {
            var filteredData = FilteredData();
            ExportToXml(filteredData);
        }

        public void ExportToCsv(List<People> data)
        {
            if (data == null || data.Count == 0)
            {
                MessageBox.Show("No data to export.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Save CSV file"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (var person in data)
                    {
                        writer.WriteLine($"{person.Date:yyyy-MM-dd};{person.FirstName};{person.LastName};{person.SurName};{person.City};{person.Country}");
                    }
                }
                MessageBox.Show("Data successfully exported to CSV!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportAllToCsv_Click_1(object sender, EventArgs e)
        {
            ExportToCsv((List<People>)_peopleRepository.GetAll());
        }

        public List<People> ImportFromCsv(string filePath)
        {
            List<People> peopleList = new List<People>();
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(';');
                        if (values.Length == 6)
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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return peopleList;
        }

        private void ImportCsvBtn_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Choose CSV file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var importedData = ImportFromCsv(openFileDialog.FileName);
                if (importedData != null && importedData.Count > 0)
                {
                    foreach(var person in importedData)
                    {
                        _peopleRepository.Add(person);
                    }
                    LoadPeople();
                    MessageBox.Show($"{importedData.Count} records successfully imported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RemoveAndImportCSV_Click(object sender, EventArgs e)
        {
            var allData = _peopleRepository.GetAll();
            foreach(var data in allData)
            {
                _peopleRepository.Delete(data.IdPeople);
            }
            ImportCsvBtn_Click(sender, e);
        }
    }
}
