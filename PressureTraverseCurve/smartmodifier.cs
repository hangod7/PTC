using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Calculator
{

    public  class smartmodifier
    {
        public List<string> parts;
        public List<int[]> parts2;
        public int selection;
        public Form f2;
        public Dictionary<string, int> ids;
        public smartmodifier(Form f)
        {
            parts = new List<string>();
            parts2 = new List<int[]>();
            selection = 0;
            f2 = f;
            ids = new Dictionary<string, int>();
            string[] ps = { "(", ")", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "sin(", "cos(", "tan(", "RV(", "+", "-", "*", "/", "LOG(", "π", ".", "^" };
            int[] idd =   { 0  , 0  , 0  , 0  , 0  , 0  , 0  , 0  , 0  , 0  , 0  , 0  , 1     , 2     , 3     , 11    , 7  , 6  , 5  , 4  , 8     , 5  , 2  , 3 };
            for (int i = 0; i < ps.Length; i++)
            {
                parts.Add(ps[i]);
                ids.Add(ps[i], idd[i]);
            }
            prepare();
            return;
        }
        private void prepare()
        {
            int i = 0;
            foreach (string s in SortByLength(parts))
            {
                parts[i] = s;
                i++;
            }
        }
        public string insert(int y, int y2, string ins, string area)
        {
            //string area = f2.area.Text;
            selection = y;
            area = remove(y, y2, area, with: 0);

            string p = "";
            bool g = true;
            for (int rj = 0; rj < parts.Count; rj++)
            {
                p = parts[rj];
                if (!g)
                {
                    break;
                }
                for (int i = 0; i < area.Length - p.Length + 1; i++)
                {
                    if (area.Substring(i, p.Length) == p)
                    {
                        //int[,] minipart = new int[1,2];
                        //minipart[0, 0] = i;
                        //minipart[0, 1] = i+p.Length;
                        if (y > i && y < i + p.Length)
                        {
                            g = false;
                            break;
                        }
                    }
                }
            }
            if (g)
            {
                area = area.Substring(0, selection) + ins + area.Substring(selection);
                selection += ins.Length;
            }
            return area;
        }
        public string remove(int y, int y2, string area, int with = 1)
        {
            // string area = f2.area.Text;

            bool g = true;
            string p = "";
            int ro = 1;
            if (y2 > 0)
            {
                ro = 0;
            }
            for (int rj = 0; rj < parts.Count; rj++)
            {
                p = parts[rj];
                if (!g)
                {
                    break;
                }
                for (int i = 0; i < area.Length - p.Length + 1; i++)
                {
                    if (area.Substring(i, p.Length) == p)
                    {
                        //int[,] minipart = new int[1,2];
                        //minipart[0, 0] = i;
                        //minipart[0, 1] = i+p.Length;

                        if (y > i - 1 + ro && y < i + p.Length + 1 * ro && !(y2 == 0 && with == 0))
                        {
                            area = area.Substring(0, i) + area.Substring(i + p.Length);
                            y2 = y2 - p.Length + y - i;
                            y -= y - i;
                            //i -= p.Length;
                            rj = -1;
                            selection = i;
                            i -= 1;
                            if (y2 <= 0)
                            {

                                g = false;
                            }
                            break;
                        }
                    }
                }
            }


            return area;
        }
        public string fillparts2(string area)
        {
            // string area = f2.area.Text;
            parts2 = new List<int[]>();
            string p = "";
            for (int rj = 0; rj < parts.Count; rj++)
            {
                p = parts[rj];

                for (int i = 0; i < area.Length - p.Length + 1; i++)
                {
                    var g = true;
                    // 4tan(
                    foreach (var p2 in parts2)
                    {
                        if (i >= p2[0] && i + p.Length <= p2[0] + p2[1])
                        {
                            g = false;
                            break;
                        }
                    }
                    if (!g)
                    {
                        continue;
                    }
                    if (area.Substring(i, p.Length) == p)
                    {
                        int[] m = { i, p.Length };
                        parts2.Add(m);

                    }
                }
            }
            var i2 = 0;
            foreach (var c in SortbyIndex(parts2))
            {
                parts2[i2] = c;
                i2 += 1;
            }

            return area;
        }
        static IEnumerable<int[]> SortbyIndex(IEnumerable<int[]> e)
        {
            // Use LINQ to sort the array received and return a copy.
            var sorted = from s in e
                         orderby s[0] ascending
                         select s;
            return sorted;
        }

        static IEnumerable<string> SortByLength(IEnumerable<string> e)
        {
            // Use LINQ to sort the array received and return a copy.
            var sorted = from s in e
                         orderby s.Length descending
                         select s;
            return sorted;
        }
    }
}
