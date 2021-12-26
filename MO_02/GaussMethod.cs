using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO_02
{
    /// <summary>
    /// Метод Гаусса
    /// </summary>
    class GaussMethod
    {
        Task task;
        bool error = false; //Если true - то система несовместна
        OrdinaryFraction[] fun_one;
        OrdinaryFraction[] fun_two;
        /// <summary>
        /// Для 2х уравнений
        /// </summary>
        /// <param name="fun_one"></param>
        /// <param name="fun_two"></param>
        public GaussMethod(OrdinaryFraction[] fun_one, OrdinaryFraction[] fun_two)
        {
            this.fun_one = fun_one;
            this.fun_two = fun_two;
        }
        /// <summary>
        /// Поиск точек пересечения прямых
        /// </summary>
        /// <returns></returns>
        public List<Point> MainFindPoit()
        {
            Task task = AddBorders();
            List<Point> points = new List<Point>();
            for (int i = 0; i < task.condition.Length; i++)
            {
                for (int j = i + 1; j < task.condition.Length; j++)
                {
                    fun_one = task.CopyArray(task.condition[i]);
                    fun_two = task.CopyArray(task.condition[j]);
                    fun_one = MaxDelOneAndTwoFun(fun_one, 0);
                    fun_two = MaxDelOneAndTwoFun(fun_two, 0);
                    fun_two = SubstractTwoRows(fun_one, fun_two, 0);
                    fun_two = MaxDelOneAndTwoFun(fun_two, 1);
                    fun_one = SubstractTwoRows(fun_two, fun_one, 1);
                    if (!RowNULL(fun_one))
                    {
                        points.Add(new Point(fun_one[fun_one.Length - 1], fun_two[fun_two.Length - 1]));
                    }
                }
            }
            return points;
        }
        /// <summary>
        /// Добавляет границы рамки 
        /// </summary>
        /// <returns></returns>
        public Task AddBorders()
        {
            OrdinaryFraction[][] borders = new OrdinaryFraction[2][];
            borders[0] = new OrdinaryFraction[] { new OrdinaryFraction(1), new OrdinaryFraction(0), new OrdinaryFraction(336) };
            borders[1] = new OrdinaryFraction[] { new OrdinaryFraction(0), new OrdinaryFraction(1), new OrdinaryFraction(339) };
            Task new_task = task.Clone();
            OrdinaryFraction[][] new_conditions = new OrdinaryFraction[task.condition.Length + 2][];
            for (int i = 0; i < task.condition.Length; i++)
                new_conditions[i] = task.CopyArray(task.condition[i]);
            new_conditions[new_conditions.Length - 2] = borders[0];
            new_conditions[new_conditions.Length - 1] = borders[1];
            new_task.condition = new_conditions;

            string[] new_sign = new string[task.sign.Length + 2];
            for (int i = 0; i < task.sign.Length; i++)
                new_sign[i] = task.sign[i];
            new_sign[new_sign.Length - 2] = "<=";
            new_sign[new_sign.Length - 1] = "<=";
            new_task.sign = new_sign;

            return new_task;
        }
        /// <summary>
        /// Деление коэфициентов функции fun на коэфициент в столбце number_col 
        /// </summary>
        public OrdinaryFraction[] MaxDelOneAndTwoFun(OrdinaryFraction[] fun, int number_col)
        {
            OrdinaryFraction del = fun[number_col].Copy();
            if (!del.Equally(new OrdinaryFraction(0)))
            {
                for (int i = number_col; i < fun.Length; i++)
                    if (!fun[i].Equally(new OrdinaryFraction(0)))
                        fun[i] = fun[i].Division(del);
            }
            return fun;
        }
        /// <summary>
        /// Разность двух уравнений
        /// </summary>
        /// <param name="fun_one"></param>
        /// <param name="fun_two"></param>
        /// <param name="temp"></param>
        /// <returns></returns>
        public OrdinaryFraction[] SubstractTwoRows(OrdinaryFraction[] fun_one, OrdinaryFraction[] fun_two, int temp)
        {
            OrdinaryFraction x = fun_two[temp];
            for (int i = 0; i < fun_one.Length; i++)
            {
                fun_two[i] = fun_two[i].Subtraction(fun_one[i].Multiplication(x));
            }

            return fun_two;
        }
        /// <summary>
        /// Возвращает true если прямые параллельны
        /// </summary>
        public bool MiniGausseMethod()
        {
            fun_one = MaxDelOneAndTwoFun(fun_one, 0);
            fun_two = MaxDelOneAndTwoFun(fun_two, 0);
            fun_two = SubstractTwoRows(fun_one, fun_two, 0);
            if (RowNULL(fun_two))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Для Симплекс метода
        /// </summary>
        /// <param name="task"></param>
        public GaussMethod(Task task)
        {
            this.task = task;
        }
        /// <summary>
        /// Главный метод для решения задачи методом гаусса
        /// </summary>
        /// <returns></returns>
        public Task MainGaussMethod()
        {
            if (task.answer == null)
            {
                //Console.WriteLine("Изначально:");
                //print();
                SortRowsForBasis();
                Sort(0);
                //Console.WriteLine("После сортировки: ");
                //print();
                for (int i = 0; i < task.condition.Length; i++)
                {
                    MaxDel(i);
                    if (task.condition != null)
                        SubtractionRows(i);
                    else
                        return null;//???
                    //Console.WriteLine("Преобразование шаг " + (i + 1));
                    //print();
                    Sort(i);
                }
                //Array.Reverse(task.condition);
                DeleteRowNULL();
                ExpressionBasicVar();
                if (error)
                    task.answer = "Система условий задачи несовместна";
                //Console.WriteLine("Ответ: ");
                //print();
                //string name = Console.ReadLine();
            }
            return task;
        }
        /// <summary>
        /// Выражает базисные переменные
        /// </summary>
        public void ExpressionBasicVar()
        {
            for (int j = task.condition.Length - 1; j > 0; j--)
            {
                OrdinaryFraction y = task.condition[j - 1][j];
                for (int i = 0; i < task.condition[j].Length; i++)
                {
                    OrdinaryFraction x = task.condition[j][i].Multiplication(y);
                    task.condition[j - 1][i] = task.condition[j - 1][i].Subtraction(x);
                }
            }

        }
        /// <summary>
        /// Переставляет строки для определения базисных переменных 
        /// </summary>
        public void SortRowsForBasis()
        {
            for (int i = 0; i < task.basis.Length; i++)
            {
                for (int j = i + 1; j < task.basis.Length; j++)
                {
                    if (task.basis[i][1] < task.basis[j][1])
                    {
                        int[] count = task.basis[j];
                        task.basis[j] = task.basis[i];
                        task.basis[i] = count;

                        OrdinaryFraction[] cout = new OrdinaryFraction[task.condition.Length];
                        for (int k = 0; k < task.condition.Length; k++)
                        {
                            OrdinaryFraction t = task.condition[k][i];
                            task.condition[k][i] = task.condition[k][j];
                            task.condition[k][j] = t;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Сортировка условий по убыванию значения столбца temp
        /// </summary>
        /// <param name="temp"></param>
        public void Sort(int temp)
        {
            for (int i = task.condition.Length - 1; i > 0; i--)
            {
                for (int j = temp; j < i; j++)
                {
                    if (task.condition[j][temp].Min(task.condition[j + 1][temp]))
                    {
                        OrdinaryFraction[] cout = task.condition[j];
                        task.condition[j] = task.condition[j + 1];
                        task.condition[j + 1] = cout;
                    }
                }
            }
        }
        /// <summary>
        /// Делит строку на максимальный коэфциент в столбце temp
        /// </summary>
        /// <param name="temp"></param>
        public void MaxDel(int temp)
        {
            OrdinaryFraction x = new OrdinaryFraction(0, 1);
            for (int i = temp; i < task.condition.Length; i++)
            {
                OrdinaryFraction max = task.condition[i][temp];
                for (int j = 0; j < task.condition[i].Length; j++)
                {
                    if (!task.condition[i][j].Equally(x) && !max.Equally(x))
                    {
                        task.condition[i][j] = task.condition[i][j].Division(max);
                    }
                }
            }
        }
        /// <summary>
        /// Вычитание строк
        /// </summary>
        /// <param name="temp"></param>
        public void SubtractionRows(int temp)
        {
            for (int i = temp + 1; i < task.condition.Length; i++)
            {
                if (!task.condition[i][temp].Equally(new OrdinaryFraction(0, 1)))
                {
                    for (int j = 0; j < task.condition[i].Length; j++)
                        task.condition[i][j] = task.condition[i][j].Subtraction(task.condition[temp][j]);
                }
            }
        }
        /// <summary>
        /// 0*x = 0
        /// </summary>
        /// <param name="row"></param>
        /// <returns>true, если строка нулевая</returns>
        public bool RowNULL(OrdinaryFraction[] row)
        {
            OrdinaryFraction x = new OrdinaryFraction(0, 1);
            int count = 0;
            for (int i = 0; i < row.Length - 1; i++)
            {
                if (row[i].Equally(x))
                    count++;
            }
            if (count == row.Length - 1)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 0*х = constant
        /// </summary>
        /// <param name="row"></param>
        /// <returns>true, если строка ошибочна</returns>
        public bool Error(OrdinaryFraction[] row)
        {
            if (row[row.Length - 1].Equally(new OrdinaryFraction(0, 1)))
                return false;
            else
                return true;
        }
        /// <summary>
        /// Удаляет нулевые строки
        /// </summary>
        public void DeleteRowNULL()
        {
            int count = 0;

            while (RowNULL(task.condition[task.condition.Length - count - 1]))
            {
                if (Error(task.condition[task.condition.Length - count - 1]))
                    error = true;
                count++;
                if (task.condition.Length - count - 1 < 0)
                    break;
            }
            Array.Resize(ref task.condition, task.condition.Length - count);
        }
        /// <summary>
        /// Печатает уравнения полученные в ходе преобразований
        /// </summary>
        /// <param name="constans"></param>
        /*public void print()
        {
            if (error || (task.condition.Length == 0))
            {
                Console.WriteLine("Система несовместна.");
                return;
            }
            //Console.WriteLine("В ходе преобразований была получена следующая система:");
            for (int i = 0; i < task.condition.Length; i++)
            {
                for (int j = 0; j < task.condition[i].Length; j++)
                {
                    if (j == task.condition[i].Length - 2)
                        Console.Write(task.condition[i][j].Print() + " * x" + (j + 1) + " = ");
                    else if (j == task.condition[i].Length - 1)
                        Console.Write(task.condition[i][j].Print());
                    else
                        Console.Write(task.condition[i][j].Print() + " * x" + (j + 1) + " + ");
                }
                Console.Write("\n");
            }
        }*/
    }
}
