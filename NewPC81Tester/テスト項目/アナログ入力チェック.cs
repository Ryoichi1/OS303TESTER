using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class アナログ入力チェック
    {
        public enum NAME { AN0, AN1, AN2, AN3 }
        public enum INPUT { _0Per, _50Per, _100Per }

        private static void SetViewModel(NAME name, INPUT input, double val, bool result)
        {
            switch (name)
            {
                case NAME.AN0:
                    switch (input)
                    {
                        case INPUT._0Per:
                            State.VmTestResults.An0_00V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn0_00V = Brushes.HotPink;
                            break;
                        case INPUT._50Per:
                            State.VmTestResults.An0_25V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn0_25V = Brushes.HotPink;
                            break;
                        case INPUT._100Per:
                            State.VmTestResults.An0_50V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn0_50V = Brushes.HotPink;
                            break;
                    }
                    break;

                case NAME.AN1:
                    switch (input)
                    {
                        case INPUT._0Per:
                            State.VmTestResults.An1_00V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn1_00V = Brushes.HotPink;
                            break;
                        case INPUT._50Per:
                            State.VmTestResults.An1_25V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn1_25V = Brushes.HotPink;
                            break;
                        case INPUT._100Per:
                            State.VmTestResults.An1_50V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn1_50V = Brushes.HotPink;
                            break;
                    }
                    break;

                case NAME.AN2:
                    switch (input)
                    {
                        case INPUT._0Per:
                            State.VmTestResults.An2_00V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn2_00V = Brushes.HotPink;
                            break;
                        case INPUT._50Per:
                            State.VmTestResults.An2_25V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn2_25V = Brushes.HotPink;
                            break;
                        case INPUT._100Per:
                            State.VmTestResults.An2_50V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn2_50V = Brushes.HotPink;
                            break;
                    }
                    break;

                case NAME.AN3:
                    switch (input)
                    {
                        case INPUT._0Per:
                            State.VmTestResults.An3_00V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn3_00V = Brushes.HotPink;
                            break;
                        case INPUT._50Per:
                            State.VmTestResults.An3_50V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn3_50V = Brushes.HotPink;
                            break;
                        case INPUT._100Per:
                            State.VmTestResults.An3_100V = val.ToString("F2") + "V";
                            if (!result) State.VmTestResults.ColorAn3_100V = Brushes.HotPink;
                            break;
                    }
                    break;
            }
        }
        private static void ResetViewModel(NAME name)
        {
            switch (name)
            {
                case NAME.AN0:
                    State.VmTestResults.An0_00V = "";
                    State.VmTestResults.ColorAn0_00V = Brushes.Transparent;
                    State.VmTestResults.An0_25V = "";
                    State.VmTestResults.ColorAn0_25V = Brushes.Transparent;
                    State.VmTestResults.An0_50V = "";
                    State.VmTestResults.ColorAn0_50V = Brushes.Transparent;
                    break;

                case NAME.AN1:
                    State.VmTestResults.An1_00V = "";
                    State.VmTestResults.ColorAn1_00V = Brushes.Transparent;
                    State.VmTestResults.An1_25V = "";
                    State.VmTestResults.ColorAn1_25V = Brushes.Transparent;
                    State.VmTestResults.An1_50V = "";
                    State.VmTestResults.ColorAn1_50V = Brushes.Transparent;
                    break;

                case NAME.AN2:
                    State.VmTestResults.An2_00V = "";
                    State.VmTestResults.ColorAn2_00V = Brushes.Transparent;
                    State.VmTestResults.An2_25V = "";
                    State.VmTestResults.ColorAn2_25V = Brushes.Transparent;
                    State.VmTestResults.An2_50V = "";
                    State.VmTestResults.ColorAn2_50V = Brushes.Transparent;
                    break;

                case NAME.AN3:
                    State.VmTestResults.An3_00V = "";
                    State.VmTestResults.ColorAn3_00V = Brushes.Transparent;
                    State.VmTestResults.An3_50V = "";
                    State.VmTestResults.ColorAn3_50V = Brushes.Transparent;
                    State.VmTestResults.An3_100V = "";
                    State.VmTestResults.ColorAn3_100V = Brushes.Transparent;
                    break;
            }
        }


        public static async Task<bool> CheckAnIn(NAME name)
        {
            bool AllResult = false;
            double convertVolData = 0;
            double Max = 0;//ＡＤ値上限規格値
            double Min = 0;//ＡＤ値下限規格値

            double voltUnit = 0;//AD値を電圧変換する際の、１デジットあたりの電圧値
            double inputVol = 0;//SS7012からの入力電圧

            try
            {
                return AllResult = await Task<bool>.Run(() =>
                {
                    ResetViewModel(name);
                    Flags.AddDecision = false;
                    General.signalSource.StopSource();//SS7012からの出力を停止

                    //これ重要
                    //TB201 1-2、3-4を短絡しないと計測値が低めに出ます
                    General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.H);


                    switch (name)
                    {
                        case NAME.AN0:
                            General.SetLY6(true);
                            break;

                        case NAME.AN1:
                            General.SetLY7(true);
                            break;

                        case NAME.AN2:
                            General.SetLY8(true);
                            break;

                        case NAME.AN3:
                            General.SetLY9(true);
                            break;
                    }

                    foreach (INPUT input in Enum.GetValues(typeof(INPUT)))
                    {
                        if (Flags.ClickStopButton)
                        {
                            General.signalSource.StopSource();//SS7012からの出力を停止
                            return false;
                        }

                        General.signalSource.StopSource();
                        Thread.Sleep(300);
                        switch (name)
                        {
                            case NAME.AN0:
                            case NAME.AN1:
                            case NAME.AN2:
                                switch (input)
                                {
                                    case INPUT._0Per:
                                        inputVol = 0.0;
                                        Max = State.TestSpec.an1min_Max;
                                        Min = State.TestSpec.an1min_Min;
                                        break;
                                    case INPUT._50Per:
                                        inputVol = 2.5;
                                        Max = State.TestSpec.an1mid_Max;
                                        Min = State.TestSpec.an1mid_Min;
                                        break;
                                    case INPUT._100Per:
                                        inputVol = 5.0;
                                        Max = State.TestSpec.an1max_Max;
                                        Min = State.TestSpec.an1max_Min;
                                        break;
                                }
                                voltUnit = (double)5.0 / 4095;
                                break;

                            case NAME.AN3:
                                switch (input)
                                {
                                    case INPUT._0Per:
                                        inputVol = 0.0;
                                        Max = State.TestSpec.an2min_Max;
                                        Min = State.TestSpec.an2min_Min;
                                        break;
                                    case INPUT._50Per:
                                        inputVol = 5.0;
                                        Max = State.TestSpec.an2mid_Max;
                                        Min = State.TestSpec.an2mid_Min;
                                        break;
                                    case INPUT._100Per:
                                        inputVol = 10.0;
                                        Max = State.TestSpec.an2max_Max;
                                        Min = State.TestSpec.an2max_Min;
                                        break;
                                }
                                voltUnit = (double)10.0 / 4095;
                                break;
                        }

                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\n" + name.ToString() + " " + input.ToString().Trim('_', 'P', 'e', 'r') + "％入力チェック";
                        General.signalSource.OutDcV(inputVol);
                        Thread.Sleep(1500);
                        if (!Target.SendData(Target.Port.Rs232C_1, "Read" + name.ToString())) return false;
                        Thread.Sleep(100);
                        int hexData = Convert.ToInt32(Target.RecieveData.Substring(3), 16);//製品からの返信 例）AN0xxx
                        convertVolData = hexData * voltUnit;
                        var result = (convertVolData >= Min && convertVolData <= Max);
                        SetViewModel(name, input, convertVolData, result);
                        if (result)
                        {
                            State.VmTestStatus.TestLog += "---PASS";
                        }
                        else
                        {
                            State.VmTestStatus.TestLog += "---FAIL";
                            return false;
                        }


                    }

                    return true;
                });


            }
            finally
            {
                if (!AllResult)
                {
                    State.VmTestStatus.Spec = "規格値 : " + Min.ToString("F2") + " ～ " + Max.ToString("F2") + "V";
                    State.VmTestStatus.MeasValue = "計測値 : " + convertVolData.ToString("F2") + "V";
                }
                State.VmTestStatus.TestLog += "\r\n";

                General.signalSource.StopSource();
                General.ResetRelay_7062_7012();
                //TB201 1-2、3-4を開放に戻す処理
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.L);
                Thread.Sleep(100);
            }
        }



    }
}