using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class Animal : Form
    {
        int userID;
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";

        public Animal(int user)
        {
            userID = user;
            InitializeComponent();
            LoadAnimals();
        }

        private void LoadAnimals()
        {
            // Настройка ListView
            listView1.Clear();
            listView1.Font = new Font("Century Gothic", 10);
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.MultiSelect = false;
            

            // Добавление колонок
            listView1.Columns.Add("ID животного", 80);
            listView1.Columns.Add("Кличка", 150);
            listView1.Columns.Add("Пол", 100);
            listView1.Columns.Add("Порода", 200);
            listView1.Columns.Add("Возраст", 100);
            listView1.Columns.Add("Рост", 100);
            listView1.Columns.Add("Вес", 100);
            listView1.Columns.Add("Дата поступления", 120);

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT [Idживотного], [Кличка], [Пол], [Порода], 
                           [Возраст], [Рост], [Вес], [Дата поступления] 
                           FROM [Животные]";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["Idживотного"].ToString());
                                item.SubItems.Add(reader["Кличка"].ToString());
                                item.SubItems.Add(reader["Пол"].ToString());
                                item.SubItems.Add(reader["Порода"].ToString());
                                item.SubItems.Add(reader["Возраст"].ToString());
                                item.SubItems.Add(reader["Рост"].ToString());
                                item.SubItems.Add(reader["Вес"].ToString());

                                // Форматируем дату поступления
                                object admissionDate = reader["Дата поступления"];
                                item.SubItems.Add(admissionDate != DBNull.Value ?
                                    Convert.ToDateTime(admissionDate).ToString("dd.MM.yyyy") : "Нет данных");

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
                MessageBox.Show($"Ошибка при загрузке данных о животных:\n{ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Animal_Load(object sender, EventArgs e)
        {
            // Дополнительная инициализация при загрузке формы
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Обработка выбора животного
        }

        private void button2_Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите животное для удаления", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ListViewItem selectedItem = listView1.SelectedItems[0];
            int animalId;

            if (!int.TryParse(selectedItem.SubItems[0].Text, out animalId))
            {
                MessageBox.Show("Ошибка: Некорректный ID животного", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string message = $"Вы уверены, что хотите удалить данные о животном?\n" +
                            $"ID: {animalId}\n" +
                            $"Кличка: {selectedItem.SubItems[1].Text}\n" +
                            $"Порода: {selectedItem.SubItems[3].Text}";

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

                    string deleteQuery = @"DELETE FROM [Животные] WHERE [Idживотного] = ?";

                    using (OleDbCommand cmd = new OleDbCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", animalId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            listView1.Items.Remove(selectedItem);
                            MessageBox.Show("Данные о животном удалены!", "Успех",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Животное не найдено в БД", "Ошибка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (OleDbException ex) when (ex.Message.Contains("FOREIGN KEY"))
            {
                MessageBox.Show("Нельзя удалить животное: есть связанные медицинские записи!", "Ошибка",
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

        private void ShowAnimalInfoForm()
        {
            // Создаем экземпляр формы AnimalInfo
            AnimalInfo animalInfoForm = new AnimalInfo(userID);

            // Подписываемся на событие добавления животного
            animalInfoForm.AnimalAdded += () =>
            {
                // Обновляем список животных при срабатывании события
                LoadAnimals();

                // Можно также добавить дополнительную логику, например:
                // - Вывести сообщение об успешном добавлении
                // - Выбрать в списке только что добавленное животное
                // - Обновить другие связанные данные
            };

            // Показываем форму
            animalInfoForm.Show();
        }

        private void button1_Add_Click(object sender, EventArgs e)
        {
            ShowAnimalInfoForm();
        }
    }
}