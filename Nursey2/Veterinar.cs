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
    public partial class Veterinar : Form
    {
        int userID;
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        public event Action MedicalInfoAdded;
        public Veterinar(int user)
        {
            userID = user;

            InitializeComponent();

            LoadMedicalCards();


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        private void LoadMedicalCards()
        {
            // Настройка ListView
            listView1.Clear();
            listView1.Font = new Font("Century Gothic", 10);
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.MultiSelect = false;

            // Добавление колонок с указанием ширины (рассчитано для размера 1364x508)
            listView1.Columns.Add("ID карты", 80);
            listView1.Columns.Add("Дата регистрации", 120);
            listView1.Columns.Add("Диагноз", 200);
            listView1.Columns.Add("Лечение", 200);
            listView1.Columns.Add("Вакцина", 150);
            listView1.Columns.Add("Дата вакцинации", 120);
            listView1.Columns.Add("Процедуры", 250);
            listView1.Columns.Add("ID ветеринара", 100);

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT [Дата регистрации], [Диагноз], [Лечение], 
                           [Название вакцины], [Дата введения вакцины], 
                           [Записи о процедурах], [Id медицинской карты], 
                           [Id Ветеринара] 
                           FROM [Медицинская карта]";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["Id медицинской карты"].ToString());

                                // Форматируем дату регистрации
                                item.SubItems.Add(Convert.ToDateTime(reader["Дата регистрации"]).ToString("dd.MM.yyyy"));

                                // Основные медицинские данные
                                item.SubItems.Add(reader["Диагноз"].ToString());
                                item.SubItems.Add(reader["Лечение"].ToString());

                                // Данные о вакцинации
                                item.SubItems.Add(reader["Название вакцины"].ToString());
                                object vaccineDate = reader["Дата введения вакцины"];
                                item.SubItems.Add(vaccineDate != DBNull.Value ?
                                    Convert.ToDateTime(vaccineDate).ToString("dd.MM.yyyy") : "Нет данных");

                                // Процедуры
                                item.SubItems.Add(reader["Записи о процедурах"].ToString());

                                // ID ветеринара
                                item.SubItems.Add(reader["Id Ветеринара"].ToString());

                                listView1.Items.Add(item);
                            }
                        }
                    }
                }

                // Автоподбор ширины последней колонки
                if (listView1.Columns.Count > 0)
                {
                    listView1.Columns[listView1.Columns.Count - 1].Width = -2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке медицинских карт:\n{ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowMedInfoForm()
        {
            MedInfo medInfoForm = new MedInfo(userID);

            // Сначала подписываемся на событие
            medInfoForm.MedicalInfoAdded += () =>
            {
                // Этот код выполнится при срабатывании события
                LoadMedicalCards(); // Или другой метод обновления данных
            };

            // Затем показываем форму
            medInfoForm.Show();
        }


        private void Veterinar_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Add_Click(object sender, EventArgs e)
        {
            ShowMedInfoForm();
        }

        private void button2_Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите запись для удаления", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ListViewItem selectedItem = listView1.SelectedItems[0];
            int medicalRecordId;

            // Получаем ID (предполагаем, что он в первом SubItem)
            if (!int.TryParse(selectedItem.SubItems[0].Text, out medicalRecordId))
            {
                MessageBox.Show("Ошибка: Некорректный ID записи", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Подтверждение удаления
            string message = $"Вы уверены, что хотите удалить ВСЕ данные этой записи?\n" +
                            $"ID: {medicalRecordId}\n" +
                            $"Диагноз: {selectedItem.SubItems[1].Text}\n" +
                            $"Дата: {selectedItem.SubItems[2].Text}";

            if (MessageBox.Show(message, "Подтверждение удаления",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    // Удаляем запись из таблицы "Медицинская карта"
                    string deleteQuery = @"
                DELETE FROM [Медицинская карта] 
                WHERE [Id медицинской карты] = ?";

                    using (OleDbCommand cmd = new OleDbCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", medicalRecordId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            listView1.Items.Remove(selectedItem);
                            MessageBox.Show("Все данные записи удалены!", "Успех",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Запись не найдена в БД", "Ошибка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (OleDbException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("Нельзя удалить запись: есть связанные данные!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}