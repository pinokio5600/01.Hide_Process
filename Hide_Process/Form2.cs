using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hide_Process{
    public partial class Form2 : Form{
        public Form2(){
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e){
            linkLabel1.Text = "상업적 이용을 금지합니다." + Environment.NewLine + Environment.NewLine + "ⓒ 2019. 김형태 All Rights Reserved.";
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;

            textBox1.Text = Properties.Settings.Default.hotKey;

            this.btnKey.Click += FunGetKey;
            this.btnSave.Click += FunSaveKey;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e){
            System.Diagnostics.Process.Start("https://blog.naver.com/rlagudxo789");
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e){
            textBox1.Text = Convert.ToString(e.KeyData);
            this.KeyPreview = false;
        }

        private void FunGetKey(object sender, EventArgs e){
            this.KeyPreview = true;
        }

        private void FunSaveKey(object sender, EventArgs e){
            Properties.Settings.Default.hotKey = textBox1.Text;
            Properties.Settings.Default.Save();

            Application.OpenForms["Form2"].Close();
        }
    }
}
