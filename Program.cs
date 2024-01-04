using System.Text.RegularExpressions;

namespace Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = "(64 - 15)*4 + 20/4";
            Console.WriteLine("input: " + input);
            Calculator.Evaluate(input, true);
        }
    }

    internal class Calculator
    {
        private static int get_oper_priority(string oper)
        {
            switch (Char.Parse(oper))
            {
                case '*':
                case '/':
                case '%':
                case '^':
                    return 2;
                case '+':
                case '-':
                    return 1;
                default:
                    return -1;
            }
        }

        private static void to_add_oper(Stack<string> stack, Queue<string> argQueue, string oper)
        {
            if (oper != "(" && oper != ")")
            {
                if (stack.Count == 0)
                {
                    stack.Push(oper);
                }
                else if (get_oper_priority(oper) > get_oper_priority(stack.Peek()))
                {
                    stack.Push(oper);
                }
                else if (get_oper_priority(oper) == get_oper_priority(stack.Peek()))
                {
                    argQueue.Enqueue(stack.Pop());
                    stack.Push(oper);
                }
                else if (get_oper_priority(oper) < get_oper_priority(stack.Peek()))
                {
                    argQueue.Enqueue(stack.Pop());
                    to_add_oper(stack, argQueue, oper);
                }
            }
            else
            {
                if (oper == "(")
                {
                    stack.Push(oper);
                }
                else if (oper == ")")
                {
                    bool go_on = true;
                    while (go_on)
                    {
                        string curOper = stack.Pop();
                        if (curOper == "(")
                        {
                            go_on = false;
                        }
                        else
                        {
                            argQueue.Enqueue(curOper);
                        }
                    }
                }
            }
        }

        private static float Exec_two_args(float arg1, float arg2, string oper)
        {
            switch (char.Parse(oper))
            {
                case '*': return arg1 * arg2;
                case '/': return arg1 / arg2;
                case '+': return arg1 + arg2;
                case '-': return arg1 - arg2;
                case '%': return arg1 % arg2;
                case '^': return (float)Math.Pow(arg1, arg2);
                default: return 0;
            }
        }

        public static Queue<string> postfix_notation_sorting(string input)
        {
            const string pattern = @"(?<argument>[\d\,]+)|(?<operator>[\(\)\+\-\*\/\%\^])";
            Stack<string> stack = new Stack<string>();
            Queue<string> argQueue = new Queue<string>();
            foreach (Match match in Regex.Matches(input, pattern))
            {
                if (match.Groups["argument"].Value != String.Empty)
                {
                    argQueue.Enqueue(match.Groups["argument"].Value);
                }
                else if (match.Groups["operator"].Value != String.Empty)
                {
                    to_add_oper(stack, argQueue, match.Groups["operator"].Value);
                }
            }
            while (stack.Count > 0)
            {
                argQueue.Enqueue(stack.Pop());
            }
            return argQueue;
        }

        public static float Calculate(Queue<string> argQueue, bool show_steps)
        {

            Stack<float> stack = new Stack<float>();
            int i = 1;
            foreach (string arg in argQueue)
            {
                if (arg == "+" || arg == "*" || arg == "/" || arg == "-")
                {
                    float arg1 = stack.Pop();
                    float arg2 = stack.Pop();
                    stack.Push(Exec_two_args(arg2, arg1, arg));
                    if (show_steps) Console.WriteLine(i + ") " + arg2 + " " + arg + " " + arg1 + " = " + stack.Peek());
                    i++;
                }
                //else if (arg == "-")
                //{
                //    stack.Push(Exec_two_args(0, stack.Pop(), arg));
                //}
                else
                {
                    stack.Push(float.Parse(arg));

                }


            }
            return stack.Pop();
        }

        public static void Evaluate(string input, bool show_steps)
        {
            Queue<string> argQueue = postfix_notation_sorting(input);
            Console.WriteLine(Calculate(argQueue, show_steps));
        }
    }
}