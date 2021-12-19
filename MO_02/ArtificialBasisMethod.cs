using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MO_02
{
    /// <summary>
    /// Нахождение базиса методом искусственного базиса
    /// </summary>
    class ArtificialBasisMethod
    {
        Task task;
        public ArtificialBasisMethod(Task task)
        {
            this.task = task;
            task.basis = new int[task.function.Length + task.condition.Length][];
            for (int i = 0; i < task.condition.Length; i++)
                task.basis[i] = new int[] { (i + task.function.Length + 1), 1 };
            for (int i = task.condition.Length; i < task.basis.Length; i++)
                task.basis[i] = new int[] { (i + 1 - task.condition.Length), 0 };

        }
        /// <summary>
        /// Главный метод решения задачи методом искуственного базиса
        /// </summary>
        public void MainArtificialBasisMethod(bool step_by_step)
        {
            SimplexTable findBasis = new SimplexTable(task);
            OrdinaryFraction[][] table = findBasis.MainSimplexTambleForBasis();
            if (task.answer == null)
            {
                NewTable(table);
                NewBasis();
                //Console.WriteLine("Новая таблица");
                //print();

            }
            SimplexTable simplexTable = new SimplexTable(task);
            //Временно 
            //simplexTable.MainSimplexTable(step_by_step);
        }
        /// <summary>
        /// Убирает из таблицы вспомогательные переменные
        /// </summary>
        public void NewTable(OrdinaryFraction[][] table)
        {
            for (int i = 0; i < task.condition[0].Length; i++)
                for (int j = 0; j < task.condition.Length; j++)
                {
                    int count = -1;
                    if (i < task.condition[0].Length - 1)
                        count = task.basis[i + task.condition.Length][0];
                    if (count > task.function.Length)
                    {
                        if (i == j)
                            task.condition[j][i] = new OrdinaryFraction(1);
                        else
                            task.condition[j][i] = new OrdinaryFraction(0);
                    }
                    else
                    {
                        task.condition[j][i] = table[j][i];
                        task.condition[j][i] = table[j][i];
                    }
                }

        }
        /// <summary>
        /// Создает новый базис для решения задачи
        /// </summary>
        public void NewBasis()
        {
            int[][] new_basis = new int[task.function.Length][];
            for (int i = 0, j = 0; i < task.basis.Length; i++)
            {
                if (task.basis[i][0] <= task.function.Length)
                {
                    new_basis[j++] = task.basis[i];
                }
            }
            task.basis = new_basis;
        }
        /// <summary>
        /// Выводит на консоль таблицу
        /// </summary>
        /*public void print()
        {
            for (int i = 0; i < task.condition.Length; i++)
            {
                for (int j = 0; j < task.condition[i].Length; j++)
                {
                    if (j == task.condition[i].Length - 2)
                        Console.Write(task.condition[i][j].Print() + " * x" + task.basis[j][0] + " = ");
                    else if (j == task.condition[i].Length - 1)
                        Console.Write(task.condition[i][j].Print());
                    else
                        Console.Write(task.condition[i][j].Print() + " * x" + task.basis[j][0] + " + ");
                }
                Console.Write("\n");
            }
        }*/
    }
}
