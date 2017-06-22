using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System;

namespace NewPC81Tester
{


    public static class General
    {

        //インスタンス変数の宣言
        public static EPX64S io;
        public static SoundPlayer player = null;
        public static SoundPlayer soundPass = null;
        public static SoundPlayer soundPassLong = null;
        public static SoundPlayer soundFail = null;
        public static SoundPlayer soundAlarm = null;
        public static SoundPlayer soundKuru = null;
        public static SoundPlayer soundCutin = null;
        public static SoundPlayer soundContinue = null;
        public static SoundPlayer soundBattery = null;
        public static SoundPlayer soundBgm1 = null;
        public static SoundPlayer soundBgm2 = null;
        public static SoundPlayer soundSerialLabel = null;


        //インスタンスを生成する必要がある周辺機器
        public static USBRH thermometer;
        public static Camera cam;
        public static Multimeter multimeter;
        public static SignalSource signalSource;

        static General()
        {
            //オーディオリソースを取り出す
            General.soundPass = new SoundPlayer(@"Resources\Pass.wav");
            General.soundPassLong = new SoundPlayer(@"Resources\PassLong.wav");
            General.soundFail = new SoundPlayer(@"Resources\Fail.wav");
            General.soundAlarm = new SoundPlayer(@"Resources\Alarm.wav");
            General.soundKuru = new SoundPlayer(@"Resources\Kuru.wav");
            General.soundCutin = new SoundPlayer(@"Resources\CutIn.wav");
            General.soundContinue = new SoundPlayer(@"Resources\Continue.wav");
            General.soundBgm1 = new SoundPlayer(@"Resources\bgm01.wav");
            General.soundBgm2 = new SoundPlayer(@"Resources\bgm02.wav");
            General.soundBattery = new SoundPlayer(@"Resources\battery.wav");
        }


        public static void SetMetalMode()
        {
            //メタルモードにするかどうかの判定（夕方5時～朝6時まで突入）
            var Time = Int32.Parse(System.DateTime.Now.ToString("HH"));
            Flags.MetalMode = (Time >= 17 || Time >= 0 && Time <= 6);
        }

        public static void SetBgm()
        {
            if (!Flags.MetalMode || State.VmTestStatus.CheckUnitTest == true) return;
            //シード値を指定しないとシード値として Environment.TickCount が使用される
            System.Random r = new System.Random();

            if (Flags.LoveBig)
            {
                PlaySoundLoop(soundBgm1);
            }
            else
            {
                PlaySoundLoop(soundBgm2);
            }

        }

        public static void SetCutIn()
        {

            //メタルモードにするかどうかの判定（夕方5時～朝6時まで突入）
            var battleTime = Int32.Parse(System.DateTime.Now.ToString("HH"));
            if (battleTime >= 17 || (battleTime >= 0 && battleTime <= 6))
            {
                Flags.MetalMode = true;

                //シード値を指定しないとシード値として Environment.TickCount が使用される
                System.Random r = new System.Random();

                //0以上100未満の乱数を整数で返す
                int random = r.Next(100);
                if (random > 50)
                {
                    PlaySound(soundCutin);
                }
                else
                {
                    PlaySound(soundKuru);
                }

            }
            else
            {
                Flags.MetalMode = false;
            }
        }


        //試験機のリレーK100、K101の接点が溶着していないかチェックする
        //電源をOFF（K100、K101をOFF）してからチェックする
        public static bool CheckPowRelay()
        {
            var P7Data = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC5);
            Flags.StateK100 = (P7Data & 0x40) == 0x40;
            Flags.StateK101 = (P7Data & 0x80) == 0x80;
            return (Flags.StateK100 && Flags.StateK101);
        }

        public static bool CheckComm()
        {
            string buff = "";
            try
            {
                buff = Target.Port_232C_1.ReadLine();
                //先頭のSTXを取り除いた文字列を抽出する
                buff = buff.Trim((char)0x02);
                State.VmComm.DATA_RX = buff;

                return buff.Contains("ReqData");
            }
            catch
            {
                return false;
            }

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
                State.VmMainWindow.SerialNumber,
                "AssemblyVer " + State.AssemblyInfo,
                "TestSpecVer " + State.TestSpec.TestSpecVer,
                System.DateTime.Now.ToString("yyyy年MM月dd日(ddd) HH：mm：ss"),
                State.VmMainWindow.Operator,

                State.TestSpec.FwVer,
                State.TestSpec.FwSum,

                State.VmTestResults.Curr24V,
                State.VmTestResults.Curr3V,
                State.VmTestResults.VolVcc,
                State.VmTestResults.VolVref,
                State.VmTestResults.VolMinus5V,

                State.VmTestResults.CurrK1,
                State.VmTestResults.CurrK2,
                State.VmTestResults.CurrK3,
                State.VmTestResults.CurrK4,
                State.VmTestResults.CurrK5,

                State.VmTestResults.An0_00V,
                State.VmTestResults.An0_25V,
                State.VmTestResults.An0_50V,
                State.VmTestResults.An1_00V,
                State.VmTestResults.An1_25V,
                State.VmTestResults.An1_50V,
                State.VmTestResults.An2_00V,
                State.VmTestResults.An2_25V,
                State.VmTestResults.An2_50V,
                State.VmTestResults.An3_00V,
                State.VmTestResults.An3_50V,
                State.VmTestResults.An3_100V,

                State.VmTestResults.Loop1Ad,
                State.VmTestResults.Loop2Ad,
                State.VmTestResults.TempUSBRH,
                State.VmTestResults.TempTarget,
                State.VmTestResults.Ch1_25,
                State.VmTestResults.Ch1_100,
                State.VmTestResults.Ch1_400,
                State.VmTestResults.Ch2_25,
                State.VmTestResults.Ch2_100,
                State.VmTestResults.Ch2_400,

                State.VmTestResults.TimeHost2,
                State.VmTestResults.TimeTarget2,

                State.VmTestResults.VolBatt,
                State.VmTestResults.VolD4,
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
            byte buff = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC4);
            return (buff & 0x20) != 0x20;
        }

        //**************************************************************************
        //EPX64のリセット
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static void ResetIo() //P7:0 P6:1 P5:1 P4:1  P3:0 P2:1 P1:1 P0:1  
        {
            //IOを初期化する処理（出力をすべてＬに落とす）
            io.OutByte(EPX64S.PORT.P0, 0x00);
            io.OutByte(EPX64S.PORT.P1, 0x00);
            io.OutByte(EPX64S.PORT.P2, 0x00);
            io.OutByte(EPX64S.PORT.P3, 0x00);
            io.OutByte(EPX64S.PORT.P4, 0x00);
            io.OutByte(EPX64S.PORT.P5, 0x00);

            //CMOSバスバッファTC74VHC541FTの出力をZにする処理
            io.OutByte(EPX64S.PORT.P6, 0xFF);

            Flags.PowOn = false;
        }

        public static void PowSupply(bool sw)
        {
            if (Flags.PowOn == sw) return;

            if (sw)
            {
                io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.H);
                io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b6, EPX64S.OUT.H);
            }
            else
            {
                io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.L);
                io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b6, EPX64S.OUT.L);
            }

            Flags.PowOn = sw;
        }

        public static void SetDcFun(bool sw)
        {
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
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
            State.VmMainWindow.SerialNumber = State.VmMainWindow.Opecode + "-" + State.NewSerial.ToString("D4") + State.CheckerNumber;
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

            //通信ログのクリア
            State.VmComm.DATA_RX = "";
            State.VmComm.DATA_TX = "";


            State.VmTestStatus.Message = Constants.MessSet;
            State.VmMainWindow.EnableOtherButton = true;

            //各種フラグの初期化
            Flags.PowOn = false;
            Flags.ClickStopButton = false;
            Flags.Testing = false;


            //テーマ透過度を元に戻す
            State.VmMainWindow.ThemeOpacity = State.CurrentThemeOpacity;

            State.VmTestStatus.ColorRetry = Brushes.Transparent;
            State.VmTestStatus.TestSettingEnable = true;
            State.VmMainWindow.OperatorEnable = true;
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

                    Flags.StateEpx64 = General.io.InitEpx64S(0x7F);//0111 1111  ※P7のみ入力
                    if (Flags.StateEpx64)
                    {
                        //IOボードのリセット（出力をすべてLする ※P6はTC74HC541APの出力をZにするため、すべてHとする）
                        General.ResetIo();

                        //電源リレーK100、K101が溶着していないかチェックしてから非同期処理を終了する
                        Thread.Sleep(1000);
                        CheckPowRelay();
                        break;
                    }

                    Thread.Sleep(250);
                }
                StopEpx64s = true;
            });

            //HIOKISS7012の初期化
            bool StopSS7012 = false;
            Task.Run(() =>
            {
                signalSource = new HIOKI7012();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateSS7012 = signalSource.Init();
                    if (Flags.StateSS7012) break;

                    Thread.Sleep(300);
                }
                StopSS7012 = true;
            });

            //VOAC7602の初期化
            bool StopVOAC7602 = false;
            Task.Run(() =>
            {
                multimeter = new VOAC7602();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateVOAC7602 = multimeter.Init();
                    if (Flags.StateVOAC7602) break;

                    Thread.Sleep(300);
                }
                StopVOAC7602 = true;
            });

            //カメラ（CMS_V37BK）の初期化
            bool StopCAMERA = false;
            Task.Run(() =>
            {
                General.cam = new Camera(0);
                State.SetCamProp();

                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateCamera = General.cam.InitCamera();
                    if (Flags.StateCamera) break;

                    Thread.Sleep(300);
                }
                StopCAMERA = true;
            });

            //USB温度計の初期化
            bool Stop温度計 = false;
            Task.Run(() =>
            {
                General.thermometer = new USBRH();
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.State温度計 = General.thermometer.Init();
                    if (Flags.State温度計) break;

                    Thread.Sleep(300);
                }
                Stop温度計 = true;
            });

            //LTM2882Aの初期化
            bool StopLTM2882A = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateLTM2882A = Target.Init(Target.Port.Rs232C_1);
                    if (Flags.StateLTM2882A) break;

                    Thread.Sleep(300);
                }
                StopLTM2882A = true;
            });

            //LTM2882Bの初期化
            bool StopLTM2882B = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateLTM2882B = Target.Init(Target.Port.Rs232C_2);
                    if (Flags.StateLTM2882B) break;

                    Thread.Sleep(300);
                }
                StopLTM2882B = true;
            });

            //LTM2882Cの初期化
            bool StopLTM2882C = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateLTM2882C = Target.Init(Target.Port.Rs232C_3);
                    if (Flags.StateLTM2882C) break;

                    Thread.Sleep(300);
                }
                StopLTM2882C = true;
            });


            //LTM2882Dの初期化
            bool StopLTM2882D = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.StateLTM2882D = Target.Init(Target.Port.Rs232C_4);
                    if (Flags.StateLTM2882D) break;

                    Thread.Sleep(300);
                }
                StopLTM2882D = true;
            });

            ////1150Aの初期化
            bool Stop1150A = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.State1150A = Target.Init(Target.Port.Rs422_1);
                    if (Flags.State1150A) break;

                    Thread.Sleep(300);
                }
                Stop1150A = true;
            });

            ////1150Bの初期化
            bool Stop1150B = false;
            Task.Run(() =>
            {
                while (true)
                {
                    if (Flags.StopInit周辺機器)
                    {
                        break;
                    }

                    Flags.State1150B = Target.Init(Target.Port.Rs422_2);
                    if (Flags.State1150B) break;

                    Thread.Sleep(300);
                }
                Stop1150B = true;
            });

            Task.Run(() =>
            {
                while (true)
                {
                    Flags.AllOk周辺機器接続 = (Flags.StateEpx64 && Flags.StateSS7012 && Flags.StateVOAC7602 && Flags.StateCamera && Flags.State温度計 &&
                                              Flags.StateLTM2882A && Flags.StateLTM2882B && Flags.StateLTM2882C && Flags.StateLTM2882D &&
                                              Flags.State1150A && Flags.State1150B &&
                                              Flags.StateK100 && Flags.StateK101);

                    //EPX64Sの初期化の中で、K100、K101の溶着チェックを行っているが、これがNGだとしてもInit周辺機器()は終了する
                    var IsAllStopped = StopEpx64s && StopSS7012 && StopVOAC7602 && StopCAMERA && Stop温度計 &&
                                        StopLTM2882A && StopLTM2882B && StopLTM2882C && StopLTM2882D &&
                                        Stop1150A && Stop1150B;

                    if (Flags.AllOk周辺機器接続 || IsAllStopped) break;
                    Thread.Sleep(400);

                }
                Flags.Initializing周辺機器 = false;
            });
        }

        public static void ResetInputPort()
        {
            //CMOSバスバッファTC74HC541APの出力をZにする処理
            io.OutByte(EPX64S.PORT.P6, 0xFF);
        }


        public static void ResetRelay_7062_7012()
        {
            General.SetLY1(false);
            General.SetLY2(false);
            General.SetLY3(false);
            General.SetLY4(false);
            General.SetLY5(false);
            General.SetLY6(false);
            General.SetLY7(false);
            General.SetLY8(false);
            General.SetLY9(false);

            General.SetRL1(false);
            General.SetRL2(false);
            General.SetRL3(false);
            General.SetRL4(false);
            General.SetRL5(false);
            General.SetRL6(false);
            General.SetRL7(false);


        }

        //試験機リレー制御
        public static void SetK100(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //K100
        public static void SetK101(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //K101
        public static void SetLY1(bool sw) { General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY1

        public static void SetLY2(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY2
        public static void SetLY3(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY3
        public static void SetLY4(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY4
        public static void SetLY5(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY5
        public static void SetLY6(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY6
        public static void SetLY7(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY7
        public static void SetLY8(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY8
        public static void SetLY9(bool sw) { General.io.OutBit(EPX64S.PORT.P1, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //LY9

        public static void SetRL1(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL1
        public static void SetRL2(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL2
        public static void SetRL3(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL3
        public static void SetRL4(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL4
        public static void SetRL5(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL5
        public static void SetRL6(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL6
        public static void SetRL7(bool sw) { General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L); } //RL7


    }

}

