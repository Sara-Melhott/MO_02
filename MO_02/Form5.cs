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
    public partial class Form5 : Form
    {
        Task task;
        GraphicalMethod graphicalMethod;
        bool ordinaryFraction;
        List<Point> points;
        public Form5(Task task, bool ordinaryFraction)
        {
            this.task = task;
            this.ordinaryFraction = ordinaryFraction;
            graphicalMethod = new GraphicalMethod(task.Clone());
            InitializeComponent();
            textBox2.ScrollBars = ScrollBars.Vertical;
            textBox2.Multiline = true;
            textBox2.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Multiline = true;
            textBox1.ReadOnly = true;
            textBox1.Text = task.print(ordinaryFraction);
            //Main();
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
        //Сортирует точки для удобного рисования графика
        private void SortPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].x.Min(points[j].x))
                    {
                        Point p = new Point (points[i].x.Copy(), points[i].y.Copy());
                        points[i] = new Point(points[j].x.Copy(), points[j].y.Copy());
                        points[j] = p;
                    }
                    else if (points[i].x.Equally(points[j].x))
                    {
                        if (points[i].y.Min(points[j].y))
                        {
                            Point p = new Point(points[i].x.Copy(), points[i].y.Copy());
                            points[i] = new Point(points[j].x.Copy(), points[j].y.Copy());
                            points[j] = p;
                        }
                    }
                }
            }
        }
        private void Main()
        {
            string answer = graphicalMethod.MainGraphicalMethod(ordinaryFraction);
            textBox2.Text = answer;
            points = graphicalMethod.points;

            Graphics graphics = pictureBox1.CreateGraphics();
            int x = pictureBox1.Width;
            int y = pictureBox1.Height;
            Pen pen = new Pen(Color.Black, 2f);
            Pen pen2 = new Pen(Color.Red, 2f);
            Pen pen3 = new Pen(Color.Yellow, 4f);
            SolidBrush br = new SolidBrush(Color.Green);
            SolidBrush br2 = new SolidBrush(Color.Yellow);

            //Оси 
            graphics.DrawLine(pen, new System.Drawing.Point(10, 0), new System.Drawing.Point(10, y));
            graphics.DrawLine(pen, new System.Drawing.Point(0, y - 10), new System.Drawing.Point(x, y - 10));

            //Получившаяся область 
            System.Drawing.Point[] polygon = new System.Drawing.Point[points.Count];
            SortPoints();
            for (int i=0; i<points.Count; i++)
            {

            }
            polygon[0] = new System.Drawing.Point(10, y - 10);
            polygon[1] = new System.Drawing.Point(10, y - 50);
            polygon[2] = new System.Drawing.Point(35, y - 50);
            polygon[3] = new System.Drawing.Point(50, y - 35);
            polygon[4] = new System.Drawing.Point(50, y - 10);

            graphics.DrawPolygon(pen, polygon);
            graphics.FillPolygon(br, polygon);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main();
        }
    }
}
