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
        List<Variable> Vars = new List<Variable>();

        public static string GetHTMLFromSSHTML(string page)
        {
           
            string pg = page;
            //pg = pg.Replace("TEST", "TESTSPLIT");
            while (pg.Contains("<s#c") || pg.Contains("<s#"))
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
                    }
                    else if (splitc)
                    {
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
                }
            }
            return pg;
        }

        public static String GetTextBetween(String source, String leftWord, String rightWord)
        {
            return
                Regex.Match(source, String.Format(@"{0}\s(?<words>[\w\s]+)\s{1}", leftWord, rightWord),
                            RegexOptions.IgnoreCase).Groups["words"].Value;
        }
    }
}
