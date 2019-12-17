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

namespace Hide_Process{
    public partial class Form1 : Form{
        public delegate bool EnumWindowCallback(int hwnd, int lParam);//알아내고자하는 핸들대상, 윈도우의 캡션값을 반환

        /* Win32API들 */
        [DllImport("user32.dll")]//핸들값을 넘겨주면 캡션값을 반환해줌
        public static extern int EnumWindows(EnumWindowCallback callback, int y);//y : 받을 텍스트값의 최대크기

        [DllImport("user32.dll")]
        public static extern int GetParent(int hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(int hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetClassLong(IntPtr hwnd, int nIndex);

        const int GCL_HICON = -14; //GetWindowLong을 호출할 때 쓸 인자
        const int GCL_HMODULE = -16;
        ImageList imgList;//ListView의 Image로 쓸 리스트

        public Form1(){
            InitializeComponent();

            imgList = new ImageList();
            imgList.ImageSize = new Size(16, 16);
            listView1.SmallImageList = imgList;
            listView1.View = View.List;
            //CallBack 델리게이트 생성
            EnumWindowCallback callback = new EnumWindowCallback(EnumWindowsProc);
            EnumWindows(callback, 0);
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
                        listView1.Items.Add(new ListViewItem(Buf.ToString(), listView1.Items.Count));
                    }
                }
            }
            return true;
        }
    }
}
