using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MO_02
{
    /// <summary>
    /// Клас обработки Xml файлов
    /// </summary>
    class XmlFiles
    {
        public bool Error = false;
        public Task ReadFile(string file)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(file);
            XmlElement xRoot = xDoc.DocumentElement;
            int size = 0;
            List<OrdinaryFraction> function = new List<OrdinaryFraction>();
            List<OrdinaryFraction[]> condition = new List<OrdinaryFraction[]>();
            List<string> sign = new List<string>();
            List<int[]> basis = new List<int[]>();
            string extr = null;

            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name == "function")
                    {
                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (childnode.Name == "coefficient")
                            {
                                size++;
                                if (new OrdinaryFraction(childnode.InnerText) != null)
                                    function.Add(new OrdinaryFraction(childnode.InnerText));
                                else
                                    Error = true;
                            }
                            if (childnode.Name == "extremum")
                            {
                                extr = childnode.InnerText;
                            }
                        }
                        //Console.WriteLine($"Компания: {childnode.InnerText}");
                    }
                    // если узел condition
                    if (xnode.Name == "condition")
                    {
                        List<OrdinaryFraction> row = new List<OrdinaryFraction>();
                        foreach (XmlNode node in xnode.ChildNodes)
                        {
                            if (node.Name == "coefficient" || node.Name == "b")
                            {
                                if (new OrdinaryFraction(node.InnerText) != null)
                                    row.Add(new OrdinaryFraction(node.InnerText));
                                else
                                    Error = true;

                            }
                            if (node.Name == "sign")
                            {
                                string pattern = @"^[<\|>]?=$";
                                if (Regex.IsMatch(node.InnerText, pattern))
                                    sign.Add(node.InnerText);
                            }
                        }
                        OrdinaryFraction[] row2 = new OrdinaryFraction[row.Count];
                        for (int i = 0; i < row2.Length; i++)
                            row2[i] = row[i];
                        condition.Add(row2);
                        //Console.WriteLine($"Возраст: {childnode.InnerText}");
                    }
                    if (xnode.Name == "basis")
                    {
                        int i = 1;
                        string pattern = @"^0$";
                        string pattern2 = @"^[1-9][0-9]*$";
                        foreach (XmlNode node in xnode.ChildNodes)
                        {

                            if (node.Name == "x")
                            {
                                if (Regex.IsMatch(node.InnerText, pattern) || Regex.IsMatch(node.InnerText, pattern2))
                                    basis.Add(new int[] { i++, int.Parse(node.InnerText) });
                                else
                                    Error = true;
                            }

                        }
                    }
                    //Console.WriteLine();
                }
            }
            OrdinaryFraction[] function2 = new OrdinaryFraction[function.Count];
            OrdinaryFraction[][] condition2 = new OrdinaryFraction[condition.Count][];
            string[] sign2 = new string[sign.Count];
            int[][] basis2 = new int[basis.Count][];
            for (int i = 0; i < function2.Length; i++)
                function2[i] = function[i];
            for (int i = 0; i < condition2.Length; i++)
                condition2[i] = condition[i];
            for (int i = 0; i < sign2.Length; i++)
                sign2[i] = sign[i];
            for (int i = 0; i < basis2.Length; i++)
                basis2[i] = basis[i];
            if (basis2.Length == 0)
                basis2 = null;
            return new Task(size, function2, condition2, sign2, basis2, extr, null);
        }
        public void WriteFile(Task task)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlElement xRoot = xDoc.CreateElement("task");

            XmlElement function = xDoc.CreateElement("function");
            for (int i = 0; i < task.function.Length; i++)
            {
                XmlElement coefficient = xDoc.CreateElement("coefficient");
                XmlText coefficient_value = xDoc.CreateTextNode(task.function[i].Print(true));
                coefficient.AppendChild(coefficient_value);
                function.AppendChild(coefficient);
            }
            XmlElement extremum = xDoc.CreateElement("extremum");
            XmlText extremum_value = xDoc.CreateTextNode(task.extr);
            extremum.AppendChild(extremum_value);
            function.AppendChild(extremum);
            xRoot.AppendChild(function);

            for (int i = 0; i < task.condition.Length; i++)
            {
                XmlElement condition = xDoc.CreateElement("condition");
                for (int j = 0; j < task.condition[i].Length - 1; j++)
                {
                    XmlElement coefficient = xDoc.CreateElement("coefficient");
                    XmlText coefficient_value = xDoc.CreateTextNode(task.condition[i][j].Print(true));
                    coefficient.AppendChild(coefficient_value);
                    condition.AppendChild(coefficient);
                }
                XmlElement b = xDoc.CreateElement("b");
                XmlText b_value = xDoc.CreateTextNode(task.condition[i][task.condition[i].Length - 1].Print(true));
                b.AppendChild(b_value);
                condition.AppendChild(b);
                XmlElement sign = xDoc.CreateElement("sign");
                XmlText sign_value = xDoc.CreateTextNode(task.sign[i]);
                sign.AppendChild(sign_value);
                condition.AppendChild(sign);
                xRoot.AppendChild(condition);
            }

            if (task.basis != null)
            {
                XmlElement basis = xDoc.CreateElement("basis");
                for (int i = 0; i < task.basis.Length; i++)
                {
                    XmlElement x = xDoc.CreateElement("x");
                    XmlText x_value = xDoc.CreateTextNode(task.basis[i][1].ToString());
                    x.AppendChild(x_value);
                    basis.AppendChild(x);
                }
                xRoot.AppendChild(basis);
            }
            xDoc.AppendChild(xRoot);
            xDoc.Save("output.xml");
        }

    }
    //Классы для решения задачи сиплекс методом/методом искуственного базиса 
    /// <summary>
    /// Описывает входную функцию
    /// </summary>
    public class Function
    {
        /// <summary>
        /// Коэфициенты перед неизвестными
        /// </summary>
        public String coefficient;
        /// <summary>
        /// Атрибут тега coefficient
        /// </summary>
        public int number_x;
        /// <summary>
        /// min/max
        /// </summary>
        public String extremum;
    }
    /// <summary>
    /// Описывает условия/ограничения задачи
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// Коэфициенты перед неизвестными
        /// </summary>
        public String coefficient;
        /// <summary>
        /// Атрибут тега coefficient
        /// </summary>
        public int number_x;
        /// <summary>
        /// Хранит значение b
        /// </summary>
        public String b;
        /// <summary>
        ///  =, <=, >=
        /// </summary>
        public String sign;
    }
    /// <summary>
    /// Описывает начальный базис (если он есть)
    /// </summary>
    public class Basis
    {
        /// <summary>
        /// Хранит зачение элемента х
        /// </summary>
        public string x;
    }
}

