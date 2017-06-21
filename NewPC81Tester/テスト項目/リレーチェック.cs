using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class リレーチェック
    {
        public enum NAME { K1, K2, K3, K4, K5 }

        public class RelayTestSpec
        {
            public NAME name;
            public bool Output;
        }

        public static List<RelayTestSpec> ListTestSpec;

        private static double Alloff電流;

        private static void InitList()
        {
            ListTestSpec = new List<RelayTestSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListTestSpec.Add(new RelayTestSpec { name = (NAME)n });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool SetMeasCoilCurr()
        {
            try
            {
                General.ResetIo();
                Thread.Sleep(200);
                General.ResetRelay_7062_7012();
                if (!General.multimeter.SetCurrDc()) return false;
                Thread.Sleep(500);

                General.PowSupply(true);
                Thread.Sleep(3000);
                //この時点で消費電流は安定しているはず
                //マルチメータの電流ラインを並列に接続する
                General.SetLY1(true);
                Thread.Sleep(1500);
                //K100をOFFする
                General.SetK100(false);
                Thread.Sleep(1000);
                //この時点でマルチメータを通して安定した電流が流れているはず
                //ｺｲﾙ電流計測
                General.multimeter.Measure();
                var 消費電流 = General.multimeter.CurrData;
                Alloff電流 = 消費電流 * 1000;

                return true;
            }
            catch
            {
                General.SetLY1(false);
                Thread.Sleep(200);
                General.ResetIo();
                return false;
            }
        }


        public static bool SetRelayOff()
        {
            try
            {
                if (!Target.SendData(Target.Port.Rs232C_1, "K1Off")) return false;
                if (!Target.SendData(Target.Port.Rs232C_1, "K2Off")) return false;
                if (!Target.SendData(Target.Port.Rs232C_1, "K3Off")) return false;
                if (!Target.SendData(Target.Port.Rs232C_1, "K4Off")) return false;
                if (!Target.SendData(Target.Port.Rs232C_1, "K5Off")) return false;
                Thread.Sleep(300);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private static void AnalysisData(byte data, NAME onName, bool ExpAllOff = false)
        {
            bool k1out = (data & 0x01) == 0x01;
            bool k2out = (data & 0x02) == 0x02;
            bool k3out = (data & 0x04) == 0x04;
            bool k4out = (data & 0x08) == 0x08;
            bool k5out = (data & 0x10) == 0x10;

            ListTestSpec.FirstOrDefault(L => L.name == NAME.K1).Output = k1out;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.K2).Output = k2out;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.K3).Output = k3out;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.K4).Output = k4out;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.K5).Output = k5out;

            //ビューモデルの更新
            if (ExpAllOff)
            {
                State.VmTestResults.ColorK1Exp = Brushes.Transparent;
                State.VmTestResults.ColorK2Exp = Brushes.Transparent;
                State.VmTestResults.ColorK3Exp = Brushes.Transparent;
                State.VmTestResults.ColorK4Exp = Brushes.Transparent;
                State.VmTestResults.ColorK5Exp = Brushes.Transparent;
            }
            else
            {
                State.VmTestResults.ColorK1Exp = onName == NAME.K1 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorK2Exp = onName == NAME.K2 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorK3Exp = onName == NAME.K3 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorK4Exp = onName == NAME.K4 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorK5Exp = onName == NAME.K5 ? Brushes.DodgerBlue : Brushes.Transparent;
            }


            State.VmTestResults.ColorK1Out = k1out ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorK2Out = k2out ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorK3Out = k3out ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorK4Out = k4out ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorK5Out = k5out ? Brushes.DodgerBlue : Brushes.Transparent;
        }

        private static void ResetViewModel()
        {
            State.VmTestResults.ColorK1Exp = Brushes.Transparent;
            State.VmTestResults.ColorK2Exp = Brushes.Transparent;
            State.VmTestResults.ColorK3Exp = Brushes.Transparent;
            State.VmTestResults.ColorK4Exp = Brushes.Transparent;
            State.VmTestResults.ColorK5Exp = Brushes.Transparent;

            State.VmTestResults.ColorK1Out = Brushes.Transparent;
            State.VmTestResults.ColorK2Out = Brushes.Transparent;
            State.VmTestResults.ColorK3Out = Brushes.Transparent;
            State.VmTestResults.ColorK4Out = Brushes.Transparent;
            State.VmTestResults.ColorK5Out = Brushes.Transparent;

            State.VmTestResults.CurrK1 = "";
            State.VmTestResults.ColorCurrK1 = Brushes.Transparent;
            State.VmTestResults.CurrK2 = "";
            State.VmTestResults.ColorCurrK2 = Brushes.Transparent;
            State.VmTestResults.CurrK3 = "";
            State.VmTestResults.ColorCurrK3 = Brushes.Transparent;
            State.VmTestResults.CurrK4 = "";
            State.VmTestResults.ColorCurrK4 = Brushes.Transparent;
            State.VmTestResults.CurrK5 = "";
            State.VmTestResults.ColorCurrK5 = Brushes.Transparent;

        }


        private static void SetViewModelCoil(NAME name, double CoilCurr, bool result)
        {
            switch (name)
            {
                case NAME.K1:
                    State.VmTestResults.CurrK1 = CoilCurr.ToString("F2") + "mA";
                    State.VmTestResults.ColorCurrK1 = result ? Brushes.Transparent : Brushes.HotPink;
                    break;

                case NAME.K2:
                    State.VmTestResults.CurrK2 = CoilCurr.ToString("F2") + "mA";
                    State.VmTestResults.ColorCurrK2 = result ? Brushes.Transparent : Brushes.HotPink;
                    break;

                case NAME.K3:
                    State.VmTestResults.CurrK3 = CoilCurr.ToString("F2") + "mA";
                    State.VmTestResults.ColorCurrK3 = result ? Brushes.Transparent : Brushes.HotPink;
                    break;

                case NAME.K4:
                    State.VmTestResults.CurrK4 = CoilCurr.ToString("F2") + "mA";
                    State.VmTestResults.ColorCurrK4 = result ? Brushes.Transparent : Brushes.HotPink;
                    break;

                case NAME.K5:
                    State.VmTestResults.CurrK5 = CoilCurr.ToString("F2") + "mA";
                    State.VmTestResults.ColorCurrK5 = result ? Brushes.Transparent : Brushes.HotPink;
                    break;
            }
        }


        public static async Task<bool> CheckRelay()
        {
            bool resultOn = false;
            bool resultOff = false;
            bool resultCoilCurr = false;

            double coilCurr = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    ResetViewModel();

                    Flags.AddDecision = false;
                    InitList();//テストスペック毎回初期化
                    SetMeasCoilCurr();//電源ONして、24Vラインをマルチメータに接続して消費電流を計測する準備をする



                    return ListTestSpec.All(L =>
                    {
                        resultOn = false;
                        resultOff = false;
                        resultCoilCurr = false;

                        if (Flags.ClickStopButton) return false;

                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " ONチェック";

                        SetRelayOff();
                        //ONチェック
                        if (!Target.SendData(Target.Port.Rs232C_1, L.name.ToString() + "On")) return false;
                        Thread.Sleep(400);
                        var p7data = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC5);
                        AnalysisData(p7data, L.name);

                        resultOn = ListTestSpec.All(list =>
                        {
                            if (list.name == L.name)
                            {
                                return list.Output;
                            }
                            else
                            {
                                return !list.Output;
                            }
                        });

                        if (resultOn)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                        //コイル電流のチェック
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " コイル電流チェック";

                        //ｺｲﾙ電流計測
                        General.multimeter.Measure();
                        var 消費電流 = General.multimeter.CurrData;
                        coilCurr = (消費電流 * 1000) - Alloff電流;
                        resultCoilCurr = (coilCurr >= State.TestSpec.coilCurrentMin && coilCurr <= State.TestSpec.coilCurrentMax);
                        SetViewModelCoil(L.name, coilCurr, resultCoilCurr);
                        if (resultCoilCurr)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                        //リレーOFFチェック
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " OFFチェック";

                        SetRelayOff();
                        Thread.Sleep(400);
                        p7data = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC5);
                        AnalysisData(p7data, L.name, true);

                        resultOff = ListTestSpec.All(list =>
                        {
                            return !list.Output;
                        });

                        if (resultOff)
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---PASS";
                            return true;
                        }
                        else
                        {
                            //テストログの更新
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }

                    });

                });
            }
            finally
            {
                General.SetLY1(false);
                Thread.Sleep(200);
                General.ResetIo();

                State.VmTestStatus.TestLog += "\r\n";

                if (!resultOn)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
                else if (!resultCoilCurr)
                {
                    State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.coilCurrentMin.ToString("F2") + "～" + State.TestSpec.coilCurrentMax.ToString("F2") + "mA";
                    State.VmTestStatus.MeasValue = "計測値 : " + coilCurr.ToString("F2") + "mA";
                }
                else if (!resultOff)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }


            }



        }
    }
}