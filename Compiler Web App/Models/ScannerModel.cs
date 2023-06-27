using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace compiler_projecto.Models
{
    public class ScannerModel
    {
        static int[,] TransTable = new int[200, 100];
        static int[] State = new int[200];
        static string[] ReturnToken = new string[200];
        public const int SIZEOFCHAR = (int)2e2, ASCINUMBER = 300, SIZEOFID = (int)1e4;
        static int[,] ReservedWords = new int[SIZEOFCHAR, ASCINUMBER];
        static string[] AllReservedWord = new string[SIZEOFCHAR];
        static int idxword = 0;
        static int prvidx = 0;
        static int curidx = 0;
        static int[,] ParserTable = new int[SIZEOFCHAR, ASCINUMBER];
        static IDictionary<string, int> mydictionary = new Dictionary<string, int>();
        static string[] parserAccState = new string[200];
        static void pre()
        {
            for (int i = 0; i < 200; i++) ReturnToken[i] = "Erorr";
            for (char i = 'a'; i <= 'z'; i++) State[i] = 1;
            for (char i = 'A'; i <= 'Z'; i++) State[i] = 1;
            for (char i = '0'; i <= '9'; i++) State[i] = 2;
            int idx = 3;
            State[' '] = 4;
            foreach (string line in System.IO.File.ReadLines(@"f:\compiler\IDXOFCHAR.txt"))
            {
                State[line[0]] = idx;
                idx++;
                if (idx == 4) idx++;
            }
            State['\t'] = 4;
            State['\n'] = idx;
            State[','] = 28;
        }
        static void Precompile()
        {
            pre();
            int co = 0;
            foreach (string line in System.IO.File.ReadLines(@"f:\compiler\TRANSTABLE.txt"))
            {
                int num = 0, row = 1;
                foreach (char j in line)
                {
                    int idxt = State[j];
                    if (idxt != 4)
                    {
                        num *= 10; num += (j - '0');
                    }
                    else if (idxt == 4)
                    {
                        TransTable[co, row] = num;
                        row++;
                        num = 0;
                    }
                }
                TransTable[co, row] = num;
                co++;
            }
        }
        public static void AddWords(string str, string Type)
        {
            int curidx = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int goidx = ReservedWords[curidx, str[i]];
                if (goidx == 0)
                {
                    ReservedWords[curidx, str[i]] = ++idxword; goidx = idxword;
                }
                curidx = goidx;
            }
            AllReservedWord[curidx] = Type;
        }
        public static bool Check(char x)
        {

            if (x == ' ') return false;
            return true;
        }
        public static void PreCompile()
        {
            foreach (string line in System.IO.File.ReadLines(@"f:\compiler\CompilerProject.txt"))
            {
                int flag = 0;
                string Word = "", Type = "";
                foreach (char j in line)
                {
                    if (Check(j) && flag == 0) Word += j;
                    else if (flag == 0) { flag = 1; continue; }
                    if (flag == 1) Type += j;
                }
                AddWords(Word, Type);
            }
        }
        public static string FindWords(string str)
        {
            int curidx = 0;
            string str2 = "";
            foreach (char i in str)
            {
                if (i == ' ') continue;
                str2 += i;
            }
            for (int i = 0; i < str2.Length; i++)
            {
                curidx = ReservedWords[curidx, str2[i]];
                if (curidx == 0)
                {
                    return null;
                }
            }
            return AllReservedWord[curidx];
        }
        static int DFA(char s)
        {
            int idxgo = State[s];
            if (s == '\n') idxgo = 27;
            if (TransTable[curidx, idxgo] == 0 && idxgo != 4) return 2;
            prvidx = curidx;
            curidx = TransTable[curidx, idxgo];
            if (curidx == 130) return 1;
            return 0;
        }

        static void parserread()
        {
            foreach (string line in System.IO.File.ReadLines(@"f:\compiler\parserwords.txt"))
            {
                string word = "";
                int num = 0, flag = 0;
                foreach (char i in line)
                {

                    if (i != ' ' && flag == 0) word += i;
                    else if (flag == 0) flag = 1;
                    else num = i - '0';
                }
                mydictionary.Add(word, num);
            }
            mydictionary.Add("Arithmetic Operation", 9);
            mydictionary.Add("Logic Operation", 9);
            mydictionary.Add("relational operation", 9);
            mydictionary.Add("Access Operator", 9);
            mydictionary.Add("Quotation Mark", 9);
            int co = 0;
            foreach (string line in System.IO.File.ReadLines(@"f:\compiler\Parsetable.txt"))
            {
                int num = 0, row = 1;
                foreach (char j in line)
                {
                    int idxt = State[j];
                    if (idxt != 4)
                    {
                        num *= 10; num += (j - '0');
                    }
                    else if (idxt == 4)
                    {
                        ParserTable[co, row] = num;
                        row++;
                        num = 0;
                    }
                }
                ParserTable[co, row] = num;
                co++;
            }
            foreach (string line in System.IO.File.ReadLines(@"f:\compiler\ParserTableAcState.txt"))
            {
                string AccState = ""; int num = 0, flag = 0;
                foreach (char j in line)
                {
                    if (j != ' ' && flag == 0) num = j - '0';
                    else if (flag == 1 && j != ' ') AccState += j;
                    else flag = 1;
                }
                parserAccState[num] = AccState;
            }
        }

        static int curidxpraser = 0, PrvParseridx = 0;
        static int parserfind(string str)
        {

            int idxgo = mydictionary[str];
            PrvParseridx = curidxpraser;
            curidxpraser = ParserTable[curidxpraser, idxgo];
            if (curidxpraser == 0) return 2;
            if (curidxpraser == 130) return 1;
            return 0;

        }
        static public string Main(List<string> args, bool f1)
        {
            PreCompile();
            Precompile();
            parserread();
            int LineNumber = 0;
            string ansscanner = "", ansparser = "";
            string str = "", str2 = "";
            do
            {
                string line = args[LineNumber];
                LineNumber++;
                int idx = 0;
                int maxret = 0;
                str = "";
                curidxpraser = 0;
                foreach (char i in line)
                {
                    idx++;
                    int ret = DFA(i);
                    str += i;
                    str2 += i;
                    if (ret > maxret) maxret = ret;
                    string BackToken = FindWords(str2);
                    if (BackToken != null)
                    {
                        int stateparser = parserfind(BackToken);
                        if (stateparser == 1)
                        {
                            ansparser += "Matched with ";
                            ansparser += parserAccState[PrvParseridx];
                            ansparser += '\n';
                            curidxpraser = 0;
                        }
                        str2 = "";
                    }
                    if (ret == 1)
                    {
                        string BackToken1 = FindWords(str);
                        if (BackToken1 != null && prvidx != 9 && maxret != 2)
                        {
                            ansscanner += ("Line : " + LineNumber + " Token Text:" + str + " Token Type: " + BackToken1);
                            ansscanner += '\n';
                        }
                        else if (prvidx == 1)
                        {
                            parserfind("ID");
                            ansscanner += ("Line : " + LineNumber + " Token Text:" + str + " Token Type: " + "Identifier");
                            ansscanner += '\n';
                        }
                        else if (prvidx != 7 && prvidx != 10 && maxret != 2)
                        {
                            
                            ansscanner += ("Line : " + LineNumber + " Token Text:" + str + " Token Type: " + "Constatnt");
                            ansscanner += '\n';
                        }
                        else if (maxret != 2)
                        {
                            ansparser += ("Matched with comment \n");
                            curidxpraser = 0;
                            ansscanner += ("Line : " + LineNumber + " Token Text:" + " Token Type: " + "comment");
                            ansscanner += '\n';
                        }
                        else
                        {
                            ansscanner += ("Line : " + LineNumber + " Token in " + str + " Token Is erorr");
                            ansscanner += '\n';
                        }
                        curidx = 0;
                        str = "";
                        maxret = 0;
                        str2 = "";
                    }
                }
            } while (LineNumber < args.Count);
            return f1 ? ansscanner : ansparser;
        }
    }

}

/*
 /$ This is main function $/ 
Worthless decrease ( ) { 
int 3num = 5 ; 
Loopwhen ( counter < num ) { 
reg3 = reg3 - 1 ; } } 
*/
