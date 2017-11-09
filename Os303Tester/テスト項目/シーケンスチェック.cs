using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Os303Tester
{
    public static class シーケンスチェック
    {
        public enum NAME { PC1, PC2, PC3, PC4, LED1, LED2, LED3, }

        private static SolidColorBrush OnBrush = new SolidColorBrush();

        public static string ErrTitle { get; set; }



        static シーケンスチェック()
        {
            OnBrush.Color = Colors.DodgerBlue;
            OnBrush.Opacity = 0.6;
        }

        public class DoutTestSpec
        {
            public NAME name;
            public bool Exp;
            public bool Output;
        }

        public static List<DoutTestSpec> ListTestSpec;

        private static void InitList()
        {
            ListTestSpec = new List<DoutTestSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListTestSpec.Add(new DoutTestSpec { name = (NAME)n });
            }
        }


        private static void SetExp(bool ExpPc1, bool ExpPc2, bool ExpPc3, bool ExpPc4, bool ExpLed1, bool ExpLed2, bool ExpLed3)
        {
            //ビューモデル（期待値）の更新
            ResetViewModel();
            ListTestSpec.FirstOrDefault(L => L.name == NAME.LED1).Exp = ExpLed1;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.LED2).Exp = ExpLed2;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.LED3).Exp = ExpLed3;

            ListTestSpec.FirstOrDefault(L => L.name == NAME.PC1).Exp = ExpPc1;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.PC2).Exp = ExpPc2;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.PC3).Exp = ExpPc3;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.PC4).Exp = ExpPc4;

            State.VmTestResults.ColorLed1Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.LED1).Exp ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorLed2Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.LED2).Exp ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorLed3Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.LED3).Exp ? OnBrush : Brushes.Transparent;

            State.VmTestResults.ColorPc1Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC1).Exp ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorPc2Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC2).Exp ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorPc3Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC3).Exp ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorPc4Exp = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC4).Exp ? OnBrush : Brushes.Transparent;
            Thread.Sleep(150);
        }

        private static void SetInput(bool RL1, bool RL2, bool RL3, bool RL4, bool RL5, bool RL6, bool S1)
        {
            //ビューモデル（期待値）の更新
            State.VmTestResults.ColorRL1 = RL1 ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorRL2 = RL2 ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorRL3 = RL3 ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorRL4 = RL4 ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorRL5 = RL5 ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorRL6 = RL6 ? OnBrush : Brushes.Transparent;
            State.VmTestResults.ColorS1 = S1 ? OnBrush : Brushes.Transparent;
            Thread.Sleep(150);
        }

        private static bool AnalysisInputData()
        {
            try
            {
                //
                Test_Led.CheckLed();
                bool outLed1 = Double.Parse(State.VmTestResults.LED1Value) >= State.TestSpec.LedOnCount;
                bool outLed2 = Double.Parse(State.VmTestResults.LED2Value) >= State.TestSpec.LedOnCount;
                bool outLed3 = Double.Parse(State.VmTestResults.LED3Value) >= State.TestSpec.LedOnCount;

                ListTestSpec.FirstOrDefault(L => L.name == NAME.LED1).Output = outLed1;
                ListTestSpec.FirstOrDefault(L => L.name == NAME.LED2).Output = outLed2;
                ListTestSpec.FirstOrDefault(L => L.name == NAME.LED3).Output = outLed3;

                General.io.ReadInputData(EPX64S.PORT.P3);
                var p3Data = General.io.P3InputData;

                bool outPc1 = (p3Data & 0x02) == 0x00;
                bool outPc2 = (p3Data & 0x04) == 0x00;
                bool outPc3 = (p3Data & 0x20) == 0x00;
                bool outPc4 = (p3Data & 0x40) == 0x00;

                ListTestSpec.FirstOrDefault(L => L.name == NAME.PC1).Output = outPc1;
                ListTestSpec.FirstOrDefault(L => L.name == NAME.PC2).Output = outPc2;
                ListTestSpec.FirstOrDefault(L => L.name == NAME.PC3).Output = outPc3;
                ListTestSpec.FirstOrDefault(L => L.name == NAME.PC4).Output = outPc4;

                //ビューモデルの更新
                State.VmTestResults.ColorLed1Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.LED1).Output ? OnBrush : Brushes.Transparent;
                State.VmTestResults.ColorLed2Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.LED2).Output ? OnBrush : Brushes.Transparent;
                State.VmTestResults.ColorLed3Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.LED3).Output ? OnBrush : Brushes.Transparent;

                State.VmTestResults.ColorPc1Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC1).Output ? OnBrush : Brushes.Transparent;
                State.VmTestResults.ColorPc2Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC2).Output ? OnBrush : Brushes.Transparent;
                State.VmTestResults.ColorPc3Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC3).Output ? OnBrush : Brushes.Transparent;
                State.VmTestResults.ColorPc4Out = ListTestSpec.FirstOrDefault(L => L.name == NAME.PC4).Output ? OnBrush : Brushes.Transparent;

                Thread.Sleep(300);
                return ListTestSpec.All(L => L.Output == L.Exp);
            }
            catch
            {
                return false;
            }
        }

        private static void ResetViewModel()
        {
            State.VmTestResults.ColorPc1Out = Brushes.Transparent;
            State.VmTestResults.ColorPc2Out = Brushes.Transparent;
            State.VmTestResults.ColorPc3Out = Brushes.Transparent;
            State.VmTestResults.ColorPc4Out = Brushes.Transparent;
            State.VmTestResults.ColorLed1Out = Brushes.Transparent;
            State.VmTestResults.ColorLed2Out = Brushes.Transparent;
            State.VmTestResults.ColorLed3Out = Brushes.Transparent;

            State.VmTestResults.ColorPc1Exp = Brushes.Transparent;
            State.VmTestResults.ColorPc2Exp = Brushes.Transparent;
            State.VmTestResults.ColorPc3Exp = Brushes.Transparent;
            State.VmTestResults.ColorPc4Exp = Brushes.Transparent;
            State.VmTestResults.ColorLed1Exp = Brushes.Transparent;
            State.VmTestResults.ColorLed2Exp = Brushes.Transparent;
            State.VmTestResults.ColorLed3Exp = Brushes.Transparent;
        }


        public static async Task<bool> CheckDout()
        {
            const bool on = true;
            const bool off = false;

            bool FlagTimeout = false;
            var tmTimeout = new System.Timers.Timer();
            tmTimeout.Elapsed += (sender, e) =>
            {
                FlagTimeout = true;
            };

            return await Task<bool>.Run(() =>
            {
                try
                {
                    General.ResetIo();
                    Thread.Sleep(1500);


                    Flags.AddDecision = false;
                    ResetViewModel();
                    InitList();//テストスペック初期化

                    //テストログの更新
                    State.VmTestStatus.TestLog += "\r\nテストモード + 電源投入";

                    General.SetRL1(false);
                    General.SetRL2(false);
                    General.SetRL3(false);
                    General.SetRL4(false);
                    General.SetRL5(true);
                    General.SetRL6(true);
                    Thread.Sleep(500);

                    General.PowSupply(true);//RL1
                    SetInput(on, off, off, off, on, on, off);
                    SetExp(off, off, off, off, off, on, off);//マイコン特採対応 2017.11.9 ※限定です
                    //SetExp(off, off, off, off, on, off, off);
                    Thread.Sleep(2000);

                    State.VmTestStatus.TestLog += "\r\nソフトVerチェック";
                    ErrTitle = "ソフトVerチェック";


                    if (!AnalysisInputData()) return false;
                    State.VmTestStatus.TestLog += "---PASS";

                    //マイコン特採対応 2017.11.9 ※限定です
                    State.VmTestStatus.TestLog += "\r\nLED2(緑)カラーチェック";
                    ErrTitle = "LED2(緑)カラーチェック";
                    if (!Test_Led.CheckColor(Test_Led.NAME.LED2)) return false;
                    State.VmTestStatus.TestLog += "---PASS";

                    //State.VmTestStatus.TestLog += "\r\nLED1(黄)カラーチェック";
                    //ErrTitle = "LED1(黄)カラーチェック";
                    //if (!Test_Led.CheckColor(Test_Led.NAME.LED1)) return false;
                    //State.VmTestStatus.TestLog += "---PASS";

                    //ここで製品のRY1、RY2がOFFしているはずなので、VCC2（RelayOff状態）を計測する
                    State.VmTestStatus.TestLog += "\r\n電源電圧チェック Vcc2(RY1,RY2 ON)";
                    ErrTitle = "電源電圧チェック Vcc2(RY1,RY2 ON)";
                    if (!電圧チェック.CheckVolt(電圧チェック.CH.VCC2_RELAY_OFF)) return false;
                    State.VmTestStatus.TestLog += "---PASS";

                    State.VmTestStatus.TestLog += "\r\nS1 OnOffチェック";
                    ErrTitle = "S1 OnOffチェック";
                    SetInput(on, off, off, off, on, on, on);
                    SetExp(off, off, off, off, off, off, off);
                    General.S1On();
                    Thread.Sleep(2000);
                    if (!AnalysisInputData()) return false;
                    State.VmTestStatus.TestLog += "---PASS";

                    State.VmTestStatus.TestLog += "\r\nフロートスイッチ下限チェック";
                    ErrTitle = "フロートスイッチ下限チェック";
                    SetInput(on, on, off, off, on, on, off);
                    SetExp(off, off, off, off, on, off, off);
                    General.SetRL2(true);
                    Thread.Sleep(2000);
                    if (!AnalysisInputData()) return false;
                    State.VmTestStatus.TestLog += "---PASS";


                    State.VmTestStatus.TestLog += "\r\nフロートスイッチ上限チェック 1";
                    ErrTitle = "フロートスイッチ上限チェック 1";
                    SetInput(on, on, on, off, on, on, off);
                    SetExp(off, off, off, on, off, on, off);
                    FlagTimeout = false;
                    tmTimeout.Interval = 2000;
                    tmTimeout.Start();
                    General.SetRL3(true);
                    while (true)
                    {
                        if (FlagTimeout) return false;
                        if (AnalysisInputData()) break;
                    }
                    State.VmTestStatus.TestLog += "---PASS";

                    State.VmTestStatus.TestLog += "\r\nフロートスイッチ上限チェック 2";
                    ErrTitle = "フロートスイッチ上限チェック 2";
                    SetExp(off, on, off, on, off, on, off);
                    FlagTimeout = false;
                    tmTimeout.Interval = 4000;
                    tmTimeout.Start();
                    while (true)
                    {
                        if (FlagTimeout) return false;
                        if (AnalysisInputData()) break;
                    }
                    State.VmTestStatus.TestLog += "---PASS";

                    State.VmTestStatus.TestLog += "\r\nLED2(緑)カラーチェック";
                    ErrTitle = "LED2(緑)カラーチェック";
                    if (!Test_Led.CheckColor(Test_Led.NAME.LED2)) return false;
                    State.VmTestStatus.TestLog += "---PASS";


                    State.VmTestStatus.TestLog += "\r\nフロートスイッチ漏れチェック①";
                    ErrTitle = "フロートスイッチ漏れチェック①";
                    SetInput(on, on, on, on, on, on, off);
                    SetExp(off, off, off, off, off, off, on);
                    General.SetRL4(true);
                    Thread.Sleep(2000);
                    if (!AnalysisInputData()) return false;
                    State.VmTestStatus.TestLog += "---PASS";

                    State.VmTestStatus.TestLog += "\r\nLED3(赤)カラーチェック";
                    ErrTitle = "LED3(赤)カラーチェック";
                    if (!Test_Led.CheckColor(Test_Led.NAME.LED3)) return false;
                    State.VmTestStatus.TestLog += "---PASS";


                    State.VmTestStatus.TestLog += "\r\nフロートスイッチ漏れチェック②";
                    ErrTitle = "フロートスイッチ漏れチェック②";
                    SetInput(on, on, on, on, off, on, off);
                    SetExp(off, off, on, off, off, off, off);
                    General.SetRL5(false);
                    Thread.Sleep(2000);
                    if (!AnalysisInputData()) return false;
                    State.VmTestStatus.TestLog += "---PASS";
                    Thread.Sleep(2000);

                    return true;
                }
                catch
                {
                    return false;
                }


            });


        }
    }



}
