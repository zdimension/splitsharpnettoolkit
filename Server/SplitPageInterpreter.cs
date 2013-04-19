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
        public static string GetHTMLFromSSHTML(string page)
        {
           
            string pg = page;
            //pg = pg.Replace("TEST", "TESTSPLIT");
            while (pg.IndexOf("<s#") != -1)
            {
                int i = pg.IndexOf("<s#");
                MessageBox.Show(pg);
                string abc = GetTextBetween(pg, "<s#", "s#>");
                bool a = false;
                int abcd;
                for (int j = i; j < pg.Length; j++)
                {
                    //if (!a)
                    //{
                        if (pg[j] == 's')
                        {

                        }
                        else if (pg[j] == '#')
                        {

                        }
                        else if (pg[j] == '>')
                        {
                            a = true;
                        }
                        else
                        {
                            if (a)
                            {
                                abcd = i;
                                break;
                            }
                        }
                    //}
                }

                MessageBox.Show(abc);
                break;

                /*string a = "";
                string b = "";
                string c = "";

                for (int j = i; j < pg.Length; j++)
                {
                    if (pg[j] == 's')
                    {
                        if (b == "")
                        {
                            b += pg[j];
                        }
                    }
                    else if (pg[j] == '#')
                    {
                        if (b != "")
                        {
                            b += pg[j];
                        }
                    }
                    else if (pg[j] == '>')
                    {
                        if (b != "")
                        {
                            b += pg[j];
                            break;
                        }
                    }
                    else
                    {
                        a += pg[j];
                    }
                    c += pg[j];
                }*/
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
