using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//تم عمل المشروع بواسطة محمد ياسر
//رابط السلسلة أشرح فيها بالكامل كيف عملت المشروع
//https://www.youtube.com/watch?v=tNf97hymK_w&list=PLsXQEsz9IQ-q-l05rrZwjRnX3SCzGKDJZ
//رابط القناة
//https://www.youtube.com/channel/UCyK_zVDWCutbm8Ji_m3QDWQ
namespace Calculator
{
    class operation
    {
        public smartmodifier sm;
        enum type { function, operatorm, digit, variable, up, down, symbol, nothing }
        type cover;
        string equation;
        List<operation> opers;
        Dictionary<string, type> parttypes;
        Dictionary<string, decimal> variablvalues;
        decimal num;
        int coverid;
        bool issimple;//"1"
        public operation(string eq, smartmodifier sm2, Dictionary<string, decimal> v)
        {
            sm = sm2;
            equation = eq;
            opers = new List<operation>();
            cover = type.nothing;
            variablvalues = v;
        }
        public void analyse()
        {
            sm.fillparts2(equation);//sin( 3 0 )
            filltypes();
            for (int i = 0; i < sm.parts2.Count; i++)
            {
                var q = sm.parts2[i];
                var g = equation.Substring(q[0], q[1]);
                switch (parttypes[g])
                {
                    case type.function:
                        //sin( 30 ) 
                        var endof = getend(i);
                        var len = 0;
                        for (int i2 = i + 1; i2 <= endof; i2++)
                        {
                            len += sm.parts2[i2][1];
                        }
                        var neweq = equation.Substring(q[0] + q[1], len);
                        operation op = new operation(neweq, sm, variablvalues);
                        op.cover = type.function;
                        op.coverid = sm.ids[g];
                        i = endof + 1;
                        opers.Add(op);
                        break;
                    case type.operatorm:
                        operation op2 = new operation("", sm, variablvalues);
                        op2.cover = type.operatorm;
                        op2.coverid = sm.ids[equation.Substring(q[0], q[1])];
                        opers.Add(op2);
                        break;
                    case type.up:
                        endof = getend(i);
                        var len2 = 0;
                        for (int ii = i + 1; ii <= endof; ii++)
                        {
                            len2 += sm.parts2[ii][1];
                        }
                        string neweq3 = equation.Substring(q[0] + q[1], len2);
                        operation op3 = new operation(neweq3, sm, variablvalues);
                        opers.Add(op3);
                        i = endof + 1;
                        break;
                    case type.down:

                        throw new Exception("End of bracket without an open bracket");
                    case type.symbol:
                        try
                        {
                            operation op4 = new operation("", sm, variablvalues);
                            op4.opers.Add(opers[i - 1]);
                            op4.cover = type.symbol;
                            op4.coverid = sm.ids[equation.Substring(q[0], q[1])];
                            opers.RemoveAt(i - 1);
                            opers.Add(op4);
                        }
                        catch
                        {

                            throw new Exception("! symbol without anyting before it");
                        }
                        break;
                    case type.digit:
                        string neweq5 = equation.Substring(q[0], q[1]);
                        if (i == 0 || opers[opers.Count - 1].cover != type.digit)
                        {
                            operation op5 = new operation(neweq5, sm, variablvalues);
                            op5.cover = type.digit;
                            op5.issimple = true;
                            opers.Add(op5);
                        }
                        else
                        {
                            opers[opers.Count - 1].equation += neweq5;
                        }
                        break;
                    case type.variable:
                        string val = equation.Substring(q[0], q[1]);
                        operation op6 = new operation("", sm, variablvalues);
                        op6.num = variablvalues[val];
                        op6.issimple = true;
                        opers.Add(op6);
                        break;

                }
            }

        }
        public decimal calc()
        {
            decimal mainans = 0;
            if (issimple)
            {
                if (equation == "")
                {
                    return num;
                }
                else
                {
                    return Convert.ToDecimal(equation);
                }
            }
            analyse();
            if (opers.Count == 0)
            {
                throw new Exception("Empty field");//sin()
            }
            List<int[]> pirority = new List<int[]>();//1+2
            for (int i = 0; i < opers.Count; i++)
            {
                if (opers[i].cover == type.operatorm)
                {
                    if (opers[i].coverid == 6)
                    {
                        if (i == 0 || opers[i - 1].cover == type.operatorm)
                        {


                            operation op = new operation("-" + opers[i + 1].equation, sm, variablvalues);
                            opers.RemoveAt(i + 1);
                            opers[i] = op;
                            continue;
                        }

                    }
                    int[] m = { opers[i].coverid, i - 1, i + 1 };
                    pirority.Add(m);
                }
            }
            //() ^*/+- 
            //2+3/2
            //5C2^3
            int ii = 0;
            foreach (var c in Sortbypirority(pirority))
            {
                pirority[ii] = c;
                ii++;
            }
            int U = 0;
            foreach (var p in pirority)
            {
                if (U == 5)
                {

                }
                var ans = operatorcalc(p[0], p[1], p[2]);
                U++;
                operation op = new operation("", sm, variablvalues);
                op.num = ans;
                op.issimple = true;
                opers.Add(op);
                int newindx = opers.Count - 1;
                int s1 = p[1];
                int s2 = p[2];
                foreach (var p2 in pirority)
                {
                    if (p2[1] == s1 || p2[1] == s2)
                    {
                        p2[1] = newindx;
                    }
                    if (p2[2] == s1 || p2[2] == s2)
                    {
                        p2[2] = newindx;
                    }
                }
            }
            if (pirority.Count == 0)
            {
                opers[0].issimple = true;
                mainans = opers[0].calc();
            }
            else
            {
                mainans = opers[pirority[pirority.Count - 1][1]].calc();
            }
            switch (cover)
            {
                case type.function:
                    switch (coverid)
                    {
                        case 1:
                            mainans = Convert.ToDecimal(Math.Sin(Convert.ToDouble(mainans * Convert.ToDecimal(3.14159265358979) / Convert.ToDecimal(180))));
                            break;
                        case 2:
                            mainans = Convert.ToDecimal(Math.Cos(Convert.ToDouble(mainans * Convert.ToDecimal(3.14159265358979) / Convert.ToDecimal(180))));
                            break;
                        case 3:
                            mainans = Convert.ToDecimal(Math.Tan(Convert.ToDouble(mainans * Convert.ToDecimal(3.14159265358979) / Convert.ToDecimal(180))));
                            break;
                        case 5:
                            mainans = Convert.ToDecimal(Math.Asin(Convert.ToDouble(mainans))) / Convert.ToDecimal(3.14159265358979) * Convert.ToDecimal(180);
                            break;
                        case 6:
                            mainans = Convert.ToDecimal(Math.Acos(Convert.ToDouble(mainans))) / Convert.ToDecimal(3.14159265358979) * Convert.ToDecimal(180);
                            break;
                        case 7:
                            mainans = Convert.ToDecimal(Math.Atan(Convert.ToDouble(mainans))) / Convert.ToDecimal(3.14159265358979) * Convert.ToDecimal(180);
                            break;
                        case 8:
                            mainans = Convert.ToDecimal(Math.Log10(Convert.ToDouble(mainans)));
                            break;
                        case 9:
                            mainans = Convert.ToDecimal(Math.Log(Convert.ToDouble(mainans)));
                            break;
                        case 10:
                            mainans = Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(mainans)));
                            break;
                        case 11:
                            mainans = Convert.ToDecimal(Math.Abs(Convert.ToDouble(mainans)));
                            break;
                    }
                    break;
                case type.symbol:
                    switch (coverid)
                    {
                        case 1:
                            mainans = calcmadroob(mainans);
                            break;
                    }
                    break;
            }
            return mainans;
        }
        decimal operatorcalc(int opid, int i1, int i2)
        {
            decimal ans1 = 0;
            if (opid != 6)
            {
                ans1 = opers[i1].calc();
            }
            var ans2 = opers[i2].calc();

            switch (opid)
            {
                case 1:
                    if (ans1 < ans2)
                    {
                        throw new Exception("For nCr n must be greater than r");
                    }
                    return calcmadroob(ans1) / (calcmadroob(ans2) * calcmadroob(ans1 - ans2));
                case 2:
                    if (ans1 < ans2)
                    {

                        throw new Exception("For nPr n must be greater than r");
                    }
                    return calcmadroob(ans1) / (calcmadroob(ans1 - ans2));
                case 3:
                    try
                    {
                        return Convert.ToDecimal(Math.Pow(Convert.ToDouble(ans1), Convert.ToDouble(ans2)));

                    }
                    catch
                    {

                        throw new Exception("Mathematical error ! ");
                    }
                case 4:
                    if (ans2 == 0)
                    {

                        throw new Exception("Attempt to divide by zero");
                    }
                    return ans1 / ans2;
                case 5:
                    return ans1 * ans2;
                case 6:
                    decimal g1 = 0;
                    if (i1 >= 0)
                    {
                        g1 = opers[i1].calc();
                    }
                    return g1 - ans2;
                case 7:
                    return ans1 + ans2;
            }
            return 0;
        }
        decimal calcmadroob(decimal val)
        {
            if (Convert.ToInt32(val) != val || val < 0)
            {
                throw new Exception("! , P , C must be integers and positive");
            }
            if (val == 0)
            {
                return 1;
            }
            return val * calcmadroob(val - 1);
        }
        static IEnumerable<int[]> Sortbypirority(IEnumerable<int[]> e)
        {
            // Use LINQ to sort the array received and return a copy.
            var sorted = from s in e
                         orderby s[0] ascending
                         select s;
            return sorted;
        }

        int getend(int start)
        {
            int numofparances = 1;
            int ans = start;
            for (int i = start + 1; i < sm.parts2.Count; i++)
            {
                var q = sm.parts2[i];
                var s = equation.Substring(q[0], q[1]);
                switch (parttypes[s])
                {
                    case type.function:
                        numofparances += 1;
                        break;
                    case type.up:
                        numofparances += 1;
                        break;
                    case type.down:
                        numofparances -= 1;
                        break;
                }
                if (numofparances == 0)
                {
                    return i - 1;
                }
                ans = i;
            }
            if (numofparances != 0)
            {
                //  errorposition = ans;
                throw new Exception("Open bracket without end");
            }

            return ans;
        }
        void filltypes2()
        {
            parttypes = new Dictionary<string, type>();
            parttypes.Add("+", type.operatorm);
            parttypes.Add("π", type.variable);
            parttypes.Add(".", type.digit);
            parttypes.Add("-", type.operatorm);
            parttypes.Add("*", type.operatorm);
            parttypes.Add("/", type.operatorm);
            parttypes.Add("^", type.operatorm);
            parttypes.Add("sin(", type.function);
            parttypes.Add("RV(", type.function);
            parttypes.Add("cos(", type.function);
            parttypes.Add("tan(", type.function);
            parttypes.Add("LOG(", type.function);
            parttypes.Add("(", type.up);
            parttypes.Add(")", type.down);
            parttypes.Add("0", type.digit);
            parttypes.Add("1", type.digit);
            parttypes.Add("2", type.digit);
            parttypes.Add("3", type.digit);
            parttypes.Add("4", type.digit);
            parttypes.Add("5", type.digit);
            parttypes.Add("6", type.digit);
            parttypes.Add("7", type.digit);
            parttypes.Add("8", type.digit);
            parttypes.Add("9", type.digit);
        }
        void filltypes()
        {
            filltypes2();
            return;
           
        }
    }
}
