using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class デジタル出力チェック
    {
        public enum NAME { OUT00, OUT01, OUT02, OUT03, OUT04, OUT05, OUT06, OUT07, OUT08, OUT09, OUT10, OUT11, OUT20, OUT21 }

        public class DoutTestSpec
        {
            public NAME name;
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

        public static bool SetAllOff()
        {
            try
            {
                var NAME_ARRAY = Enum.GetValues(typeof(NAME));
                foreach (var n in NAME_ARRAY)
                {
                    if (!Target.SendData(Target.Port.Rs232C_1, n.ToString() + "Off")) return false;
                    if (!Target.RecieveData.Contains(n.ToString() + "OffOK")) return false;
                    Thread.Sleep(50);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        private static void AnalysisData(byte dataIC1, byte dataIC2, NAME onName, bool ExpAllOff = false)
        {
            bool out10 = (dataIC1 & 0x01) == 0x01;
            bool out11 = (dataIC1 & 0x02) == 0x02;
            bool out08 = (dataIC1 & 0x04) == 0x04;
            bool out09 = (dataIC1 & 0x08) == 0x08;
            bool out06 = (dataIC1 & 0x10) == 0x10;
            bool out07 = (dataIC1 & 0x20) == 0x20;
            bool out04 = (dataIC1 & 0x40) == 0x40;
            bool out05 = (dataIC1 & 0x80) == 0x80;

            bool out02 = (dataIC2 & 0x01) == 0x01;
            bool out03 = (dataIC2 & 0x02) == 0x02;
            bool out00 = (dataIC2 & 0x04) == 0x04;
            bool out01 = (dataIC2 & 0x08) == 0x08;
            bool out21 = (dataIC2 & 0x10) == 0x10;
            bool out20 = (dataIC2 & 0x20) == 0x20;

            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT00).Output = out00;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT01).Output = out01;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT02).Output = out02;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT03).Output = out03;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT04).Output = out04;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT05).Output = out05;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT06).Output = out06;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT07).Output = out07;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT08).Output = out08;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT09).Output = out09;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT10).Output = out10;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT11).Output = out11;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT20).Output = out20;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.OUT21).Output = out21;


            //ビューモデルの更新
            if (ExpAllOff)
            {
                State.VmTestResults.ColorDo000Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo001Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo002Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo003Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo004Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo005Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo006Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo007Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo008Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo009Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo010Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo011Exp = Brushes.Transparent;
                State.VmTestResults.ColorDo400Exp = Brushes.Transparent;//OUT20
                State.VmTestResults.ColorDo401Exp = Brushes.Transparent;//OUT21

            }
            else
            {
                State.VmTestResults.ColorDo000Exp = onName == NAME.OUT00 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo001Exp = onName == NAME.OUT01 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo002Exp = onName == NAME.OUT02 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo003Exp = onName == NAME.OUT03 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo004Exp = onName == NAME.OUT04 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo005Exp = onName == NAME.OUT05 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo006Exp = onName == NAME.OUT06 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo007Exp = onName == NAME.OUT07 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo008Exp = onName == NAME.OUT08 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo009Exp = onName == NAME.OUT09 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo010Exp = onName == NAME.OUT10 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo011Exp = onName == NAME.OUT11 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo400Exp = onName == NAME.OUT20 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDo401Exp = onName == NAME.OUT21 ? Brushes.DodgerBlue : Brushes.Transparent;
            }


            State.VmTestResults.ColorDo000Out = out00 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo001Out = out01 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo002Out = out02 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo003Out = out03 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo004Out = out04 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo005Out = out05 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo006Out = out06 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo007Out = out07 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo008Out = out08 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo009Out = out09 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo010Out = out10 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo011Out = out11 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo400Out = out20 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDo401Out = out21 ? Brushes.DodgerBlue : Brushes.Transparent;

        }

        private static void ResetViewModel()
        {
            State.VmTestResults.ColorDo000Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo001Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo002Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo003Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo004Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo005Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo006Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo007Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo008Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo009Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo010Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo011Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo400Exp = Brushes.Transparent;
            State.VmTestResults.ColorDo401Exp = Brushes.Transparent;

            State.VmTestResults.ColorDo000Out = Brushes.Transparent;
            State.VmTestResults.ColorDo001Out = Brushes.Transparent;
            State.VmTestResults.ColorDo002Out = Brushes.Transparent;
            State.VmTestResults.ColorDo003Out = Brushes.Transparent;
            State.VmTestResults.ColorDo004Out = Brushes.Transparent;
            State.VmTestResults.ColorDo005Out = Brushes.Transparent;
            State.VmTestResults.ColorDo006Out = Brushes.Transparent;
            State.VmTestResults.ColorDo007Out = Brushes.Transparent;
            State.VmTestResults.ColorDo008Out = Brushes.Transparent;
            State.VmTestResults.ColorDo009Out = Brushes.Transparent;
            State.VmTestResults.ColorDo010Out = Brushes.Transparent;
            State.VmTestResults.ColorDo011Out = Brushes.Transparent;
            State.VmTestResults.ColorDo400Out = Brushes.Transparent;
            State.VmTestResults.ColorDo401Out = Brushes.Transparent;
        }


        public static async Task<bool> CheckDout()
        {
            bool resultOn = false;
            bool resultOff = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    ResetViewModel();

                    Flags.AddDecision = false;
                    InitList();//テストスペック毎回初期化

                    SetAllOff();//出力初期化

                    return ListTestSpec.All(L =>
                    {
                        resultOn = false;
                        resultOff = false;

                        if (Flags.ClickStopButton) return false;

                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " ONチェック";

                        //ONチェック
                        if (!Target.SendData(Target.Port.Rs232C_1, L.name.ToString() + "On")) return false;
                        Thread.Sleep(100);
                        var dataIC1 = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC1);
                        var dataIC2 = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC2);
                        AnalysisData(dataIC1, dataIC2, L.name);

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

                        //OFFチェック
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " OFFチェック";
                        if (!Target.SendData(Target.Port.Rs232C_1, L.name.ToString() + "Off")) return false;
                        Thread.Sleep(100);
                        dataIC1 = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC1);
                        dataIC2 = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC2);
                        AnalysisData(dataIC1, dataIC2, L.name, true);

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
                State.VmTestStatus.TestLog += "\r\n"; 

                if (!resultOn || !resultOff)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
            }



        }
    }
}