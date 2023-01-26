using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IncrementationProgram
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public bool incUpCheck = true;
        public bool incDownCheck;
        public int numberInBox; 

        private void increment_TextChanged(object sender, EventArgs e)
        {

            if(System.Text.RegularExpressions.Regex.IsMatch(increment.Text, " ^[0-9]"))
            {
                increment.Text = "";
            }
        }

        private void upByOne_Click(object sender, EventArgs e)
        {
            if(increment.Text == "")
            {
                increment.Text = 1.ToString();
            }
            else
            {
                int i = Convert.ToInt32(increment.Text);
                i++;
                increment.Text = i.ToString();
            }

            numberInBox = Convert.ToInt32(increment.Text);
        }

        private void downByOne_Click(object sender, EventArgs e)
        {
            if (increment.Text == "" || increment.Text == "0")
            {
                //do nothing
            }
            else
            {
                int i = Convert.ToInt32(increment.Text);
                i--;
                increment.Text = i.ToString();
            }

            numberInBox = Convert.ToInt32(increment.Text);
        }

        private void incUp_CheckedChanged(object sender, EventArgs e)
        {
            incUpCheck = true;

            if (incUp.Checked)
            {
                incUpCheck = true;
                incDownCheck = false;
            }
            else
            {
                incUpCheck = false;
                incDownCheck = true;
            }
        }

        private void incDown_CheckedChanged(object sender, EventArgs e)
        {
            if (incDown.Checked)
            {
                incUpCheck = false;
                incDownCheck = true;
            }
            else
            {
                incUpCheck = true;
                incDownCheck = false;
            }
        }
    }
}
