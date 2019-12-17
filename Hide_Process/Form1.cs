using System;
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
        public static extern int EnumWindows(EnumWindowCallback callback, int y);

        [DllImport("user32.dll")]
        public static extern int GetParent(int hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count); //1:파라미터 2:반환값 3:

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(int hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("user32")]
        public static extern int ShowWindow(int hwnd, int nCmdShow);

        const int GCL_HICON = -14; //GetWindowLong을 호출할 때 쓸 인자
        const int GCL_HMODULE = -16;
        ImageList imgList;//ListView의 Image로 쓸 리스트

        ArrayList handleArray = null; //숨길 핸들러 저장

        public Form1(){
            InitializeComponent();

            imgList = new ImageList();
            imgList.ImageSize = new Size(16, 16);
            processListView.SmallImageList = imgList;
            processListView.View = View.List;

            //EnumWindowCallback callback = new EnumWindowCallback(EnumWindowsProc);
            //EnumWindows(callback, 0);

            this.hideBtn.Click += FuncHide;
        }

        private void Form1_Load(object sender, EventArgs e){
            handleArray = new ArrayList();
            loadProcessList();
        }

        public bool EnumWindowsProc(int hWnd, int lParam){
            //윈도우 핸들로 그 윈도우의 스타일을 얻어옴
            UInt32 style = (UInt32)GetWindowLong(hWnd, GCL_HMODULE);
            //해당 윈도우의 캡션이 존재하는지 확인
            if ((style & 0x10000000L) == 0x10000000L && (style & 0x00C00000L) == 0x00C00000L){
                //부모가 바탕화면인지 확인
                if (GetParent(hWnd) == 0){
                    StringBuilder Buf = new StringBuilder(256);
                    //응용프로그램의 이름을 얻어온다
                    if (GetWindowText(hWnd, Buf, 256) > 0){
                        try{
                            //HICON 아이콘 핸들을 얻어온다
                            IntPtr hIcon = GetClassLong((IntPtr)hWnd, GCL_HICON);
                            //아이콘 핸들로 Icon 객체를 만든다
                            Icon icon = Icon.FromHandle(hIcon);
                            imgList.Images.Add(icon);
                        }catch (Exception){
                            //예외의 경우는 자기 자신의 윈도우인 경우이다.
                            imgList.Images.Add(this.Icon);
                        }
                        processListView.Items.Add(new ListViewItem(Buf.ToString(), processListView.Items.Count));
                 
                    }
                }
            }
            return true;
        }

        private void loadProcessList(){
            Process[] processVar = Process.GetProcesses();

            processListView.Items.Clear();
            handleArray.Clear();
            foreach (Process process in processVar){
                if (!String.IsNullOrEmpty(process.MainWindowTitle)){
                    IntPtr mhandle = process.MainWindowHandle;
                    handleArray.Add(mhandle.ToInt32());
                    processListView.Items.Add(process.MainWindowTitle, "123");
                }
            }
        }

        public void FuncHide(object sender, EventArgs e){
            //ShowWindow((int)processList[1], 0);

        }
    }
}
