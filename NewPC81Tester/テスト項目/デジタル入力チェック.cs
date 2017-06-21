using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class デジタル入力チェック
    {
        public enum NAME
        {
            IN000, IN001, IN002, IN003, IN004, IN005, IN006, IN007,
            IN008, IN009, IN010, IN011, IN012, IN013, IN014, IN015, IN400
        }

        public class DoutTestSpec
        {
            public NAME name;
            public bool Input;
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
                General.io.OutByte(EPX64S.PORT.P4, 0x00);
                General.io.OutByte(EPX64S.PORT.P5, 0x00);
                General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b2, EPX64S.OUT.L);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private static bool AnalysisData(NAME onName, bool ExpAllOff = false)
        {
            if (!Target.SendData(Target.Port.Rs232C_1, "ReadPD")) return false;
            int PdData = Convert.ToInt32(Target.RecieveData.Substring(2), 16);//製品からの返信 例）PDxx

            if (!Target.SendData(Target.Port.Rs232C_1, "ReadPE")) return false;
            int PeData = Convert.ToInt32(Target.RecieveData.Substring(2), 16);

            if (!Target.SendData(Target.Port.Rs232C_1, "ReadPH")) return false;
            int PhData = Convert.ToInt32(Target.RecieveData.Substring(2), 16);


            bool in000 = (PdData & 0x01) == 0x01;
            bool in001 = (PdData & 0x02) == 0x02;
            bool in002 = (PdData & 0x04) == 0x04;
            bool in003 = (PdData & 0x08) == 0x08;
            bool in004 = (PdData & 0x10) == 0x10;
            bool in005 = (PdData & 0x20) == 0x20;
            bool in006 = (PdData & 0x40) == 0x40;
            bool in007 = (PdData & 0x80) == 0x80;

            bool in008 = (PeData & 0x01) == 0x01;
            bool in009 = (PeData & 0x02) == 0x02;
            bool in010 = (PeData & 0x04) == 0x04;
            bool in011 = (PeData & 0x08) == 0x08;
            bool in012 = (PeData & 0x10) == 0x10;
            bool in013 = (PeData & 0x20) == 0x20;
            bool in014 = (PeData & 0x40) == 0x40;
            bool in015 = (PeData & 0x80) == 0x80;

            bool in400 = (PhData & 0x01) == 0x01;


            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN000).Input = in000;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN001).Input = in001;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN002).Input = in002;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN003).Input = in003;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN004).Input = in004;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN005).Input = in005;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN006).Input = in006;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN007).Input = in007;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN008).Input = in008;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN009).Input = in009;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN010).Input = in010;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN011).Input = in011;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN012).Input = in012;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN013).Input = in013;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN014).Input = in014;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN015).Input = in015;
            ListTestSpec.FirstOrDefault(L => L.name == NAME.IN400).Input = in400;



            //ビューモデルの更新
            if (ExpAllOff)
            {
                State.VmTestResults.ColorDi000Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi001Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi002Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi003Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi004Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi005Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi006Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi007Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi008Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi009Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi010Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi011Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi012Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi013Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi014Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi015Exp = Brushes.Transparent;
                State.VmTestResults.ColorDi400Exp = Brushes.Transparent;
            }
            else
            {
                State.VmTestResults.ColorDi000Exp = onName == NAME.IN000 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi001Exp = onName == NAME.IN001 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi002Exp = onName == NAME.IN002 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi003Exp = onName == NAME.IN003 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi004Exp = onName == NAME.IN004 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi005Exp = onName == NAME.IN005 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi006Exp = onName == NAME.IN006 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi007Exp = onName == NAME.IN007 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi008Exp = onName == NAME.IN008 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi009Exp = onName == NAME.IN009 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi010Exp = onName == NAME.IN010 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi011Exp = onName == NAME.IN011 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi012Exp = onName == NAME.IN012 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi013Exp = onName == NAME.IN013 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi014Exp = onName == NAME.IN014 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi015Exp = onName == NAME.IN015 ? Brushes.DodgerBlue : Brushes.Transparent;
                State.VmTestResults.ColorDi400Exp = onName == NAME.IN400 ? Brushes.DodgerBlue : Brushes.Transparent;
            }


            State.VmTestResults.ColorDi000In = in000 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi001In = in001 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi002In = in002 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi003In = in003 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi004In = in004 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi005In = in005 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi006In = in006 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi007In = in007 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi008In = in008 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi009In = in009 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi010In = in010 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi011In = in011 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi012In = in012 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi013In = in013 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi014In = in014 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi015In = in015 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorDi400In = in400 ? Brushes.DodgerBlue : Brushes.Transparent;

            return true;
        }

        private static void ResetViewModel()
        {
            State.VmTestResults.ColorDi000Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi001Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi002Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi003Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi004Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi005Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi006Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi007Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi008Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi009Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi010Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi011Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi012Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi013Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi014Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi015Exp = Brushes.Transparent;
            State.VmTestResults.ColorDi400Exp = Brushes.Transparent;

            State.VmTestResults.ColorDi000In = Brushes.Transparent;
            State.VmTestResults.ColorDi001In = Brushes.Transparent;
            State.VmTestResults.ColorDi002In = Brushes.Transparent;
            State.VmTestResults.ColorDi003In = Brushes.Transparent;
            State.VmTestResults.ColorDi004In = Brushes.Transparent;
            State.VmTestResults.ColorDi005In = Brushes.Transparent;
            State.VmTestResults.ColorDi006In = Brushes.Transparent;
            State.VmTestResults.ColorDi007In = Brushes.Transparent;
            State.VmTestResults.ColorDi008In = Brushes.Transparent;
            State.VmTestResults.ColorDi009In = Brushes.Transparent;
            State.VmTestResults.ColorDi010In = Brushes.Transparent;
            State.VmTestResults.ColorDi011In = Brushes.Transparent;
            State.VmTestResults.ColorDi012In = Brushes.Transparent;
            State.VmTestResults.ColorDi013In = Brushes.Transparent;
            State.VmTestResults.ColorDi014In = Brushes.Transparent;
            State.VmTestResults.ColorDi015In = Brushes.Transparent;
            State.VmTestResults.ColorDi400In = Brushes.Transparent;
        }


        private static bool SetInput(NAME name, bool sw)
        {
            try
            {
                switch (name)
                {
                    case NAME.IN000:
                        General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN014:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN015:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN012:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN013:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN010:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN011:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN008:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN009:
                        General.io.OutBit(EPX64S.PORT.P4, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN006:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b0, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN007:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b1, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN400:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b2, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN005:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b3, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN004:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b4, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN003:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b5, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN002:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b6, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;

                    case NAME.IN001:
                        General.io.OutBit(EPX64S.PORT.P5, EPX64S.BIT.b7, sw ? EPX64S.OUT.H : EPX64S.OUT.L);
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<bool> CheckDinput()
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

                    SetAllOff();//入力初期化

                    return ListTestSpec.All(L =>
                    {
                        resultOn = false;
                        resultOff = false;

                        if (Flags.ClickStopButton) return false;

                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\n" + L.name.ToString() + " ONチェック";

                        //ONチェック
                        if (!SetInput(L.name, true)) return false;
                        Thread.Sleep(100);
                        AnalysisData(L.name);

                        resultOn = ListTestSpec.All(list =>
                        {
                            if (list.name == L.name)
                            {
                                return list.Input;
                            }
                            else
                            {
                                return !list.Input;
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
                        if (!SetInput(L.name, false)) return false;
                        Thread.Sleep(100);
                        AnalysisData(L.name, true);

                        resultOff = ListTestSpec.All(list =>
                        {
                            return !list.Input;
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

                Thread.Sleep(200);
                SetAllOff();

                if (!resultOn || !resultOff)
                {
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
            }



        }
    }
}