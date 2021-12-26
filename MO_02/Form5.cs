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
        private void SortPoints(bool temp)
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
                        if (temp)
                        {
                            if (points[i].y.Max(points[j].y))
                            {
                                Point p = new Point(points[i].x.Copy(), points[i].y.Copy());
                                points[i] = new Point(points[j].x.Copy(), points[j].y.Copy());
                                points[j] = p;
                            }
                        }
                        else
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
            Pen pen4 = new Pen(Color.Green, 2f);
            Pen pen5 = new Pen(Color.Orange, 3f);
            SolidBrush br = new SolidBrush(Color.Green);
            SolidBrush br2 = new SolidBrush(Color.Yellow);

            //Оси 
            graphics.DrawLine(pen, new System.Drawing.Point(20, 0), new System.Drawing.Point(20, y));
            graphics.DrawLine(pen, new System.Drawing.Point(0, y - 20), new System.Drawing.Point(x, y - 20));
            string axis_x = "x1";
            string axis_y = "x2";
            //Для вектора градиента
            int n1 = 1;
            int n2 = 2;
            if (task.basis != null)
            for (int i = 0, count = 0; count < 2; i++)
            {
                if (task.basis[i][1] == 0)
                {
                    count++;
                    if (count == 1)
                    {
                        axis_x = "x" + task.basis[i][0];
                        n1 = task.basis[i][0];
                    }
                    else
                    {
                        axis_y = "x" + task.basis[i][0];
                        n2 = task.basis[i][0];
                    }
                        
                }
            }
            graphics.DrawString(axis_x, new Font(Font.FontFamily, 8) , new SolidBrush(Color.Black), x - 20, y - 18);
            graphics.DrawString(axis_y, new Font(Font.FontFamily, 8), new SolidBrush(Color.Black), 3, 5);

            //Получившаяся область 
            System.Drawing.Point[] polygon = new System.Drawing.Point[points.Count];
            SortPoints(true);
            for (int i=0; i<points.Count; i++)
            {
                int x0 = (int)(Math.Round(points[i].x.toDecimalFraction(), 1)*20);
                int y0 = (int)(Math.Round(points[i].y.toDecimalFraction(), 1)*20);

                polygon[i] = new System.Drawing.Point(x0 + 20, y - y0 - 20);
            }
            graphics.DrawPolygon(pen4, polygon);
            graphics.FillPolygon(br, polygon);

            SortPoints(false);
            for (int i = 0; i < points.Count; i++)
            {
                int x0 = (int)(Math.Round(points[i].x.toDecimalFraction(), 1) * 20);
                int y0 = (int)(Math.Round(points[i].y.toDecimalFraction(), 1) * 20);

                polygon[i] = new System.Drawing.Point(x0 + 20, y - y0 - 20);
            }
            graphics.DrawPolygon(pen4, polygon);
            graphics.FillPolygon(br, polygon);

            //Рисуем ответ
            if (graphicalMethod.answer_point.Count == 2)
            {
                //Отрезок
                int x0 = (int)(Math.Round(graphicalMethod.answer_point[0].x.toDecimalFraction(), 1) * 20);
                int y0 = (int)(Math.Round(graphicalMethod.answer_point[0].y.toDecimalFraction(), 1) * 20);
                int x1 = (int)(Math.Round(graphicalMethod.answer_point[1].x.toDecimalFraction(), 1) * 20);
                int y1 = (int)(Math.Round(graphicalMethod.answer_point[1].y.toDecimalFraction(), 1) * 20);

                graphics.DrawLine(pen3, x0 + 20, y - y0 - 20, x1 + 20, y - y1 - 20);
                string answer_point = "(" + graphicalMethod.answer_point[0].x.toDecimalFraction() + ", " + graphicalMethod.answer_point[0].y.toDecimalFraction() + ")";
                graphics.DrawString(answer_point, new Font(Font.FontFamily, 8), new SolidBrush(Color.Red), x0 + 20, y - y0 - 20);
                answer_point = "(" + graphicalMethod.answer_point[1].x.toDecimalFraction() + ", " + graphicalMethod.answer_point[1].y.toDecimalFraction() + ")";
                graphics.DrawString(answer_point, new Font(Font.FontFamily, 8), new SolidBrush(Color.Red), x1 + 20, y - y1 - 20);
            }
            else if ((graphicalMethod.answer_point.Count == 1) && graphicalMethod.line)
            {
                //Луч

            }
            else if ((graphicalMethod.answer_point.Count == 1) && !graphicalMethod.line)
            {
                //Точка
                int x0 = (int)(Math.Round(graphicalMethod.answer_point[0].x.toDecimalFraction(), 1) * 20);
                int y0 = (int)(Math.Round(graphicalMethod.answer_point[0].y.toDecimalFraction(), 1) * 20);

                graphics.FillEllipse(br2, x0 - 4 + 20, y - y0 - 2 - 20, 6, 6);
                string answer_point = "(" + graphicalMethod.answer_point[0].x.toDecimalFraction() + ", " + graphicalMethod.answer_point[0].y.toDecimalFraction() + ")";
                graphics.DrawString(answer_point, new Font(Font.FontFamily, 8), new SolidBrush(Color.Red), x0 + 20, y - y0 - 20);
            }

            //Рисуем вектор градиент
            n1 = (int)(Math.Round(graphicalMethod.new_task.function[0].toDecimalFraction(), 1) * 20);
            n2 = (int)(Math.Round(graphicalMethod.new_task.function[1].toDecimalFraction(), 1) * 20);
            graphics.DrawLine(pen5, 20, y-20, n1 + 20, y - n2 - 20);

            /*polygon[0] = new System.Drawing.Point(10, y - 10); точка (0,0)
            polygon[1] = new System.Drawing.Point(10, y - 50);
            polygon[2] = new System.Drawing.Point(35, y - 50);
            polygon[3] = new System.Drawing.Point(50, y - 35);
            polygon[4] = new System.Drawing.Point(50, y - 10);*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Main();
            }
            catch
            {
                MessageBox.Show("Эту задачу мы решить не сможем =(");
            }
        }
    }
}
