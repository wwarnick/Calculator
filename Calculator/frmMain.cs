using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Calculator
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private double evaluate(List<element> ex)
        {
            // search for parentheses and evaluate them first recursively
            bool moreParens = true;
            while(moreParens)
            {
                moreParens = false;
                for (int i = 0; i < ex.Count; i++)
                {
                    // if left parenthese is found...
                    if (ex[i].isOperator('('))
                    {
                        moreParens = true;

                        bool closParFound = false;
                        // find closing parenthese
                        int count = 0;
                        for (int j = i + 1; j < ex.Count; j++)
                        {
                            // if another opening parenthese is found...
                            if (ex[j].isOperator('('))
                            {
                                count++;
                            }
                            else if (ex[j].isOperator(')')) // if a right parenthese is found
                            {
                                // if it's the closing parenthese, evaluate
                                if (count == 0)
                                {
                                    double temp = evaluate(ex.GetRange(i + 1, j - i - 1));
                                    ex.RemoveRange(i, j - i + 1);
                                    ex.Insert(i, new element(temp));
                                    closParFound = true;
                                    break;
                                }
                                else // if internal...
                                {
                                    count--;
                                }
                            }
                        }
                        if (!closParFound)
                        {
                            throw new Exception("No closing parenthese found");
                        }
                    }
                }
            }

            // now that there are no parentheses, evaluate the expression

            // first, apply unary operators

            for (int i = 0; i < ex.Count; i++)
            {
                if (ex[i].isOperator('!'))
                {
                    int buf = (int)ex[i - 1].Number;

                    for (int j = buf - 1; j > 1; j--)
                    {
                        buf *= j;
                    }

                    ex[i - 1] = new element((double)buf);
                    ex.RemoveAt(i--);
                }
            }

            while (ex.Count != 1)
            {
                // search for operator of the highest precedence

                int precedence = 0;
                int index = 0;
                for (int i = 0; i < ex.Count; i++)
                {
                    int temp1 = 0;
                    if (!ex[i].IsNumber && (temp1 = element.ord[indexOf(element.ops, ex[i].getOperator())]) > precedence)
                    {
                        precedence = temp1;
                        index = i;
                    }
                }

                double buf = 0d;
                switch (ex[index].getOperator())
                {
                    case '^':
                        buf = Math.Pow(ex[index - 1].Number, ex[index + 1].Number);
                        break;
                    case '*':
                        buf = ex[index - 1].Number * ex[index + 1].Number;
                        break;
                    case '/':
                        buf = ex[index - 1].Number / ex[index + 1].Number;
                        break;
                    case '%':
                        buf = ex[index - 1].Number % ex[index + 1].Number;
                        break;
                    case '+':
                        buf = ex[index - 1].Number + ex[index + 1].Number;
                        break;
                    case '-':
                        buf = ex[index - 1].Number - ex[index + 1].Number;
                        break;
                    default:
                        throw new Exception("invalid character: " + ex[index].getOperator());
                }

                // replace expression with result
                ex.RemoveRange(index - 1, 3);
                ex.Insert(index - 1, new element(buf));
            }

            return ex[0].Number;
        }

        private List<element> tokenize(string pExpression)
        {
            // replace all unary '-'s before parentheses with "-1*"
            StringBuilder buf = new StringBuilder(pExpression);
            for (;;)
            {
                int firstParen = indexOf(buf, '(');
                
                if (firstParen > 0 && buf[firstParen - 1] == '-' && (firstParen == 1 || indexOf<char>(element.ops, buf[firstParen - 2]) != -1))
                {
                    buf.Insert(firstParen, "1*");
                }
                else
                {
                    break;
                }
            }
            pExpression = buf.ToString();

            // tokenize the expression
            List<element> ex = new List<element>();
            for (int i = 0; i < pExpression.Length; i++)
            {
                // if a number
                if (char.IsDigit(pExpression[i]) || pExpression[i] == '.' || (pExpression[i] == '-' && (i == 0 || indexOf(element.ops, pExpression[i - 1]) > 2)))
                {
                    int j = (pExpression[i] != '-') ? i : i + 1;
                    for (; j < pExpression.Length && (char.IsDigit(pExpression[j]) || pExpression[j] == '.'); j++) { }

                    ex.Add(new element(Double.Parse(pExpression.Substring(i, j - i))));

                    i = j - 1;
                }
                else if (indexOf<char>(element.ops, pExpression[i]) != -1)
                {
                    ex.Add(new element((element.OPS)indexOf(element.ops, pExpression[i])));
                }
                else
                {
                    throw new Exception("Character not recognized: " + pExpression[i]);
                }
            }

            return ex;
        }

        private int indexOf<T>(T[] array, T val)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(val))
                {
                    return i;
                }
            }

            return -1;
        }

        private int indexOf(StringBuilder str, char val)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Equals(val))
                {
                    return i;
                }
            }

            return -1;
        }

        private void txtExpression_TextChanged(object sender, EventArgs e)
        {
            try
            {
                txtResult.Text = evaluate(tokenize(txtExpression.Text.Replace(" ", ""))).ToString();
            }
            catch (Exception ex)
            {
                txtResult.Text = "NaN";
            }
        }
    }
}
