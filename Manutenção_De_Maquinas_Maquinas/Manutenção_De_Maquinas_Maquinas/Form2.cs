using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manutenção_De_Maquinas_Maquinas
{
    public partial class Form2 : Form
    {
        int i = 0;
        int ii = 100;
        public Form2()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i < 100)
            {
                timer2.Enabled = true;
            }
            if (i == 100)
            {
                timer3.Enabled = true;
            }
            if (ii == 0)
            {
                Close();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            i++;
            Opacity = i * 0.01;
            timer2.Enabled = false;
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            ii--;
            Opacity = ii * 0.01;
            timer3.Enabled = false;
        }

        private void Form2_Load_1(object sender, EventArgs e)
        {
           // animator.ShowSync();
        }
    }
}
