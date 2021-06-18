using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CafeSystem.Clients;
using CafeSystem.Mechanics;
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

        //Переменные нужны для запоминания кол-ва пользователей/компьютеров,
        //чтобы лишний раз не пересохранять данные
        private int ComputersCount;
        private int UsersCount; 
 
        public MainForm()
        {
            InitializeComponent();
            _users = new List<User>();
            _computers = new List<Computer>();
            LogBox.Rtb = richTextBox1;

            Client tgClient = new TelegramBot(_computers, _users);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Считывание списка компьютеров с CSV

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

            #region Считывание списка пользователей с CSV

            try
            {
                using (var reader = new StreamReader("Users.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<UserMap>();

                    var records = csv.GetRecords<User>().ToList();

                    foreach (var user in records)
                    {
                        _users.Add(user);
                    }
                    LogBox.Log($"Найдено пользователей в базе данных: {_users.Count}.");
                }
            }
            catch (Exception k)
            {
                MessageBox.Show($"Ошибка: {k.Message}");
            }

            #endregion

            ComputersCount = _computers.Count;
            UsersCount = _users.Count;

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
            #region Сохранение списка ПК и Пользователей в CSV
            if (_computers.Count != ComputersCount)
            {
                using (var writer = new StreamWriter("Computers.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ComputerMap>();
                    csv.WriteRecords(_computers);
                }
            }
            if (_users.Count != UsersCount)
            {
                using (var writer = new StreamWriter("Users.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<UserMap>();
                    csv.WriteRecords(_users);
                }
            }

            #endregion
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedItem == null) return;
            metroLabel1.Show();

            if (metroRadioButton1.Checked)
            {
                var selectedPc = metroComboBox1.SelectedItem as Computer;
                string reserved = selectedPc.Reserved ? "Используется" : "Свободен"; // Статус пк
                string reservation = selectedPc.Reserved ? $"({selectedPc.Reservation})" : ""; // Данные брони

                metroLabel1.Text = $"Имя компьютера: {selectedPc}.\nЦена: {selectedPc.PricePerHour} руб./ч.\n" +
                                   $"Статус: {reserved}.\n{reservation}";
            } else if (metroRadioButton2.Checked)
            {
                var user = metroComboBox1.SelectedItem as User;
                metroLabel1.Text = $"{user.PermToStr()}\nИмя: {user.Name}\nИдентификатор: {user.UserId}\n" +
                   $"Время общего бронирования: {Math.Round(user.VisitedTime / 60, 1)}мин.";
            }
        }

        private void ShowComputersDatabase_Click(object sender, EventArgs e)
        {
            var dbShow = new DatabaseShow
            {
                Text = "База данных подключенных компьютеров."
            };
            var table = new DataTable();

            table.Columns.AddRange(new[] {new DataColumn("Название ПК"),new DataColumn("Цена (руб./ч.)"), new DataColumn("Подключенные устройства")});

            for (var i = 0; i < _computers.Count; i++)
            {
                var pc = _computers[i];
                table.Rows.Add(pc.Name, pc.PricePerHour, pc.GetDeviceString());
            }

            //show.Activate();
            dbShow.Show();
            dbShow.data = table;
        }

        private void ShowUsersDatabase_Click(object sender, EventArgs e)
        {
            var dbShow = new DatabaseShow
            {
                Text = "База данных зарегестрированных пользователей."
            };
            var table = new DataTable();

            table.Columns.AddRange(new[] { new DataColumn("Идентификатор пользователя"), new DataColumn("Имя пользователя"), new DataColumn("Общее время брони"), new DataColumn("Доступ") });

            for (var i = 0; i < _users.Count; i++)
            {
                var user = _users[i];
                table.Rows.Add(user.UserId, user.Name, $"{Math.Round(user.VisitedTime/60, 1)} мин.", user.Perms);
            }

            //show.Activate();
            dbShow.Show();
            dbShow.data = table;
        }

        //Кнопка сохранения логов
        private void SaveLogsButton_Click(object sender, EventArgs e)
        {
            var text = richTextBox1.Text;
            if (text == string.Empty)
            {
                MessageBox.Show("Журнал пустой, нечего сохранять!");
                return;
            }

            Task.Run(() =>
            {
                var fileTime = DateTime.Now.ToFileTime();
                File.WriteAllText($"Лог_{fileTime}.txt", text);

                LogBox.Rtb.BeginInvoke(new MethodInvoker(() => { LogBox.Rtb.Text = string.Empty; }));

                LogBox.Log("Лог файл сохранён успешно!", LogBox.LogType.Success);
                var res = MessageBox.Show("Лог файл сохранён успешно! Открыть?", "Открыть сохранённый лог файл?", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("notepad.exe", Directory.GetCurrentDirectory()+ $"\\Лог_{fileTime}.txt");
                }
            });
        }

        //Кнопка "показ компов"
        private void metroRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!metroRadioButton1.Checked) return;
            metroLabel1.Text = "Выберите элемент из списка...";

            metroComboBox1.Items.Clear();
            metroComboBox1.Items.AddRange(_computers.ToArray());
        }

        //Кнопка "показ пользователей"
        private void metroRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!metroRadioButton2.Checked) return;
            metroLabel1.Text = "";

            metroComboBox1.Items.Clear();
            metroComboBox1.Items.AddRange(_users.ToArray());
        }

        //Кнопка "Изменить"
        private void metroChangeButton_Click(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала выберите элемент в выпадающем списке!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;            
            }
            if (metroRadioButton2.Checked)
            {
                ChangeUserForm form = new ChangeUserForm(metroComboBox1.SelectedItem as User);
                form.FormClosing += Form_FormClosing;
                form.ShowDialog();
            }
            
        }

        //Когда форма изменения уровня доступа пользователя закрывается
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (var writer = new StreamWriter("Users.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<UserMap>();
                csv.WriteRecords(_users);
            }
            metroComboBox1.Items.Clear();
            metroComboBox1.Items.AddRange(_users.ToArray());
        }
    }
}