using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class 熱電対チェック
    {
        public enum NAME
        {
            AN4,/*Loop1*/
            AN5 /*Loop2*/
        }

        public enum INPUT { _25, _100, _400 }




        public static async Task<bool> InitLoop()
        {
            const int WaitTime = 20000;
            bool FlagTimeout = false;
            bool result = false;

            //タイマーの設定
            var tmTimeOut = new System.Timers.Timer();
            tmTimeOut.Enabled = false;
            tmTimeOut.Elapsed += (sender, e) =>
            {
                tmTimeOut.Stop();
                FlagTimeout = true;
            };

            try
            {
                return await Task<bool>.Run(() =>
                {
                    Flags.AddDecision = false;
                    State.VmTestStatus.TestLog += "\r\nLoop1補正値取得";

                    General.ResetIo();
                    //an4側にSS7012のソース端子を接続する処理
                    General.SetLY4(true);
                    Thread.Sleep(200);

                    General.PowSupply(true);
                    Thread.Sleep(200);

                    if (!General.signalSource.OutTc_K(0.0)) return false;

                    //15秒待ち
                    tmTimeOut.Stop();
                    tmTimeOut.Interval = WaitTime;
                    FlagTimeout = false;
                    tmTimeOut.Start();

                    while (true)
                    {
                        if (Flags.ClickStopButton)
                        {
                            return false;
                        }
                        if (FlagTimeout) break;
                    }

                    //オフセットコマンド送信
                    if (!Target.SendData(Target.Port.Rs232C_1, "SetLoop1", Wait: 10000)) return false;

                    if (!Target.RecieveData.Contains("LOOPOK1"))
                    {
                        State.VmTestResults.Loop1Ad = Convert.ToInt32(Target.RecieveData.Substring(10), 16).ToString();
                        State.VmTestResults.ColorLoop1Ad = Brushes.HotPink;
                        return false;
                    }

                    //LOOP1_CAL_VALの取得
                    if (!Target.SendData(Target.Port.Rs232C_1, "CalLoop1")) return false;//EEPROMに書き込んだオフセット値を取得する(若ちゃんより指示)

                    int Cal_Dec = Convert.ToInt32(Target.RecieveData.Substring(6), 16);//16進だとわかりずらいので10進に変換
                    State.VmTestResults.Loop1Ad = Cal_Dec.ToString();
                    General.signalSource.StopSource(); //いったんソースOFF
                    State.VmTestStatus.TestLog += "---PASS";

                    /////////////////////////////////////////////////////////////////////////////
                    State.VmTestStatus.TestLog += "\r\nLoop2補正値取得";
                    General.PowSupply(false);
                    Thread.Sleep(200);
                    General.ResetIo();
                    //an4側にSS7012のソース端子を接続する処理
                    General.SetLY5(true);
                    Thread.Sleep(200);

                    General.PowSupply(true);
                    Thread.Sleep(200);

                    if (!General.signalSource.OutTc_K(0.0)) return false;

                    //15秒待ち
                    tmTimeOut.Stop();
                    tmTimeOut.Interval = WaitTime;
                    FlagTimeout = false;
                    tmTimeOut.Start();

                    while (true)
                    {
                        if (Flags.ClickStopButton) return false;
                        if (FlagTimeout) break;
                    }

                    //オフセットコマンド送信
                    if (!Target.SendData(Target.Port.Rs232C_1, "SetLoop2", Wait: 10000)) return false;

                    if (!Target.RecieveData.Contains("LOOPOK2"))
                    {
                        State.VmTestResults.Loop2Ad = Convert.ToInt32(Target.RecieveData.Substring(10), 16).ToString();
                        State.VmTestResults.ColorLoop2Ad = Brushes.HotPink;
                        return false;
                    }

                    //LOOP2_CAL_VALの取得
                    if (!Target.SendData(Target.Port.Rs232C_1, "CalLoop2")) return false;//EEPROMに書き込んだオフセット値を取得する(若ちゃんより指示)

                    Cal_Dec = Convert.ToInt32(Target.RecieveData.Substring(6), 16);//16進だとわかりずらいので10進に変換
                    State.VmTestResults.Loop2Ad = Cal_Dec.ToString();
                    General.signalSource.StopSource(); //いったんソースOFF
                    State.VmTestStatus.TestLog += "---PASS";

                    return result = true;
                });
            }
            finally
            {
                if (!result)
                {
                    State.VmTestStatus.TestLog += "---FAIL";
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                }
                General.signalSource.StopSource(); //ソースOFF
                General.ResetRelay_7062_7012();
            }

        }

        public static async Task<bool> CheckU6()
        {
            bool result = false;
            double 温度差 = 0;
            bool FlagCheckedTempErr = false;//USB温度計とU6双方から温度データを取得し、比較が完了したかどうかのフラグ
            try
            {
                try
                {
                    return await Task<bool>.Run(() =>
                    {
                        General.ResetRelay_7062_7012();
                        Thread.Sleep(200);

                        //ＬＯＯＰ１，２をショートさせる（シグナルソースから0.0℃を入力する）
                        General.signalSource.OutDcV(0.0);
                        Thread.Sleep(1000);
                        General.SetLY4(true);
                        General.SetLY5(true);
                        Thread.Sleep(2000);

                        //①ＵＳＢ温度計から温度を取り込む
                        double USB温度計の表示値 = Double.Parse(State.VmTestStatus.Temp.Trim('℃'));
                        State.VmTestResults.TempUSBRH = USB温度計の表示値.ToString("F3");

                        //②ＡＮ６（Ｕ６）のＡＤ値を取り込む
                        if (!Target.SendData(Target.Port.Rs232C_1, "ReadAN6")) return false;

                        int buf = Convert.ToInt32(Target.RecieveData.Substring(3), 16);
                        double buf2 = (double)buf / 1000;
                        buf2 = Math.Sqrt(2.1962e+6 + (1.8639 - buf2) / (3.88e-6));
                        double u6data = -1481.96 + buf2;
                        State.VmTestResults.TempTarget = u6data.ToString("F3");
                        温度差 = Math.Abs(USB温度計の表示値 - u6data);
                        FlagCheckedTempErr = true;
                        return result = 温度差 < State.TestSpec.ErrTemp_UsbRh_U6;
                    });
                }
                catch
                {
                    return result = false;
                }
            }
            finally
            {
                General.signalSource.StopSource();
                General.ResetRelay_7062_7012();
                State.VmTestResults.ColorTempTarget = result ? Brushes.Transparent : Brushes.HotPink;

                if (!result)
                {
                    State.VmTestStatus.Spec = "規格値 : 温度計とU6の差 " + State.TestSpec.ErrTemp_UsbRh_U6.ToString("F3") + "℃以内";
                    if (FlagCheckedTempErr)
                    {
                        State.VmTestStatus.MeasValue = "計測値 : 温度計とU6の差 " + 温度差.ToString("F3") + "℃";
                    }
                    else
                    {
                        State.VmTestStatus.MeasValue = "計測値 : 温度計とU6の差 ---℃ ";
                    }
                }
            }
        }


        private static void SetViewModel(NAME name, INPUT input, double val, bool result)
        {
            switch (name)
            {
                case NAME.AN4:
                    switch (input)
                    {
                        case INPUT._25:
                            State.VmTestResults.Ch1_25 = val.ToString("F2") + "℃";
                            if (!result) State.VmTestResults.ColorCh1_25 = Brushes.HotPink;
                            break;
                        case INPUT._100:
                            State.VmTestResults.Ch1_100 = val.ToString("F2") + "℃";
                            if (!result) State.VmTestResults.ColorCh1_100 = Brushes.HotPink;
                            break;
                        case INPUT._400:
                            State.VmTestResults.Ch1_400 = val.ToString("F2") + "℃";
                            if (!result) State.VmTestResults.ColorCh1_400 = Brushes.HotPink;
                            break;
                    }
                    break;

                case NAME.AN5:
                    switch (input)
                    {
                        case INPUT._25:
                            State.VmTestResults.Ch2_25 = val.ToString("F2") + "℃";
                            if (!result) State.VmTestResults.ColorCh2_25 = Brushes.HotPink;
                            break;
                        case INPUT._100:
                            State.VmTestResults.Ch2_100 = val.ToString("F2") + "℃";
                            if (!result) State.VmTestResults.ColorCh2_100 = Brushes.HotPink;
                            break;
                        case INPUT._400:
                            State.VmTestResults.Ch2_400 = val.ToString("F2") + "℃";
                            if (!result) State.VmTestResults.ColorCh2_400 = Brushes.HotPink;
                            break;
                    }
                    break;
            }
        }
        private static void ResetViewModel(NAME name)
        {
            switch (name)
            {
                case NAME.AN4:
                    State.VmTestResults.Ch1_25 = "";
                    State.VmTestResults.ColorCh1_25 = Brushes.Transparent;
                    State.VmTestResults.Ch1_100 = "";
                    State.VmTestResults.ColorCh1_100 = Brushes.Transparent;
                    State.VmTestResults.Ch1_400 = "";
                    State.VmTestResults.ColorCh1_400 = Brushes.Transparent;
                    break;

                case NAME.AN5:
                    State.VmTestResults.Ch2_25 = "";
                    State.VmTestResults.ColorCh2_25 = Brushes.Transparent;
                    State.VmTestResults.Ch2_100 = "";
                    State.VmTestResults.ColorCh2_100 = Brushes.Transparent;
                    State.VmTestResults.Ch2_400 = "";
                    State.VmTestResults.ColorCh2_400 = Brushes.Transparent;
                    break;
            }
        }

        public static async Task<bool> CheckTemp(NAME name)
        {
            bool AllResult = false;

            double loopCalVal = 0;
            double Max = 0;//ＡＤ値上限規格値
            double Min = 0;//ＡＤ値下限規格値
            double inputTemp = 0;//SS7012からの入力電圧

            try
            {
                return AllResult = await Task<bool>.Run(() =>
                {
                    ResetViewModel(name);
                    General.signalSource.StopSource();//SS7012からの出力を停止
                    General.ResetRelay_7062_7012();
                    Flags.AddDecision = false;

                    switch (name)
                    {
                        case NAME.AN4:
                            //LOOP1_CAL_VAL
                            loopCalVal = Int32.Parse(State.VmTestResults.Loop1Ad);
                            General.SetLY4(true);
                            break;

                        case NAME.AN5:
                            //LOOP2_CAL_VAL
                            loopCalVal = Int32.Parse(State.VmTestResults.Loop2Ad);
                            General.SetLY5(true);
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

                        switch (input)
                        {
                            case INPUT._25:
                                inputTemp = 25.0;//シグナルソースからの熱電対入力（MIN）
                                Max = State.TestSpec.temp1Max;
                                Min = State.TestSpec.temp1Min;
                                break;
                            case INPUT._100:
                                inputTemp = 100.0;//シグナルソースからの熱電対入力（MID）
                                Max = State.TestSpec.temp2Max;
                                Min = State.TestSpec.temp2Min;
                                break;
                            case INPUT._400:
                                inputTemp = 400.0;//シグナルソースからの熱電対入力（MAX）
                                Max = State.TestSpec.temp3Max;
                                Min = State.TestSpec.temp3Min;
                                break;
                        }


                        //テストログの更新
                        State.VmTestStatus.TestLog += "\r\n" + name.ToString() + " " + input.ToString().Trim('_') + "℃入力チェック";
                        General.signalSource.OutTc_K(inputTemp);
                        Thread.Sleep(6000);
                        if (!Target.SendData(Target.Port.Rs232C_1, "Read" + name.ToString())) return false;
                        Thread.Sleep(100);
                        int Temp補正前_Dec = Convert.ToInt32(Target.RecieveData.Substring(3), 16);

                        double tempData = (double)(Temp補正前_Dec - loopCalVal) / 179.43137; //固体差の打ち消し & オペアンプ倍率消し
                        tempData = tempData / 0.0378;//温度だし
                        var result = (tempData >= Min && tempData <= Max);
                        SetViewModel(name, input, tempData, result);

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
                State.VmTestStatus.TestLog += "\r\n";
                General.signalSource.StopSource();//SS7012からの出力を停止
                General.ResetRelay_7062_7012();
            }



        }


    }
}
