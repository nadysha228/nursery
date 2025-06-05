using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class UserInfo : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        public event Action UserAdded;

        private int? _userId;
        private bool _isEditMode => _userId.HasValue;


        public UserInfo(int? userId = null)
        {
            InitializeComponent();
            _userId = userId;

            LoadRoles();

            if (_isEditMode)
            {
                this.Text = "Редактирование сотрудника";
                LoadEmployeeData();
            }
            else
            {
                this.Text = "Добавление нового сотрудника";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        private void LoadRoles()
        {
            comboBox1.Items.AddRange(new object[] { "Администратор", "Ветеринар", "Волонтёр" });
            comboBox1.SelectedIndex = 0;
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM [Сотрудники] WHERE [Id сотрудника] = ?";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", _userId);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBox1.Text = reader["Фамилия"].ToString();
                                textBox2.Text = reader["Имя"].ToString();
                                textBox2_Di.Text = reader["Отчество"].ToString();
                                comboBox1.SelectedItem = reader["Роль"].ToString();
                                dateTimePicker1.Value = Convert.ToDateTime(reader["Дата рождения"]);
                                textBox3.Text = reader["Телефон"].ToString();
                                textBox4.Text = reader["Пароль"].ToString();
                                textBox5.Text = reader["Логин"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (textBox3.Text.Length != 11 || !textBox3.Text.All(char.IsDigit))
            {
                MessageBox.Show("Телефон должен содержать ровно 11 цифр!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Debug.WriteLine("Открыто соединение с БД");

                    using (OleDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            if (_isEditMode)
                            {
                                // Режим редактирования - UPDATE
                                string updateQuery = @"UPDATE [Сотрудники] SET 
                                                    [Фамилия] = ?,
                                                    [Имя] = ?,
                                                    [Отчество] = ?,
                                                    [Роль] = ?,
                                                    [Дата рождения] = ?,
                                                    [Телефон] = ?,
                                                    [Пароль] = ?,
                                                    [Логин] = ?
                                                    WHERE [Id сотрудника] = ?";

                                Debug.WriteLine($"UPDATE Query: {updateQuery}");

                                using (OleDbCommand command = new OleDbCommand(updateQuery, connection, transaction))
                                {
                                    command.Parameters.Add(new OleDbParameter("@Фамилия", OleDbType.VarWChar) { Value = textBox1.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Имя", OleDbType.VarWChar) { Value = textBox2.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Отчество", OleDbType.VarWChar) { Value = textBox2_Di.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Роль", OleDbType.VarWChar) { Value = comboBox1.SelectedItem.ToString() });
                                    command.Parameters.Add(new OleDbParameter("@ДатаРождения", OleDbType.Date) { Value = dateTimePicker1.Value });
                                    command.Parameters.Add(new OleDbParameter("@Телефон", OleDbType.VarWChar) { Value = textBox3.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Пароль", OleDbType.VarWChar) { Value = textBox4.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Логин", OleDbType.VarWChar) { Value = textBox5.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Id", OleDbType.Integer) { Value = _userId });

                                    Debug.WriteLine("Параметры UPDATE:");
                                    foreach (OleDbParameter param in command.Parameters)
                                    {
                                        Debug.WriteLine($"{param.ParameterName}: {param.Value} ({param.OleDbType})");
                                    }

                                    int rowsAffected = command.ExecuteNonQuery();
                                    Debug.WriteLine($"Обновлено записей: {rowsAffected}");

                                    if (rowsAffected > 0)
                                    {
                                        transaction.Commit();
                                        MessageBox.Show("Данные сотрудника успешно обновлены!", "Успех",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        UserAdded?.Invoke();
                                        this.DialogResult = DialogResult.OK;
                                        this.Close();
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        MessageBox.Show("Не удалось обновить данные сотрудника", "Ошибка",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else
                            {
                                // Режим добавления - INSERT
                                string insertQuery = @"INSERT INTO [Сотрудники] (
                                                    [Фамилия],
                                                    [Имя],
                                                    [Отчество],
                                                    [Роль],
                                                    [Дата рождения],
                                                    [Телефон],
                                                    [Пароль],
                                                    [Логин]
                                                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                                Debug.WriteLine($"INSERT Query: {insertQuery}");

                                using (OleDbCommand command = new OleDbCommand(insertQuery, connection, transaction))
                                {
                                    command.Parameters.Add(new OleDbParameter("@Фамилия", OleDbType.VarWChar) { Value = textBox1.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Имя", OleDbType.VarWChar) { Value = textBox2.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Отчество", OleDbType.VarWChar) { Value = textBox2_Di.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Роль", OleDbType.VarWChar) { Value = comboBox1.SelectedItem.ToString() });
                                    command.Parameters.Add(new OleDbParameter("@ДатаРождения", OleDbType.Date) { Value = dateTimePicker1.Value });
                                    command.Parameters.Add(new OleDbParameter("@Телефон", OleDbType.VarWChar) { Value = textBox3.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Пароль", OleDbType.VarWChar) { Value = textBox4.Text.Trim() });
                                    command.Parameters.Add(new OleDbParameter("@Логин", OleDbType.VarWChar) { Value = textBox5.Text.Trim() });

                                    Debug.WriteLine("Параметры INSERT:");
                                    foreach (OleDbParameter param in command.Parameters)
                                    {
                                        Debug.WriteLine($"{param.ParameterName}: {param.Value} ({param.OleDbType})");
                                    }

                                    int rowsAffected = command.ExecuteNonQuery();
                                    Debug.WriteLine($"Добавлено записей: {rowsAffected}");

                                    if (rowsAffected > 0)
                                    {
                                        // Получаем ID нового сотрудника
                                        string getIdQuery = "SELECT @@IDENTITY";
                                        using (OleDbCommand getIdCommand = new OleDbCommand(getIdQuery, connection, transaction))
                                        {
                                            int newId = Convert.ToInt32(getIdCommand.ExecuteScalar());
                                            Debug.WriteLine($"Новый ID сотрудника: {newId}");
                                        }

                                        transaction.Commit();
                                        MessageBox.Show("Сотрудник успешно добавлен!", "Успех",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        UserAdded?.Invoke();
                                        
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        MessageBox.Show("Не удалось добавить сотрудника", "Ошибка",
                                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Debug.WriteLine($"Ошибка в транзакции: {ex.ToString()}");
                            MessageBox.Show($"Ошибка при сохранении данных:\n{ex.Message}",
                                          "Ошибка транзакции",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Общая ошибка: {ex.ToString()}");
                    MessageBox.Show($"Ошибка подключения к БД:\n{ex.Message}",
                                  "Критическая ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}