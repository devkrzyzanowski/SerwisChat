using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;


namespace SerwisChat {
    public partial class Form1 : Form {

        private TcpListener serwer = null;
        private TcpClient client = null;
        private string ipAddress = "127.0.0.1";
        private BinaryReader bReader = null;
        private BinaryWriter bWriter = null;
        private bool activeConnection = false;
        private int cursorPosition = 0;

        public Form1() {
            InitializeComponent();
            webBrowser1.Navigate("about:blank");
            webBrowser1.Document.Write("<html><head><style>body,table { font-size: 10pt; font-family: Verdana; margin: 3px 3px 3px 3px; font-color: black;}</style></head><body width=\"" + (webBrowser1.ClientSize.Width
            - 20).ToString() + "\">");
            IPHostEntry adresyIP = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress position in adresyIP.AddressList)
            comboBox1.Items.Add(position.ToString());
        }

        private void ComboBox1_TextChanged(object sender, EventArgs e) {
            ipAddress = comboBox1.Text;
        }

        private void Button3_Click(object sender, EventArgs e) {
            EnterTag("<b></b>");
        }

        private void Button4_Click(object sender, EventArgs e) {
            EnterTag("<i></i>");
        }

        private void EnterTag(string tag) {
            string text = textBox1.Text;
            textBox1.Text = text.Insert(cursorPosition, tag);
            textBox1.Focus();
            if (tag == "<br>" || tag == "<hr>") {
                textBox1.Select(cursorPosition + tag.Length, 0);
                cursorPosition += tag.Length;
            } else {
                textBox1.Select(cursorPosition + tag.Length / 2, 0);
                cursorPosition += tag.Length / 2;
            }
        }

        delegate void SetTextCallBack(string text);
        delegate void SetScrollCallBack();

        private void SetText(string tekst) {
            if (listBox1.InvokeRequired) {
                SetTextCallBack f = new SetTextCallBack(SetText);
                this.Invoke(f, new object[] { tekst });
            } else {
                this.listBox1.Items.Add(tekst);
            }
        }
        private void SetTextHTML(string tekst) {
            if (webBrowser1.InvokeRequired) {
                SetTextCallBack f = new SetTextCallBack(SetTextHTML);
                this.Invoke(f, new object[] { tekst });
            } else {
                this.webBrowser1.Document.Write(tekst);
            }
        }
        private void SetScroll() {
            if (webBrowser1.InvokeRequired) {
                SetScrollCallBack s = new SetScrollCallBack(SetScroll);
                this.Invoke(s);
            } else {
                this.webBrowser1.Document.Window.ScrollTo(1, Int32.MaxValue);
            }
        }
        private void WpiszTekst(string kto, string wiadomosc) {
            SetTextHTML("<table><tr><td width=\"10%\"><b>" + kto + "</b></td><td width=\"90%\">(" + DateTime.Now.ToShortTimeString() + "):</td></tr>"); SetTextHTML("<tr><td colspan=2>" + wiadomosc + "</td></tr></table>"); SetTextHTML("<hr>"); SetScroll();
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e) {
            cursorPosition = textBox1.SelectionStart;
        }
        private void TextBox1_MouseUp(object sender, MouseEventArgs e) {
            cursorPosition = textBox1.SelectionStart;
        }

        private void WyczyśćToolStripMenuItem_Click(object sender, EventArgs e) {
            this.webBrowser1.Navigate("about:blank");
        }

        private void ZapiszToolStripMenuItem_Click(object sender, EventArgs e) {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK) {
                using ( System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName))
                    try {
                        sw.Write(webBrowser1.DocumentText);
                    } catch {
                        MessageBox.Show("Nie można zapisać pliku: " + saveFileDialog1.FileName);
                    }
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            IPAddress serwerIP;
            try {
                serwerIP = IPAddress.Parse(ipAddress);
            } catch {
                MessageBox.Show("Błędny adres IP");
                activeConnection = false;
                return;
            }
            serwer = new TcpListener(serwerIP, (int)numericUpDown1.Value);
            try {
                serwer.Start();
                SetText("Oczekuje na połączenie ...");
                client = serwer.AcceptTcpClient();
                NetworkStream ns = client.GetStream();
                SetText("Klient próbuje się połączyć");
                bReader = new BinaryReader(ns);
                bWriter = new BinaryWriter(ns);
                if (bReader.ReadString() == "###HI###") {
                    SetText("Klient połączony");
                    backgroundWorker2.RunWorkerAsync();
                } else {
                    SetText("Klient nie wykonał wymaganej autoryzacji. Połączenie przerwane");
                    client.Close();
                    serwer.Stop();
                    activeConnection = false;
                }
            } catch (Exception edd){
                SetText("Połączenie zostało przerwane");
                MessageBox.Show(edd.ToString());
                activeConnection = false;
            }
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e) {
            string wiadomosc;
            try {
                while ((wiadomosc = bReader.ReadString()) != "###BYE###") {
                    WpiszTekst("ktoś", wiadomosc);
                }
                client.Close();
                serwer.Stop();
                SetText("Połączenie zostało przerwane przez klienta");
            } catch {
                SetText("Klient rozłączony");
                activeConnection = false;
                client.Close();
                serwer.Stop();
            }
        }
        private void Button1_Click(object sender, EventArgs e) {
            if (activeConnection == false) {
                activeConnection = true;
                backgroundWorker1.RunWorkerAsync();
            } else {
                activeConnection = false;
                if (client != null) client.Close();
                serwer.Stop();
                backgroundWorker1.CancelAsync();
                if (backgroundWorker2.IsBusy) backgroundWorker2.CancelAsync();
            }
        }
        private void Button2_Click(object sender, EventArgs e) {
            WpiszTekst("ja", textBox1.Text);
            if (activeConnection) bWriter.Write(textBox1.Text);
            textBox1.Text = "";
        }
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)13) this.Button2_Click(sender, e);
        }
    }
}
