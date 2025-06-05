using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class MedInfo : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        public event Action MedicalInfoAdded;

        int _user;
        public MedInfo(int user)
        {
            InitializeComponent();

            _user = user;

            LoadAnimalsToComboBox();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }


        private void LoadAnimalsToComboBox()
        {
            cb_animal.Items.Clear();
            // ПОТОМ УБРАТЬ ВДРУГОЕ МЕСТО!!!!!

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT [Idживотного], [Кличка] FROM Животные";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string displayText = $"{reader["Idживотного"]} - {reader["Кличка"]}";
                                cb_animal.Items.Add(displayText);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка выбора животного
            if (cb_animal.SelectedItem == null)
            {
                MessageBox.Show("Выберите животное из списка", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка обязательных полей
            if (string.IsNullOrEmpty(textBox2_Diagnoz.Text) || string.IsNullOrEmpty(textBox1_Lechenie.Text))
            {
                MessageBox.Show("Заполните обязательные поля: Диагноз и Лечение", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем ID животного
            string selectedAnimal = cb_animal.SelectedItem.ToString();
            int animalId;
            if (!int.TryParse(selectedAnimal.Split('-')[0].Trim(), out animalId))
            {
                MessageBox.Show("Некорректный ID животного", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Включаем журналирование SQL для отладки
                    Debug.WriteLine("Открыто соединение с БД");

                    using (OleDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Удаление старой записи
                            string deleteQuery = "DELETE FROM [Медицинская карта] WHERE [Id медицинской карты] = ?";
                            Debug.WriteLine($"DELETE Query: {deleteQuery}");

                            using (OleDbCommand deleteCommand = new OleDbCommand(deleteQuery, connection, transaction))
                            {
                                deleteCommand.Parameters.Add(new OleDbParameter("@medCardId", OleDbType.Integer) { Value = animalId });
                                int deletedRows = deleteCommand.ExecuteNonQuery();
                                Debug.WriteLine($"Удалено записей: {deletedRows}");
                            }

                            // 2. Вставка новой записи
                            string insertQuery = @"INSERT INTO [Медицинская карта] (
                        [Дата регистрации],
                        [Диагноз],
                        [Лечение],
                        [Название вакцины],
                        [Дата введения вакцины],
                        [Записи о процедурах],
                        [Id медицинской карты],
                        [Id Ветеринара]
                    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                            Debug.WriteLine($"INSERT Query: {insertQuery}");

                            using (OleDbCommand insertCommand = new OleDbCommand(insertQuery, connection, transaction))
                            {
                                // Явное указание типов параметров
                                insertCommand.Parameters.Add(new OleDbParameter("@regDate", OleDbType.Date) { Value = DateTime.Now });
                                insertCommand.Parameters.Add(new OleDbParameter("@diagnoz", OleDbType.VarWChar) { Value = textBox2_Diagnoz.Text });
                                insertCommand.Parameters.Add(new OleDbParameter("@lechenie", OleDbType.VarWChar) { Value = textBox1_Lechenie.Text });

                                // Обработка NULL для необязательных полей
                                object vaccineValue = string.IsNullOrWhiteSpace(textBox2_Prochedyra.Text) ? DBNull.Value : (object)textBox2_Prochedyra.Text;
                                insertCommand.Parameters.Add(new OleDbParameter("@vaccine", OleDbType.VarWChar) { Value = vaccineValue });

                                object vaccineDateValue = (vaccineValue == DBNull.Value) ? DBNull.Value : (object)dateTimePicker1.Value.Date;
                                insertCommand.Parameters.Add(new OleDbParameter("@vaccineDate", OleDbType.Date) { Value = vaccineDateValue });

                                object proceduresValue = string.IsNullOrWhiteSpace(textBox3.Text) ? DBNull.Value : (object)textBox3.Text;
                                insertCommand.Parameters.Add(new OleDbParameter("@procedures", OleDbType.VarWChar) { Value = proceduresValue });

                                insertCommand.Parameters.Add(new OleDbParameter("@medCardId", OleDbType.Integer) { Value = animalId });
                                insertCommand.Parameters.Add(new OleDbParameter("@vetId", OleDbType.Integer) { Value = _user });

                                // Логирование параметров
                                Debug.WriteLine("Параметры INSERT:");
                                foreach (OleDbParameter param in insertCommand.Parameters)
                                {
                                    Debug.WriteLine($"{param.ParameterName}: {param.Value} ({param.OleDbType})");
                                }

                                int insertedRows = insertCommand.ExecuteNonQuery();
                                Debug.WriteLine($"Добавлено записей: {insertedRows}");

                                if (insertedRows > 0)
                                {
                                    transaction.Commit();
                                    MessageBox.Show("Данные успешно обновлены!", "Успех",
                                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    MedicalInfoAdded?.Invoke();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    MessageBox.Show("Не удалось добавить данные", "Ошибка",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Debug.WriteLine($"Ошибка в транзакции: {ex.ToString()}");
                            MessageBox.Show($"Ошибка при сохранении данных:\n{ex.Message}\n\nПроверьте:\n1. Корректность всех данных\n2. Наличие связанных записей",
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
