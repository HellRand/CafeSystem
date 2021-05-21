using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeSystem.Structure;
using CsvHelper;
using CafeSystem.TelegramClient;
using System.Runtime.InteropServices;
using MetroFramework.Forms;

namespace CafeSystem
{
    public partial class MainForm : MetroForm
    {
        static List<User> _users;
        static List<Computer> _computers;

        public MainForm()
        {
            InitializeComponent();
            _users = new List<User>();
            _computers = new List<Computer>();

        }


        private void MainForm_Load(object sender, EventArgs e)
        {

            #region Считывание объектов с CSV
            try
            {
                using (var reader = new StreamReader("Computers.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ComputerMap>();

                    var records = csv.GetRecords<Computer>().ToList();

                    //string test = "";
                    foreach (Computer pc in records)
                    {
                        _computers.Add(pc);
                    }
                    //MessageBox.Show(test);
                }
            }
            catch (Exception k) { MessageBox.Show($"Ошибка: {k.Message}"); }
            
            #endregion


            metroComboBox1.Items.AddRange(_computers.ToArray());

            //.Text = $"Всего пк: {_computers.Count}.";

            #region Рандомные пк
            //for (int i = 0; i < 10; i++)
            //{
            //    Computer pc = new Computer();
            //    pc.Name = $"PC #{i + 1}";

            //    int devct = new Random().Next(1, 10);
            //    for (int j = 0; j < devct; j++)
            //    {
            //        pc.ConnectedDevices.Add($"Device {j + 1}");
            //    }
            //    _computers.Add(pc);
            //}
            #endregion


            Bot bot = new Bot();
            
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region Сохранение списка ПК в CSV
            using (var writer = new StreamWriter("Computers.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ComputerMap>();
                csv.WriteRecords(_computers);
            }
            #endregion
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedItem == null) return;
            var selectedPc = metroComboBox1.SelectedItem as Computer;
            metroLabel1.Text = selectedPc.GetDeviceString();
        }
    }
}
