using System;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        private int currentUserId = -1; // Поле для хранения ID текущего пользователя

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string role = comboBox1.SelectedItem?.ToString().Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверяем пользователя и получаем его ID
            int userId = CheckUserInAccess(login, password, role);

            if (userId > 0)
            {
                currentUserId = userId; // Сохраняем ID текущего пользователя

                switch (role.ToLower())
                {
                    case "администратор":
                        Form2 form2 = new Form2(currentUserId);
                        this.Hide();
                        form2.Show();
                        break;

                    case "ветеринар":
                        Veterinar form3 = new Veterinar(currentUserId);
                        this.Hide();
                        form3.Show();
                        break;
                    case "волонтёр":
                        Volonter form4 = new Volonter (currentUserId);
                        this.Hide();
                        form4.Show();
                        break;
                }
                this.Hide(); // Скрываем окно входа
            }
            else
            {
                MessageBox.Show("Неверный логин, пароль или роль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int CheckUserInAccess(string login, string password, string role)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // Модифицируем запрос, чтобы возвращать ID сотрудника
                    string query = "SELECT [Id сотрудника] FROM Сотрудники WHERE Логин=? AND Пароль=? AND Роль=?";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", login);
                        command.Parameters.AddWithValue("?", password);
                        command.Parameters.AddWithValue("?", role);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int userId = Convert.ToInt32(result);
                            MessageBox.Show($"Успешный вход. ID пользователя: {userId}");
                            return userId;
                        }
                        return -1; // Возвращаем -1 если пользователь не найден
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к БД: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}