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
        public Task task;
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
            }
            SimplexTable simplexTable = new SimplexTable(task);
            //Временно 
            simplexTable.MainSimplexTable(step_by_step);
        }
        /// <summary>
        /// Сортирует столбцы таблицы по новому базису
        /// </summary>
        public void SSS(OrdinaryFraction[][] table)
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
                        for (int k = 0; k < table.Length; k++)
                        {
                            OrdinaryFraction t = table[k][i];
                            table[k][i] = table[k][j];
                            table[k][j] = t;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Убирает из таблицы вспомогательные переменные
        /// </summary>
        public void NewTable(OrdinaryFraction[][] table)
        {
            for (int i=0; i<task.condition.Length; i++)
            {
                for (int j=0; j<task.condition.Length; j++)
                {
                    if (i == j)
                        task.condition[j][i] = new OrdinaryFraction(1);
                    else
                        task.condition[j][i] = new OrdinaryFraction(0);
                }
            }
            for (int i=0, k = task.condition.Length; i<table[0].Length-1; i++)
            {
                if (task.basis[i + task.condition.Length][0] <= task.function.Length)
                {
                    for (int j=0; j<table.Length-1; j++)
                    {
                        task.condition[j][k] = table[j][i];
                    }
                    k++;
                }
            }
            for (int i = 0; i < task.condition.Length; i++)
                task.condition[i][task.condition[i].Length - 1] = table[i][table[i].Length - 1];
        }
        /// <summary>
        /// Создает новый базис для решения задачи
        /// </summary>
        public void NewBasis()
        {
            int[][] new_basis = new int[task.function.Length][];
            int k = 0;
            for (int i=0; i<task.basis.Length; i++)
            {
                if (task.basis[i][1] == 1)
                    new_basis[k++] = task.basis[i];
                if ((task.basis[i][1] == 0) && (task.basis[i][0] <= task.function.Length))
                    new_basis[k++] = task.basis[i];
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
