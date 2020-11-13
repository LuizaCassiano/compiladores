using System;
using System.Collections.Generic;
using System.Text;

namespace Analysers.Control
{
    public class DataControl
    {
        public DataControl(string lexema, string token, string tipo)
        {
            Lexema = lexema;
            Token = token;
            Tipo = tipo;
        }

        public string Lexema { get; set; }
        public string Token { get; set; }
        public string Tipo { get; set; }
    }
}
