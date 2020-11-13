using Analysers.Control;
using Analysers.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Analysers.Analyser
{
    public class SyntheticAnalyser
    {
        static int Iniciado = 0, IniciadoC = 0;
        static int ERRO = 0;
        static int Tx_18 = 0;
        static int Tx_25 = 0;

        public static Stack<DataControl> Pilha_SEM;

        public static void GetSyntheticAnalyser()
        {
            Pilha_SEM = new Stack<DataControl>();
            Stack<int> estado = new Stack<int>();
            int state;

            LexicoAnalyser.CompleteLexicoAnalyser();
            DataControl simbolo = LexicoAnalyser.GetLexicoAnalyser(0);

            estado.Push(0);

            string erroEncontrado;

            while (true)
            {
                state = estado.Peek();

                if (state == 132)
                {
                    estado.Pop();
                    state = estado.Peek();
                    erroEncontrado = GetError(state);

                    Console.WriteLine("\nErro encontrado no estado: " + state + " - " + erroEncontrado);
                    Console.Write("\n" + simbolo.Lexema);
                    LexicoAnalyser.GetLineOfColumn();

                    return;
                }

                if ((GetAction(state, simbolo.Token) > 0) && (GetAction(state, simbolo.Token) != 151))
                {
                    estado.Push(GetAction(state, simbolo.Token));
                    Pilha_SEM.Push(simbolo);
                    simbolo = LexicoAnalyser.GetLexicoAnalyser(LexicoAnalyser.position);
                }
                else if (GetAction(state, simbolo.Token) <= 0)
                {
                    int reduce = GetAction(state, simbolo.Token) * (-1);
                    string sentenca = GetSentence(reduce);

                    int qtdSimbolosBeta = 0;
                    bool isBeta = false;

                    StringBuilder nonTerminal = new StringBuilder();
                    string ALFA = "";
                    string BETA = "";

                    for (int i = 0; i < sentenca.Length; i++)
                    {
                        if (isBeta == false && sentenca[i] != '-')
                        {
                            ALFA += sentenca[i];
                        }

                        if (sentenca[i] == '-' && sentenca[i + 1] == '>')
                        {
                            isBeta = true;
                            i = i + 2;
                        }

                        if (isBeta == true)
                        {
                            BETA += sentenca[i];
                        }

                        if (sentenca[i] == ' ' && isBeta == true)
                        {
                            qtdSimbolosBeta++;
                        }
                    }

                    int j = 0;
                    
                    while (sentenca[j] != ' ')
                    {
                        nonTerminal.Append(sentenca[j]);
                        j++;
                    }
                    
                    for (int i = 0; i < qtdSimbolosBeta; i++)
                    {
                        estado.Pop();
                    }

                    state = estado.Peek();
                    estado.Push(GettingGoTo(state, nonTerminal.ToString()));

                    Console.WriteLine("\n*********************************************\n");
                    Console.WriteLine("\nSentenca reduzida: " + sentenca);

                    string[] K = ALFA.Split(' ');

                    CallSemantic(reduce, K[0], qtdSimbolosBeta);
                }
                else if (GetAction(state, simbolo.Token) == 151)
                {
                    Console.WriteLine("\nAccept!!!");
                    writeCPoint(Tx_18, Tx_25);
                    return;
                }
            }
        }

        public static void WriteFileTxt(string texto)
        {
            if (Iniciado == 0)
            {
                using (StreamWriter writer = new StreamWriter("C:\\Analysers\\Analysers\\Utils\\text2.txt"))
                {
                    writer.WriteLine(texto);
                    Iniciado = 1;
                    writer.Close();
                }
            }
            else if (Iniciado == 1)
            {
                using (StreamWriter writer = new StreamWriter("C:\\Analysers\\Analysers\\Utils\\text2.txt", true))
                {
                    writer.WriteLine(texto);
                    writer.Close();
                }
            }
        }

        public static void CallSemantic(int reduc, string ALFA, int qtdBETA)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Rules[reduc - 1]);
            Console.ResetColor();

            Dictionary<string, DataControl> Hash_SEM = new Dictionary<string, DataControl>();
            DataControl aux;

            if (reduc == 18 || reduc == 25)
            {
                aux = new DataControl(Pilha_SEM.Peek().Lexema, Pilha_SEM.Peek().Token, Pilha_SEM.Peek().Tipo);
                Pilha_SEM.Pop();
                Hash_SEM.Add("OPRD2", aux);

                aux = new DataControl(Pilha_SEM.Peek().Lexema, Pilha_SEM.Peek().Token, Pilha_SEM.Peek().Tipo);
                Pilha_SEM.Pop();
                Hash_SEM.Add(aux.Token, aux);

                aux = new DataControl(Pilha_SEM.Peek().Lexema, Pilha_SEM.Peek().Token, Pilha_SEM.Peek().Tipo);
                Pilha_SEM.Pop();
                Hash_SEM.Add("OPRD1", aux);
            }
            else
            {
                for (int i = 0; i < qtdBETA; i++)
                {
                    aux = new DataControl(Pilha_SEM.Peek().Lexema, Pilha_SEM.Peek().Token, Pilha_SEM.Peek().Tipo);
                    Hash_SEM.Add(aux.Token, aux);
                    Pilha_SEM.Pop();
                }
            }

            DataControl S = new DataControl("", ALFA, "");

            if (reduc == 5)
            {
                WriteFileTxt("\n\n\n");
            }
            else if (reduc == 6)
            {
                DataControl ID;
                ID = Hash_SEM["id"];
                DataControl TIPO;
                TIPO = Hash_SEM["TIPO"];

                if (TIPO.Tipo == "lit")
                {
                    WriteFileTxt("literal " + ID.Lexema + ";");
                }
                else if (TIPO.Tipo == "real")
                {
                    WriteFileTxt("double " + ID.Lexema + ";");
                }
                else
                {
                    WriteFileTxt(TIPO.Tipo + " " + ID.Lexema + ";");
                }

                LexicoAnalyser.AttrType(ID.Lexema, TIPO.Tipo);
            }
            else if (reduc == 7)
            {
                S.Tipo = "int";
            }
            else if (reduc == 8)
            {
                S.Tipo = "real";
            }
            else if (reduc == 9)
            {
                S.Tipo = "lit";
            }
            else if (reduc == 11)
            {
                DataControl ID = Hash_SEM["id"];

                if (LexicoAnalyser.VerifyIfExists(ID.Lexema))
                {
                    if (ID.Tipo == "lit")
                    {
                        WriteFileTxt("scanf(\"%s\",&" + ID.Lexema + ");");
                    }
                    else if (ID.Tipo == "int")
                    {
                        WriteFileTxt("scanf(\"%d\",&" + ID.Lexema + ");");
                    }
                    else if (ID.Tipo == "real")
                    {
                        WriteFileTxt("scanf(\"%f\",&" + ID.Lexema + ");");
                    }
                }
                else
                {
                    Console.WriteLine("Erro: Variável não declarada");
                    ERRO++;
                }
            }
            else if (reduc == 12)
            {
                DataControl ARG = Hash_SEM["ARG"];

                if (ARG.Tipo == "lit")
                {
                    WriteFileTxt("printf(\"%s\"," + ARG.Lexema + ");");
                }
                else if (ARG.Tipo == "real")
                {
                    WriteFileTxt("printf(\"%lf\"," + ARG.Lexema + ");");
                }
                else if (ARG.Tipo == "int")
                {
                    WriteFileTxt("printf(\"%d\"," + ARG.Lexema + ");");
                }
                else
                {
                    WriteFileTxt("printf(" + ARG.Lexema + ");");
                }
            }
            else if (reduc == 13)
            {
                DataControl literal = Hash_SEM["literal"];
                S.Lexema = literal.Lexema;
                S.Tipo = "literal";
            }
            else if (reduc == 14)
            {
                DataControl num = Hash_SEM["num"];
                S.Lexema = num.Lexema;
                S.Tipo = "num";
            }
            else if (reduc == 15)
            {
                DataControl ID = Hash_SEM["id"];

                if (LexicoAnalyser.VerifyIfExists(ID.Lexema))
                {
                    S.Lexema = ID.Lexema;
                    S.Tipo = ID.Tipo;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n-->Erro: Variável não declarada.\n");
                    LexicoAnalyser.GetLineOfColumn();
                    Console.ResetColor();
                    ERRO++;
                }
            }
            else if (reduc == 17)
            {
                DataControl ID = Hash_SEM["id"];

                if (LexicoAnalyser.VerifyIfExists(ID.Lexema))
                {
                    DataControl LD = Hash_SEM["LD"];
                    if ((ID.Tipo == LD.Tipo) || (ID.Tipo == "real" && (LD.Tipo == "num" || LD.Tipo == "int")))
                    {
                        DataControl rcb = Hash_SEM["rcb"];
                        rcb.Tipo = "=";
                        WriteFileTxt(ID.Lexema + " " + rcb.Tipo + " " + LD.Lexema + ";");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n-->Erro: Tipos diferentes para atribuição.\n");
                        LexicoAnalyser.GetLineOfColumn();
                        Console.ResetColor();
                        ERRO++;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n-->Erro: Variável não declarada.\n");
                    LexicoAnalyser.GetLineOfColumn();
                    Console.ResetColor();
                    ERRO++;
                }
            }
            else if (reduc == 18)
            {
                string tx = "T" + Tx_18;

                DataControl OPRD1 = Hash_SEM["OPRD1"];
                DataControl OPRD2 = Hash_SEM["OPRD2"];

                string[] tipos_equivalentes = { "num", "real", "int" };

                if (OPRD1.Tipo != "lit" && OPRD2.Tipo != "lit")
                {
                    S.Lexema = tx;
                    S.Tipo = "int";
                    Tx_18 += 1;

                    DataControl opm = Hash_SEM["opm"];
                    WriteFileTxt(tx + " = " + OPRD1.Lexema + " " + opm.Lexema + " " + OPRD2.Lexema + ";");
                }
                else
                {
                    S.Lexema = tx;
                    S.Tipo = "num";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("-->Erro: Operandos com tipos incompatíveis.");
                    LexicoAnalyser.GetLineOfColumn();
                    Console.ResetColor();
                    ERRO++;
                }

            }
            else if (reduc == 19)
            {
                DataControl OPRD = Hash_SEM["OPRD"];
                S.Lexema = OPRD.Lexema;
                S.Tipo = OPRD.Tipo;
            }
            else if (reduc == 20)
            {
                DataControl ID = Hash_SEM["id"];

                if (LexicoAnalyser.VerifyIfExists(ID.Lexema))
                {
                    S.Lexema = ID.Lexema;
                    S.Tipo = ID.Tipo;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("-->Erro: Variável não declarada.");
                    LexicoAnalyser.GetLineOfColumn();
                    Console.ResetColor();
                    ERRO++;
                }
            }
            else if (reduc == 21)
            {
                DataControl num = Hash_SEM["num"];
                S.Lexema = num.Lexema;
                S.Tipo = num.Token;
            }
            else if (reduc == 23)
            {
                WriteFileTxt("}");
            }
            else if (reduc == 24)
            {
                DataControl EXP_R = Hash_SEM["EXP_R"];
                WriteFileTxt("if(" + EXP_R.Lexema + "){");
            }
            else if (reduc == 25)
            {
                DataControl OPRD1 = Hash_SEM["OPRD1"];
                DataControl OPRD2 = Hash_SEM["OPRD2"];

                if ((OPRD1.Tipo == "lit" && OPRD2.Tipo == "lit") || (OPRD1.Tipo != "lit" && OPRD2.Tipo != "lit"))
                {
                    string tx = "T" + Tx_25;
                    S.Lexema = tx;
                    S.Tipo = "boolean";
                    Tx_25++;

                    DataControl opr = Hash_SEM["opr"];

                    if (opr.Lexema == "==")
                    {
                        WriteFileTxt(tx + " = " + OPRD1.Lexema + " == " + OPRD2.Lexema + ";");
                    }
                    else if (opr.Lexema == "<>")
                    {
                        WriteFileTxt(tx + " = " + OPRD1.Lexema + " != " + OPRD2.Lexema + ";");
                    }
                    else
                    {
                        WriteFileTxt(tx + " = " + OPRD1.Lexema + " " + opr.Lexema + " " + OPRD2.Lexema + ";");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("-->Erro: Operandos com tipos incompatíveis.");
                    LexicoAnalyser.GetLineOfColumn();
                    Console.ResetColor();
                    ERRO++;
                }
            }
            Pilha_SEM.Push(S);
        }

        public static string GetSentence(int line)
        {
            EnumControl[] Enumeracao = new EnumControl[30];

            Enumeracao[0] = new EnumControl(1, "P' -> P");
            Enumeracao[1] = new EnumControl(2, "P -> inicio V A");
            Enumeracao[2] = new EnumControl(3, "V -> varinicio LV");
            Enumeracao[3] = new EnumControl(4, "LV -> D LV");
            Enumeracao[4] = new EnumControl(5, "LV -> varfim ;");
            Enumeracao[5] = new EnumControl(6, "D -> id TIPO ;");
            Enumeracao[6] = new EnumControl(7, "TIPO -> int");
            Enumeracao[7] = new EnumControl(8, "TIPO -> real");
            Enumeracao[8] = new EnumControl(9, "TIPO -> lit");
            Enumeracao[9] = new EnumControl(10, "A -> ES A");
            Enumeracao[10] = new EnumControl(11, "ES -> leia id ;");
            Enumeracao[11] = new EnumControl(12, "ES -> escreva ARG ;");
            Enumeracao[12] = new EnumControl(13, "ARG -> literal");
            Enumeracao[13] = new EnumControl(14, "ARG -> num");
            Enumeracao[14] = new EnumControl(15, "ARG -> id");
            Enumeracao[15] = new EnumControl(16, "A -> CMD A");
            Enumeracao[16] = new EnumControl(17, "CMD -> id rcb LD ;");
            Enumeracao[17] = new EnumControl(18, "LD -> OPRD opm OPRD");
            Enumeracao[18] = new EnumControl(19, "LD -> OPRD");
            Enumeracao[19] = new EnumControl(20, "OPRD -> id");
            Enumeracao[20] = new EnumControl(21, "OPRD -> num");
            Enumeracao[21] = new EnumControl(22, "A -> COND A");
            Enumeracao[22] = new EnumControl(23, "COND -> CABECALHO CORPO");
            Enumeracao[23] = new EnumControl(24, "CABECALHO -> se ( EXP_R ) entao");
            Enumeracao[24] = new EnumControl(25, "EXP_R -> OPRD opr OPRD");
            Enumeracao[25] = new EnumControl(26, "CORPO -> ES CORPO");
            Enumeracao[26] = new EnumControl(27, "CORPO -> CMD CORPO");
            Enumeracao[27] = new EnumControl(28, "CORPO -> COND CORP");
            Enumeracao[28] = new EnumControl(29, "CORPO -> fimse");
            Enumeracao[29] = new EnumControl(30, "A -> fim");

            return Enumeracao[line - 1].Sentence;
        }

        public static string GetError(int line)
        {
            ErrorTable tabelaerrosintatico = new ErrorTable();
            tabelaerrosintatico.hashErrorTable.Add(0, "E0 - Codigo inicializado de forma incorreta - inicio");
            tabelaerrosintatico.hashErrorTable.Add(2, "E2 - Inicio de declaracao de varaveis nao declarado - varfim");
            tabelaerrosintatico.hashErrorTable.Add(3, "E3 - Tipos de dados, elementos de estrutura condicional (entao e fimse), delimitacao de declaracao de variaveis, atribuiao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(4, "E4 - Espaco para declaracao de variaveis: Necessario declaracao das variaveis, nao sendo permitidos atribuicoes, operacoes, estruturas condicionais, parenteses ou ponto e virgula");
            tabelaerrosintatico.hashErrorTable.Add(6, "E6 - Tipos de dados, elementos de estrutura condicional (entao e fimse), delimitacao de declaracao de variaveis, atribuiao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(7, "E7 - Tipos de dados, elementos de estrutura condicional (entao e fimse), delimitacao de declaracao de variaveis, atribuiao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(8, "E8 - Tipos de dados, elementos de estrutura condicional (entao e fimse), delimitacao de declaracao de variaveis, atribuiao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(9, "E9 - Tipos de dados, elementos de estrutura condicional (entao e fim), delimitacao de declaracao de variaveis, atribuicao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(11, "E11 - Leitura de variaveis: esperavasse um identificador");
            tabelaerrosintatico.hashErrorTable.Add(12, "E12 - Escrita de variaveis: esperavasse um identificador, um inteiro ou um literal");
            tabelaerrosintatico.hashErrorTable.Add(13, "E13 - Atribuicao de variaveis: esperavasse uma atribuicao");
            tabelaerrosintatico.hashErrorTable.Add(14, "E14 - Inicio de expressao do condicional SE: esperavasse um abre parenteses");
            tabelaerrosintatico.hashErrorTable.Add(16, "E16 - Espaco para declaracao de variaveis: esperavasse um varfim ou um identificador");
            tabelaerrosintatico.hashErrorTable.Add(17, "E17 - Final de atribuicao, expressao, escrita, leitura ou espaco para declaracao de variaveis: esperavasse um ponto e virgula");
            tabelaerrosintatico.hashErrorTable.Add(18, "E18 - Espaco para declaracao de variaveis: esperavasse um tipo para o identificador");
            tabelaerrosintatico.hashErrorTable.Add(23, "E23 - Tipos de dados, elementos de estrutura condicional (entao e fim), delimitacao de declaracao de variaveis, atribuicao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(24, "E24 - Tipos de dados, elementos de estrutura condicional (entao e fim), delimitacao de declaracao de variaveis, atribuicao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(25, "E25 - Tipos de dados, elementos de estrutura condicional (entao e fim), delimitacao de declaracao de variaveis, atribuicao, operadores (relacionais e aritmeticos), inicio de programa e declaracao de variaveis, parenteses ou ponto e virgula nao permitidos");
            tabelaerrosintatico.hashErrorTable.Add(27, "E27 - Final de atribuicao, expressao, escrita, leitura ou espaco para declaracao de variaveis: esperavasse um ponto e virgula");
            tabelaerrosintatico.hashErrorTable.Add(28, "E28 - Final de atribuicao, expressao, escrita, leitura ou espaco para declaracao de variaveis: esperavasse um ponto e virgula");
            tabelaerrosintatico.hashErrorTable.Add(32, "E32 - Atribuicao de variaveis ou expressao aritmetica: esperavasse um identificador ou um numeral");
            tabelaerrosintatico.hashErrorTable.Add(33, "E33 - Atribuicao de variaveis ou expressao relacional: esperavasse um identificador ou um numeral");
            tabelaerrosintatico.hashErrorTable.Add(36, "E36 - Final de atribuicao, expressao, escrita, leitura ou espaco para declaracao de variaveis: esperavasse um ponto e virgula");
            tabelaerrosintatico.hashErrorTable.Add(44, "E44 - Final de atribuicao, expressao, escrita, leitura ou espaco para declaracao de variaveis: esperavasse um ponto e virgula");
            tabelaerrosintatico.hashErrorTable.Add(45, "E45 - Expressao aritmetica: esperavasse um operador aritmetica");
            tabelaerrosintatico.hashErrorTable.Add(48, "E48 - Fim de expressao do condicional SE: esperavasse um fecha parenteses");
            tabelaerrosintatico.hashErrorTable.Add(49, "E49 - Expressao relacional: esperavasse um operador relacional");
            tabelaerrosintatico.hashErrorTable.Add(53, "E53 - Expressao aritmetica: esperavasse um identificador ou um numeral");
            tabelaerrosintatico.hashErrorTable.Add(54, "E54 - Estrutura condicional SE: esperavasse um entao");
            tabelaerrosintatico.hashErrorTable.Add(57, "E57 - Expressao relacional: esperavasse um identificador ou um numeral");

            return (string) tabelaerrosintatico.hashErrorTable[line];
        }

        public static int GettingGoTo(int line, string nonTerminal)
        {
            int[] G = {
                128, 80, 86, 65,133, 68,134,135,136,137,138,139,140,141,142,143,
                0  ,  1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                1  ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                2  ,  0,  3,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                3  ,  0,  0,  5,  0,  0,  0,  6,  0,  7,  0,  0,  8,  9,  0,  0,
                4  ,  0,  0,  0, 15, 16,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                5  ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                6  ,  0,  0, 19,  0,  0,  0,  6,  0,  7,  0,  0,  8,  9,  0,  0,
                7  ,  0,  0, 20,  0,  0,  0,  6,  0,  7,  0,  0,  8,  9,  0,  0,
                8  ,  0,  0, 21,  0,  0,  0,  6,  0,  7,  0,  0,  8,  9,  0,  0,
                9  ,  0,  0,  0,  0,  0,  0, 23,  0, 24,  0,  0, 25,  9, 22,  0,
                10 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                11 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                12 ,  0,  0,  0,  0,  0,  0,  0, 28,  0,  0,  0,  0,  0,  0,  0,
                13 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                14 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                15 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                16 ,  0,  0,  0, 34, 16,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                17 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                18 ,  0,  0,  0,  0,  0, 36,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                19 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                20 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                21 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                22 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                23 ,  0,  0,  0,  0,  0,  0, 23,  0, 24,  0,  0, 25,  9, 40,  0,
                24 ,  0,  0,  0,  0,  0,  0, 23,  0, 24,  0,  0, 25,  9, 41,  0,
                25 ,  0,  0,  0,  0,  0,  0, 23,  0, 24,  0,  0, 25,  9, 50,  0,
                26 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                27 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                28 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                29 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                30 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                31 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                32 ,  0,  0,  0,  0,  0,  0,  0,  0,  0, 44, 45,  0,  0,  0,  0,
                33 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 49,  0,  0,  0, 48,
                34 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                35 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                36 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                37 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                38 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                39 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                40 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                41 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                42 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                43 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                44 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                45 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                46 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                47 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                48 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                49 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                50 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                51 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                52 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                53 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 55,  0,  0,  0,  0,
                54 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                55 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                56 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
                57 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 58,  0,  0,  0,  0,
                58 ,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            };

            SyntheticTable[,] tabelaGOTO = new SyntheticTable[60, 16];

            int t = 0;

            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    tabelaGOTO[i, j] = new SyntheticTable(G[t]);
                    t++;
                }
            }

            int word = 0;
            int column = 0;

            switch (nonTerminal)
            {
                case "LV":
                    word = 133;
                    break;

                case "TIPO":
                    word = 134;
                    break;

                case "ES":
                    word = 135;
                    break;

                case "ARG":
                    word = 136;
                    break;

                case "CMD":
                    word = 137;
                    break;

                case "LD":
                    word = 138;
                    break;

                case "OPRD":
                    word = 139;
                    break;

                case "COND":
                    word = 140;
                    break;

                case "CABECALHO":
                    word = 141;
                    break;

                case "CORPO":
                    word = 142;
                    break;

                case "EXP_R":
                    word = 143;
                    break;

                case "P":
                    word = 80;
                    break;

                case "V":
                    word = 86;
                    break;

                case "A":
                    word = 65;
                    break;

                case "D":
                    word = 68;
                    break;
            }

            for (int i = 0; i < 16; i++)
            {
                if (tabelaGOTO[0, i].Element == word)
                {
                    column = i;
                }
            }

            return tabelaGOTO[line + 1, column].Element;
        }

        public static int GetAction(int line, string terminal)
        {
            int[] A = {
                128, 133, 134, 135,  59, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146,  40,  41, 147, 148, 149, 150,  36,
                  0,   2, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                  1, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 151,
                  2, 132,   4, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                  3, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132, 132,  10, 132,
                  4, 132, 132,  17, 132, 132, 132, 132, 132,  18, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                  5,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,  -2,
                  6, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132, 132,  10, 132,
                  7, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132, 132,  10, 132,
                  8, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132, 132,  10, 132,
                  9, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132,  26, 132, 132,
                 10, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30, -30,
                 11, 132, 132, 132, 132, 132, 132, 132, 132,  27, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 12, 132, 132, 132, 132, 132, 132, 132, 132,  31, 132,  29,  30, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 13, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,  32, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 14, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,  33, 132, 132, 132, 132, 132, 132,
                 15,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,  -3,
                 16, 132, 132,  17, 132, 132, 132, 132, 132,  18, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 17, 132, 132, 132,  35, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 18, 132, 132, 132, 132,  37,  38, 132, 132, 132, 132,  39, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 19, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10,
                 20, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16, -16,
                 21, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22, -22,
                 22, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23, -23,
                 23, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132,  26, 132, 132,
                 24, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132,  26, 132, 132,
                 25, 132, 132, 132, 132, 132, 132, 132,  11,  13,  12, 132, 132, 132, 132,  14, 132, 132, 132, 132,  26, 132, 132,
                 26, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29, -29,
                 27, 132, 132, 132,  42, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 28, 132, 132, 132,  43, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 29, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13, -13,
                 30, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14, -14,
                 31, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15, -15,
                 32, 132, 132, 132, 132, 132, 132, 132, 132,  46, 132, 132,  47, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 33, 132, 132, 132, 132, 132, 132, 132, 132,  46, 132, 132,  47, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 34,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,  -4,
                 35,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,  -5,
                 36, 132, 132, 132,  51, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 37,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,  -7,
                 38,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,  -8,
                 39,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,  -9,
                 40, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26, -26,
                 41, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27, -27,
                 42, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11, -11,
                 43, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12, -12,
                 44, 132, 132, 132,  52, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 45, 132, 132, 132, -19, 132, 132, 132, 132, 132, 132, 132, 132, 132,  53, 132, 132, 132, 132, 132, 132, 132, 132,
                 46, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20, -20,
                 47, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21, -21,
                 48, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,  54, 132, 132, 132, 132, 132,
                 49, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,  57, 132, 132, 132,
                 50, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28, -28,
                 51,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,  -6,
                 52, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17, -17,
                 53, 132, 132, 132, 132, 132, 132, 132, 132,  46, 132, 132,  47, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 54, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,  56, 132, 132, 132, 132,
                 55, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18, -18,
                 56, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24, -24,
                 57, 132, 132, 132, 132, 132, 132, 132, 132,  46, 132, 132,  47, 132, 132, 132, 132, 132, 132, 132, 132, 132, 132,
                 58, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25, -25
            };

            SyntheticTable[,] actionTable = new SyntheticTable[60, 23];
            int t = 0;

            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 23; j++)
                {
                    actionTable[i, j] = new SyntheticTable(A[t]);
                    t++;
                }
            }

            int word = 0;
            int column = 0;

            switch (terminal)
            {
                case "inicio":
                    word = 133;
                    break;

                case "varinicio":
                    word = 134;
                    break;

                case "varfim":
                    word = 135;
                    break;

                case "inteiro":
                    word = 136;
                    break;

                case "real":
                    word = 137;
                    break;

                case "lit":
                    word = 138;
                    break;

                case "leia":
                    word = 139;
                    break;

                case "escreva":
                    word = 141;
                    break;

                case "literal":
                    word = 142;
                    break;

                case "num":
                    word = 143;
                    break;

                case "rcb":
                    word = 144;
                    break;

                case "opm":
                    word = 145;
                    break;

                case "se":
                    word = 146;
                    break;

                case "entao":
                    word = 147;
                    break;

                case "opr":
                    word = 148;
                    break;

                case "fimse":
                    word = 149;
                    break;

                case "fim":
                    word = 150;
                    break;

                case "pt_v":
                    word = 59;
                    break;

                case "ab_p":
                    word = 40;
                    break;

                case "fc_p":
                    word = 41;
                    break;

                case "EOF":
                    word = 36;
                    break;

                case "id":
                    word = 140;
                    break;
            }

            for (int i = 0; i < 23; i++)
            {
                if (actionTable[0, i].Element == word)
                {
                    column = i;
                }
            }

            return actionTable[line + 1, column].Element;
        }

        public static void WriteSecondText(string texto)
        {
            if (IniciadoC == 0)
            {
                using (StreamWriter writer = new StreamWriter("C:\\Analysers\\Analysers\\Utils\\Program.c")) 
                {
                    writer.WriteLine(texto);
                    IniciadoC = 1;
                    writer.Close();

                }
            }
            else if (IniciadoC == 1)
            {
                using (StreamWriter writer = new StreamWriter("C:\\Analysers\\Analysers\\Utils\\Program.c", true)) 
                {
                    writer.WriteLine(texto);
                    writer.Close();
                }
            }
        }

        public static void writeCPoint(int tx_18, int tx_25)
        {
            var linhas = File.ReadAllLines("C:\\Analysers\\Analysers\\Utils\\text2.txt");

            WriteSecondText("#include<stdio.h>");
            WriteSecondText("typedef char literal[256];");
            WriteSecondText("void main(void){");
            WriteSecondText("/*----Variaveis temporarias----*/");

            for (int i = 0; i < tx_18; i++)
            {
                WriteSecondText("int T" + i + ";");
            }
            for (int i = tx_18; i < tx_18 + tx_25; i++)
            {
                WriteSecondText("int T" + i + ";");
            }

            WriteSecondText("/*------------------------------*/");

            string[] k = File.ReadAllLines("C:\\Analysers\\Analysers\\Utils\\text2.txt"); 

            foreach (string value in k)
            {
                WriteSecondText(value);
            }

            WriteSecondText("}");
        }

        public static string[] Rules = {
            "-",
            "-",
            "-",
            "-",
            "Imprimir três linhas brancas no arquivo objeto;",
            "id.tipo <- TIPO.tipo\nImprimir ( TIPO.tipo id.lexema ; )",
            "TIPO.tipo <- inteiro.tipo",
            "TIPO.tipo <- real.tipo",
            "TIPO.tipo <- literal.tipo",
            "-",
            "Verificar se o campo tipo do identificador está preenchido indicando a declaração do identificador (execução da regra semântica de número 6).\nSe sim, então:\n\t Se id.tipo = literal Imprimir ( scanf(“%s”, id.lexema); )\n\t Se id.tipo = inteiro Imprimir ( scanf(“%d”, &id.lexema); )\n\t Se id.tipo = real Imprimir ( scanf(“%lf”, &id.lexema); )\nCaso Contrário:\n\tEmitir na tela “Erro: Variável não declarada”.",
            "Gerar código para o comando escreva no arquivo objeto.\nImprimir ( printf(“ARG.lexema”); )",
            "ARG.atributos <- literal.atributos (Copiar todos os atributos de literal para os atributos de ARG).",
            "ARG.atributos <- num.atributos (Copiar todos os atributos de literal para os atributos de ARG).",
            "Verificar se o identificador foi declarado (execução da regra semântica de número 6).\nSe sim, então:\n\tARG.atributos <- id.atributos (copia todos os atributos de id para os de ARG).\nCaso Contrário:\n\tEmitir na tela “Erro: Variável não declarada”",
            "-",
            "Verificar se id foi declarado (execução da regra semântica de número 6).\nSe sim, então:\n\tRealizar verificação do tipo entre os operandos id e LD (ou seja, se ambos são do mesmo tipo).\n\tSe sim, então:\n\t\tImprimir (id.lexema rcb.tipo LD.lexema) no arquivo objeto.\n\tCaso contrário emitir:”Erro: Tipos diferentes para atribuição”.\nCaso contrário emitir “Erro: Variável não declarada”.",
            "Verificar se tipo dos operandos são equivalentes e diferentes de literal.\nSe sim, então:\n\tGerar uma variável numérica temporária Tx, em que x é um número gerado sequencialmente.\nLD.lexema <- Tx\nImprimir (Tx = OPRD.lexema opm.tipo OPRD.lexema) no arquivo objeto.\nCaso contrário emitir “Erro: Operandos com tipos incompatíveis”.",
            "LD.atributos <- OPRD.atributos (Copiar todos os atributos de OPRD para os atributos de LD).",
            "Verificar se o identificador está declarado.\nSe sim, então:\n\tOPRD.atributos	<- id.atributos\nCaso contrário emitir “Erro: Variável não declarada”",
            "OPRD.atributos	<- num.atributos (Copiar todos os atributos de num para os atributos de OPRD).",
            "-",
            "Imprimir ( } ) no arquivo objeto.",
            "Imprimir ( if (EXP_R.lexema) { ) no arquivo objeto.",
            "Verificar se os tipos de dados de OPRD são iguais ou equivalentes para a realização de comparação relacional.\nSe sim, então:\n\tGerar uma variável booleana temporária Tx, em que x é um número gerado sequencialmente.\n\tEXP_R.lexema <-	Tx\n\tImprimir (Tx = OPRD.lexema opr.tipo OPRD.lexema) no arquivo objeto.\nCaso contrário emitir “Erro: Operandos com tipos incompatíveis”.",
            "-",
            "-",
            "-",
            "-",
            "-"
        };
    }
}
