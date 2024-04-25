using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualPlayer.Converters
{
    public class NumberModifierConverter : IMultiValueConverter
    {

        //  CONST

        private static readonly char[] EQUATION_CHARACTERS = new char[]
        {
            '+', '-', '*', '/',
        };


        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0 || !(parameter is string equation))
                return null;

            var doubleValues = values.Where(v => v is double).Select(v => (double)v).ToArray();

            return SolveEquation(equation, doubleValues);
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #region MATH METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Solve equation. </summary>
        /// <param name="equation"> Equation. </param>
        /// <param name="params"> Params (which require "_index" to be used in equation). </param>
        /// <returns> Result. </returns>
        public static double SolveEquation(string equation, double[] @params)
        {
            double result = 0d;

            List<string> postfix = InfixToPostfix(equation, @params);
            Stack<double> stack = new Stack<double>();

            foreach (var token in postfix)
            {
                if (int.TryParse(token, out int value))
                {
                    stack.Push(value);
                }
                else if (EQUATION_CHARACTERS.Any(e => $"{e}" == token))
                {
                    if (stack.Count < 2)
                        throw new ArgumentException("Invalid equation");
                }

                double b = stack.Pop();
                double a = stack.Pop();
                double innerResult = 0;

                switch (token)
                {
                    case "+":
                        innerResult = a + b;
                        break;
                    case "-":
                        innerResult = a - b;
                        break;
                    case "*":
                        innerResult = a * b;
                        break;
                    case "/":
                        if (b == 0)
                            throw new DivideByZeroException("Division by zero");
                        innerResult = a / b;
                        break;
                }

                stack.Push(innerResult);
            }

            if (stack.Count == 1)
                result = stack.Pop();
            else
                throw new ArgumentException("Invalid equation");

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get math character precedence. </summary>
        /// <param name="op"> Math character. </param>
        /// <returns> Precedence index. </returns>
        private static int GetPrecedence(char op)
        {
            if (op == '+' || op == '-')
                return 1;
            else if (op == '*' || op == '/')
                return 2;
            else
                return 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert the equation from Infix to Postfix. </summary>
        /// <param name="equation"> Infix equation. </param>
        /// <returns> Postfix equation. </returns>
        private static List<string> InfixToPostfix(string equation, double[] @params)
        {
            var postfix = new List<string>();
            var stack = new Stack<char>();

            for (int i = 0; i < equation.Length; i++)
            {
                char c = equation[i];

                //  Check if is number or parameter index.
                if (c == '_' || char.IsDigit(c))
                {
                    string number = c.ToString();

                    while (i + 1 < equation.Length && (char.IsDigit(equation[i + 1]) || equation[i + 1] == '.'))
                    {
                        number += equation[i + 1];
                        i++;
                    }

                    if (number.StartsWith("_"))
                    {
                        int paramIndex = int.Parse(number.Substring(1));
                        number = @params[paramIndex].ToString().Replace(',', '.');
                    }

                    postfix.Add(number);
                }

                //  Check starting bracket.
                else if (c == '(')
                {
                    stack.Push(c);
                }

                //  Check ending bracket.
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        postfix.Add(stack.Pop().ToString());
                    }

                    if (stack.Count == 0)
                        throw new ArgumentException("Invalid equation");

                    stack.Pop(); // Pop the '('
                }

                //  Check math equation character.
                else if (EQUATION_CHARACTERS.Contains(c))
                {
                    while (stack.Count > 0 && GetPrecedence(stack.Peek()) >= GetPrecedence(c))
                    {
                        postfix.Add(stack.Pop().ToString());
                    }

                    stack.Push(c);
                }
            }

            while (stack.Count > 0)
            {
                if (stack.Peek() == '(')
                    throw new ArgumentException("Invalid equation");

                postfix.Add(stack.Pop().ToString());
            }

            return postfix;
        }

        #endregion MATH METHODS

    }
}
