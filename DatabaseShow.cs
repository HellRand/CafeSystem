using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeSystem.TelegramClient
{
    public partial class DatabaseShow : Form
    {
        public DataTable data { get; set; }
        public DatabaseShow()
        {
            InitializeComponent();
        }

        private void DatabaseShow_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                while (data == null) Thread.Sleep(100);
                dataGridView1.Invoke(new Action(() => 
                { 
                    dataGridView1.DataSource = data; 
                }
                ));
            });
        }

    }
}
