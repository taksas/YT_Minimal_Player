using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
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


namespace YT_Minimal_Player
{

    public partial class Form1 : Form
    {
        public int m;
        class HotKey
    {
        [DllImport("user32", SetLastError = true)]
        private static extern int RegisterHotKey(IntPtr hWnd,
                                                 int id,
                                                 int fsModifier,
                                                 int vk);

        [DllImport("user32", SetLastError = true)]
        private static extern int UnregisterHotKey(IntPtr hWnd,
                                                   int id);

        public HotKey(IntPtr hWnd, int id, Keys key)
        {
            this.hWnd = hWnd;
            this.id = id;

            // Keys列挙体の値をWin32仮想キーコードと修飾キーに分離
            int keycode = (int)(key & Keys.KeyCode);
            int modifiers = (int)(key & Keys.Modifiers) >> 16;

            this.lParam = new IntPtr(modifiers | keycode << 16);

                if (RegisterHotKey(hWnd, id, modifiers, keycode) == 0) ;
                // ホットキーの登録に失敗
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public void Unregister()
        {
            if (hWnd == IntPtr.Zero)
                return;

            if (UnregisterHotKey(hWnd, id) == 0)
                // ホットキーの解除に失敗
           //     throw new Win32Exception(Marshal.GetLastWin32Error());

            hWnd = IntPtr.Zero;
        }

        public IntPtr LParam
        {
            get { return lParam; }
        }

        private IntPtr hWnd; // ホットキーの入力メッセージを受信するウィンドウのhWnd
        private readonly int id; // ホットキーのID(0x0000〜0xBFFF)
        private readonly IntPtr lParam; // WndProcメソッドで押下されたホットキーを識別するためのlParam値
    }




        /// <summary>webviewのコントロール（今回はわかりやすい様に、デザイナーを使わずにコード側で実装します。）</summary>

        /*   private WebView2 WebView = new WebView2
            {
                Source = new Uri("https://www.youtube.com/?app=m&persist_app=1"),




            };
    */



        // client configuration
        /*     const string clientID = "909637769108-5gp3onnsc1qvdu5r88iljibe80s8rr3i.apps.googleusercontent.com";
             const string clientSecret = "6SJni7tj599tNYOy01w0NJjX";
             const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
             const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
             const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";
        */

        public string UserAgent { get; set; }

        public Form1()
        {
            InitializeComponent();
            InitializeAsync();

            async void InitializeAsync()
            {
                await WebView.EnsureCoreWebView2Async(null);
                string strip = "Mozilla/5.0 (SMART-TV; Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.164 Safari/537.36";
                WebView.CoreWebView2.Settings.UserAgent = strip;
            }
            this.TopMost = !this.TopMost;

            this.ShowInTaskbar = false;

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
            WebView.NavigationStarting += WebView_NavigationStarting;

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



            // コンテキストメニュー
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();



            ToolStripMenuItem toolStripMenuItemhome = new ToolStripMenuItem();
            toolStripMenuItemhome.Text = "ホーム";
            toolStripMenuItemhome.Click += ToolStripMenuItem_Clickhome;
            contextMenuStrip.Items.Add(toolStripMenuItemhome);


/*
            ToolStripMenuItem toolStripMenuItemexp = new ToolStripMenuItem();
            toolStripMenuItemexp.Text = "検索";
            toolStripMenuItemexp.Click += ToolStripMenuItem_Clickexp;
            contextMenuStrip.Items.Add(toolStripMenuItemexp);
*/
            ToolStripMenuItem toolStripMenuItemchannel = new ToolStripMenuItem();
            toolStripMenuItemchannel.Text = "登録チャンネル";
            toolStripMenuItemchannel.Click += ToolStripMenuItem_Clickchannel;
            contextMenuStrip.Items.Add(toolStripMenuItemchannel);

            ToolStripMenuItem toolStripMenuItemlib = new ToolStripMenuItem();
            toolStripMenuItemlib.Text = "ライブラリ";
            toolStripMenuItemlib.Click += ToolStripMenuItem_Clicklib;
            contextMenuStrip.Items.Add(toolStripMenuItemlib);



            /*
                      contextMenuStrip.Items.Add("-");


                      ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem();
                      toolStripMenuItem1.Text = "進む";
                      toolStripMenuItem1.Click += ToolStripMenuItem_Click1;
                      contextMenuStrip.Items.Add(toolStripMenuItem1);

                      ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
                      toolStripMenuItem2.Text = "戻る";
                      toolStripMenuItem2.Click += ToolStripMenuItem_Click2;
                      contextMenuStrip.Items.Add(toolStripMenuItem2);
          
            contextMenuStrip.Items.Add("-");

            ToolStripMenuItem toolStripMenuItemstop = new ToolStripMenuItem();
            toolStripMenuItemstop.Text = "ページの読み込み中止";
            toolStripMenuItemstop.Click += ToolStripMenuItem_Clickstop;
            contextMenuStrip.Items.Add(toolStripMenuItemstop);
*/
            contextMenuStrip.Items.Add("-");

            ToolStripMenuItem toolStripMenuItemreload = new ToolStripMenuItem();
            toolStripMenuItemreload.Text = "再読み込み";
            toolStripMenuItemreload.Click += ToolStripMenuItem_Clickreload;
            contextMenuStrip.Items.Add(toolStripMenuItemreload);

            ToolStripMenuItem toolStripMenuItemurl = new ToolStripMenuItem();
            toolStripMenuItemurl.Text = "URL取得";
            toolStripMenuItemurl.Click += ToolStripMenuItem_Clickurl;
            contextMenuStrip.Items.Add(toolStripMenuItemurl);


            contextMenuStrip.Items.Add("-");

            ToolStripMenuItem toolStripMenuItemhelp = new ToolStripMenuItem();
            toolStripMenuItemhelp.Text = "ヘルプ";
            toolStripMenuItemhelp.Click += ToolStripMenuItem_Clickhelp;
            contextMenuStrip.Items.Add(toolStripMenuItemhelp);



            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "&終了";
            toolStripMenuItem.Click += ToolStripMenuItem_Click;
            contextMenuStrip.Items.Add(toolStripMenuItem);






            notifyIcon.ContextMenuStrip = contextMenuStrip;


        }


        private void ToolStripMenuItem_Clickhome(object sender, EventArgs e)
        {
            WebView.CoreWebView2.Navigate("https://www.youtube.com/tv#/");
        }
/*
        private void ToolStripMenuItem_Clickexp(object sender, EventArgs e)
        {
            WebView.CoreWebView2.Navigate("https://www.youtube.com/tv#/search?");
        }
*/
        private void ToolStripMenuItem_Clickchannel(object sender, EventArgs e)
        {
            WebView.CoreWebView2.Navigate("https://www.youtube.com/tv#/browse?c=FEsubscriptions");
        }

        private void ToolStripMenuItem_Clicklib(object sender, EventArgs e)
        {
            WebView.CoreWebView2.Navigate("https://www.youtube.com/tv#/browse?c=FEmy_youtube");
        }




   /*     private void ToolStripMenuItem_Click1(object sender, EventArgs e)
        {
            WebView.GoForward();
        }

        private void ToolStripMenuItem_Click2(object sender, EventArgs e)
        {
            WebView.GoBack();
        }
   */

        private void ToolStripMenuItem_Clickstop(object sender, EventArgs e)
        {
            WebView.Stop();
        }
        private void ToolStripMenuItem_Clickreload(object sender, EventArgs e)
        {
            WebView.Reload();
        }
        private void ToolStripMenuItem_Clickurl(object sender, EventArgs e)
        {
            string msgtext1raw = WebView.Source.ToString();
            string msgtext1 = msgtext1raw.Replace("tv#/", "");
            if (MessageBox.Show(msgtext1, "現在再生中の動画URL（OKをクリックでクリップボードにコピーされます）", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            { System.Windows.Forms.Clipboard.SetText(msgtext1); }


        }
        private void ToolStripMenuItem_Clickhelp(object sender, EventArgs e)
        {
            string msgtext = "https://github.com/taksas/YT_Minimal_Player";
            if (MessageBox.Show(msgtext, "詳細はこちらを確認してください（OKをクリックでクリップボードにコピーされます）", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            { System.Windows.Forms.Clipboard.SetText(msgtext); }
        }





        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);




        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            string strip = "Mozilla/5.0 (SMART-TV; Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.164 Safari/537.36";
            WebView.CoreWebView2.Settings.UserAgent = strip;
        }
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
/*
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {

            if (m == 0)

            {
                this.WindowState = FormWindowState.Minimized;
                m = 1;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                m = 0;
            }
        }
*/

        private HotKey hotkeyStartStop;
  //      private HotKey hotkeyNotify;
        private HotKey hotkeyb15s;
        private HotKey hotkeya15s;
        const int WM_HOTKEY = 0x312;

        protected override void OnLoad(EventArgs e)
        {
            const Keys modifierWinKey = (Keys)0x00080000; // Windowsロゴキー

            // Ctrl+SpaceをID 0のホットキーとして登録
            hotkeyStartStop = new HotKey(this.Handle, 0, Keys.Control | Keys.Space);

            // Win+EnterをID 1のホットキーとして登録
 //           hotkeyNotify = new HotKey(this.Handle, 1, modifierWinKey | Keys.Enter);

            hotkeyb15s = new HotKey(this.Handle, 2, Keys.Control | Keys.A);
            hotkeya15s = new HotKey(this.Handle, 2, Keys.Control | Keys.D);

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // 登録しているホットキーを解除
            hotkeyStartStop.Unregister();
            hotkeyb15s.Unregister();
            hotkeya15s.Unregister();
        }

        // ホットキーの入力メッセージを処理する
        protected override void WndProc(ref Message m)
        {



            if (m.Msg == WM_HOTKEY && m.LParam == hotkeyStartStop.LParam)
            {
                // フォームをアクティブにする
                Activate();
                //           this.Activate();
                          SendKeys.Send(" ");
            }
  /*          else if (m.Msg == WM_HOTKEY && m.LParam == hotkeyNotify.LParam)
            {
                // 通知音を鳴らす
                SystemSounds.Beep.Play();
            }
  */



            else if (m.Msg == WM_HOTKEY && m.LParam == hotkeyb15s.LParam)
            {
                // フォームをアクティブにする
                              Activate();
                             SendKeys.Send("J");
                             SendKeys.Send("{ENTER}");


                
            }

            else if (m.Msg == WM_HOTKEY && m.LParam == hotkeya15s.LParam)
            {
                // フォームをアクティブにする
                Activate();
                SendKeys.Send("L");
                SendKeys.Send("{ENTER}");
            }

            else
            {
                base.WndProc(ref m);
            }
        }


    }
}