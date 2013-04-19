using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Server
{
    class SplitPageInterpreter
    {
        static List<Variable> Vars = new List<Variable>();

        public static string GetHTMLFromSSHTML(string page, /*ref*/ WebServer srv)
        {
            #region OLD
            /*string pg = page;
            string pg2 = "";
            //pg = pg.Replace("TEST", "TESTSPLIT");
            while (pg2.Contains("<s#c") || pg2.Contains("<s#"))
            {
                bool s = false;
                bool s1 = false;
                bool s2 = false;
                bool s3 = false;
                bool split = false;
                bool splitc = false;
                int j1 = 0;
                int j2 = 0;
                string splitcode = "";
                for (int i = 0; i < pg.Length; i++)
                {
                    if (split)
                    {
                        if (pg[i] == 's')
                        {
                            s = true;
                        }
                        else if (pg[i] == '#')
                        {
                            if (s)
                                s1 = true;
                        }
                        else if (pg[i] == '>')
                        {
                            if (s1)
                            {
                                s3 = true;
                                split = false;
                                s = false;
                                s1 = false;
                                s2 = false;
                                s3 = false;
                            }
                        }
                        else
                        {
                            if (s2)
                            {
                                s3 = true;
                                split = true;
                                s = false;
                                s1 = false;
                                s2 = false;
                                s3 = false;
                            }
                        }
                    }
                    else if (splitc)
                    {
                        if (pg[i] == 's')
                        {
                            s = true;
                        }
                        else if (pg[i] == '#')
                        {
                            if (s)
                                s1 = true;
                        }
                        else if (pg[i] == '>')
                        {
                            if (s2)
                            {
                                s3 = true;
                                split = false;
                                s = false;
                                s1 = false;
                                s2 = false;
                                s3 = false;
                            }
                        }
                        else
                        {
                            if (s2)
                            {
                                s3 = true;
                                split = true;
                                s = false;
                                s1 = false;
                                s2 = false;
                                s3 = false;
                            }
                        }
                    }
                    else
                    {
                        if (pg[i] == '<')
                        {
                            s = true;
                        }
                        else if (pg[i] == 's')
                        {
                            if (s)
                                s1 = true;
                        }
                        else if (pg[i] == '#')
                        {
                            if (s1)
                                s2 = true;
                        }
                        else if (pg[i] == 'c')
                        {
                            if (s2)
                            {
                                s3 = true;
                                splitc = true;
                                s = false;
                                s1 = false;
                                s2 = false;
                                s3 = false;
                            }
                        }
                        else
                        {
                            pg2 += pg[i];
                        }
                    }
                }
            }
            return pg2;*/
            #endregion
            string pg = page;

            // Thanks to Josh for this good parser!


            // Variables //  
            String[] lines = new String[pg.Replace("\n", "").Split('\r').Length];
            bool parse = false;
            string DOCUMENT = "";
            // End Vars //

            lines = pg.Replace("" + (char)10, "").Split((char)13); //Read inputted text file.

            for (int i = 0; i < lines.Length; i++)
            {
                for (int x = 0; x < lines[i].Length/* - 1*/; x++)
                {

                    if (lines[i][x] == '<' && lines[i][x + 1] == 's' && lines[i][x + 2] == '#')
                    {
                        parse = true; //Begin parsing.
                    }
                    else if (lines[i][x] == '#' && lines[i][x + 1] == 's' && lines[i][x + 2] == '>')
                    {
                        parse = false; //End parsing.
                    }

                    if (parse)
                    {
                       
                    }
                    else
                    {
                        if (lines[i][x] == '>')
                        {
                            //MessageBox.Show("" + lines[i][x]);
                            DOCUMENT = DOCUMENT + lines[i][x]; //Add char to document.
                        }
                        else
                        {
                            DOCUMENT = DOCUMENT + lines[i][x]; //Add char to document.
                        }
                    }
                }

                if (parse)
                {
                    //This will parse line by line.
                    //Add the result of the parsing to the DOCUMENT var.
                    string c = lines[i];
                    c = c.Replace("<s#", "");
                    c = c.Replace("s#>", "");
                    if (c.Split(';').Count > 0)
                    {

                    }
                    if (c.StartsWith("var:"))
                    {
                        if (!c.EndsWith(";"))
                        {
                            srv.SendLog("Error in Split# code!");
                            DOCUMENT += "<h1>Split# Error</h1>\n";
                            DOCUMENT += "<h2>Line " + (i + 1) + ": Unexpected ';'</h2>";
                            parse = false;
                            continue;
                        }
                        else
                        {
                            if (c.Contains("="))
                            {
                                string type = "";
                                string name = "";
                                string value = "";

                                string left = "";
                                string right = "";
                                left = c.Split(' ')[0];
                                right = c.Split(' ')[1];

                                type = c.Split(' ')[0].Split(':')[1];
                                name = c.Split(' ')[1];
                                value = c.Split(' ')[3];
                                value = value.Replace(";", "");
                                value = value.Replace("\"", "");
                                Vars.Add(new Variable() { Name = name, Value = value, Type = VarType.String });
                            }
                            else
                            {

                            }
                        }
                    }
                    else if (c.StartsWith("echo"))
                    {
                        c = c.Replace(";", "");
                        string varname = c.Split(' ')[1];
                        DOCUMENT += Vars.Find(item => item.Name == varname).Value;
                    }
                }
            }
            //You should send the var DOCUMENT over the socket connection.

            return DOCUMENT;
        }

        public static String GetTextBetween(String source, String leftWord, String rightWord)
        {
            return
                Regex.Match(source, String.Format(@"{0}\s(?<words>[\w\s]+)\s{1}", leftWord, rightWord),
                            RegexOptions.IgnoreCase).Groups["words"].Value;
        }
    }
}
