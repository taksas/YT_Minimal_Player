using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;

namespace YT_Minimal_Player
{

    public partial class Form1 : Form
    {/// <summary>webviewのコントロール（今回はわかりやすい様に、デザイナーを使わずにコード側で実装します。）</summary>

        /*   private WebView2 WebView = new WebView2
            {
                Source = new Uri("https://www.youtube.com/?app=m&persist_app=1"),




            };
    */



        // client configuration
        const string clientID = "909637769108-5gp3onnsc1qvdu5r88iljibe80s8rr3i.apps.googleusercontent.com";
        const string clientSecret = "6SJni7tj599tNYOy01w0NJjX";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
        const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";


        public string UserAgent { get; set; }

        public Form1()
        {
            InitializeComponent();
            InitializeAsync();





            async void InitializeAsync()
            {
                await WebView.EnsureCoreWebView2Async(null);
            }


            /*
    * 表示位置(Top)を調整。
    * 「ディスプレイの作業領域の高さ」-「表示するWindowの高さ」
    */
            StartPosition = FormStartPosition.Manual;

            /*  
                         //表示位置を右下固定にする設定を判断する！
                         int ScreenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                         int Screenheigth = Screen.PrimaryScreen.WorkingArea.Height;
                         int AppWidth = this.Width;
                         int AppHeight = this.Height;

                         int AppLeftXPos = ScreenWidth - AppWidth;
                         int AppLeftYPos = Screenheigth - AppHeight;

                                 Rectangle tempRect = new Rectangle(AppLeftXPos, AppLeftYPos, AppWidth, AppHeight);
                                 this.DesktopBounds = tempRect;

                         this.SetBounds(AppLeftXPos, AppLeftYPos, AppWidth, AppHeight);
                         this.SetDesktopBounds(AppLeftXPos, AppLeftYPos, AppWidth, AppHeight);
               */


            this.Controls.Add(WebView);


            //    this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormBorderStyle = FormBorderStyle.None;

            //WebView2のサイズをフォームのサイズに合わせる
            WebView.Size = this.Size;
            this.SizeChanged += Form1_SizeChanged;

            //WebView2のロード完了時のイベント
            WebView.NavigationCompleted += WebView_NavigationCompleted;

            //WebView2のロード完了時のイベント
            WebView.SourceChanged += WebView_SourceChanged;

            //WebView2のロードスタート時のイベント
            //         WebView.NavigationStarting += WebView_NavigationStarting;

            int left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            int top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            DesktopBounds = new Rectangle(left, top, this.Width, this.Height);

            //      WebView.CoreWebView2.Navigate("https://www.youtube.com/?app=m&persist_app=1");

            // WebView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (iPhone; U; CPU like Mac OS X; en) AppleWebKit/420+ (KHTML, like Gecko) Version/3.0 Mobile/1A543a Safari/419.3";

            //   string ua = "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X)" + "AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25";
            //   Uri targetUri = new Uri("https://www.youtube.com/watch?v=0oNYJ1glXUM");
            //  HttpRequestMessage hrm = new HttpRequestMessage(HttpMethod.Get, targetUri);
            //   hrm.Headers.Add("User-Agent", ua);
            //   WebView.NavigateWithHttpRequestMessage(hrm);
            //     WebView.Top = 1280;
            //     WebView.Height = 720;
        }



        private static ushort WM_KEYDOWN = 0x0100;
        private static ushort WM_KEYUP = 0x0101;
        private static ushort VK_SPACE = 0x20;


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr GetDesktopWindow();

        //送信するためのメソッド(文字も可能)
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 Msg, Int32 wParam, Int32 lParam);


        /// <summary>WebView2のロード完了時</summary>
        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //           this.Activate();
            //          SendKeys.Send(" ");
            //  IntPtr hwnd = GetDesktopWindow();
            // メモ帳のウインドウハンドル取得
            //  hwnd = FindWindowEx(hwnd, IntPtr.Zero, "YT_Minimal_Player", null);
            // メモ帳ウインドウ内の「edit」ウインドウのハンドル取得
            //   hwnd = FindWindowEx(hwnd, IntPtr.Zero, "WebView2", null);

            //        IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;


            //     PostMessage((int)hwnd, WM_KEYDOWN, VK_SPACE, 0);
            //         PostMessage((int)hwnd, WM_KEYUP, VK_SPACE, 0);


            WebView.CoreWebView2.ExecuteScriptAsync("document.querySelector('body').style.overflow='scroll';" +
               "var style=document.createElement('style');" +
               "style.type = 'text/css';" +
               "style.innerHTML='::-webkit-scrollbar{display:none}';" +
               "document.getElementsByTagName('body')[0].appendChild(style);");

            //WebView2のコントロールから CoreWebView2を取り出す
            //WebView2のロードが完了する前に CoreWebView2を取り出そうとすると nullになる。
            if (WebView.CoreWebView2 != null)
            {
                //ブラウザーのポップアップ発生時のイベント
                //              WebView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            }
            else MessageBox.Show("WebView.CoreWebView2 == null");
        }


        private void WebView_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {


            //   WebView.CoreWebView2.Navigate("https://www.youtube.com/watch?v=0oNYJ1glXUM");

        }

        /// <summary>WebView2でのポップアップを抑止したい</summary>
        /*      private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
              {
                  //本来なら、これでポップアップを抑止できるはず
                  //だが、ロードの速度の都合で e.NewWindow(CoreWebView2)が nullになる場合があり、エラーが発生する
                  if (e.NewWindow != null)
                  {
                      e.NewWindow.Stop();
                      MessageBox.Show("ポップアップを抑止しました");
                  }
                  else
                  {
                      //ダミーのCoreWebView2を読み込ませてポップアップを抑止する
                      //e.NewWindow = new Microsoft.Web.WebView2.Core.CoreWebView2();
                      //↑ちなみに、これはできない
                      e.NewWindow = DummyWebView.CoreWebView2;
                      e.NewWindow.Stop();

                      //これでJavaScriptが実行できる
                      WebView.ExecuteScriptAsync("alert(\"ポップアップを抑止しました\");");
                  }
            }
         */
        /// <summary>ポップアップ抑止に使用するダミーのWebView2（の中のCoreWebView2）</summary>
        private WebView2 DummyWebView = new WebView2
        {
            Source = new Uri("https://www.google.co.jp/"),
        };

        /// <summary>サイズ変更時のイベントでWebView2のサイズをフォームに合わせる</summary>
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            WebView.Size = this.Size;
        }



    }
}