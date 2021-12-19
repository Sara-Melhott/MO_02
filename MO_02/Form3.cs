using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MO_02
{
    public partial class Form3 : Form
    {
        DataTable Data = new DataTable();
        DataTable Basis = new DataTable();
        bool grafics_method = false;
        public Form3()
        {
            InitializeComponent();
            comboBox1.Items.AddRange (new string[] { "Графический метод", "Симплекс метод" });
            comboBox1.SelectedItem = comboBox1.Items[1];
            comboBox2.Items.AddRange(new string[] { "Метод искусственного базиса", "Вручную" });
            comboBox2.SelectedItem = comboBox2.Items[0];
            comboBox3.Items.AddRange(new string[] {"Автоматический режим", "Пошаговый режим" });
            comboBox3.SelectedItem = comboBox3.Items[0];
            comboBox4.Items.AddRange(new string[] {"Обыкновенные", "Десятичные" });
            comboBox4.SelectedItem = comboBox4.Items[0];
            comboBox5.Items.AddRange(new string[] { "Минимизация", "Максимизация"});
            comboBox5.SelectedItem = comboBox5.Items[0];
            number_x.Value = 1;
            number_x.Minimum = 1;
            number_x.Maximum = 16;
            number_condition.Value = 1;
            number_condition.Minimum = 1;
            number_condition.Maximum = 16;
            label6.Visible = false;
            dataGridView2.Visible = false;

            openFileDialog1.Filter = "Example(*.xml) | *.xml";
            saveFileDialog1.Filter = "Output (*.xml) | *.xml";

            initData(grafics_method);
            initBasis();
        }
        /// <summary>
        /// Кнопка "Применить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            initData(grafics_method);
            initBasis();
            if ((comboBox1.SelectedItem.ToString() == "Графический метод") && (number_x.Value > 2))
            {
                label4.Visible = true;
                comboBox2.Visible = true;
                comboBox2_Visible();
            }
            if ((comboBox1.SelectedItem.ToString() == "Графический метод") && (number_x.Value <= 2))
            {
                label4.Visible = false;
                comboBox2.Visible = false;
                label6.Visible = false;
                dataGridView2.Visible = false;
            }
        }
        /// <summary>
        /// Выбор файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filepath = openFileDialog1.FileName;
            XmlFiles xml = new XmlFiles();
            Task task = xml.ReadFile(filepath);
            if (task.answer != null)
            {
                MessageBox.Show("Данные в файле не подходят для решения задачи!");
            }
            else if (task.function.Length > 16 || task.condition.Length > 16)
            {
                MessageBox.Show("Слишком много переменных или условий!\n(Максимум 16)");
            }
            else
            {
                Form2 form2 = new Form2(task);
                form2.FormClosed += OnFormClosed;
                Hide();
                form2.Show();
            }
        }
        /// <summary>
        /// Кнопка "Решить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Task task = NewTask();
            if (task.answer != null)
            {
                MessageBox.Show("Данные задачи не корректны!");
                return;
            }
            if (checkBox1.Checked == true)
            {
                XmlFiles xml = new XmlFiles();
                xml.WriteFile(task);
                MessageBox.Show("Задача сохранена в файл output.xml");
            }
            if (comboBox1.SelectedItem == "Графический метод")
            {
                Form5 form = new Form5(task);
                form.FormClosed += OnFormClosed;
                Hide();
                form.Show();
            }
            else
            {
                bool step = false;
                bool ordinaryFraction = true;
                if (comboBox3.SelectedItem == "Пошаговый режим")
                    step = true;
                if (comboBox4.SelectedItem == "Десятичные")
                    ordinaryFraction = false;
                Form4 form = new Form4(task, step, ordinaryFraction);
                form.FormClosed += OnFormClosed;
                Hide();
                form.Show();
            }
        }
        /// <summary>
        /// Выбор метода решения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Графический метод")
            {
                //Отклбчение выбора режима решения 
                comboBox3.Visible = false;
                label5.Visible = false;
                grafics_method = true;
                initData(grafics_method);
                if (number_x.Value <= 2)
                {
                    //Отключение выбора базиса
                    label4.Visible = false;
                    comboBox2.Visible = false;
                    label6.Visible = false;
                    dataGridView2.Visible = false;
                }
                else
                {
                    label4.Visible = true;
                    comboBox2.Visible = true;
                    comboBox2_Visible();
                }
            }
            else
            {
                //если симплекс метод - включение выбора режима
                comboBox3.Visible = true;
                label5.Visible = true;
                grafics_method = false;
                label4.Visible = true;
                comboBox2.Visible = true;
                comboBox2_Visible();
                initData(grafics_method);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2_Visible();
        }
        private void comboBox2_Visible()
        {
            if (comboBox2.SelectedItem == "Вручную")
            {
                label6.Visible = true;
                dataGridView2.Visible = true;
                initBasis();
            }
            else
            {
                label6.Visible = false;
                dataGridView2.Visible = false;
            }
        }
        private Task NewTask()
        {
            int y = Data.Rows.Count;
            int x = Data.Columns.Count;
            int size = x - 3;
            OrdinaryFraction[] function = new OrdinaryFraction[x - 3];
            OrdinaryFraction[][] conditions = new OrdinaryFraction[y - 3][];
            string[] sign = new string[y - 3];
            int[][] basis = new int[x - 3][];
            string extr;

            for (int i = 1; i < x - 2; i++)
                function[i-1] = new OrdinaryFraction(dataGridView1.Rows[1].Cells[i].Value.ToString());
            for (int j = 3, k = 0; j < y; j++)
            {
                OrdinaryFraction[] row = new OrdinaryFraction[x - 2];
                for (int i = 1; i < x - 2; i++)
                    row[i-1] = new OrdinaryFraction(dataGridView1.Rows[j].Cells[i].Value.ToString());
                row[x - 3] = new OrdinaryFraction(dataGridView1.Rows[j].Cells[x-1].Value.ToString());
                sign[k] = dataGridView1.Rows[j].Cells[x-2].Value.ToString();
                conditions[k++] = row;
            }
            if (comboBox2.SelectedItem == "Метод искусственного базиса")
                basis = null;
            else
                for (int i = 0; i < x - 3; i++)
                    basis[i] = new int[] { i + 1, int.Parse(dataGridView2.Rows[1].Cells[i].Value.ToString()) };
            if (comboBox5.SelectedItem == "Минимизация")
                extr = "min";
            else
                extr = "max";
            return new Task(size, function, conditions, sign, basis, extr, null);
        }
        private void OnFormClosed(object sender, EventArgs e)
        {
            (sender as Form).FormClosed -= OnFormClosed;
            Show();
        }
        private void initBasis()
        {
            Basis.Rows.Clear();
            Basis.Columns.Clear();
            int x = 1;
            int y = 2;
            //Инициализация таблицы
            x = int.Parse(number_x.Value.ToString());
            for (int i = 0; i < x; i++)
                Basis.Columns.Add();
            for (int i = 0; i < y; i++)
                Basis.Rows.Add();
            for (int i=0; i<x; i++)
            {
                Basis.Rows[0][i] = "x" + (i + 1);
                Basis.Rows[1][i] = "0";
            }
            dataGridView2.DataSource = Basis;
            dataGridView2.Rows[0].ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.AllowUserToResizeRows = false;
            dataGridView2.AllowUserToResizeColumns = false;
            for (int i=0; i<x; i++)
            {
                dataGridView2.Rows[0].Cells[i].Style.BackColor = Color.LightGray;
                dataGridView2.Columns[i].HeaderText = "";
                dataGridView2.Columns[i].Width = 50;
                dataGridView2.Columns[i].Resizable = DataGridViewTriState.False;
                dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void initData(bool grafics_method)
        {
            Data.Rows.Clear();
            Data.Columns.Clear();
            int x = 1;
            int y = 1;
            try
            {
                //Инициализация таблицы
                x = int.Parse(number_x.Value.ToString());
                y = int.Parse(number_condition.Value.ToString());
                for (int i = 0; i < x + 2 + 1; i++)
                    Data.Columns.Add();
                for (int i = 0; i < y + 3; i++)
                    Data.Rows.Add();
                //Декоративные записи
                Data.Rows[2][x + 2] = "b";
                for (int i = 1; i < x + 1; i++)
                {
                    Data.Rows[0][i] = "x" + i;
                    Data.Rows[1][i] = "0";
                    Data.Rows[2][i] = "x" + i;
                    for (int j = 3; j < y + 3; j++)
                        Data.Rows[j][i] = 0;
                }
                Data.Rows[2][x+1] = "sign";
                Data.Rows[0][x+1] = "";
                for (int i = 3; i < y + 3; i++)
                {
                    if (!grafics_method || x > 2)
                    {
                        Data.Rows[i][x + 1] = "=";
                    }
                    else
                    {
                        Data.Rows[i][x + 1] = ">=";
                    }
                    Data.Rows[i][0] = "f" + (i - 2);
                    Data.Rows[i][x + 2] = "0";
                }
                Data.Rows[1][0] = "f0";
                //Редактирование внешнего вида dataGridView1
                dataGridView1.DataSource = Data;
                dataGridView1.Rows[0].ReadOnly = true;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Rows[2].ReadOnly = true;
                dataGridView1.Rows[1].Cells[x+1].ReadOnly = true;
                dataGridView1.Rows[1].Cells[x+2].ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.AllowUserToResizeColumns = false;
                if (!grafics_method || x > 2)
                {
                    dataGridView1.Columns[x + 1].ReadOnly = true;
                }
                for (int i = 0; i < x + 2 + 1; i++)
                {
                    dataGridView1.Columns[i].HeaderText = "";
                    dataGridView1.Columns[i].Width = 50;
                    dataGridView1.Columns[i].Resizable = DataGridViewTriState.False;
                    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.Red;
                }
                //Окраска ячеек
                for (int i = 0; i < x + 2 + 1; i++)
                {
                    dataGridView1.Rows[0].Cells[i].Style.BackColor = Color.LightGray;
                    dataGridView1.Rows[2].Cells[i].Style.BackColor = Color.LightGray;
                }
                for (int i=0; i < y + 3; i++)
                    dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                
            }
            catch
            {
                MessageBox.Show("Неправильное значение количества неизвестных или количества условий");
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            int y = e.RowIndex;
            int x = e.ColumnIndex;
            string var = e.FormattedValue.ToString();
            if (!((x >= Data.Columns.Count-2) && (y == 1)))
            if (y != 0 && y != 2 && x != 0)
            {
                if (x == Data.Columns.Count - 2)
                {
                    if (grafics_method && (number_x.Value <= 2))
                    {
                        string pattern1 = @"^>=$";
                        string pattern2 = @"^<=$";
                        if (!(Regex.IsMatch(var, pattern1) || Regex.IsMatch(var, pattern2)))
                        {
                            MessageBox.Show("Неправильно значение ячейки.\nДопустимые значения: >=; <=;");
                            //Запрет ухода из ячейки
                            e.Cancel = true;
                        }
                    }
                }
                else
                {
                    string pattern1 = @"^-?[1-9][0-9]*$";
                    string pattern2 = @"^0$";
                    string pattern3 = @"^0/[1-9]*$";
                    string pattern6 = @"^-?[1-9][0-9]*/-?[1-9]*$";
                    string pattern4 = @"^-?[1-9][0-9]*,[0-9]*$";
                    string pattern5 = @"^-?0,[0-9]*$";
                    if (!(Regex.IsMatch(var, pattern1) || Regex.IsMatch(var, pattern2) || Regex.IsMatch(var, pattern3)
                        || Regex.IsMatch(var, pattern5) || Regex.IsMatch(var, pattern4) || Regex.IsMatch(var, pattern6)))
                    {
                        MessageBox.Show("Неправильно значение ячейки.\nДопустимые значения: 1; 1/25; 1,23;");
                        //Запрет ухода из ячейки
                        e.Cancel = true;
                    }
                }
            }
        }

        private void dataGridView2_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            int y = e.RowIndex;
            int x = e.ColumnIndex;
            string var = e.FormattedValue.ToString();
            if (y != 0)
            {
                string pattern1 = @"^[1-9][0-9]*$";
                string pattern2 = @"^0$";
                string pattern3 = @"^0/[1-9]*$";
                string pattern6 = @"^[1-9][0-9]*/[1-9]*$";
                string pattern4 = @"^[1-9][0-9]*,[0-9]*$";
                string pattern5 = @"^0,[0-9]*$";
                if (!(Regex.IsMatch(var, pattern1) || Regex.IsMatch(var, pattern2) || Regex.IsMatch(var, pattern3)
                    || Regex.IsMatch(var, pattern5) || Regex.IsMatch(var, pattern4) || Regex.IsMatch(var, pattern6)))
                {
                    MessageBox.Show("Неправильно значение ячейки.\nДопустимые значения: 1; 1/25; 1,23;");
                    //Запрет ухода из ячейки
                    e.Cancel = true;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
