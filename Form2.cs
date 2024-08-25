using System.Windows.Forms;
using System;

namespace Windows_Duster_FlatUi_Edition
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        Form1 frm1 = new Form1();

        private void Form2_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // Set the interval before starting the timer
            timer1.Start();
            frm1.Show();
            frm1.StartPosition = FormStartPosition.CenterScreen;
            frm1.Update();
            frm1.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value + 35 <= progressBar1.Maximum)
            {
                progressBar1.Value += 35;
            }
            else
            {
                progressBar1.Value = progressBar1.Maximum; // Ensure it doesn't exceed the max value
            }

            if (progressBar1.Value == progressBar1.Maximum)
            {
                timer1.Stop();
                frm1.Visible = true;
                this.Visible = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
