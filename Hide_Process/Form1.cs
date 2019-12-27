﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices; // DllImport
using System.Diagnostics;
using System.Collections;

namespace Hide_Process{
    public partial class Form1 : Form{
        public delegate bool EnumWindowCallback(int hwnd, int lParam);//알아내고자하는 핸들대상, 윈도우의 캡션값을 반환

        /* Win32API들 */
        [DllImport("user32.dll")]
        public static extern IntPtr GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowCallback callback, int y);

        [DllImport("user32.dll")]
        public static extern int GetParent(int hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count); //1:파라미터 2:반환값 3:

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(int hWnd, int nIndex);

        [DllImport("user32")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);

        const int GCL_HICON = -14; //GetWindowLong을 호출할 때 쓸 인자
        const int GCL_HMODULE = -16;
        ImageList imgList;//ListView의 Image로 쓸 리스트
        ArrayList handleArray = null; //숨길 핸들러 저장

        public Form1(){
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e){
            processListView.View = View.List;
            imgList = new ImageList();
            imgList.ImageSize = new Size(16, 16);
            processListView.SmallImageList = imgList;

            //processListView.HeaderStyle = ColumnHeaderStyle.Clickable;
            processListView.CheckBoxes = true;

            handleArray = new ArrayList();
            loadProcessList();

            this.btnHide.Click += FuncHide;
            this.btnShow.Click += FuncShow;
            this.btnSetting.Click += SettingFormShow;
        }

        private void loadProcessList(){
            Process[] processVar = Process.GetProcesses();

            processListView.Items.Clear();
            handleArray.Clear();

            foreach (Process process in processVar){
                if (!String.IsNullOrEmpty(process.MainWindowTitle)){
                    IntPtr mainHandle = process.MainWindowHandle;

                    processListView.SmallImageList = imgList;

                    try{
                        //HICON 아이콘 핸들을 얻어온다
                        IntPtr hIcon = GetClassLong((IntPtr)mainHandle, GCL_HICON);
                        //아이콘 핸들로 Icon 객체를 만든다
                        Icon icon = Icon.FromHandle(hIcon);
                        imgList.Images.Add(icon);
                    }catch (Exception){
                        //예외의 경우는 자기 자신의 윈도우인 경우이다.
                        imgList.Images.Add(this.Icon);
                    }
                    
                    handleArray.Add(mainHandle.ToInt32());
                    processListView.Items.Add(process.MainWindowTitle, handleArray.Count-1);
                }
            }
        }

        private void FuncHide(object sender, EventArgs e){
            /*foreach(ListViewItem item in processListView.CheckedItems){
                ShowWindow((int)handleArray[3], 0);
            }*/

            for (int i = 0; i < processListView.Items.Count; i++){
                if (processListView.Items[i].Checked == true) {
                    ShowWindow((int)handleArray[i], 0);
                }
            }
        }

        private void FuncShow(object sender, EventArgs e){
            /*foreach (ListViewItem item in processListView.CheckedItems){
                ShowWindow((int)handleArray[3], 5);
            }*/

            for (int i = 0; i < processListView.Items.Count; i++){
                if (processListView.Items[i].Checked == true){
                    ShowWindow((int)handleArray[i], 5);
                }
            }
        }

        //오버라이드
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData){
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch(key){
                case Keys.F:
                    if((keyData & Keys.Control) != 0){
                        MessageBox.Show("Ctrl + F");
                        return true;
                    }
                    break;
                case Keys.F5:
                    MessageBox.Show("F5");
                    return true;
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SettingFormShow(object sender, EventArgs e){
            Form2 popForm = new Form2();
            popForm.ShowDialog();
        }
    }
}
