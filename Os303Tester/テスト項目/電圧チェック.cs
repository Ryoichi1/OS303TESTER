using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Os303Tester
{
    public static class 電圧チェック
    {
        public enum CH { VDD, VCC1, VCC2_RELAY_OFF, VCC2_RELAY_ON, AC100 }

        public static async Task<bool> CheckVoltAsync(CH ch)
        {
            return await Task<bool>.Run(() => {
                return CheckVolt(ch);
            });
        }

        public static bool CheckVolt(CH ch)
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;
            double Min = 0;

            //AC100V計測用（安全のため、トランスで降圧して計測するので、出力電圧に係数をかけた値で判定する）
            double k = 8.5106;//100/11.75 = 8.5106 → 100:11.75=トランス入力電圧:トランス出力電圧から、トランス入力電圧 = (100/11.75)*トランス出力電圧

            try
            {

                General.ResetRelay_7502();
                Thread.Sleep(200);

                if (ch == CH.AC100)
                {
                    General.PowSupply(false);
                }
                else
                {
                    if (!Flags.PowOn)
                    {
                        General.PowSupply(true);
                        Thread.Sleep(1500);
                    }
                }

                switch (ch)
                {
                    case CH.VDD:
                        Max = State.TestSpec.VddMax;
                        Min = State.TestSpec.VddMin;
                        General.SetK14(true);//VDDと7062のV端子を接続する処理
                        break;

                    case CH.VCC1:
                        Max = State.TestSpec.Vcc1Max;
                        Min = State.TestSpec.Vcc1Min;
                        General.SetK15(true);//Vcc1と7062のV端子を接続する処理
                        break;

                    case CH.VCC2_RELAY_OFF:
                        Max = State.TestSpec.Vcc2RelayOffMax;
                        Min = State.TestSpec.Vcc2RelayOffMin;
                        General.SetK16(true);//Vcc2と7062のV端子を接続する処理
                        break;

                    case CH.VCC2_RELAY_ON:
                        Max = State.TestSpec.Vcc2RelayOnMax;
                        Min = State.TestSpec.Vcc2RelayOnMin;
                        General.SetK16(true);//Vcc2と7062のV端子を接続する処理
                        break;

                    case CH.AC100:
                        Max = State.TestSpec.Ac100Max;
                        Min = State.TestSpec.Ac100Min;
                        General.SetK17(true);//AC100V計測用トランスと7062のV端子を接続する処理
                        break;
                }


                Thread.Sleep(400);

                if (ch == CH.AC100)
                {
                    if (!General.multimeter.SetVoltAc()) return false;
                }
                else
                {
                    if (!General.multimeter.SetVoltDc()) return false;
                }

                Thread.Sleep(600);

                if (!General.multimeter.Measure()) return false;

                if (ch == CH.AC100)
                {
                    measData = General.multimeter.VoltData * k;
                }
                else
                {
                    measData = General.multimeter.VoltData;
                }

                return result = (measData > Min && measData < Max);
            }
            catch
            {
                return result = false;
            }

            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_7502();

                //ビューモデルの更新
                switch (ch)
                {
                    case CH.VDD:
                        State.VmTestResults.VolVdd = measData.ToString("F2") + "V";
                        State.VmTestResults.ColorVolVdd = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case CH.VCC1:
                        State.VmTestResults.VolVcc1 = measData.ToString("F2") + "V";
                        State.VmTestResults.ColorVolVcc1 = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case CH.VCC2_RELAY_OFF:
                        State.VmTestResults.VolVcc2Off = measData.ToString("F2") + "V";
                        State.VmTestResults.ColorVolVcc2Off = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case CH.VCC2_RELAY_ON:
                        State.VmTestResults.VolVcc2On = measData.ToString("F2") + "V";
                        State.VmTestResults.ColorVolVcc2On = result ? Brushes.Transparent : General.NgBrush;
                        break;

                    case CH.AC100:
                        State.VmTestResults.VolAc100 = measData.ToString("F2") + "V";
                        State.VmTestResults.ColorVolAc100 = result ? Brushes.Transparent : General.NgBrush;
                        break;

                }

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    switch (ch)
                    {
                        case CH.VDD:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.VddMin.ToString("F2") + "～" + State.TestSpec.VddMax.ToString("F2") + "V";
                            break;

                        case CH.VCC1:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.Vcc1Min.ToString("F2") + "～" + State.TestSpec.Vcc1Max.ToString("F2") + "V";
                            break;

                        case CH.VCC2_RELAY_OFF:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.Vcc2RelayOffMin.ToString("F2") + "～" + State.TestSpec.Vcc2RelayOffMax.ToString("F2") + "V";
                            break;

                        case CH.VCC2_RELAY_ON:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.Vcc2RelayOnMin.ToString("F2") + "～" + State.TestSpec.Vcc2RelayOnMax.ToString("F2") + "V";
                            break;

                        case CH.AC100:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.Ac100Min.ToString("F2") + "～" + State.TestSpec.Ac100Max.ToString("F2") + "V";
                            break;


                    }

                    string data = measData.ToString("F2");

                    State.VmTestStatus.MeasValue = "計測値 : " + data + "V";

                }


            }
        }


    }
}
