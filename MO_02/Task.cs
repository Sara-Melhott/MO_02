using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO_02
{
    public class Task
    {
        /// <summary>
        /// Кол-во неизвестных
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// Содержит коэфициенты функции
        /// </summary>
        public OrdinaryFraction[] function { get; set; }
        /// <summary>
        /// Содержит коэфициенты условий
        /// </summary>
        public OrdinaryFraction[][] condition;
        /// <summary>
        /// =, <=, >=
        /// </summary>
        public string[] sign;
        /// <summary>
        /// Описывает базисные переменный
        /// </summary>
        public int[][] basis;
        /// <summary>
        /// Поиск max или min<
        /// </summary>
        public string extr;

        public string answer;
        /// <summary>
        /// Задача для обыкновенных дробей
        /// </summary>
        /// <param name="size">Кол-во неизвестных</param>
        /// <param name="function_with_fractions">Коэфициенты функции</param>
        /// <param name="condition_with_fractions">Коэфициенты условий</param>
        /// <param name="basis">Базис</param>
        /// <param name="extr">Поиск max или min</param>
        public Task(int size, OrdinaryFraction[] function, OrdinaryFraction[][] condition, string[] sign, int[][] basis, string extr, string answer)
        {
            this.size = size;
            this.function = function;
            this.condition = condition;
            this.sign = sign;
            this.basis = basis;
            this.extr = extr;
            this.answer = answer;
            if (!Check())
            {
                this.answer = "Неправильный ввод данных!";
                return;
            }
            DDD();
        }
        /// <summary>
        /// Если в уравнении коэфициент b < 0, то данное уравнение домножается на -1
        /// </summary>
        public void DDD()
        {
            for (int i = 0; i < condition.Length; i++)
            {
                if (condition[i][condition[0].Length - 1].NegativeFraction())
                {
                    for (int j = 0; j < condition[i].Length; j++)
                    {
                        condition[i][j] = condition[i][j].Multiplication(new OrdinaryFraction(-1));
                    }
                    if (sign[i] == ">=")
                        sign[i] = "<=";
                    else if (sign[i] == "<=")
                        sign[i] = ">=";
                }
            }
        }

        /*public OrdinaryFraction[] AuxiliaryTask()
        {
            for (int j = condition.Length - 1; j > 0; j--)
            {
                OrdinaryFraction y = condition[j - 1][j];
                for (int i = 0; i < condition[j].Length; i++)
                {
                    OrdinaryFraction x = condition[j][i].Multiplication(y);
                    condition[j - 1][i] = condition[j - 1][i].Subtraction(x);
                }
            }
            
        }*/
        public OrdinaryFraction[] CopyArray(OrdinaryFraction[] array)
        {
            OrdinaryFraction[] copy = new OrdinaryFraction[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                copy[i] = array[i].Copy();
            }
            return copy;
        }
        public Task Clone()
        {
            OrdinaryFraction[] new_function = new OrdinaryFraction[function.Length];
            OrdinaryFraction[][] new_condition = new OrdinaryFraction[condition.Length][];
            //Копированеи условий
            for (int i = 0; i < new_condition.Length; i++)
            {
                OrdinaryFraction[] row_con = new OrdinaryFraction[condition[0].Length];
                for (int j = 0; j < condition[0].Length; j++)
                {
                    row_con[j] = condition[i][j].Copy();
                }
                new_condition[i] = row_con;
            }
            //Копирование функции
            for (int i = 0; i < function.Length; i++)
            {
                new_function[i] = function[i].Copy();
            }
            if (basis != null)
            {
                int[][] new_basis = new int[basis.Length][];
                for (int i = 0; i < basis.Length; i++)
                {
                    int[] row = new int[2];
                    for (int j = 0; j < 2; j++)
                    {
                        row[j] = basis[i][j];
                    }
                    new_basis[i] = row;
                }
                return new Task(size, new_function, new_condition, sign, new_basis, extr, answer);
            }
            else
                return new Task(size, new_function, new_condition, sign, null, extr, answer);
        }
        /// <summary>
        /// Функция, условия, знаки, экстремум должны быть не null.
        /// Размер базиса равень размеру функции.
        /// Размер всех условий одинаковый и равен размер функции + 1.
        /// Кол-во неизвестных (size) равно размеру функции. 
        /// Кол-во знаков равно кол-ву условий
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            if (function == null || condition == null || sign == null || extr == null)
                return false;
            if (condition.Length == 0)
                return false;
            if (basis != null)
                if ((basis.Length != 0) && (function != null))
                    if ((basis.Length != function.Length)&&(basis.Length != function.Length + condition.Length))
                        return false;
            int size_con = function.Length + 1;
            if (condition != null)
            {
                size_con = condition[0].Length;
                for (int i = 1; i < condition.Length; i++)
                {
                    if (condition[i] != null)
                    {
                        if (size_con != condition[i].Length)
                            return false;
                        for (int j = 0; j < condition[i].Length; j++)
                            if (condition[i][j].Error)
                                return false;
                    }
                    else
                        return false;

                }

            }
            for (int i = 0; i < function.Length; i++)
                if (function[i].Error)
                    return false;
            if (size_con != function.Length + 1)
                return false;
            if (sign.Length != condition.Length)
                return false;

            return true;
        }
        /// <summary>
        /// Печатает базис задачи
        /// </summary>
        /// <param name="ordinaryFraction"></param>
        /// <returns></returns>
        public string printBasis (bool ordinaryFraction)
        {
            int[][] new_basis = new int[basis.Length][];
            for (int i = 0; i < basis.Length; i++)
            {
                int[] row = new int[2];
                for (int j = 0; j < 2; j++)
                {
                    row[j] = basis[i][j];
                }
                new_basis[i] = row;
            }
            for (int i = 0; i < new_basis.Length; i++)
            {
                for (int j = i + 1; j < new_basis.Length; j++)
                {
                    if (new_basis[i][0] > new_basis[j][0])
                    {
                        int[] count = new_basis[j];
                        new_basis[j] = new_basis[i];
                        new_basis[i] = count;
                    }
                }
            }
            String taskWindow = "\r\nБазис:\r\nX = (";
            for (int i = 0; i < new_basis.Length; i++)
            {
                if (i == basis.Length - 1)
                    taskWindow += new_basis[i][1];
                else
                    taskWindow += new_basis[i][1] + ", ";
            }
            taskWindow += ")";
            return taskWindow;
        }
        /// <summary>
        /// Превращает условие задачи в строку
        /// </summary>
        /// <returns></returns>
        public string print(bool ordinaryFraction)
        {
            String taskWindow = "Условие задачи:\r\n";
            for (int i=0; i< function.Length; i++)
            {
                if (i == function.Length - 1)
                    taskWindow = taskWindow + function[i].Print(ordinaryFraction) + " * x" + (i + 1) + " -> ";
                else
                    taskWindow = taskWindow + function[i].Print(ordinaryFraction) + " * x" + (i + 1) + " + ";
            }
            taskWindow += extr + "\r\n\r\n";
            //Console.WriteLine("В ходе преобразований была получена следующая система:");
            for (int i = 0; i < condition.Length; i++)
            {
                for (int j = 0; j < condition[i].Length; j++)
                {
                    if (j == condition[i].Length - 2)
                        taskWindow = taskWindow + condition[i][j].Print(ordinaryFraction) + " * x" + (j + 1) +" "+sign[i]+" ";
                    else if (j == condition[i].Length - 1)
                        taskWindow = taskWindow + condition[i][j].Print(ordinaryFraction);
                    else
                        taskWindow = taskWindow + condition[i][j].Print(ordinaryFraction) + " * x" + (j + 1) + " + ";
                }
                taskWindow = taskWindow + "\r\n";
            }
            if (basis != null)
                taskWindow += printBasis(ordinaryFraction);
            return taskWindow;
        }
    }
}
