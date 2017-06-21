using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class 消費電流チェック
    {

        public static async Task<bool> CheckCurrent24V()
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
                        if (!General.multimeter.SetCurrDc()) return false;
                        Thread.Sleep(500);


                        Max = State.TestSpec.amp24vMax / 1000.0;
                        Min = State.TestSpec.amp24vMin / 1000.0;

                        General.PowSupply(true);
                        Thread.Sleep(3000);
                        //この時点で消費電流は安定しているはず
                        //マルチメータの電流ラインを並列に接続する
                        General.SetLY1(true);
                        Thread.Sleep(1500);
                        //K100をOFFする
                        General.SetK100(false);
                        Thread.Sleep(3000);
                        //この時点でマルチメータを通して安定した電流が流れているはず

                        if (!General.multimeter.Measure()) return false;
                        measData = General.multimeter.CurrData;

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
                General.PowSupply(false);
                Thread.Sleep(300);
                //リレーを初期化する処理
                General.ResetRelay_7062_7012();

                //ビューモデルの更新
                State.VmTestResults.Curr24V = (measData * 1000).ToString("F2") + "mA";
                State.VmTestResults.ColorCurr24V = result ? Brushes.Transparent : Brushes.OrangeRed;

                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.amp24vMin.ToString("F2") + "～" + State.TestSpec.amp24vMax.ToString("F2") + "mA";
                    State.VmTestStatus.MeasValue = "計測値 : " + (measData * 1000).ToString("F2") + "mA";
                }
            }
        }

        public static async Task<bool> CheckCurrent3V()
        {
            bool result = false;
            Double measData = 0;
            double Max = 0;
            double Min = 0;

            double FirstInput = 2.75;

            //タイマの設定
            bool FlagTimeout = false;
            var TmTimeOut = new System.Timers.Timer();
            TmTimeOut.Interval = 60000;
            TmTimeOut.Elapsed += (sender, e) =>
            {
                TmTimeOut.Stop();
                FlagTimeout = true;
            };

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        General.ResetRelay_7062_7012();
                        if (!General.multimeter.SetCurrDc()) return false;
                        Thread.Sleep(500);

                        Max = State.TestSpec.amp3vMax / 1000000.0;
                        Min = State.TestSpec.amp3vMin / 1000000.0;

                        while (true)
                        {
                            if (Flags.ClickStopButton) return false;
                            //まずはじめに、C9に充放電して2.9Vくらいにする
                            General.signalSource.OutDcV(FirstInput);
                            Thread.Sleep(1000);
                            General.SetLY3(true);//１秒チャージ
                            Thread.Sleep(1000);
                            General.SetLY3(false);//１秒チャージ

                            General.signalSource.StopSource();
                            Thread.Sleep(500);
                            General.SetLY2(true);
                            General.SetRL7(true);
                            Thread.Sleep(1000);
                            General.signalSource.OutDcV(3.0);

                            Thread.Sleep(1000);
                            General.multimeter.Measure();
                            var data1 = General.multimeter.CurrData;
                            State.VmTestResults.Curr3V = (data1 * 1000000).ToString("F2") + "uA";
                            Thread.Sleep(1500);
                            General.multimeter.Measure();
                            var data2 = General.multimeter.CurrData;
                            State.VmTestResults.Curr3V = (data2 * 1000000).ToString("F2") + "uA";

                            if (data1 > data2) break;

                            General.signalSource.StopSource();
                            Thread.Sleep(300);
                            General.ResetRelay_7062_7012();
                            Thread.Sleep(300);

                            FirstInput -= 0.05;
                            if (FirstInput < 2.5) return false;

                        }


                        TmTimeOut.Stop();
                        FlagTimeout = false;
                        TmTimeOut.Start();

                        while (true)
                        {
                            if (FlagTimeout || Flags.ClickStopButton) return false;
                            //消費電流が7uA以下になるまで計測し続ける
                            if (!General.multimeter.Measure()) continue;
                            measData = General.multimeter.CurrData;
                            State.VmTestResults.Curr3V = (measData * 1000000).ToString("F2") + "uA";
                            if (measData < 0.000007) break;
                            Thread.Sleep(1000);
                        }

                        Thread.Sleep(5000);
                        if (!General.multimeter.Measure()) return false;
                        measData = General.multimeter.CurrData;



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
                General.signalSource.StopSource();
                Thread.Sleep(500);
                //リレーを初期化する処理
                General.ResetRelay_7062_7012();
                //ビューモデルの更新

                State.VmTestResults.Curr3V = (measData * 1000000).ToString("F2") + "uA";
                State.VmTestResults.ColorCurr3V = result ? Brushes.Transparent : Brushes.OrangeRed;
                //NGだった場合、エラー詳細情報の規格値を更新する
                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : " + State.TestSpec.amp3vMin.ToString("F2") + "～" + State.TestSpec.amp3vMax.ToString("F2") + "uA";
                    State.VmTestStatus.MeasValue = "計測値 : " + (measData * 1000000).ToString("F2") + "uA";
                }
            }
        }

    }
}
