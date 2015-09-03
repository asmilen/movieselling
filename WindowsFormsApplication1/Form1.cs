using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var time = DateTime.UtcNow.AddHours(7).AddMinutes(60);
            
            string startTime = "08:00";
            while (time < DateTime.UtcNow.AddHours(25))
            {
                var timeNow = Int32.Parse(time.Hour + "" + time.Minute);
                if (time.Minute < 10) timeNow *= 10;
                if (timeNow < Int32.Parse(startTime.Replace(":", "")))
                {
                    richTextBox1.Text += timeNow + "\n";
                }
                time = time.AddSeconds(1);
            }
        }
    }
}
