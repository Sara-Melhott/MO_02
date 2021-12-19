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
    public partial class Form1 : Form
    {
        DataTable Data = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.Rows.Clear();
            Data.Columns.Clear();
            int x = 0;
            int y = 0;
            try
            {
                //Инициализация таблицы
                x = int.Parse(number_x.Text);
                y = int.Parse(number_condition.Text);
                if (x < 1 || y < 1)
                {
                    MessageBox.Show("Неправильное значение количества неизвестных или количества условий");
                    return;
                }
                for (int i = 0; i < x + 2; i++) 
                    Data.Columns.Add();
                for (int i = 0; i < y + 3; i++)
                    Data.Rows.Add();
                //Декоративные записи
                for (int i = 1; i < x+1; i++)
                {
                    Data.Rows[0][i] = "x" + i;
                    Data.Rows[2][i] = "x" + i;
                }
                Data.Rows[2][x+1] = "b";
                for (int i = 3; i < y + 3; i++)
                {
                    Data.Rows[i][0] = "f" + (i - 2);
                }
                Data.Rows[1][0] = "f0";
                dataGridView1.DataSource = Data;
                dataGridView1.Rows[0].ReadOnly = true;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Rows[2].ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.AllowUserToResizeColumns = false;
                for (int i = 0; i < x + 2; i++)
                {
                    dataGridView1.Columns[i].HeaderText = "";
                    dataGridView1.Columns[i].Width = 50;
                    dataGridView1.Columns[i].Resizable = DataGridViewTriState.False;
                    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dataGridView1.Rows[i].Cells[0].Style.BackColor = Color.Red;
                }
                    
                //data_conditions.ItemsSource = Data;
            }
            catch
            {
                MessageBox.Show("Неправильное значение количества неизвестных или количества условий");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics graphics = pictureBox1.CreateGraphics();
            int x = pictureBox1.Width;
            int y = pictureBox1.Height;
            Pen pen = new Pen(Color.Black, 3f);
            Pen pen2 = new Pen(Color.Red, 2f);
            Pen pen3 = new Pen(Color.Yellow, 4f);

            //Оси 
            graphics.DrawLine(pen2, new System.Drawing.Point(10,0), new System.Drawing.Point(10,y));
            graphics.DrawLine(pen2, new System.Drawing.Point(0,y-10), new System.Drawing.Point(x,y-10));
            SolidBrush br = new SolidBrush(Color.Green);
            SolidBrush br2 = new SolidBrush(Color.Yellow);

            System.Drawing.Point[] polygon = new System.Drawing.Point[5];
            polygon[0] = new System.Drawing.Point(10,y - 10);
            polygon[1] = new System.Drawing.Point(10, y - 50);
            polygon[2] = new System.Drawing.Point(35, y - 50);
            polygon[3] = new System.Drawing.Point(50, y - 35);
            polygon[4] = new System.Drawing.Point(50, y - 10);
            
            graphics.DrawPolygon(pen, polygon);
            graphics.FillPolygon(br, polygon);

            //Точки пересечения прямых
            graphics.FillEllipse(br2, 33, y - 52, 5, 5);
            graphics.FillEllipse(br2, 8, y - 12, 5, 5); //x = 10 y = y-10

            System.Drawing.Point[] points = new System.Drawing.Point[100];
            for (int i = 0, j = points.Length - 1; i<points.Length; i++, j--)
            {
                points[i] = new System.Drawing.Point(i, j);

            }
            //graphics.DrawLines(pen, points);
            //graphics.DrawLine(pen, new Point(0,y), new Point(x,0));
        }
    }
}
