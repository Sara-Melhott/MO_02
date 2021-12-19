using System;
using System.Collections;
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
    public partial class Form4 : Form
    {
        Task task;
        bool step_by_step;
        bool ordinaryFraction;
        DataTable Table = new DataTable();
        SimplexTable simplex;
        int number_table = 0;
        List<int[]> listEl;
        List<int[]> AllListEl = new List<int[]>();
        List<int[]> BestListEl = new List<int[]>();
        public Form4(Task task, bool step, bool ordinaryFraction)
        {
            this.task = task;
            
            GaussMethod gaussMethod = new GaussMethod(task.Clone());
            simplex = new SimplexTable(gaussMethod.MainGaussMethod());
            this.step_by_step = step;
            this.ordinaryFraction = ordinaryFraction;
            InitializeComponent();
            if (!step)
            {
                button2.Visible = false;
                button3.Visible = false;
            }
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox2.Multiline = true;
            textBox2.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.Text = task.print(ordinaryFraction);
            
            if (task.basis == null)
                ArtificialBasis();
            simplex.stack.Push(new ArrayList { simplex.tableCopy(), simplex.task.Clone() });
            Main();
        }

        /// <summary>
        /// Кнопка "Далее"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            simplex.stack.Push(new ArrayList { simplex.tableCopy(), simplex.task.Clone() });
            listEl = simplex.FindReferenceElement();
            int[] el = ChooseEl(listEl);
            if (el == null) return;
            //Получение ответа от пользователя (выбор опорного элемента)
            simplex.NewSimplexTable(el);
            ++number_table;
            button2.Enabled = true;
            //dataGridView1.DataSource = Table;
            Main();
        }
        /// <summary>
        /// Кнопка "Назад"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            ArrayList list = simplex.stack.Pop();
            simplex.table = (OrdinaryFraction[][])list[0];
            simplex.task = (Task)list[1];
            listEl = simplex.FindReferenceElement();
            --number_table;
            button3.Enabled = true;
            textBox2.Text = "";
            //Удаление последней таблицы в dataGridView1
            PrintDeleteTable();
            if (Table.Rows.Count == simplex.table.Length+2) button2.Enabled = false;
        }
        /// <summary>
        /// Кнопка "В главное меню"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        /// <summary>
        /// Главный метод решения 
        /// </summary>
        private void Main()
        {
            if (step_by_step)
            {
                if (task.answer == null)
                {
                    if (!simplex.FindSolution())
                    {
                        button3.Enabled = true;
                        //Печатаем таблицу number_table
                        listEl = simplex.FindReferenceElement();
                        textBox2.Text = "";
                        PrintAddTable();
                    }
                    else
                    {
                        textBox2.Text = "Ответ: " + simplex.Answer(ordinaryFraction);
                        button3.Enabled = false;
                        PrintAddTable();
                    }
                }
                else
                {
                    textBox2.Text = "Ответ: " + task.answer;
                    button3.Enabled = false;
                }
            }
            else
            {
                if (task.answer != null)
                {
                    textBox2.Text = "Ответ: " + task.answer;
                    return;
                }
                while (!simplex.FindSolution())
                {
                    textBox2.Text = "";
                    listEl = simplex.FindReferenceElement();
                    
                    //Печатаем таблицу number_table
                    PrintAddTable();
                    simplex.stack.Push(new ArrayList { simplex.tableCopy(), simplex.task.Clone() });
                    int[] best = simplex.MaxReferenceElement(listEl);
                    simplex.NewSimplexTable(best);
                    ++number_table;

                    //Заносим в список опрные элементы для окраски
                    int old_y = Table.Rows.Count - simplex.table.Length - 2;
                    if (number_table == 0)
                    {
                        for (int i = 0; i < listEl.Count; i++)
                            AllListEl.Add(new int[] { listEl[i][0]+1, listEl[i][1]+1 });
                        BestListEl.Add(new int[] { best[0] +1, best[1]+1 });
                    }
                    else
                    {
                        for (int i = 0; i < listEl.Count; i++)
                            AllListEl.Add(new int[] { listEl[i][0] + 1, listEl[i][1] + 1 + old_y });
                        BestListEl.Add(new int[] { best[0] + 1, best[1] + 1 + old_y });
                    }
                }
                textBox2.Text = "Ответ: " + simplex.Answer(ordinaryFraction);
                PrintAddTable();
            }
        }
        private int[] ChooseEl(List<int[]> listEl)
        {
            int old_y = Table.Rows.Count - simplex.table.Length - 2;
            if (dataGridView1.CurrentCell != null)
            {
                int x = dataGridView1.CurrentCell.ColumnIndex;
                int y = dataGridView1.CurrentCell.RowIndex;
                if (number_table == 0)
                {
                    for (int i = 0; i < listEl.Count; i++)
                        if ((x - 1 == listEl[i][0]) && (y - 1 == listEl[i][1]))
                            return listEl[i];
                }
                else
                {
                    for (int i = 0; i < listEl.Count; i++)
                        if ((x - 1 == listEl[i][0]) && (y - 1 - old_y == listEl[i][1]))
                            return listEl[i];
                }
                MessageBox.Show("Вы выбрали неправильный опорный элемент!");
            }
            else
            {
                MessageBox.Show("Выберите опорный элемент!");
            }
            return null;
        }
        /// <summary>
        /// Удаляет последнюю таблицу в Table
        /// </summary>
        private void PrintDeleteTable()
        {
            int old_y = Table.Rows.Count;
            int y = simplex.table.Length;
            for (int i = 0, j = old_y - 1; i < y+2; i++, j--)
                Table.Rows.RemoveAt(j);

            dataGridView1.DataSource = Table;
            dataGridView1.ReadOnly = true;
        }
        /// <summary>
        /// Добавляет таблицу в Table
        /// </summary>
        private void PrintAddTable()
        {
            int old_y = Table.Rows.Count;
            int x = simplex.table[0].Length;
            int y = simplex.table.Length;

            if (number_table == 0)
            {
                Table.Rows.Add();
                for (int i = 0; i < x + 1; i++)
                    Table.Columns.Add();
                for (int i = 0; i < y + 1; i++)
                    Table.Rows.Add();
                for (int i = 1; i < y + 1; i++)
                    for (int j = 1; j < x + 1; j++)
                        Table.Rows[i][j] = simplex.table[i - 1][j - 1].Print(ordinaryFraction);
                //Декор
                Table.Rows[0][0] = "X[" + number_table + "]";
                for (int i = 1; i < simplex.basis_var+1; i++)
                    Table.Rows[i][0] = "x" + simplex.task.basis[i - 1][0];
                for (int i = simplex.basis_var, j = 1; i < simplex.task.basis.Length; i++, j++)
                    Table.Rows[0][j] = "x" + simplex.task.basis[i][0];
                button2.Enabled = false;
            }
            else
            {
                Table.Rows.Add();
                for (int i = 0; i < y + 1; i++)
                    Table.Rows.Add();
                for (int i = old_y + 1; i < old_y + y + 1; i++)
                {
                    for (int j = 1; j < x + 1; j++)
                    {
                        Table.Rows[i][j] = simplex.table[i - old_y - 1][j - 1].Print(ordinaryFraction);
                    }
                }
                //Декор
                Table.Rows[old_y][0] = "X[" + number_table + "]";
                for (int i = 1, j = old_y+1; i < simplex.basis_var + 1; i++, j++)
                    Table.Rows[j][0] = "x" + simplex.task.basis[i-1][0];
                for (int i = simplex.basis_var, j = 1; i < simplex.task.basis.Length; i++, j++)
                    Table.Rows[old_y][j] = "x" + simplex.task.basis[i - 1][0];
                button2.Enabled = true;
            }

            dataGridView1.DataSource = Table;
        }
        private void Decor(int old_y)
        {
            int x = simplex.table[0].Length;
            int y = simplex.table.Length;

            for (int i = 0; i < x + 1; i++)
            {
                dataGridView1.Columns[i].Width = 50;
                dataGridView1.Columns[i].HeaderText = "";
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                for (int j = 0; j < Table.Rows.Count; j += y + 2)
                    dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.LightGray;
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
            if (textBox2.Text == "")
            {
                //Выделение опорный элементов
                int[] best = simplex.MaxReferenceElement(listEl);
                if (number_table == 0)
                {
                    dataGridView1.Rows[best[1] + 1].Cells[best[0] + 1].Style.BackColor = Color.GreenYellow;
                    for (int i = 0; i < listEl.Count; i++)
                    {
                        if ((listEl[i][0] != best[0] - 1) && (listEl[i][1] != best[1] - 1))
                            dataGridView1.Rows[listEl[i][1] + 1].Cells[listEl[i][0] + 1].Style.BackColor = Color.LightGreen;
                    }
                }
                else
                {
                    dataGridView1.Rows[best[1] + 1 + old_y].Cells[best[0] + 1].Style.BackColor = Color.GreenYellow;
                    for (int i = 0; i < listEl.Count; i++)
                    {
                        if ((listEl[i][0] != best[0] - 1) && (listEl[i][1] != best[1] - 1 - old_y))
                            dataGridView1.Rows[listEl[i][1] + 1 + old_y].Cells[listEl[i][0] + 1].Style.BackColor = Color.LightGreen;
                    }
                }
                
                
                
                
            }
            if (!step_by_step)
            {
                //Выделение опорный элементов
                for (int i = 0; i < AllListEl.Count; i++)
                    dataGridView1.Rows[AllListEl[i][1]].Cells[AllListEl[i][0]].Style.BackColor = Color.LightGreen;
                for (int i = 0; i < BestListEl.Count; i++)
                    dataGridView1.Rows[BestListEl[i][1]].Cells[BestListEl[i][0]].Style.BackColor = Color.GreenYellow;
            }
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.ReadOnly = true;
        }
        /// <summary>
        /// C искусственным базисом
        /// </summary>
        private void ArtificialBasis()
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int old_y = Table.Rows.Count - simplex.table.Length - 2;
            Decor(old_y);
        }
    }
}
