using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Analysers.Control;
using Analysers.Tables;

namespace Analysers.Analyser
{
    public class LexicoAnalyser
    {
        public static int position = 0, error = 0, lineOfError = 1, columnOfError = 0;
        public static Dictionary<string, DataControl> simbolTable = new Dictionary<string, DataControl>();

        public static void CompleteLexicoAnalyser()
        {
            var lexico1 = new DataControl("inicio", "inicio", " ");
            simbolTable.Add("inicio", lexico1);

            var lexico2 = new DataControl("varinicio", "varinicio", " ");
            simbolTable.Add("varinicio", lexico2);

            var lexico3 = new DataControl("varfim", "varfim", " ");
            simbolTable.Add("varfim", lexico3);

            var lexico4 = new DataControl("escreva", "escreva", " ");
            simbolTable.Add("escreva", lexico4);

            var lexico5 = new DataControl("leia", "leia", " ");
            simbolTable.Add("leia", lexico5);

            var lexico6 = new DataControl("se", "se", " ");
            simbolTable.Add("se", lexico6);

            var lexico7 = new DataControl("entao", "entao", " ");
            simbolTable.Add("entao", lexico7);

            var lexico8 = new DataControl("fimse", "fimse", " ");
            simbolTable.Add("fimse", lexico8);

            var lexico9 = new DataControl("fim", "fim", " ");
            simbolTable.Add("fim", lexico9);

            var lexico10 = new DataControl("inteiro", "inteiro", " ");
            simbolTable.Add("inteiro", lexico10);

            var lexico11 = new DataControl("literal", "literal", " ");
            simbolTable.Add("literal", lexico11);

            var lexico12 = new DataControl("real", "real", " ");
            simbolTable.Add("real", lexico12);
        }

        public static DataControl GetLexicoAnalyser(int pos) 
        {
            position = pos; 
            long tam; 

            int[] S = {128,  1,  0, 69, 43, 45, 42, 47, 62, 60, 61, 40, 41, 59, 34,129,123,125,130, 10, 32, 32, 46, 95,
                       131, 19, 25,132, 11, 12, 13, 14,  4,  1, 26,  9, 10,  8, 15,132, 17,132,  3,131,131,131,132,132,
                         1,  0,  0,  0,  0,  6,  0,  0,  7,  0,  2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         2,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         3,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         4,  0,  0,  0,  0,  0,  0,  0,  0,  0,  5,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         5,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         6,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         7,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         8,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                         9,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        10,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        11,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        12,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        13,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        14,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 16, 132, 15, 15,132, 15, 15, 15, 15, 15,
                        16,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 17, 132, 17, 18,132, 17, 17, 17, 17, 17,
                        18,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        19, 19,  0, 22,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 20,  0,
                        20, 21,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,
                        21, 21,  0, 22,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        22, 24,132,132, 23, 23,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,
                        23, 24,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,
                        24, 24,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                        25, 25, 25,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 25,
                        26,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                       132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132,132};

            Stack<int> states = new Stack<int>();
            TransitionTable[,] transitionTable = new TransitionTable[29, 24];

            int t = 0;         
            
            for (int i = 0; i < 29; i++)                               
            {                                                         
                for (int j = 0; j < 24; j++)                          
                {                                                   
                    transitionTable[i, j] = new TransitionTable(S[t]);     
                    t++;                                          
                }                                                
            }                                                   

            var tabelahash = new ErrorTable();

            tabelahash.hashErrorTable.Add(1, "Identificador não permitido");                           
            tabelahash.hashErrorTable.Add(16, "Constantes de literais nao permitidas");                  
            tabelahash.hashErrorTable.Add(18, "Erro de foramatacao de comentario (chaves)");         
            tabelahash.hashErrorTable.Add(21, "Constantes numericas nao são permitidas");              
            tabelahash.hashErrorTable.Add(23, "Constantes numericas nao são permitidas");             
            tabelahash.hashErrorTable.Add(24, "Constantes numericas nao são permitidas");            

            try
            {
                StringBuilder builder = new StringBuilder();
                StreamReader stream = File.OpenText("C:\\Analysers\\Analysers\\Utils\\text.txt");

                int caracter = 0, linha = 1, coluna = 0;
                stream.BaseStream.Position = position;
                tam = stream.BaseStream.Length;
                states.Push(transitionTable[linha, coluna].Element);

                while (position <= tam || error == 0)
                {
                    caracter = stream.Read();
                    int test = 0;

                    for (int i = 0; i < 24; i++)                    
                    {                                              
                        if (transitionTable[0, i].Element == caracter)   
                        {                                        
                            coluna = i;                         
                            test = 1;                          
                        }                                     
                    }                                        

                    if (test == 0 && caracter != 10 && caracter != 13 && caracter == 32)
                    {
                        coluna = 17;
                    }

                    if (test == 0 && (caracter == 10 || caracter == 13 || caracter == 32))
                    {
                        coluna = 19;
                    }

                    if (caracter == -1)
                    {
                        coluna = 18;
                    }

                    if (char.IsDigit((char)caracter))
                    {
                        coluna = 1;
                    }

                    if (coluna == 21 && caracter == 32 && states.Peek() == 15)
                    {
                        builder.Append((char)caracter);
                    }

                    if (caracter == 92 && states.Peek() == 15)
                    {
                        coluna = 2;
                    }

                    if (char.IsLetter((char)caracter))
                    {
                        coluna = 2;
                    }

                    if ((caracter == 69 || caracter == 101) && (states.Peek() == 19 || states.Peek() == 21))
                    {
                        coluna = 3;
                    }

                    for (int i = 0; i < 29; i++)
                    {
                        if (transitionTable[i, 0].Element == states.Peek())
                        {
                            linha = i;
                        }
                    }

                    states.Push(transitionTable[linha, coluna].Element);

                    if (transitionTable[linha, coluna].Element == 0)
                    {
                        if (simbolTable.ContainsKey(builder.ToString()))
                        {
                            DataControl aux = simbolTable[builder.ToString()];
                            return aux;
                        }
                        else
                        {
                            DataControl simaux;
                            states.Pop();

                            switch (states.Peek())
                            {
                                case 1:
                                    simaux = new DataControl(builder.ToString(), "opr", " ");
                                    return simaux;
                                case 2:
                                    simaux = new DataControl(builder.ToString(), "opr", " ");
                                    return simaux;
                                case 3:
                                    simaux = new DataControl("EOF", "EOF", " ");
                                    return simaux;
                                case 4:
                                    simaux = new DataControl(builder.ToString(), "opr", " ");
                                    return simaux;
                                case 5:
                                    simaux = new DataControl(builder.ToString(), "opr", " ");
                                    return simaux;
                                case 6:
                                    simaux = new DataControl(builder.ToString(), "rcb", " ");
                                    return simaux;
                                case 7:
                                    simaux = new DataControl(builder.ToString(), "opr", " ");
                                    return simaux;
                                case 8:
                                    simaux = new DataControl(builder.ToString(), "pt_v", " ");
                                    return simaux;
                                case 9:
                                    simaux = new DataControl(builder.ToString(), "ab_p", " ");
                                    return simaux;
                                case 10:
                                    simaux = new DataControl(builder.ToString(), "fc_p", " ");
                                    return simaux;
                                case 11:
                                    simaux = new DataControl(builder.ToString(), "opm", " ");
                                    return simaux;
                                case 12:
                                    simaux = new DataControl(builder.ToString(), "opm", " ");
                                    return simaux;
                                case 13:
                                    simaux = new DataControl(builder.ToString(), "opm", " ");
                                    return simaux;
                                case 14:
                                    simaux = new DataControl(builder.ToString(), "opm", " ");
                                    return simaux;
                                case 26:
                                    simaux = new DataControl(builder.ToString(), "opm", " ");
                                    return simaux;
                                case 16:
                                    simaux = new DataControl(builder.ToString(), "literal", "literal");
                                    return simaux;
                                case 18:
                                    Console.WriteLine("Comment " + builder);
                                    break;
                                case 19:
                                    simaux = new DataControl(builder.ToString(), "num", "inteiro");
                                    return simaux;
                                case 21:
                                    simaux = new DataControl(builder.ToString(), "num", "real");
                                    return simaux;
                                case 24:
                                    simaux = new DataControl(builder.ToString(), "num", "real");
                                    return simaux;
                                case 25:
                                    simaux = new DataControl(builder.ToString(), "id", " ");
                                    simbolTable.Add(simaux.Lexema, simaux);
                                    return simaux;
                                default:
                                    Console.WriteLine("Erro na leitura da pilha ou formato do comentário errado!");
                                    Console.WriteLine("\nLinha" + lineOfError + " Coluna " + columnOfError);
                                    error = 1;
                                    break;
                            }
                        }

                        states.Clear();
                        states.Push(transitionTable[1, coluna].Element);
                        builder.Remove(0, builder.Length);
                    }

                    if (transitionTable[linha, coluna].Element == 132)
                    {

                        Console.WriteLine("ERRO ENCONTRADO - " + tabelahash.hashErrorTable[linha]);        
                        Console.WriteLine("\nLinha na tabela: " + linha + " Coluna: " + coluna);          
                        Console.WriteLine("\nLinha: " + lineOfError + " Coluna: " + columnOfError);           
                        error = 1;                                                                       
                        DataControl simerro = new DataControl("ERRO", "ERRO", " ");
                        return simerro;                                                               
                    }

                    if (caracter != 10 && caracter != 13 && caracter != 32)
                    {
                        builder.Append((char)caracter);
                    }

                    position++;

                    if (caracter == 13)
                    {
                        lineOfError++;
                        columnOfError = 0;
                    }
                    else
                    {
                        columnOfError++;
                    }
                }

                stream.Close();

            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error while open file:" + e + "\n");
            }
            return null;

        }

        public static void GetLineOfColumn()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nLine: " + lineOfError + " Column: " + columnOfError);
            Console.ResetColor();
        }

        public static void AttrType(string id, string type)
        {
            simbolTable[id].Tipo = type;
        }

        public static bool VerifyIfExists(string lexema)
        {
            if (simbolTable.ContainsKey(lexema))
            {
                if (simbolTable[lexema].Tipo != " ")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
