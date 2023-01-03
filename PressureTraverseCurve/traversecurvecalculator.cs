using Calculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
namespace PressureTraverseCurve
{
    public class traversecurvecalculator
    {
        public List<paramter> paramters = new List<paramter>();
        public Dictionary<string,paramter> paramters2 = new Dictionary<string, paramter>();
        public List<string> equations = new List<string>();
        public List<string> leftsides = new List<string>();
        public smartmodifier rm;
        public   Dictionary<string, decimal> variables = new Dictionary<string, decimal>();
        public List<double> consts = new List<double>();
        public void loadconsts()
        {
          foreach(var a in File.ReadAllLines("const.txt"))
            {
                var d = Convert.ToDouble(a);
                consts.Add(d);
            }
        }
        public void load_fromText(string[] cons,string[] eqs)
        {
            foreach (var a1 in cons)
            {
                var d = Convert.ToDouble(a1);
                consts.Add(d);
            }
            var a = eqs;
            for (int y = 0; y < a.Length; y++)
            {
                var a2 = a[y].Trim().Replace("\r", "").Replace("\n", "").ToUpper();
                var fl = a2.Split('=');
                var first = fl[0].Trim();
                var last = fl[1].Trim();
                paramter pr = new paramter();
                pr.type = type.iterator;
                pr.name = first;
                if (first == "P")
                {
                    pr.set(paramters2["PHF"].firstvalue, 0);
                }
                if (first == "TEMP")
                {
                    pr.set(paramters2["THF"].firstvalue, 0);
                }
                if (first == "DEPTH")
                {
                    pr.set(0, 0);
                }
                if (!paramters2.ContainsKey(first))
                {
                    paramters2.Add(pr.name, pr);
                    paramters.Add(pr);
                }
                equations.Add(last);
                leftsides.Add(first);
            }
        }
        public void loadequations()
        {
            var a = File.ReadAllLines("equations.txt");
            for(int y=0; y<a.Length; y++)
            {
                var a2 = a[y].Trim().Replace("\r","").Replace("\n","").ToUpper();
                var fl = a2.Split('=');
                var first = fl[0].Trim();
                var last = fl[1].Trim();
                paramter pr = new paramter();
                pr.type = type.iterator;
                pr.name = first;
                if (first == "P")
                {
                    pr.set(paramters2["PHF"].firstvalue,0);
                }
                if (first == "TEMP")
                {
                    pr.set(paramters2["THF"].firstvalue,0);
                }
                if (first == "DEPTH")
                {
                    pr.set(0, 0);
                }
                if (!paramters2.ContainsKey(first))
                {
                    paramters2.Add(pr.name, pr);
                    paramters.Add(pr);
                }
                equations.Add(last);
                leftsides.Add(first);
            }
        }

         double calc(string eq,int n)
        {
            eq = eq.ToUpper();
            eq = eq.Replace("[", "(");
            eq = eq.Replace("]", ")");
            eq= eq.Replace("CONST", consts[n] + "");
            var arr = paramters.OrderBy(aux => -aux.name.Length).ToArray();
            foreach (var a in arr)
            {
                if (a.type == type.staticparam) {
                    eq = eq.Replace(a.name,a.get().ToString("00000000000.000000")); 
                }
                else
                {
                    if (a.name == "P" || a.name == "TEMP")
                    {

                    }
                    var old = eq;
                    eq = eq.Replace(a.name+"(N)", a.get(n).ToString("00000000000.000000"));
                    eq = eq.Replace(a.name+"(0)", a.get(0).ToString("0000000000.000000"));
                    eq = eq.Replace(a.name+"(N-1)", a.get(n-1).ToString("00000000000.000000"));
                    eq = eq.Replace(a.name, a.get().ToString("00000000000.000000"));
                    if (old != eq)
                    {

                    }
                }
            }
           // DataTable dt=new DataTable();
            
           //var al= dt.Compute(eq.Replace("^", "**"), string.Empty);
           // var s = Convert.ToDouble(al);
            operation opers = new operation(eq,rm,variables);
            return (double)opers.calc();
        }
        public void loadparameters()
        {
            string[] params1 = { "TID","API","DO","OM","GLR","YG","PHF","THF","TWF","QL","WC","Q","YW","A0","A1","A2","A3","A4","A5","A6","A7","A8","A9","A10","A11","A12","A13","A14","A15","B1","B2","B3","B4","B5"                                                                                                                ,"C1","C2","C3","C4","C5","D1","D2","D3","D4","D5" };
            double[] values = {  1.995,40   ,9700 ,5  ,75   ,0.7 ,200  , 80  , 180 , 758 , 10 , 30 ,1.05, -2.462, 2.97, -2.862 / 10, 8.054 / 1000, 2.808, -3.498, 3.603 / 10, -1.044 / 100, -7.933 / 10, 1.396 , -1.491 / 10, 4.41 / 1000, 8.393 / 100, -1.864 / 10, 2.033 / 100, -6.095 / 10000, -2.69851, 0.15840954, -0.55099756, 0.54784917, -0.12194578, -0.10306578, 0.617774, -0.632946, 0.29598, -0.0401, 0.91162574, -4.82175636, 1232.25036621, -22253.57617, 116174.28125 };
            for(int u = 0; u < params1.Length; u++)
            {
                paramter p = new paramter();
                p.name = params1[u];
                p.set(values[u]);
                if (!paramters2.ContainsKey(p.name))
                {
                    paramters2.Add(p.name, p);
                    paramters.Add(p);
                }
            }
        }
        public int stepsreached = 0;
        public void calc(int steps)
        {
            stepsreached = 0;
            for (int n = 0; n < steps; n++)
            {
                stepsreached = n;
                for (int i = 0; i < equations.Count; i++)
                {
                    try
                    {
                       
                        var s = leftsides[i];
                        
                        if ((s == "DEPTH"||s=="P"||s== "TEMP") &&n==0)
                        {
                           // paramters2[leftsides[i]].set(paramters2[leftsides[i]].get(), n);
                            continue;
                        }
                        
                        var af = calc(equations[i], n);
                        paramters2[leftsides[i]].set(af, n);
                        if (n == 1)
                        {
                            if ((s == "TEMP" || s == "P" || s == "TPR"))
                            {
                            }
                        }

                    }
                    catch(Exception e)
                    {
                        //paramters2[leftsides[i]].set(calc(equations[i], n), n);
                        var a = e.Message;
                    }
                }
            }
        }
    }
    public enum type { staticparam,iterator}
    public class paramter
    {
        public string name;
        public type type;
        public List<double> values=new List<double>();
        public double firstvalue;
        public double get(int id=-1)
        {
            if (id >=values.Count||id<0 || type == type.staticparam)
            {
                return firstvalue;
            }
            else
            {
                return values[id];
            }
        }
        public void set(double val,int id = -1)
        {
            if (type == type.staticparam)
            {
                firstvalue = val;
            }
            else
            {
                values.Add( val);
                if (id == 0)
                {
                    firstvalue = val;
                }
            }
        }
    }
}
