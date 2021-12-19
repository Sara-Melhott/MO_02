using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO_02
{
    class Point
    {
        public OrdinaryFraction x;
        public OrdinaryFraction y;
        /// <summary>
        /// Описывает координаты точки на графике
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(OrdinaryFraction x, OrdinaryFraction y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// Возвращает true, если точки совпадают
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equally(Point other)
        {
            if (x.Equally(other.x) && y.Equally(other.y))
                return true;
            else
                return false;
        }
    }
    class GraphicalMethod
    {
        public Task task;
        public List<Point> points;
        public GraphicalMethod(Task task)
        {
            this.task = task;
        }
        public string MainGraphicalMethod(bool ordinaryFraction)
        {
            if (task.function.Length == 2)
            {
                GaussMethod gaussMethod = new GaussMethod(task.Clone());
                points = gaussMethod.MainFindPoit();
                Check(task);
                if (task.answer == null)
                {
                    List<Point> answer_point = FindAnswer(task);
                    answer_point = UniqPoints(answer_point);
                    if (answer_point.Count == 0)
                    {
                        task.answer = "Решения нет";
                    }
                    else if (answer_point.Count == 1)
                    {
                        int i = FindLines(task, answer_point[0]);
                        GaussMethod method = new GaussMethod(task.CopyArray(task.function), task.CopyArray(task.condition[i]));
                        if (method.MiniGausseMethod())
                        {
                            task.answer = "Луч с началом в точке А = (" + answer_point[0].x.Print(ordinaryFraction) + ", " + answer_point[0].y.Print(ordinaryFraction) + ")";
                        }
                        else
                        {
                            task.answer = "Точка А = (" + answer_point[0].x.Print(ordinaryFraction) + ", " + answer_point[0].y.Print(ordinaryFraction) + ")";
                        }
                    }
                    else if (answer_point.Count > 1)
                    {
                        task.answer = "Отрезок AB: A = (" + answer_point[0].x.Print(ordinaryFraction) + ", " + answer_point[0].y.Print(ordinaryFraction) + ") B = ("
                            + answer_point[1].x.Print(ordinaryFraction) + ", " + answer_point[1].y.Print(ordinaryFraction) + ")";
                    }
                }
            }
            else
            {
                int count = 0;
                for (int i = 0; i < task.sign.Length; i++)
                    if (task.sign[i] == "=")
                        count++;
                if (task.function.Length - (task.condition.Length - task.function.Length) > 2)
                    task.answer = "Графического двумерного решения не существует";
                else
                {
                    //Преобазование задачи для использования Метода Гаусса
                    Task new_task = TaskСonversionForGaussMethod();

                    //Преобразование с помощью метода Гаусса
                    GaussMethod gaussMethod = new GaussMethod(new_task.Clone());
                    new_task = gaussMethod.MainGaussMethod();

                    //Убираем из уравнений базисные переменные и приводим к двумерному виду 
                    TaskConversionIntoTwoDimensionalRepresentation(new_task);

                    //Поиск решения в двумерной задаче
                    GaussMethod gaussMethod2 = new GaussMethod(new_task.Clone());
                    points = gaussMethod2.MainFindPoit();
                    Check(new_task);
                    if (task.answer == null)
                    {
                        List<Point> answer_point = FindAnswer(new_task);
                        answer_point = UniqPoints(answer_point);
                        if (answer_point.Count == 0)
                        {
                            task.answer = "Решения нет";
                        }
                        else if (answer_point.Count == 1)
                        {
                            OrdinaryFraction[] var = new OrdinaryFraction[task.function.Length];
                            for (int j = 0; j < var.Length; j++)
                            {
                                if (new_task.basis[j][1] == 1)
                                    var[new_task.basis[j][0] - 1] = new_task.condition[j][2].Subtraction(new_task.condition[j][0].Multiplication(answer_point[0].x).Addition(new_task.condition[j][1].Multiplication(answer_point[0].y)));
                                else if (new_task.basis[j][1] == 0)
                                {
                                    var[new_task.basis[j][0] - 1] = answer_point[0].x;
                                    var[new_task.basis[j + 1][0] - 1] = answer_point[0].y;
                                    break;
                                }
                            }

                            int i = FindLines(new_task, answer_point[0]);
                            GaussMethod method = new GaussMethod(new_task.CopyArray(new_task.function), new_task.CopyArray(new_task.condition[i]));
                            if (method.MiniGausseMethod())
                            {
                                task.answer = "Луч с началом в точке А = (";
                                for (int j = 0; j < var.Length - 1; j++)
                                {
                                    task.answer = task.answer + var[j].Print(ordinaryFraction) + ", ";
                                }
                                task.answer = task.answer + var[var.Length - 1].Print(ordinaryFraction) + ")";
                            }
                            else
                            {
                                task.answer = "Точка А = (";
                                for (int j = 0; j < var.Length - 1; j++)
                                {
                                    task.answer = task.answer + var[j].Print(ordinaryFraction) + ", ";
                                }
                                task.answer = task.answer + var[var.Length - 1].Print(ordinaryFraction) + ")";
                            }
                        }
                        else if (answer_point.Count > 1)
                        {
                            OrdinaryFraction[] var1 = new OrdinaryFraction[task.function.Length];
                            OrdinaryFraction[] var2 = new OrdinaryFraction[task.function.Length];
                            for (int j = 0; j < var1.Length; j++)
                            {
                                if (task.basis[j][1] == 1)
                                {
                                    var1[task.basis[j][0] - 1] = new_task.condition[j][2].Subtraction(new_task.condition[j][0].Multiplication(answer_point[0].x).Addition(new_task.condition[j][1].Multiplication(answer_point[0].y)));
                                    var2[task.basis[j][0] - 1] = new_task.condition[j][2].Subtraction(new_task.condition[j][0].Multiplication(answer_point[1].x).Addition(new_task.condition[j][1].Multiplication(answer_point[1].y)));
                                }
                                else if (task.basis[j][1] == 0)
                                {
                                    var1[task.basis[j][0] - 1] = answer_point[0].x;
                                    var2[task.basis[j][0] - 1] = answer_point[1].x;
                                    var1[task.basis[j + 1][0] - 1] = answer_point[0].y;
                                    var2[task.basis[j + 1][0] - 1] = answer_point[1].y;
                                    break;
                                }
                            }
                            task.answer = "Отрезок AB: A = (";
                            for (int j = 0; j < var1.Length - 1; j++)
                            {
                                task.answer = task.answer + var1[j].Print(ordinaryFraction) + ", ";
                            }
                            task.answer = task.answer + var1[var1.Length - 1].Print(ordinaryFraction) + ") B = (";
                            for (int j = 0; j < var2.Length - 1; j++)
                            {
                                task.answer = task.answer + var2[j].Print(ordinaryFraction) + ", ";
                            }
                            task.answer = task.answer + var2[var2.Length - 1].Print(ordinaryFraction) + ")";
                        }
                    }
                }
            }
            return task.answer;
        }
        /// <summary>
        /// Преобразование задачи к двумерному виду
        /// </summary>
        /// <param name="new_task"></param>
        public void TaskConversionIntoTwoDimensionalRepresentation(Task new_task)
        {
            OrdinaryFraction[][] new_condition = new OrdinaryFraction[task.function.Length - 2][];
            SimplexTable simplexTable = new SimplexTable(new_task);
            OrdinaryFraction[][] table = simplexTable.tableCopy();
            if (new_task.extr == "max")
                for (int i = 0; i < table[0].Length; i++)
                    table[table.Length - 1][i] = table[table.Length - 1][i].Multiplication(new OrdinaryFraction(-1));
            //new_condition[new_condition.Length - 1] = new_task.CopyArray(table[table.Length - 1]);
            new_task.function = table[table.Length - 1];
            new_condition = new OrdinaryFraction[new_task.condition.Length + 2][];
            for (int i = 0; i < new_task.condition.Length; i++)
                new_condition[i] = new_task.CopyArray(table[i]);

            OrdinaryFraction temp1 = new OrdinaryFraction(0);
            OrdinaryFraction temp2 = new OrdinaryFraction(1);
            new_condition[new_condition.Length - 2] = new OrdinaryFraction[] { temp2, temp1, temp1 };
            new_condition[new_condition.Length - 1] = new OrdinaryFraction[] { temp1, temp2, temp1 };
            new_task.condition = new_condition;
        }
        /// <summary>
        /// Меняет вид задачи для применения метода Гаусса
        /// </summary>
        /// <returns></returns>
        public Task TaskСonversionForGaussMethod()
        {
            OrdinaryFraction[][] new_condition = new OrdinaryFraction[task.function.Length - 2][];
            for (int i = 0, j = 0; i < task.sign.Length; i++)
                if (task.sign[i] == "=")
                {
                    new_condition[j++] = task.condition[i];
                    task.sign[i] = "<=";
                }
            string[] new_sign = new string[task.sign.Length - task.function.Length + 2];
            for (int i = 0; i < new_sign.Length - 2; i++)
                new_sign[i] = task.sign[i];
            new_sign[new_sign.Length - 2] = ">=";
            new_sign[new_sign.Length - 1] = ">=";
            return new Task(task.size, task.function, new_condition, new_sign, task.basis, task.extr, task.answer);
        }
        /// <summary>
        /// Ищет прямую, которой принадлежит точка point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int FindLines(Task task, Point point)
        {
            int i = 0;
            while (!BoolCheck(task, i, point))
                i++;
            return i; ;
        }
        /// <summary>
        /// Ищет точку/точки который подходят для ответа
        /// </summary>
        /// <returns></returns>
        public List<Point> FindAnswer(Task task)
        {
            List<Point> new_points = new List<Point>();
            if (task.extr == "max")
            {
                Point max = points[0];
                OrdinaryFraction temp1 = max.x.Multiplication(task.function[0]);
                OrdinaryFraction temp2 = max.y.Multiplication(task.function[1]);
                OrdinaryFraction max_fun = temp1.Addition(temp2);

                for (int i = 0; i < points.Count; i++)
                {
                    OrdinaryFraction temp10 = points[i].x.Multiplication(task.function[0]);
                    OrdinaryFraction temp20 = points[i].y.Multiplication(task.function[1]);
                    OrdinaryFraction max_fun2 = temp10.Addition(temp20);
                    if (max_fun.Min(max_fun2))
                    {
                        max = points[i];
                        max_fun = max_fun2;
                    }
                }

                for (int i = 0; i < points.Count; i++)
                {
                    OrdinaryFraction temp10 = points[i].x.Multiplication(task.function[0]);
                    OrdinaryFraction temp20 = points[i].y.Multiplication(task.function[1]);
                    OrdinaryFraction max_fun2 = temp10.Addition(temp20);
                    if (max_fun.Equally(max_fun2))
                    {
                        new_points.Add(points[i]);
                    }
                }
            }
            else if (task.extr == "min")
            {
                Point min = points[0];
                OrdinaryFraction temp1 = min.x.Multiplication(task.function[0]);
                OrdinaryFraction temp2 = min.y.Multiplication(task.function[1]);
                OrdinaryFraction min_fun = temp1.Addition(temp2);

                for (int i = 0; i < points.Count; i++)
                {
                    OrdinaryFraction temp10 = points[i].x.Multiplication(task.function[0]);
                    OrdinaryFraction temp20 = points[i].y.Multiplication(task.function[1]);
                    OrdinaryFraction min_fun2 = temp10.Addition(temp20);
                    if (min_fun.Max(min_fun2))
                    {
                        min = points[i];
                        min_fun = min_fun2;
                    }
                }

                for (int i = 0; i < points.Count; i++)
                {
                    OrdinaryFraction temp10 = points[i].x.Multiplication(task.function[0]);
                    OrdinaryFraction temp20 = points[i].y.Multiplication(task.function[1]);
                    OrdinaryFraction min_fun2 = temp10.Addition(temp20);
                    if (min_fun.Equally(min_fun2))
                    {
                        new_points.Add(points[i]);
                    }
                }
            }
            return new_points;
        }
        /// <summary>
        /// Удаление тех точек, которые не лежат в области решений
        /// </summary>
        public void Check(Task task)
        {
            for (int j = 0; j < task.condition.Length; j++)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (!BoolCheck(task, j, points[i]))
                    {
                        points.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /// <summary>
        /// Возвращает true, если точка принадлежит области решения 
        /// </summary>
        /// <param name="number_con"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool BoolCheck(Task task, int number_con, Point point)
        {
            OrdinaryFraction temp1 = point.x.Multiplication(task.condition[number_con][0]);
            OrdinaryFraction temp2 = point.y.Multiplication(task.condition[number_con][1]);
            OrdinaryFraction temp3 = temp1.Addition(temp2);
            if (task.sign[number_con] == ">=")
            {

                if (temp3.Max(task.condition[number_con][2]) || temp3.Equally(task.condition[number_con][2]))
                    return true;
                else
                    return false;
            }
            else if (task.sign[number_con] == "<=")
            {
                if (temp3.Min(task.condition[number_con][2]) || temp3.Equally(task.condition[number_con][2]))
                    return true;
                else
                    return false;
            }
            else
            {
                task.answer = "Неправильный ввод данных (знак условия)";
                return false;
            }
        }
        /// <summary>
        /// Удаляет одинаковые точки
        /// </summary>
        public List<Point> UniqPoints(List<Point> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                    if (points[i].Equally(points[j]))
                    {
                        points.RemoveAt(j);
                        j--;
                    }
            }
            return points;
        }
    }
}
