using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class 電圧チェック
    {
        public enum CH { VCC, MINUS5V, VREF, BATTERY, D4, BATTERY_ARI_NASHI }

        public static async Task<bool> CheckVolt(CH ch)
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;
            double Min = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        General.ResetRelay_7062_7012();
                        Thread.Sleep(500);

                        switch (ch)
                        {
                            case CH.VCC:
                            case CH.MINUS5V:
                            case CH.VREF:
                                if (!Flags.PowOn)
                                {
                                    General.PowSupply(true);
                                    Thread.Sleep(2000);

                                }
                                break;

                            default:
                                break;
                        }


                        switch (ch)
                        {
                            case CH.VCC:
                                Max = State.TestSpec.vccMax;
                                Min = State.TestSpec.vccMin;
                                General.SetRL1(true);//Vccと7062のV端子を接続する処理
                                General.SetRL5(true);//製品GNDと7062のCOM端子を接続する処理
                                break;
                            case CH.MINUS5V:
                                Max = State.TestSpec.minus5vMax;
                                Min = State.TestSpec.minus5vMin;
                                General.SetRL2(true);//-5Vと7062のV端子を接続する処理
                                General.SetRL5(true);//製品GNDと7062のCOM端子を接続する処理
                                break;
                            case CH.VREF:
                                Max = State.TestSpec.vRefMax;
                                Min = State.TestSpec.vRefMin;
                                General.SetRL3(true);//Vrefと7062のV端子を接続する処理
                                General.SetRL5(true);//製品GNDと7062のCOM端子を接続する処理

                                //これ重要！！！ TB201の1-2、3-4を短絡、又は0.0V入力しないと4.7Vくらいになってしまう
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.H);//K6,7,8,9をON K6はTB1の1-2、3-4短絡に使用します（2017.6.19改造）
                                Thread.Sleep(300);
                                break;
                            case CH.D4:
                                Max = State.TestSpec.d4Max;
                                Min = State.TestSpec.d4Min;

                                General.PowSupply(true);
                                Thread.Sleep(1500);
                                General.PowSupply(false);
                                //この時点でC9の両端電圧は4.6Vくらいになっている

                                //C9に充放電して3.0Vにする
                                General.signalSource.OutDcV(3.0);
                                Thread.Sleep(1000);
                                General.SetLY3(true);
                                Thread.Sleep(2500);//2～3秒チャージ
                                General.SetLY3(false);
                                Thread.Sleep(1000);
                                General.signalSource.StopSource();
                                Thread.Sleep(500);

                                General.SetRL4(true);//C9のプラス側と7062のV端子を接続する処理
                                General.SetRL7(true);//3Vと7062のCOM端子を接続する処理
                                break;
                            case CH.BATTERY:
                                Max = State.TestSpec.batteryMax;
                                Min = State.TestSpec.batteryMin;
                                General.SetRL6(true);//3Vと7062のV端子を接続する処理
                                General.SetRL5(true);//製品GNDと7062のCOM端子を接続する処理
                                break;

                            case CH.BATTERY_ARI_NASHI:
                                Max = 2.5;//バッテリーが付いていなければ1.0以下 VOAC7602で計測すると少し高め（1.0～2.0）に出るので、余裕見て2.5Vくらいにしておく
                                Min = -2.0;//ノイズ等でマイナス側に振れる可能性があるため
                                General.SetRL6(true);//3Vと7062のV端子を接続する処理
                                General.SetRL5(true);//製品GNDと7062のCOM端子を接続する処理
                                Thread.Sleep(1000);
                                break;
                        }


                        Thread.Sleep(1000);

                        if (!General.multimeter.SetVoltDc()) return false;
                        if (!General.multimeter.Measure()) return false;

                        measData = General.multimeter.VoltData;

                        if (ch == CH.D4)
                        {
                            measData = -(measData);//+-逆なので反転させる
                        }


                        return result = (measData > Min && measData < Max);
                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                //リレーを初期化する処理
                General.ResetRelay_7062_7012();
                General.signalSource.StopSource();
                //ビューモデルの更新
                switch (ch)
                {
                    case CH.VCC:
                        State.VmTestResults.VolVcc = measData.ToString("F3") + "V";
                        State.VmTestResults.ColorVolVcc = result ? Brushes.Transparent : Brushes.OrangeRed;
                        break;
                    case CH.MINUS5V:
                        State.VmTestResults.VolMinus5V = measData.ToString("F3") + "V";
                        State.VmTestResults.ColorVolMinus5V = result ? Brushes.Transparent : Brushes.OrangeRed;
                        break;
                    case CH.VREF:
                        State.VmTestResults.VolVref = measData.ToString("F4") + "V";
                        State.VmTestResults.ColorVolVref = result ? Brushes.Transparent : Brushes.OrangeRed;
                        break;
                    case CH.BATTERY:
                        State.VmTestResults.VolBatt = measData.ToString("F3") + "V";
                        State.VmTestResults.ColorVolBatt = result ? Brushes.Transparent : Brushes.OrangeRed;
                        break;
                    case CH.D4:
                        State.VmTestResults.VolD4 = measData.ToString("F3") + "V";
                        State.VmTestResults.ColorVolD4 = result ? Brushes.Transparent : Brushes.OrangeRed;
                        break;
                }

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    switch (ch)
                    {
                        case CH.VCC:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.vccMin.ToString("F2") + "～" + State.TestSpec.vccMax.ToString("F2") + "V";
                            break;
                        case CH.MINUS5V:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.minus5vMin.ToString("F2") + "～" + State.TestSpec.minus5vMax.ToString("F2") + "V";
                            break;
                        case CH.VREF:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.vRefMin.ToString("F3") + "～" + State.TestSpec.vRefMax.ToString("F3") + "V";
                            break;
                        case CH.BATTERY:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.batteryMin.ToString("F2") + "～" + State.TestSpec.batteryMax.ToString("F2") + "V";
                            break;
                        case CH.D4:
                            State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.d4Min.ToString("F2") + "～" + State.TestSpec.d4Max.ToString("F2") + "V";
                            break;
                        case CH.BATTERY_ARI_NASHI:
                            State.VmTestStatus.Spec = "規格値 : リチウム電池がついていないこと（1.0V以下）";
                            break;

                    }

                    string data = (ch == CH.VREF) ? measData.ToString("F3") : measData.ToString("F2");

                    State.VmTestStatus.MeasValue = "計測値 : " + data + "V";

                }


            }
        }


    }
}
