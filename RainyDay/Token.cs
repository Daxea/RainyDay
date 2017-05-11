using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainyDay
{
    public class Token
    {
        public string Type { get; }
        public object Value { get; }

        public Token(string type)
        {
            Type = type;
            Value = null;
        }

        public Token(string type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}