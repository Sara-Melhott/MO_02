using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MO_02
{
    public partial class Form2 : Form
    {
        Task task;
        public Form2(Task task)
        {
            this.task = task;
            InitializeComponent();
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            string text = task.print(true);
            textBox1.Text = text;

            comboBox1.Items.AddRange(new string[] { "Графический метод", "Симплекс метод" });
            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox3.Items.AddRange(new string[] { "Обыкновенные", "Десятичные" });
            comboBox3.SelectedItem = comboBox3.Items[0];
            comboBox4.Items.AddRange(new string[] { "Автоматический режим", "Пошаговый режим" });
            comboBox4.SelectedItem = comboBox4.Items[0];
            if (task.basis != null)
            {
                label4.Visible = false;
                textBox2.Visible = false;
            }
            if (task.function.Length <= 2)
            {
                label4.Visible = false;
                textBox2.Visible = false;
            }
        }
        /// <summary>
        /// Кнопка "Отмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Кнопка "Решить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (task.function.Length > 2)
                {
                    for (int i = 0; i < task.sign.Length; i++)
                    {
                        if (task.sign[i] == "<=" || task.sign[i] == ">=")
                        {
                            MessageBox.Show("Задача не приведена к канонической форме!");
                            return;
                        }
                    }
                }
                if (task.function.Length <= 2)
                {
                    bool sign1 = false;
                    bool sign2 = false;
                    for (int i = 0; i < task.sign.Length; i++)
                    {
                        if (task.sign[i] == "<=" || task.sign[i] == ">=")
                            sign1 = true;
                        if (task.sign[i] == "=")
                            sign2 = false;
                    }
                    if (sign1 && sign2)
                    {
                        MessageBox.Show("Задача не приведена к канонической форме!");
                        return;
                    }
                }
                if (comboBox1.SelectedItem == "Графический метод")
                {
                    bool ordinaryFraction = true;
                    if (comboBox3.SelectedItem == "Десятичные")
                        ordinaryFraction = false;
                    Form5 form = new Form5(task, ordinaryFraction);
                    form.FormClosed += OnFormClosed;
                    Hide();
                    form.Show();
                }
                else
                {
                    bool step = false;
                    bool ordinaryFraction = true;
                    if (comboBox4.SelectedItem == "Пошаговый режим")
                        step = true;
                    if (comboBox3.SelectedItem == "Десятичные")
                        ordinaryFraction = false;
                    Form4 form = new Form4(task, step, ordinaryFraction);
                    form.FormClosed += OnFormClosed;
                    Hide();
                    form.Show();
                }
            }
            catch
            {
                MessageBox.Show("Эту задачу мы решить не сможем =(");
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Симплекс метод")
            {
                label1.Visible = true;
                comboBox4.Visible = true;
                if (task.function.Length <= 2)
                {
                    bool sign1 = true;
                    for (int i=0; i<task.sign.Length; i++)
                    {
                        if (task.sign[i] == "=")
                        {
                            sign1 = false;
                            break;
                        }
                    }
                    if (sign1)
                    {
                        comboBox1.SelectedItem = comboBox1.Items[0];
                        MessageBox.Show("Данную задачу можно решить только графическим методом!");
                    }
                }
                else
                {
                    if (task.basis == null)
                    {
                        label4.Visible = true;
                        textBox2.Visible = true;
                    }
                    
                }
            }
            else
            {
                if (task.function.Length <= 2)
                {
                    bool sign1 = false;
                    for (int i=0; i<task.sign.Length; i++)
                    {
                        if (task.sign[i] == "=")
                        {
                            sign1 = true;
                            break;
                        }
                    }
                    if (sign1)
                    {
                        comboBox1.SelectedItem = comboBox1.Items[1];
                        MessageBox.Show("Данную задачу можно решить только симплекс методом!");
                        return;
                    }
                }
                label1.Visible = false;
                comboBox4.Visible = false;
                label4.Visible = false;
                textBox2.Visible = false;
            }
        }
        private void OnFormClosed(object sender, EventArgs e)
        {
            (sender as Form).FormClosed -= OnFormClosed;
            Show();
        }
    }
}
