using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO_02
{
    /// <summary>
    /// Симплекс таблицы
    /// </summary>
    class SimplexTable
    {
        /// <summary>
        /// Сиплекс-таблица
        /// </summary>
        public OrdinaryFraction[][] table;
        /// <summary>
        /// Количество базисных переменных
        /// </summary>
        public int basis_var;
        /// <summary>
        /// Количество свободных переменных
        /// </summary>
        public int free_var;
        /// <summary>
        /// Условия задачи
        /// </summary>
        public Task task;
        /// <summary>
        /// Хранит все сиплекс-таблицы (для пошагового режима)
        /// </summary>
        public Stack<ArrayList> stack = new Stack<ArrayList>();
        /// <summary>
        /// Создание начальной сиплекс таблицы (симлекс метод)
        /// </summary>
        /// <param name="task"></param>
        public SimplexTable(Task task)
        {
            this.task = task;
            if (task.answer == null)
            {
                basis_var = 0;
                while (task.basis[basis_var][1] == 1)
                {
                    basis_var++;
                }
                free_var = task.basis.Length - basis_var;
                table = new OrdinaryFraction[basis_var + 1][];
                if (task.basis.Length == task.function.Length)
                {
                    for (int i = 0; i < basis_var; i++)
                    {
                        OrdinaryFraction[] row = new OrdinaryFraction[free_var + 1];
                        for (int j = basis_var; j < task.condition[i].Length; j++)
                        {
                            row[j - basis_var] = task.condition[i][j];
                        }
                        table[i] = row;
                    }
                }
                else
                {
                    for (int i = 0; i < basis_var; i++)
                    {
                        OrdinaryFraction[] row = new OrdinaryFraction[free_var + 1];
                        for (int j = 0; j < task.condition[i].Length; j++)
                        {
                            row[j] = task.condition[i][j];
                        }
                        table[i] = row;
                    }
                }
                //Высчитывание последней строки
                if (task.basis.Length == task.function.Length)
                {
                    OrdinaryFraction[] last_row = new OrdinaryFraction[free_var + 1];
                    for (int i = 0; i < free_var + 1; i++)
                    {
                        OrdinaryFraction sum = new OrdinaryFraction(0);
                        for (int j = 0; j < basis_var; j++)
                        {
                            sum = sum.Addition(table[j][i].Multiplication(task.function[task.basis[j][0] - 1]));
                        }
                        if (i < free_var)
                            last_row[i] = sum.Multiplication(new OrdinaryFraction(-1)).Addition(task.function[task.basis[basis_var + i][0] - 1]);
                        else
                            last_row[i] = sum.Multiplication(new OrdinaryFraction(-1));
                    }
                    table[table.Length - 1] = last_row;
                    Extr();
                }
                else
                {
                    OrdinaryFraction[] last_row = new OrdinaryFraction[free_var + 1];
                    for (int j = 0; j < free_var + 1; j++)
                    {
                        OrdinaryFraction sum = new OrdinaryFraction(0);
                        for (int i = 0; i < basis_var; i++)
                            sum = sum.Addition(table[i][j]);
                        last_row[j] = sum.Multiplication(new OrdinaryFraction(-1));
                    }
                    table[table.Length - 1] = last_row;
                }
            }
        }
        /// <summary>
        /// Главный метод решения задачи с использованием сиплекс-таблиц
        /// </summary>
        public String MainSimplexTable(bool step_by_step)
        {
            if (task.answer == null)
            {
                int i = 0;
                while (!FindSolution())
                {
                    //Console.WriteLine("Сипмплекс-таблица х(" + i + ")");
                    //print();
                    List<int[]> list = FindReferenceElement();
                    if (task.answer != null)
                        break;
                    if (step_by_step)
                    {
                        i = Step(i, list);
                        continue;
                    }
                    else
                    {
                        NewSimplexTable(list[0]);
                        stack.Push(new ArrayList { tableCopy(), task.Clone() });
                    }
                }
                //Console.WriteLine("Сипмплекс-таблица х(" + i + ")");
                //print();
            }
            //Console.WriteLine("Конец решения");
            return Answer(true);
        }
        /// <summary>
        /// Главный метод для решения вспомогательной задачи (нахождение базиса)
        /// </summary>
        public OrdinaryFraction[][] MainSimplexTambleForBasis()
        {
            int i = 0;
            while (!FindBasis())
            {
                Console.WriteLine("Сипмплекс-таблица ~х(" + i + ")");
                print();
                List<int[]> list = FindReferenceElement();
                if (task.answer != null)
                    break;
                //Здесь можно выбрать опрный элемент из списка list (по умолчанию первый)
                NewSimplexTable(list[0]);
                i++;
            }
            Console.WriteLine("Сипмплекс-таблица ~х(" + i + ")");
            print();
            if (task.answer == null)
                Console.WriteLine("Базис найден");
            return tableCopy();
        }
        /// <summary>
        /// Домножение функции на -1 при поиске максимума
        /// </summary>
        public void Extr()
        {
            if (task.extr == "max")
            {
                for (int i = 0; i < table[0].Length; i++)
                {
                    table[table.Length - 1][i] = table[table.Length - 1][i].Multiplication(new OrdinaryFraction(-1));
                }
            }
        }
        /// <summary>
        /// Метод для пошагового режима (обработка шага)
        /// </summary>
        public int Step(int i, List<int[]> listEl)
        {
            Console.WriteLine("Вернутся или продолжить?");
            string answer = Console.ReadLine();
            //Получение ответа от пользователя (продолжить или вернутся)
            if (answer == "back")
            {
                ArrayList list = stack.Pop();
                table = (OrdinaryFraction[][])list[0];
                task = (Task)list[1];
                return --i;
            }
            else
            {
                stack.Push(new ArrayList { tableCopy(), task.Clone() });
                //Получение ответа от пользователя (выбор опорного элемента)
                NewSimplexTable(listEl[0]);
                return ++i;
            }

        }
        /// <summary>
        /// Переход к новой сиплекс таблице
        /// </summary>
        public void NewSimplexTable(int[] index)
        {
            //Меняем местами переменные 
            int temp = task.basis[index[1]][0];
            task.basis[index[1]][0] = task.basis[index[0] + basis_var][0];
            task.basis[index[0] + basis_var][0] = temp;

            table[index[1]][index[0]].DivisionByOne();
            //Console.WriteLine("Деление 1 на опорный элемент");
            //print();

            //Высчитывание опорной строки 
            for (int j = index[1], i = 0; i < table[j].Length; i++)
                if (i != index[0])
                    table[j][i] = table[j][i].Multiplication(table[index[1]][index[0]]);

            //Высчитывание остальных строк
            for (int i = 0; i < table.Length; i++)
            {
                if (i != index[1])
                    for (int j = 0; j < table[i].Length; j++)
                        if (j != index[0])
                        {
                            OrdinaryFraction x = table[index[1]][j].Multiplication(table[i][index[0]]);
                            table[i][j] = table[i][j].Subtraction(x);
                        }
            }

            //Высчитывание опорного столбца
            for (int i = index[0], j = 0; j < table.Length; j++)
                if (j != index[1])
                    table[j][i] = table[j][i].Multiplication(table[index[1]][index[0]].Multiplication(new OrdinaryFraction(-1)));

            //Console.WriteLine("Преобразование опроной строки и столбца");
            //print();
        }
        /// <summary>
        /// Поиск опорных элементов в сиплекс-таблице
        /// </summary>
        /// <returns>Список опорных элементов (в координатах для таблицы)</returns>
        public List<int[]> FindReferenceElement()
        {
            int numberMin = 1;
            int x = -1;
            int y = -1;
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < table[0].Length - 1; i++)
            {
                if (table[table.Length - 1][i].NegativeFraction())
                {
                    OrdinaryFraction min = new OrdinaryFraction(2147483647, 1);
                    for (int j = 0; j < table.Length - 1; j++)
                    {
                        if (table[j][i].PositiveFraction())
                        {
                            OrdinaryFraction count = table[j][table[j].Length - 1].Division(table[j][i]);
                            if (count.Min(min))
                            {
                                min = count;
                                numberMin = 1;
                                x = i;
                                y = j;
                            }
                            else if (table[j][table[j].Length - 1].Division(table[j][i]).Equally(min))
                                numberMin++;
                        }
                    }
                    if (x == -1)
                    {
                        task.answer = "Решения нет. Бесконечное ребро.";
                        //Console.WriteLine("Решения нет. Бесконечное ребро.");
                        return null;
                    }
                    if (numberMin > 1)
                    {
                        for (int j = 0; j < table.Length; j++)
                        {
                            if (table[j][i].Equally(min))
                                list.Add(new int[] { x, y });
                        }
                    }
                    else
                        list.Add(new int[] { x, y });
                }

            }
            if (list.Count == 0)
            {
                if (FFF())
                {
                    for (int i = 0; i < table[0].Length - 1; i++)
                    {
                        if (task.basis[i+basis_var][0] <= task.function.Length)
                        {
                            for (int j = 0; j < table.Length - 1; j++)
                            {
                                if (table[j][i].NegativeFraction())
                                    list.Add(new int[] {i, j});
                            }
                        }
                    }
                }
                else
                    task.answer = "Система условий задачи несовместна";
            }
            return list;
        }
        /// <summary>
        /// Находит "лучший" опорный элемент
        /// </summary>
        /// <returns>Возвращает его координаты</returns>
        public int[] MaxReferenceElement(List<int[]> list)
        {
            int[] best = null;
            if (list != null)
                if (list.Count > 0)
                {
                    best = new int[2];
                    OrdinaryFraction max = table[table.Length - 1][list[0][0]];
                    best[0] = list[0][0];
                    best[1] = list[0][1];
                    for (int i=1; i<list.Count; i++)
                    {
                        if (max.Max(table[table.Length - 1][list[i][0]]))
                        {
                            max = table[table.Length - 1][list[i][0]];
                            best[0] = list[i][0];
                            best[1] = list[i][1];
                        }
                    }
                }
            return best;
        }
        /// <summary>
        /// Проверяет найден ли базис (все элементы нижней строки = 0)
        /// </summary>
        /// <returns>true, если найден</returns>
        public bool FindBasis()
        {
            if ((table[table.Length - 1][table[0].Length - 1].PositiveFraction()))
            {
                task.answer = "Система условий задачи противоричива";
                return true;
            }
            for (int i = table.Length - 1, j = 0; j < table[i].Length - 1; j++)
            {
                if (!table[i][j].Equally(new OrdinaryFraction(0)) && (task.basis[basis_var + j][0] <= task.function.Length))
                {
                    return false;
                }
            }
            //наличие искусственных переменных в базисе
            if (FFF())
                return false;
            return true;
        }
        /// <summary>
        /// Проверяет наличие искусственных переменных в базисе
        /// </summary>
        public bool FFF()
        {
            for (int i = 0; i < basis_var; i++)
                if (task.basis[i][0] > task.function.Length)
                    return true;
            return false;
        }
        /// <summary>
        /// Проверяет найдено ли решение (все элементы последней строки >= 0)
        /// </summary>
        /// <returns>true, если найдено</returns>
        public bool FindSolution()
        {
            for (int i = table.Length - 1, j = 0; j < table[0].Length - 1; j++)
            {
                if (table[i][j].NegativeFraction())
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Копирует текущую сиплекс-таблицу
        /// </summary>
        /// <returns>Копия текущей симплекс-таблицы</returns>
        public OrdinaryFraction[][] tableCopy()
        {
            OrdinaryFraction[][] new_table = new OrdinaryFraction[table.Length][];
            for (int i = 0; i < table.Length; i++)
            {
                OrdinaryFraction[] row = new OrdinaryFraction[table[i].Length];
                for (int j = 0; j < table[i].Length; j++)
                    row[j] = table[i][j].Copy();
                new_table[i] = row;
            }
            return new_table;
        }
        /// <summary>
        /// Ответ на решение задачи
        /// </summary>
        /// <returns>Вектор x* и значение f*</returns>
        public String Answer(bool ordinaryFraction)
        {
            if (task.answer == null)
            {
                OrdinaryFraction[] x = new OrdinaryFraction[task.basis.Length + 1];
                for (int i = 0; i < basis_var; i++)
                    x[task.basis[i][0] - 1] = table[i][table[0].Length - 1];
                for (int i = basis_var; i < task.basis.Length; i++)
                    x[task.basis[i][0] - 1] = new OrdinaryFraction(0);
                task.answer = "x* = (" + x[0].Print(ordinaryFraction);
                for (int i = 1; i < x.Length - 1; i++)
                {
                    task.answer = task.answer + ", " + x[i].Print(ordinaryFraction);
                }
                x[x.Length - 1] = table[table.Length - 1][table[0].Length - 1];
                task.answer = task.answer + ") f* = " + table[table.Length - 1][table[0].Length - 1].Print(ordinaryFraction);

                //Печать на консоль
                Console.WriteLine(task.answer);
            }
            Console.WriteLine(task.answer);
            return task.answer;
        }
        /// <summary>
        /// Выводит на консоль текущую симплекс-таблицу
        /// </summary>
        public void print()
        {
            for (int i = 0; i < table.Length; i++)
            {
                for (int j = 0; j < table[i].Length; j++)
                {
                    if (j == table[i].Length - 2)
                        Console.Write(table[i][j].Print(true) + " * x" + task.basis[basis_var + j][0] + " = ");
                    else if (j == table[i].Length - 1)
                        Console.Write(table[i][j].Print(true));
                    else
                        Console.Write(table[i][j].Print(true) + " * x" + task.basis[basis_var + j][0] + " + ");
                }
                Console.Write("\n");
            }
        }
    }
}
