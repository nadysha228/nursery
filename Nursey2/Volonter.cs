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
    public partial class Volonter: Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        int _userId;
        public Volonter(int user)
        {
            InitializeComponent();

            _userId = user;

            LoadAdoptionsData();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        private void LoadAdoptionsData()
        {
            listView1.Clear(); // Очищаем ListView перед загрузкой новых данных

            // Настраиваем шрифт
            listView1.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            listView1.View = View.Details; // Режим отображения - таблица

            // Добавляем колонки
            listView1.Columns.Add("ID усыновителя", 100);
            listView1.Columns.Add("ID сотрудника", 100);
            listView1.Columns.Add("ID животного", 100);
            listView1.Columns.Add("Фамилия", 150);
            listView1.Columns.Add("Имя", 150);
            listView1.Columns.Add("Отчество", 150);
            listView1.Columns.Add("Телефон", 150);
            listView1.Columns.Add("Дата усыновления", 150);

            listView1.FullRowSelect = true;    // Выделение всей строки
            listView1.GridLines = true;       // Отображение линий сетки
            listView1.MultiSelect = false;    // Запрет множественного выбора
            listView1.HideSelection = false;  // Подсветка даже когда форма не в фокусе

            // Получаем данные из БД
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT [Idусыновителя], [Id сотрудника], [Id животного], " +
                                   "[Фамилия], [Имя], [Отчество], [Телефон], [Дата усыновления] " +
                                   "FROM Усыновления";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["Idусыновителя"].ToString());
                                item.SubItems.Add(reader["Id сотрудника"].ToString());
                                item.SubItems.Add(reader["Id животного"].ToString());
                                item.SubItems.Add(reader["Фамилия"].ToString());
                                item.SubItems.Add(reader["Имя"].ToString());
                                item.SubItems.Add(reader["Отчество"].ToString());
                                item.SubItems.Add(reader["Телефон"].ToString());
                                item.SubItems.Add(Convert.ToDateTime(reader["Дата усыновления"]).ToShortDateString());

                                listView1.Items.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Автоматически подгоняем ширину последней колонки под оставшееся пространство
            listView1.Columns[listView1.Columns.Count - 1].Width = -2;
        }

        private void ShowAdoptInfoForm()
        {
            AdoptInfo adoptInfoForm = new AdoptInfo(_userId);
            adoptInfoForm.AdoptionAdded += () =>
            {
                // Обновляем ListView при срабатывании события
                LoadAdoptionsData();
            };
            adoptInfoForm.Show();
        }

        private void button1_Add_Click(object sender, EventArgs e)
        {
            ShowAdoptInfoForm();
        }

        private void button2_Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите запись для удаления!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Подсветка выбранной строки перед удалением
            ListViewItem selectedItem = listView1.SelectedItems[0];
            selectedItem.BackColor = Color.Orange;  // Временная подсветка
            selectedItem.ForeColor = Color.White;
            listView1.Refresh();

            // Диалог подтверждения с информацией о выбранной записи
            string message = $"Вы уверены, что хотите удалить эту запись?\n\n" +
                           $"Усыновитель: {selectedItem.SubItems[3].Text} {selectedItem.SubItems[4].Text}\n" +
                           $"Животное: {selectedItem.SubItems[2].Text}\n" +
                           $"Дата: {selectedItem.SubItems[7].Text}";

            DialogResult result = MessageBox.Show(message, "Подтверждение удаления",
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                               MessageBoxDefaultButton.Button2);

            // Восстанавливаем цвета, если отмена
            if (result != DialogResult.Yes)
            {
                selectedItem.BackColor = listView1.BackColor;
                selectedItem.ForeColor = listView1.ForeColor;
                return;
            }

            // Удаление из БД (ваш существующий код)
            int adopterId = int.Parse(selectedItem.SubItems[0].Text);
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Усыновления WHERE [Idусыновителя] = ?";

                    using (OleDbCommand command = new OleDbCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("?", adopterId);
                        if (command.ExecuteNonQuery() > 0)
                        {
                            listView1.Items.Remove(selectedItem);
                            MessageBox.Show("Запись успешно удалена!", "Успех",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                selectedItem.BackColor = Color.Pink;  // Подсветка ошибки
                MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
