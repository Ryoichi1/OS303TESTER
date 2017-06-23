using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow
    {

        DispatcherTimer timerTextInput;
        DispatcherTimer timerStartCamera;
        DispatcherTimer timerStart温度計;


        Uri uriTestPage = new Uri("Page/Test/Test.xaml", UriKind.Relative);
        Uri uriConfPage = new Uri("Page/Config/Conf.xaml", UriKind.Relative);
        Uri uriHelpPage = new Uri("Page/Help/Help.xaml", UriKind.Relative);

        public MainWindow()
        {
            InitializeComponent();
            App._naviTest = FrameTest.NavigationService;
            App._naviConf = FrameConf.NavigationService;
            App._naviHelp = FrameHelp.NavigationService;
            App._naviErrInfo = FrameErrInfo.NavigationService;

            this.MouseLeftButtonDown += (sender, e) => this.DragMove();//ウィンドウ全体でドラッグ可能にする

            this.DataContext = State.VmMainWindow;



            //タイマーの設定
            timerTextInput = new DispatcherTimer(DispatcherPriority.Normal);
            timerTextInput.Interval = TimeSpan.FromMilliseconds(1000);
            timerTextInput.Tick += timerTextInput_Tick;
            timerTextInput.Start();

            //タイマーの設定
            timerStartCamera = new DispatcherTimer(DispatcherPriority.Normal);
            timerStartCamera.Interval = TimeSpan.FromMilliseconds(1000);
            timerStartCamera.Tick += (object sender, EventArgs e) =>
            {
                if (Flags.StateCamera)
                {
                    timerStartCamera.Stop();
                    General.cam.Start();
                    General.cam.ImageOpacity = Constants.OpacityImgMin;
                    Task.Run(() =>
                    {
                        Thread.Sleep(3000);
                        General.cam.Exposure = 0;
                        Thread.Sleep(2000);
                        General.cam.Exposure = -7;
                        Thread.Sleep(2000);
                        General.cam.Exposure = State.camProp.Exposure;

                    });

                }
            };
            timerStartCamera.Start();

            //タイマーの設定
            timerStart温度計 = new DispatcherTimer(DispatcherPriority.Normal);
            timerStart温度計.Interval = TimeSpan.FromMilliseconds(1000);
            timerStart温度計.Tick += (object sender, EventArgs e) =>
            {
                if (Flags.State温度計)
                {
                    timerStart温度計.Stop();
                    General.thermometer.MeasTemp();
                }

            };
            timerStart温度計.Start();

            GetInfo();

            //カレントディレクトリの取得
            State.CurrDir = Directory.GetCurrentDirectory();

            //コンピュータ名の取得
            var pcName = System.Net.Dns.GetHostName();
            //使用しているパソコンが1号機 or 2号機のどちらなのかチェックする
            if (pcName == "PC81TESTER1")
            {
                State.PcName = State.PC_NAME.PC1;
                State.CheckerNumber = "-0001";
            }
            else
            {
                State.PcName = State.PC_NAME.PC2;
                State.CheckerNumber = "-0002";
            }



            //試験用パラメータのロード
            State.LoadConfigData();

            General.Init周辺機器();//非同期処理です

            //システム時計の設定 ネットに接続されていた場合のみ設定する
            SystemTime.SetSystemTime();

            InitMainForm();//メインフォーム初期

            this.WindowState = WindowState.Maximized;

        }



        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                while (Flags.Initializing周辺機器) ;

                if (Flags.StateEpx64)
                {
                    General.ResetIo();
                    General.io.Close();//IO閉じる
                }

                if (Flags.StateSignalSource)
                {
                    General.signalSource.StopSource();
                    General.signalSource.ClosePort();
                }

                if (Flags.StateVOAC7602)
                {
                    General.multimeter.ClosePort();
                }

                if (Flags.StateLTM2882A)
                {
                    Target.Port_232C_1.Close();
                }

                if (Flags.StateLTM2882B)
                {
                    Target.Port_232C_2.Close();
                }

                if (Flags.StateLTM2882C)
                {
                    Target.Port_232C_3.Close();
                }

                if (Flags.StateLTM2882D)
                {
                    Target.Port_232C_4.Close();
                }

                if (Flags.State1150A)
                {
                    Target.Port_422_1.Close();
                }

                if (Flags.State1150B)
                {
                    Target.Port_422_2.Close();
                }


                if (!State.Save個別データ())
                {
                    MessageBox.Show("個別データの保存に失敗しました");
                }
                if (!General.SaveRetryLog())
                {
                    MessageBox.Show("リトライログの保存に失敗しました");
                }

            }
            catch
            {
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Flags.Testing)
            {
                e.Cancel = true;
            }
            else
            {
                Flags.StopInit周辺機器 = true;
            }
        }



        void timerTextInput_Tick(object sender, EventArgs e)
        {
            timerTextInput.Stop();
            if (!Flags.SetOpecode)
            {
                State.VmMainWindow.Opecode = "";
            }
        }

        private void cbOperator_DropDownClosed(object sender, EventArgs e)
        {
            if (cbOperator.SelectedIndex == -1)
                return;
            Flags.SetOperator = true;

            if (Flags.SetOpecode)
            {
                return;
            }

            State.VmMainWindow.ReadOnlyOpecode = false;
            tbOpecode.Focus();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (Flags.Testing) return;
            //Flags.SetOperator = false;
            Flags.SetOpecode = false;
        }

        private void tbOpecode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //１文字入力されるごとに、タイマーを初期化する
            timerTextInput.Stop();
            timerTextInput.Start();

            if (State.VmMainWindow.Opecode.Length != 13) return;
            //以降は工番が正しく入力されているかどうかの判定
            if (System.Text.RegularExpressions.Regex.IsMatch(
                State.VmMainWindow.Opecode, @"^\d-\d\d-\d\d\d\d-\d\d\d$",
                System.Text.RegularExpressions.RegexOptions.ECMAScript))
            {
                timerTextInput.Stop();
                Flags.SetOpecode = true;
                string dataFilePath = Constants.PassDataFolderPath + State.VmMainWindow.Opecode + ".csv";

                // 入力した工番の検査データが存在しているかどうか確認する
                if (!System.IO.File.Exists(dataFilePath))
                {
                    //データファイルが存在しなければ、必然的にシリアルナンバーは0001です
                    State.VmMainWindow.SerialNumber = State.VmMainWindow.Opecode + "-0001" + State.CheckerNumber;
                    return;
                }


                //データファイルが存在するなら、ファイルを開いて最終シリアルナンバーをロードする
                if (State.LoadLastSerial(dataFilePath))
                {
                    try
                    {
                        // State.LastSerialの例  3-41-1234-000-0006-0001  ※1号機で試験した工番3-41-1234-000の0006番
                        var reg1 = new Regex(@"\d-\d\d-\d\d\d\d-\d\d\d-");//工番部分を正規表現で表す
                        var reg2 = new Regex(@"-\d\d\d\d");//末尾の号機番号を正規表現で表す
                        var buff = reg1.Replace(State.LastSerial, "");//工番部分を空白で置換する
                        buff = reg2.Replace(buff, "");//号機番号部分を空白で置換する
                        int lastSerial = Int32.Parse(buff);
                        State.NewSerial = lastSerial + 1;
                        State.VmMainWindow.SerialNumber = State.VmMainWindow.Opecode + "-" + State.NewSerial.ToString("D4") + State.CheckerNumber;
                    }
                    catch
                    {
                        MessageBox.Show("シリアルナンバーの取得に失敗しました");
                        Flags.SetOpecode = false;
                    }
                }
                else
                {
                    MessageBox.Show("シリアルナンバーの取得に失敗しました");
                    Flags.SetOpecode = false;

                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TabMenu.SelectedIndex = 0;
        }




        //アセンブリ情報の取得
        private void GetInfo()
        {
            //アセンブリバージョンの取得
            var asm = Assembly.GetExecutingAssembly();
            var M = asm.GetName().Version.Major.ToString();
            var N = asm.GetName().Version.Minor.ToString();
            var B = asm.GetName().Version.Build.ToString();
            State.AssemblyInfo = M + "." + N + "." + B;

        }

        //フォームのイニシャライズ
        private void InitMainForm()
        {
            TabErrInfo.Header = "";//実行時はエラーインフォタブのヘッダを空白にして作業差に見えないようにする
            TabErrInfo.IsEnabled = false; //作業差がTABを選択できないようにします

            State.VmMainWindow.ReadOnlyOpecode = true;
            State.VmMainWindow.EnableOtherButton = true;

            State.VmTestStatus.EnableUnitTest = Visibility.Hidden;
            State.VmMainWindow.OperatorEnable = true;

        }

        //フォーカスのセット
        public void SetFocus()
        {
            if (!Flags.SetOperator)
            {

                if (!cbOperator.IsFocused)
                    cbOperator.Focus();
                return;
            }


            if (!Flags.SetOpecode)
            {
                if (!tbOpecode.IsFocused)
                    tbOpecode.Focus();
                return;
            }


        }


        private void TabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = TabMenu.SelectedIndex;
            if (index == 0)
            {
                Flags.OtherPage = false;//フラグを初期化しておく

                App._naviConf.Refresh();
                App._naviHelp.Refresh();
                App._naviTest.Navigate(uriTestPage);
                SetFocus();//テスト画面に移行する際にフォーカスを必須項目入力欄にあてる
            }
            else if (index == 1)
            {
                Flags.OtherPage = true;
                App._naviConf.Navigate(uriConfPage);
                App._naviHelp.Refresh();
            }
            else if (index == 2)
            {
                Flags.OtherPage = true;
                App._naviHelp.Navigate(uriHelpPage);
                App._naviConf.Refresh();

            }
            else if (index == 3)//ErrInfoタブ 作業者がこのタブを選択することはない。 TEST画面のエラー詳細ボタンを押した時にこのタブが選択されるようコードビハインドで記述
            {
                //Flags.OtherPage = true;
                App._naviErrInfo.Navigate(State.uriErrInfoPage);
                App._naviConf.Refresh();
                App._naviHelp.Refresh();
            }

        }

        private void MainBack_Loaded(object sender, RoutedEventArgs e)
        {
            //システム時計の設定
            if (!SystemTime.SetSystemTime())
            {
                MessageBox.Show("時刻同期できませんでした");
                //Environment.Exit(0);
            }
        }
    }
}
