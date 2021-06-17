using CafeSystem.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CafeSystem
{
    public partial class ChangeUserForm : Form
    {
        User User;
        public ChangeUserForm(User user)
        {
            User = user;
            InitializeComponent();
        }

        private void ChangeUserForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = User.UserId.ToString();
            textBox2.Text = User.Name;
            textBox3.Text = User.VisitedTime.ToString();
            comboBox1.Items.AddRange(new string[] { "Владелец", "Администратор", "Пользователь" });
            switch (User.Perms)
            {
                case User.Permissions.Owner: comboBox1.SelectedIndex = 0;
                    break;
                case User.Permissions.Admin: comboBox1.SelectedIndex = 1;
                    break;
                case User.Permissions.User: comboBox1.SelectedIndex = 2;
                    break;
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            Save();
            MessageBox.Show($"Уровень доступа пользователя {User.Name} обновлён успешно!");
            this.Close();
        }

        private void Save()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    User.Perms = User.Permissions.Owner;
                    break;
                case 1:
                    User.Perms = User.Permissions.Admin;
                    break;
                case 2:
                    User.Perms = User.Permissions.Owner;
                    break;
            }
        }

        private void ChangeUserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            User.Permissions p = User.Permissions.User;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    p = User.Permissions.Owner;
                    break;
                case 1:
                    p = User.Permissions.Admin;
                    break;
                case 2:
                    p = User.Permissions.Owner;
                    break;
            }

            if (p != User.Perms)
            {
                var msg = MessageBox.Show("Сохранить внесённые изменения?", "Сохранение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (msg == DialogResult.Yes) Save();
            }
        }
    }
}
