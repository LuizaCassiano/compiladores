using Analysers.Analyser;
using Analysers.Control;
using System;

namespace Analysers
{
    class Program
    {
        static void Main(string[] args)
        {
            ////Testando analisador Lexico
            //DataControl dataControl;

            //do
            //{
            //    dataControl = LexicoAnalyser.GetLexicoAnalyser(LexicoAnalyser.position);

            //    Console.WriteLine("TOKEN: " + dataControl.Token + " \nLEXEMA: " + dataControl.Lexema + " \nTIPO: " + dataControl.Tipo);

            //    Console.WriteLine("\n*******************************\n");
            //} 
            //while (dataControl.Lexema != "EOF" && dataControl.Lexema != "ERRO");

            //Console.ReadKey();

            //Testando analisador Sintetico
            SyntheticAnalyser.GetSyntheticAnalyser();
            Console.ReadKey();
        }
    }
}
