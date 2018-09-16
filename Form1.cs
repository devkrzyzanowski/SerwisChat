using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerwisChat
{
    public partial class Form1 : Form
    {
        private int cursorPosition = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            enterTag("<b></b>");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            enterTag("<i></i>");
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void enterTag(string tag)
        {
            string text = textBox1.Text;
            textBox1.Text = text.Insert(cursorPosition, tag);
            textBox1.Focus();
            if (tag == "<br>" || tag == "<hr>")
            {
                textBox1.Select(cursorPosition + tag.Length, 0);
                cursorPosition += tag.Length;
            } else
            {
                textBox1.Select(cursorPosition + tag.Length / 2, 0);
                cursorPosition += tag.Length / 2;
            }
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            cursorPosition = textBox1.SelectionStart;
        }
        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            cursorPosition = textBox1.SelectionStart;
        }

        private void wyczyśćToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate("about:blank");
        }

        private void zapiszToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using ( System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName))
                    try
                    {
                        sw.Write(webBrowser1.DocumentText);
                    } catch
                    {
                        MessageBox.Show("Nie można zapisać pliku: " + saveFileDialog1.FileName);
                    }
            }
        }
    }
}
