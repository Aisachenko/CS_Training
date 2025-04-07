using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsFormsApp1.Model;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Services
{
    public class DataExportService
    {
        private readonly IPeople _peopleRepository;

        public DataExportService(IPeople peopleRepository)
        {
            _peopleRepository = peopleRepository;
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
    }
}
