using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class AdoptInfo: Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        public event Action AdoptionAdded;

        int userID;
        public AdoptInfo(int user)
        {
            InitializeComponent();
            userID = user;

            LoadAnimalsToComboBox();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        private void LoadAnimalsToComboBox()
        {
            cb_animal.Items.Clear();

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
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяем, что все обязательные поля заполнены
            if (string.IsNullOrWhiteSpace(textBox2_Famil.Text) ||
                string.IsNullOrWhiteSpace(textBox1_Name.Text) ||
                string.IsNullOrWhiteSpace(textBox4_Number.Text) ||
                cb_animal.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // 1. Получаем следующий ID для усыновителя
                    int nextAdopterId = GetNextAdopterId(connection);

                    // 2. Получаем ID выбранного животного
                    int animalId = GetSelectedAnimalId();

                    // 3. Подготавливаем параметры для вставки
                    string query = @"INSERT INTO Усыновления 
                            ([Idусыновителя], [Id сотрудника], [Id животного], 
                             [Фамилия], [Имя], [Отчество], [Телефон], [Дата усыновления])
                            VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("?", nextAdopterId);
                        command.Parameters.AddWithValue("?", userID); // userID - переменная класса
                        command.Parameters.AddWithValue("?", animalId);
                        command.Parameters.AddWithValue("?", textBox2_Famil.Text.Trim());
                        command.Parameters.AddWithValue("?", textBox1_Name.Text.Trim());
                        command.Parameters.AddWithValue("?", textBox3_Otchestvo.Text.Trim());
                        command.Parameters.AddWithValue("?", textBox4_Number.Text.Trim());
                        command.Parameters.AddWithValue("?", DateTime.Now.Date);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные об усыновлении успешно сохранены!", "Успех",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();

                            AdoptionAdded?.Invoke();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось сохранить данные.", "Ошибка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для получения следующего ID усыновителя
        private int GetNextAdopterId(OleDbConnection connection)
        {
            string query = "SELECT MAX([Idусыновителя]) FROM Усыновления";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                return result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1;
            }
        }

        // Метод для получения ID выбранного животного
        private int GetSelectedAnimalId()
        {
            if (cb_animal.SelectedItem == null)
            {
                throw new Exception("Животное не выбрано");
            }

            // Получаем выбранный текст из ComboBox (в формате "ID - Кличка")
            string selectedText = cb_animal.SelectedItem.ToString();

            // Извлекаем ID (часть до дефиса)
            string idPart = selectedText.Split('-')[0].Trim();

            // Пытаемся преобразовать в число
            if (int.TryParse(idPart, out int animalId))
            {
                return animalId;
            }

            throw new Exception("Не удалось определить ID животного");
        }

        // Метод для очистки формы после успешного сохранения
        private void ClearForm()
        {
            textBox2_Famil.Clear();
            textBox1_Name.Clear();
            textBox3_Otchestvo.Clear();
            textBox4_Number.Clear();
            if (cb_animal.Items.Count > 0)
                cb_animal.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
