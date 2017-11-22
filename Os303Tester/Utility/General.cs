using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System;

namespace Os303Tester
{


    public static class General
    {

        //インスタンス変数の宣言
        public static EPX64S io;
        public static SoundPlayer player = null;
        public static SoundPlayer soundPassLong = null;
        public static SoundPlayer soundFail = null;
        public static SoundPlayer soundAlarm = null;
        public static SoundPlayer soundSave = null;


        public static SolidColorBrush NgBrush = new SolidColorBrush();


        //インスタンスを生成する必要がある周辺機器
        public static Camera cam;
        public static Multimeter multimeter;

        static General()
        {
            //オーディオリソースを取り出す
            General.soundPassLong = new SoundPlayer(@"Resources\PassLong.wav");
            General.soundFail = new SoundPlayer(@"Resources\Fail.wav");
            General.soundAlarm = new SoundPlayer(@"Resources\Alarm.wav");
            General.soundSave = new SoundPlayer(@"Resources\Save.wav");

            NgBrush.Color = Colors.HotPink;
            NgBrush.Opacity = 0.4;
        }

        public static void Show()
        {
            var T = 0.3;
            var t = 0.005;

            State.Setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
            //10msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);

            State.VmMainWindow.ThemeOpacity = 0;
            Task.Run(() =>
            {
                while (true)
                {

                    State.VmMainWindow.ThemeOpacity += State.Setting.OpacityTheme / (double)times;
                    Thread.Sleep((int)(t * 1000));
                    if (State.VmMainWindow.ThemeOpacity >= State.Setting.OpacityTheme) return;

                }
            });
        }

        public static void SetRadius(bool sw)
        {
            var T = 0.45;//アニメーションが完了するまでの時間（秒）
            var t = 0.005;//（秒）

            //5msec刻みでT秒で元のOpacityに戻す
            int times = (int)(T / t);


            Task.Run(() =>
            {
                if (sw)
                {
                    while (true)
                    {
                        State.VmMainWindow.ThemeBlurEffectRadius += 25 / (double)times;
                        Thread.Sleep((int)(t * 1000));
                        if (State.VmMainWindow.ThemeBlurEffectRadius >= 25) return;

                    }
                }
                else
                {
                    var CurrentRadius = State.VmMainWindow.ThemeBlurEffectRadius;
                    while (true)
                    {
                        CurrentRadius -= 25 / (double)times;
                        if (CurrentRadius > 0)
                        {
                            State.VmMainWindow.ThemeBlurEffectRadius = CurrentRadius;
                        }
                        else
                        {
                            State.VmMainWindow.ThemeBlurEffectRadius = 0;
                            return;
                        }
                        Thread.Sleep((int)(t * 1000));
                    }
                }

            });
        }


        //public static void Show2(bool sw)
        //{

        //    var T = 0.3;
        //    var t = 0.005;

        //    var 差 = State.CurrentThemeOpacity - Constants.OpacityMin;
        //    //10msec刻みでT秒で元のOpacityに戻す
        //    int times = (int)(T / t);


        //    Task.Run(() =>
        //    {
        //        if (sw)
        //        {
        //            while (true)
        //            {
        //                State.VmMainWindow.ThemeOpacity += 差 / (double)times;
        //                Thread.Sleep((int)(t * 1000));
        //                if (State.VmMainWindow.ThemeOpacity >= State.CurrentThemeOpacity) return;
        //            }
        //        }
        //        else
        //        {
        //            while (true)
        //            {
        //                State.VmMainWindow.ThemeOpacity -= 差 / (double)times;
        //                Thread.Sleep((int)(t * 1000));
        //                if (State.VmMainWindow.ThemeOpacity <= Constants.OpacityMin) return;
        //            }
        //        }

        //    });
        //}


        //試験機のリレーRL1の接点が溶着していないかチェックする
        //電源をOFF（RL1をOFF）してからチェックする
        public static bool CheckPowRelay()
        {
            io.ReadInputData(EPX64S.PORT.P3);
            var P3Data = io.P3InputData;
            return Flags.StateRL1 = (P3Data & 0x08) == 0x00;
        }



        //**************************************************************************
        //プレス治具のレバーが下がっているかどうかの判定
        //引数：なし
        //戻値：bool　プレスのレバーが下がっていればtrue
        //**************************************************************************

        public static bool SaveRetryLog()
        {
            if (State.RetryLogList.Count() == 0) return true;

            //出力用のファイルを開く appendをtrueにすると既存のファイルに追記、falseにするとファイルを新規作成する
            using (var sw = new System.IO.StreamWriter(Constants.fileName_RetryLog, true, Encoding.GetEncoding("Shift_JIS")))
            {
                try
                {
                    State.RetryLogList.ForEach(d =>
                    {
                        sw.WriteLine(d);
                    });

                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }



        private static List<string> MakePassTestData()//TODO:
        {
            var ListData = new List<string>
            {
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.TestSpec.TestSpecVer,
                State.VmMainWindow.Opecode,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH:mm:ss"),
                State.VmMainWindow.Operator,

                State.TestSpec.FwVer,
                State.TestSpec.FwSum,

                State.VmTestResults.VolAc100,
                State.VmTestResults.VolVdd,
                State.VmTestResults.VolVcc1,
                State.VmTestResults.VolVcc2On,
                State.VmTestResults.VolVcc2Off,
                State.VmTestResults.HueLED1,
                State.VmTestResults.HueLED2,
                State.VmTestResults.HueLED3,

            };

            return ListData;
        }

        public static bool SaveTestData()
        {
            try
            {
                var OkDataFilePath = Constants.PassDataFolderPath + State.VmMainWindow.Opecode + ".csv";

                if (!System.IO.File.Exists(OkDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.PassDataFolderPath + "Format.csv", OkDataFilePath);
                }

                var dataList = MakePassTestData();
                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", dataList);

                // appendをtrueにすると，既存のファイルに追記
                //         falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(OkDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //検査データの保存　　　　
        //引数：なし
        //戻値：なし
        //**************************************************************************

        public static bool SaveNgData(List<string> dataList)
        {
            try
            {
                var NgDataFilePath = Constants.FailDataFolderPath + State.VmMainWindow.Opecode + ".csv";
                if (!System.IO.File.Exists(NgDataFilePath))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.FailDataFolderPath + "FormatNg.csv", NgDataFilePath);
                }

                var stArrayData = dataList.ToArray();
                // リストデータをすべてカンマ区切りで連結する
                string stCsvData = string.Join(",", stArrayData);

                // appendをtrueにすると，既存のファイルに追記
                //         falseにすると，ファイルを新規作成する
                var append = true;

                // 出力用のファイルを開く
                using (var sw = new System.IO.StreamWriter(NgDataFilePath, append, Encoding.GetEncoding("Shift_JIS")))
                {
                    sw.WriteLine(stCsvData);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool CheckPressOpen()
        {
            io.ReadInputData(EPX64S.PORT.P3);
            byte buff = io.P3InputData;
            return (buff & 0x01) == 0x01;
        }

        //**************************************************************************
        //EPX64のリセット
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static void ResetIo() //P7:0 P6:0 P5:0 P4:0  P3:0 P2:0 P1:1 P0:1  
        {
            //IOを初期化する処理（出力をすべてＬに落とす）
            io.OutByte(EPX64S.PORT.P0, 0x00);
            io.OutByte(EPX64S.PORT.P1, 0x00);

            Flags.PowOn = false;
        }



        public static void PowSupply(bool sw)
        {
            if (Flags.PowOn == sw) return;
            SetRL1(sw);
            Flags.PowOn = sw;
        }

        public static async void StampOn()
        {
            io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b3, EPX64S.OUT.H);
            await Task.Delay(350);
            io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b3, EPX64S.OUT.L);
        }

        public static void S1On()
        {
            io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b4, EPX64S.OUT.H);
            Thread.Sleep(500);
            io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b4, EPX64S.OUT.L);
        }



        //**************************************************************************
        //WAVEファイルを再生する
        //引数：なし
        //戻値：なし
        //**************************************************************************  

        //WAVEファイルを再生する（非同期で再生）
        public static void PlaySound(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.Play();
        }
        //WAVEファイルを再生する（同期で再生）
        public static void PlaySound2(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.PlaySync();

        }

        public static void PlaySoundLoop(SoundPlayer p)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = p;
            //最後まで再生し終えるまで待機する
            player.PlayLooping();
        }

        //再生されているWAVEファイルを止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }



        public static void ResetViewModel()//TODO:
        {
            //ViewModel OK台数、NG台数、Total台数の更新
            State.VmTestStatus.OkCount = State.Setting.TodayOkCount.ToString() + "台";
            State.VmTestStatus.NgCount = State.Setting.TodayNgCount.ToString() + "台";
            State.VmTestStatus.TotalCount = State.Setting.TotalTestCount.ToString() + "台";


            State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Hidden;
            State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Hidden;

            State.VmTestStatus.RingVisibility = System.Windows.Visibility.Visible;



            State.VmTestStatus.TestTime = "00:00";
            State.VmTestStatus.進捗度 = 0;
            State.VmTestStatus.TestLog = "";

            State.VmTestStatus.FailInfo = "";
            State.VmTestStatus.Spec = "";
            State.VmTestStatus.MeasValue = "";


            //試験結果のクリア
            State.VmTestResults = new ViewModelTestResult();

            State.VmTestStatus.Message = Constants.MessSet;
            State.VmMainWindow.EnableOtherButton = true;

            //各種フラグの初期化
            Flags.PowOn = false;
            Flags.Testing = false;

            //テーマ透過度を元に戻す
            General.SetRadius(false);


            State.VmTestStatus.ColorRetry = Brushes.Transparent;
            State.VmTestStatus.TestSettingEnable = true;
            State.VmMainWindow.OperatorEnable = true;


            General.cam.ImageOpacity = Constants.OpacityImgMin;
        }


        public static void Init周辺機器()//TODO:
        {

            Flags.Initializing周辺機器 = true;

            //EPX64Sの初期化
            bool StopEpx64s = false;
            Task.Run(() =>
            {
                //IOボードの初期化
                General.io = new EPX64S();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateEpx64 = General.io.InitEpx64S(0x03);//0000 0011  ※P7のみ入力
                    if (Flags.StateEpx64)
                    {
                        //IOボードのリセット（出力をすべてLする）
                        General.ResetIo();
                        break;
                    }

                    Thread.Sleep(250);
                }
                StopEpx64s = true;
            });

            //リレー溶着チェック
            bool StopRelayCheck = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    //IOボードの初期化
                    if (!Flags.StateEpx64)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    //電源リレーRL1が溶着していないかチェックしてから非同期処理を終了する
                    if (CheckPowRelay()) break;
                    Thread.Sleep(500);
                }
                StopRelayCheck = true;
            });

            //VOAC7602の初期化
            bool StopVOAC7502 = false;
            Task.Run(() =>
            {
                multimeter = new VOAC7502();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateVOAC7502 = multimeter.Init();
                    if (Flags.StateVOAC7502) break;

                    Thread.Sleep(300);
                }
                StopVOAC7502 = true;
            });

            //カメラ（CMS_V37BK）の初期化
            bool StopCAMERA = false;
            Task.Run(() =>
            {
                General.cam = new Camera(0);

                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateCamera = General.cam.InitCamera();
                    if (Flags.StateCamera)
                    {
                        State.SetCamProp();
                        break;
                    }

                    Thread.Sleep(300);
                }
                StopCAMERA = true;
            });

            Task.Run(() =>
            {
                while (true)
                {
                    Flags.AllOk周辺機器接続 = (Flags.StateEpx64 && Flags.StateVOAC7502 && Flags.StateCamera && Flags.StateRL1);

                    //EPX64Sの初期化の中で、RL1の溶着チェックを行っているが、これがNGだとしてもInit周辺機器()は終了する
                    var IsAllStopped = StopEpx64s && StopRelayCheck && StopVOAC7502 && StopCAMERA;

                    if (Flags.AllOk周辺機器接続 || IsAllStopped) break;
                    Thread.Sleep(400);

                }
                Flags.Initializing周辺機器 = false;
            });
        }

        public static async Task Discharge()
        {
            SetK18(true);
            await Task.Delay(2000);
            SetK18(false);
        }


        public static void ResetRelay_7502()
        {
            SetK14(false);
            SetK15(false);
            SetK16(false);
            SetK17(false);
        }

        //試験機リレー制御
        public static void SetRL1(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL2(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL3(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL4(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL5(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetRL6(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK10_11(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK12_13(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }

        public static void SetK14(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK15(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }
        public static void SetK16(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }

        public static void SetK17(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }//AC100チェック用
        public static void SetK18(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); }//放電用


    }

}

