using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    struct element
    {
        public enum OPS { LPA, RPA, FAC, EXP, MUL, DIV, MOD, ADD, SUB, NUL };

        public static readonly char[] ops = new char[] { '(', ')', '!', '^', '*', '/', '%', '+', '-' };

        public static readonly int[] ord = new int[] { 0, 0, 0, 3, 2, 2, 2, 1, 1 };

        public bool IsNumber;

        public OPS Op;

        public double Number;

        public element(OPS op)
        {
            IsNumber = false;
            Op = op;
            Number = 0;
        }

        public element(double num)
        {
            IsNumber = true;
            Number = num;
            Op = OPS.NUL;
        }

        public bool isOperator(char op)
        {
            return !IsNumber && ops[(int)Op] == op;
        }

        public char getOperator()
        {
            return !IsNumber ? ops[(int)Op] : '0';
        }
    }
}
