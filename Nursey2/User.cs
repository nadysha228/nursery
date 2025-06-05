using System;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class User : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        private int currentUserId;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        public User(int user)
        {
            InitializeComponent();
            currentUserId = user;
            ConfigureListView();
            LoadEmployeesData();
        }

        private void ConfigureListView()
        {
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Font = new Font("Century Gothic", 9);
            listView1.Columns.Clear();

            listView1.Columns.Add("ID", 50);
            listView1.Columns.Add("Фамилия", 120);
            listView1.Columns.Add("Имя", 120);
            listView1.Columns.Add("Отчество", 120);
            listView1.Columns.Add("Роль", 100);
            listView1.Columns.Add("Дата рождения", 100);
            listView1.Columns.Add("Телефон", 100);
            listView1.Columns.Add("Пароль", 100);
            listView1.Columns.Add("Логин", 100);
        }

        private void LoadEmployeesData()
        {
            listView1.Items.Clear();

            string query = @"SELECT [Id сотрудника], [Фамилия], [Имя], [Отчество], 
                           [Роль], [Дата рождения], [Телефон], [Пароль], [Логин] 
                           FROM [Сотрудники] ORDER BY [Id сотрудника]";

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["Id сотрудника"].ToString());
                                item.SubItems.Add(reader["Фамилия"].ToString());
                                item.SubItems.Add(reader["Имя"].ToString());
                                item.SubItems.Add(reader["Отчество"].ToString());
                                item.SubItems.Add(reader["Роль"].ToString());
                                item.SubItems.Add(Convert.ToDateTime(reader["Дата рождения"]).ToString("dd.MM.yyyy"));
                                item.SubItems.Add(reader["Телефон"].ToString());
                                item.SubItems.Add(reader["Пароль"].ToString());
                                item.SubItems.Add(reader["Логин"].ToString());

                                listView1.Items.Add(item);
                            }
                        }
                    }
                }

                foreach (ColumnHeader column in listView1.Columns)
                {
                    column.Width = -2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Add_Click(object sender, EventArgs e)
        {
            UserInfo userInfoForm = new UserInfo();
            userInfoForm.UserAdded += () =>
            {
                LoadEmployeesData(); // Обновляем список после добавления
            };
            userInfoForm.ShowDialog();
        }

        private void button2_Edit_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника для редактирования", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedId = int.Parse(listView1.SelectedItems[0].Text);
            UserInfo userInfoForm = new UserInfo(selectedId);
            userInfoForm.UserAdded += () =>
            {
                LoadEmployeesData(); // Обновляем список после редактирования
            };
            userInfoForm.ShowDialog();
        }

        private void button2_Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите сотрудника для удаления", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int employeeId = int.Parse(listView1.SelectedItems[0].Text);
            string employeeName = $"{listView1.SelectedItems[0].SubItems[1].Text} {listView1.SelectedItems[0].SubItems[2].Text}";

            if (MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {employeeName} (ID: {employeeId})?",
                              "Подтверждение удаления",
                              MessageBoxButtons.YesNo,
                              MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM [Сотрудники] WHERE [Id сотрудника] = ?";

                        using (OleDbCommand command = new OleDbCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", employeeId);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Сотрудник успешно удален", "Успех",
                                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadEmployeesData();
                            }
                            else
                            {
                                MessageBox.Show("Сотрудник не найден", "Ошибка",
                                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadEmployeesData();
        }

        private void button3_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}