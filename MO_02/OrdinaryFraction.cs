using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MO_02
{
    /// <summary>
    /// Обыкновеные дроби
    /// </summary>
    public class OrdinaryFraction
    {
        public bool Error = false;
        /// <summary>
        /// Числитель
        /// </summary>
        public int numerator;
        /// <summary>
        /// Знаменатель
        /// </summary>
        public int denominator;
        /// <summary>
        /// Представление числа ноль
        /// </summary>
        public OrdinaryFraction()
        {

        }
        /// <summary>
        /// Обыкновенная дробь (из строки)
        /// </summary>
        /// <param name="ordinary_fraction"></param>
        public OrdinaryFraction(String ordinary_fraction)
        {
            string pattern1 = @"^-?[1-9][0-9]*$";
            string pattern2 = @"^0$";
            string pattern3 = @"^0/-?[1-9]*$";
            string pattern6 = @"^-?[1-9][0-9]*/-?[1-9]*$";
            string pattern4 = @"^-?[1-9][0-9]*,[0-9]*$";
            string pattern5 = @"^-?0,[0-9]*$";
            if (Regex.IsMatch(ordinary_fraction, pattern3) || Regex.IsMatch(ordinary_fraction, pattern6))
            {
                string[] numbers = ordinary_fraction.Split(new char[] { '/' });
                numerator = int.Parse(numbers[0]);
                denominator = int.Parse(numbers[1]);
                if (denominator < 0)
                {
                    this.denominator *= -1;
                    this.numerator *= -1;
                }
                ToReduce();
            }
            else if (Regex.IsMatch(ordinary_fraction, pattern1) || Regex.IsMatch(ordinary_fraction, pattern2) || Regex.IsMatch(ordinary_fraction, pattern4) || Regex.IsMatch(ordinary_fraction, pattern5))
            {
                double number = double.Parse(ordinary_fraction);
                if (number == 0.0)
                {
                    numerator = 0;
                    denominator = 1;
                }
                else
                {
                    int x = (int)(Math.Round(number - (int)number, 3, MidpointRounding.ToEven) * 1000);
                    numerator = (int)number * 1000 + x;
                    denominator = 1000;
                    ToReduce();
                }
            }
                 //new OrdinaryFraction(double.Parse(ordinary_fraction));
            else
                Error = true;
        }
        /// <summary>
        /// Обыкновенная дробь (из десятичной)
        /// </summary>
        /// <param name="number"></param>
        public OrdinaryFraction(double number)
        {
            if (number == 0.0)
            {
                numerator = 0;
                denominator = 1;
            }
            else
            {
                int x = (int)(Math.Round(number - (int)number, 3, MidpointRounding.ToEven) * 1000);
                numerator = (int)number * 1000 + x;
                denominator = 1000;
                ToReduce();
            }
        }
        /// <summary>
        /// Обыкновенная дробь (из числителя и знаменателя)
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public OrdinaryFraction(int numerator, int denominator)
        {
            this.numerator = numerator;
            this.denominator = denominator;
            if (denominator < 0)
            {
                this.denominator *= -1;
                this.numerator *= -1;
            }
            ToReduce();
        }
        /// <summary>
        /// Сокращение дроби
        /// </summary>
        public void ToReduce()
        {
            int nod = Nod(numerator, denominator);
            if (nod != 0)
            {
                numerator /= nod;
                denominator /= nod;
            }
        }
        /// <summary>
        /// Поиск набольшего общего делителя
        /// </summary>
        /// <param name="n"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public int Nod(int n, int d)
        {
            int temp;
            n = Math.Abs(n);
            d = Math.Abs(d);
            while (d != 0 && n != 0)
            {
                if (n % d > 0)
                {
                    temp = n;
                    n = d;
                    d = temp % d;
                }
                else break;
            }
            if (d != 0 && n != 0) return d;
            else return 0;
        }
        /// <summary>
        /// Умножение обыкновенных дробей
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Новый объект класса OrdinaryFraction</returns>
        public OrdinaryFraction Multiplication(OrdinaryFraction other)
        {
            int new_numerator = numerator * other.numerator;
            int new_denominator = denominator * other.denominator;
            return new OrdinaryFraction(new_numerator, new_denominator);
        }
        /// <summary>
        /// Деление дробей
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public OrdinaryFraction Division(OrdinaryFraction other)
        {
            int new_numerator = numerator * other.denominator;
            int new_denominator = denominator * other.numerator;
            return new OrdinaryFraction(new_numerator, new_denominator);
        }
        /// <summary>
        /// Деление 1 на текущую дробь (переворачивание)
        /// </summary>
        public void DivisionByOne()
        {
            int temp = numerator;
            numerator = denominator;
            denominator = temp;
        }
        /// <summary>
        /// Вычитание двух дробей
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Новый объект класса OrdinaryFraction</returns>
        public OrdinaryFraction Subtraction(OrdinaryFraction other)
        {
            int new_numerator;
            int new_denominator;
            if (denominator != other.denominator)
            {
                new_numerator = numerator * other.denominator - other.numerator * denominator;
                new_denominator = denominator * other.denominator;
            }
            else
            {
                new_numerator = numerator - other.numerator;
                new_denominator = denominator;
            }
            return new OrdinaryFraction(new_numerator, new_denominator);
        }
        /// <summary>
        /// Сложение двух дробей
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Новый объект класса OrdinaryFraction</returns>
        public OrdinaryFraction Addition(OrdinaryFraction other)
        {
            int new_numerator;
            int new_denominator;
            if (denominator != other.denominator)
            {
                new_numerator = numerator * other.denominator + other.numerator * denominator;
                new_denominator = denominator * other.denominator;
            }
            else
            {
                new_numerator = numerator + other.numerator;
                new_denominator = denominator;
            }
            return new OrdinaryFraction(new_numerator, new_denominator);
        }
        /// <summary>
        /// Максимум
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true, если текущий объект больше</returns>
        public bool Max(OrdinaryFraction other)
        {
            if (other.numerator == 2147483647)
                return false;
            if (numerator * other.denominator > other.numerator * denominator)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Минимум
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true, если текущий обьект меньше</returns>
        public bool Min(OrdinaryFraction other)
        {
            if (other.numerator == 2147483647)
                return true;
            if (numerator * other.denominator < other.numerator * denominator)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Равенство
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true, если дроби равны</returns>
        public bool Equally(OrdinaryFraction other)
        {
            if ((numerator + other.numerator == 0) || (numerator == other.numerator) && (denominator == other.denominator))
                return true;
            else
                return false;

        }
        /// <summary>
        /// Преобразование в десятичную дробь
        /// </summary>
        /// <returns>обьект типа double</returns>
        public double toDecimalFraction()
        {
            return Math.Round((double)numerator / denominator, 2);
        }
        /// <summary>
        /// Проверяет отрицательный элемент или нет
        /// </summary>
        /// <returns>bool</returns>
        public bool NegativeFraction()
        {
            if (numerator < 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Проверяет положительный элементы или нет
        /// </summary>
        /// <returns>bool</returns>
        public bool PositiveFraction()
        {
            if (numerator > 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Возвращает копию обьекта
        /// </summary>
        /// <returns></returns>
        public OrdinaryFraction Copy()
        {
            return new OrdinaryFraction(numerator, denominator);
        }
        /// <summary>
        /// Печать обыкновенной дроби
        /// </summary>
        /// <returns>Строку</returns>
        public string Print(bool ordinaryFraction)
        {
            if (ordinaryFraction)
                return $"{numerator}/{denominator}";
            else
                return toDecimalFraction().ToString();
        }
    }
}
