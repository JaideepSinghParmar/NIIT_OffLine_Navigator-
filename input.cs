using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class input : Form
    {
        public input()
        {
            InitializeComponent();
        }

        private void input_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Drawing.Pen myPen;
            myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();

            formGraphics.DrawLine(myPen, button1.Location.X, button1.Location.Y, button2.Location.X, button2.Location.Y); myPen.Dispose();
            formGraphics.Dispose();
        }
    }

  
}
