using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeSystem.Structure;
using CafeSystem.TelegramClient;
using CsvHelper;
using MetroFramework.Forms;

namespace CafeSystem
{
    public partial class MainForm : MetroForm
    {
        private static List<User> _users;
        private static List<Computer> _computers;

        public MainForm()
        {
            InitializeComponent();
            _users = new List<User>();
            _computers = new List<Computer>();
            LogBox.RTB = richTextBox1;

            
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
                    foreach (var pc in records)
                    {
                        _computers.Add(pc);
                        LogBox.Log($"{pc.Name} => Подключен.");
                    }
                    //MessageBox.Show(test);
                }
            }
            catch (Exception k)
            {
                MessageBox.Show($"Ошибка: {k.Message}");
            }

            #endregion

            // Заносит пк в список объектов
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

            //var bot = new TelegramBot();
            metroLabel1.Hide();

            
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
            metroLabel1.Show();

            var selectedPc = metroComboBox1.SelectedItem as Computer;
            metroLabel1.Text = selectedPc.GetDeviceString();
        }

        private void ShowComputersDatabase_Click(object sender, EventArgs e)
        {
            var dbShow = new DatabaseShow
            {
                Text = "База данных подключенных компьютеров."
            };
            var table = new DataTable();

            table.Columns.AddRange(new[] {new DataColumn("Название ПК"), new DataColumn("Подключенные устройства")});

            for (var i = 0; i < _computers.Count; i++)
            {
                var pc = _computers[i];
                table.Rows.Add(pc.Name, pc.GetDeviceString());
            }

            //show.Activate();
            dbShow.Show();
            dbShow.data = table;
        }

        private void SaveLogsButton_Click(object sender, EventArgs e)
        {
            string text = richTextBox1.Text;
            if (text == String.Empty)
            {
                MessageBox.Show("Журнал пустой, нечего сохранять!");
                return;
            }

            Task.Run(() =>
            {
                File.WriteAllText($"Лог_{DateTime.Now.ToFileTime()}.txt", text);

                LogBox.RTB.BeginInvoke(new MethodInvoker(() => { LogBox.RTB.Text = String.Empty; }));

                LogBox.Log("Лог файл сохранён успешно!", LogBox.LogType.Succes);
                MessageBox.Show("Лог файл сохранён успешно!");
            });
        }

    }
}