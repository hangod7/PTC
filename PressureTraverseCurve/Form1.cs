
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace PressureTraverseCurve
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            variablvalues = new Dictionary<string, decimal>();
            variablvalues.Add("π", Convert.ToDecimal(3.14159265358979323846));
            variablvalues.Add("e", Convert.ToDecimal(2.71828182846));
            variablvalues.Add("ans", Convert.ToDecimal(0));
            variablvalues.Add("X", Convert.ToDecimal(0));
            variablvalues.Add("Y", Convert.ToDecimal(0));
            variablvalues.Add("Z", Convert.ToDecimal(0));
            sm = new Calculator.smartmodifier(this);
        }
        Dictionary<string, decimal> variablvalues;
        Calculator.smartmodifier sm;
        bool done = false;
        void calc(traversecurvecalculator pv)
        {
            pv.calc(29);
            done = true;
        }
        private async  void button1_Click(object sender, EventArgs e)
        {
            if (!runing)
            {
                pvs.Clear();
                progress = 0;
                timer1.Start();
                await Task.Run(() => dodrawAsync());
            }
        }
        bool runing;
        void dodrawAsync()
        {
            //label1.Text = "11";
            //progressBar1.Show();
            runing = true;
            var cons = File.ReadAllLines("const.txt");
            var eqs = File.ReadAllLines("equations.txt");
            Stopwatch sp = new Stopwatch();
            sp.Start();
            int y = 0;
            bool first = true;
            double oldglr = 0;
            int totalcounts = 0;
            int plus = 1;
            for (double glr = Convert.ToDouble(from.Text); glr < Convert.ToDouble(to1.Text) + 1; glr += Convert.ToDouble(step1.Text))
            {
                if (glr == Convert.ToDouble(GLR.Text))
                {
                    plus = 0;
                }
                totalcounts++;
            }
            totalcounts += plus;
            y = 0;
            for (double glr = Convert.ToDouble(from.Text); glr < Convert.ToDouble(to1.Text) + 1; glr += Convert.ToDouble(step1.Text))
            {

                traversecurvecalculator pv = new traversecurvecalculator();
                pvs.Add(pv);
                pv.loadparameters();
                pv.load_fromText(cons, eqs);
                pv.paramters2["GLR"].set(glr);
                oldglr = glr;
                foreach (var c in panel1.Controls)
                {
                    try
                    {
                        TextBox t = (TextBox)c;
                        if (t.Name != "GLR" || first)
                        {
                            pv.paramters2[t.Name].set(Convert.ToDouble(t.Text));
                        }
                    }
                    catch
                    {

                    }
                }
                if (first)
                {
                    if (glr != Convert.ToDouble(GLR.Text))
                    {
                        glr = Convert.ToDouble(GLR.Text);
                    }
                    else
                    {
                        first = false;
                    }
                }

                pv.rm = sm;
                pv.variables = variablvalues;
                done = false;
                Task ts = new Task(() => calc(pv));
                ts.Start();
                while (!done)
                {
                    progress = y * 100 / totalcounts + (pv.stepsreached * 100 / totalcounts) / 29;
                    Thread.Sleep(1);
                }

                y++;
                if (first)
                {
                    glr = oldglr - Convert.ToDouble(step1.Text);
                }
                first = false;
                progress = y * 100 / totalcounts;
                // progressBar1.Value = y * 100 / totalcounts;
            }
            sp.Stop();
            runing = false;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series.Clear();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        double progress;
        List<traversecurvecalculator> pvs = new List<traversecurvecalculator>();
        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = (int)progress;
            if (progressBar1.Value > 0)
            {
                progressBar1.Show();
            }
            if (progress == 100)
            {
                chart1.Series.Clear();
                chart1.ChartAreas[0].AxisX.Title = "Pressure (psia)";
                chart1.ChartAreas[0].AxisX.TitleFont = new Font("tahoma", 14,FontStyle.Regular);

                chart1.ChartAreas[0].AxisY.Title = "Depth (ft)";
                chart1.ChartAreas[0].AxisY.TitleFont = new Font("tahoma", 14, FontStyle.Regular);

                foreach (var pv in pvs)
                {
                    double glr = pv.paramters2["GLR"].firstvalue;
                    var pressures = pv.paramters2["P"].values;
                    var depths = pv.paramters2["DEPTH"].values;
                    chart1.Series.Add($"GLR {glr}");
                    chart1.Series[$"GLR {glr}"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    chart1.ChartAreas[0].AxisY.IsReversed = true;
                    chart1.ChartAreas[0].AxisX.Interval = 500;
                    chart1.ChartAreas[0].AxisX.Minimum = 0;
                    for (int i = 0; i < pressures.Count; i++)
                    {
                        chart1.Series[$"GLR {glr}"].Points.AddXY(pressures[i], depths[i]);
                    }
                }
                timer1.Stop();
                
            }
        }
    }
}
