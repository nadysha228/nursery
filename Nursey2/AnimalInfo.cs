using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class AnimalInfo : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\OneDrive\Documents\Database1.accdb";
        public event Action AnimalAdded;

        public AnimalInfo(int user)
        {
            InitializeComponent();
            // Заполняем ComboBox полами
            comboBox1.Items.AddRange(new object[] { "МУЖ", "ЖЕН" });
            comboBox1.SelectedIndex = 0; // Установка значения по умолчанию
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка заполнения всех полей
            if (string.IsNullOrWhiteSpace(textBox_kl.Text) ||
                comboBox1.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Проверка числовых полей
            if (!int.TryParse(textBox3.Text, out int age) ||
                !int.TryParse(textBox4.Text, out int height) ||
                !int.TryParse(textBox5.Text, out int weight))
            {
                MessageBox.Show("Возраст, рост и вес должны быть целыми числами!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Добавление в БД
                int newId = AddAnimalToDatabase(
                    textBox_kl.Text.Trim(),
                    comboBox1.SelectedItem.ToString(),
                    textBox2.Text.Trim(),
                    age,
                    height,
                    weight,
                    DateTime.Now
                );

                if (newId > 0)
                {
                    MessageBox.Show($"Животное успешно добавлено! ID: {newId}", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AnimalAdded?.Invoke(); // Уведомляем главную форму о добавлении
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении животного: {ex.Message}",
                              "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int AddAnimalToDatabase(string name, string gender, string breed,
                                      int age, int height, int weight, DateTime admissionDate)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // SQL запрос для вставки (без указания Idживотного, так как это Счётчик)
                string query = @"INSERT INTO [Животные] 
                         ([Кличка], [Пол], [Порода], [Возраст], [Рост], [Вес], [Дата поступления]) 
                         VALUES (@Name, @Gender, @Breed, @Age, @Height, @Weight, @Date)";

                using (OleDbCommand cmd = new OleDbCommand(query, connection))
                {
                    // Добавляем параметры с явным указанием типов
                    cmd.Parameters.Add("@Name", OleDbType.VarWChar).Value = name;
                    cmd.Parameters.Add("@Gender", OleDbType.VarWChar).Value = gender;
                    cmd.Parameters.Add("@Breed", OleDbType.VarWChar).Value = breed;
                    cmd.Parameters.Add("@Age", OleDbType.Integer).Value = age;
                    cmd.Parameters.Add("@Height", OleDbType.Integer).Value = height;
                    cmd.Parameters.Add("@Weight", OleDbType.Integer).Value = weight;
                    cmd.Parameters.Add("@Date", OleDbType.Date).Value = admissionDate;

                    // Выполняем запрос
                    cmd.ExecuteNonQuery();

                    // Получаем ID только что добавленной записи
                    cmd.CommandText = "SELECT @@IDENTITY";
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int GetNextAnimalId(OleDbConnection connection)
        {
            string query = "SELECT MAX([Idживотного]) FROM [Животные]";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                return result == DBNull.Value ? 1 : Convert.ToInt32(result) + 1;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }
    }


}