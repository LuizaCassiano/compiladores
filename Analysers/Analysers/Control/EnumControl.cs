using System;
using System.Collections.Generic;
using System.Text;

namespace Analysers.Control
{
    public class EnumControl
    {
        public EnumControl(int enumeration, string sentence)
        {
            Enumeration = enumeration;
            Sentence = sentence;
        }

        public int Enumeration { get; set; }
        public string Sentence { get; set; }
    }
}
