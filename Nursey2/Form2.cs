using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nursey2
{
    public partial class Form2 : Form
    {
        int userID;
        public Form2(int user)
        {
            InitializeComponent();
            userID = user;
        }

       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0); // Немедленное завершение с кодом 0
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Volonter volonterForm = new Volonter(userID); // Используем сохраненный userID
            volonterForm.Show(); // Form2 останется открытой
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Veterinar veterinarForm = new Veterinar(userID);
            veterinarForm.Show();
        }

        private void button1_Add_Click(object sender, EventArgs e)
        {
            Animal animalForm = new Animal(userID);
            animalForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            User userForm = new User(userID);
            userForm.Show();

        }
    }
}

