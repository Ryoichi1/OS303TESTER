using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NewPC81Tester
{
    public static class RTCチェック
    {

        public static async Task<bool> SetTime()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    //システム時計から、現在時間を取得
                    DateTime dt1 = DateTime.Now;
                    string dtdata1 = dt1.ToString("yy/MM/dd/HH/mm/ss");
                    string setdata = "SetTime/" + dtdata1;

                    if (!Target.SendData(Target.Port.Rs232C_1, setdata)) return false;//ﾘｱﾙﾀｲﾑｸﾛｯｸ設定コマンド送信
                    return Target.RecieveData.Contains("SetTimeOK");
                }
                catch
                {
                    return false;
                }
            });
        }

        public static async Task<bool> CheckTime()
        {
            bool result = false;

            return await Task<bool>.Run(() =>
            {
                try
                {
                    DateTime dt2 = DateTime.Now;//直前にHost側の時刻を取得しておく
                    if (!Target.SendData(Target.Port.Rs232C_1, "ReqTime")) return false;  //SendRs422Data("ReqTime");//ﾘｱﾙﾀｲﾑｸﾛｯｸ確認コマンド送信
                    var TargetTime = Target.RecieveData;// 返信例）Time/17/06/19/22/31/01
                    State.VmTestResults.TimeTarget = TargetTime.Substring(5) ;

                    var HostTime = dt2.ToString("yy/MM/dd/HH/mm/ss");
                    State.VmTestResults.TimeHost = HostTime;
                    //Host側の累積時間（秒）を算出
                    int hostHH = Int32.Parse(dt2.ToString("HH"));
                    int hostMM = Int32.Parse(dt2.ToString("mm"));
                    int hostSS = Int32.Parse(dt2.ToString("ss"));
                    int hostTotal = (hostHH * 3600) + (hostMM * 60) + hostSS;


                    //target側の累積時間（秒）を算出 返信例）Time/17/06/19/22/31/01
                    int targetHH = Int32.Parse(Target.RecieveData.Substring(14, 2));
                    int targetMM = Int32.Parse(Target.RecieveData.Substring(17, 2));
                    int targetSS = Int32.Parse(Target.RecieveData.Substring(20, 2));
                    int targetTotal = (targetHH * 3600) + (targetMM * 60) + targetSS;

                    //時間差を算出　誤差１秒以内ならOKとする
                    var 誤差 = Math.Abs(hostTotal - targetTotal).ToString();
                    State.VmTestResults.RtcTimeError = 誤差 + "秒";
                    result = (Math.Abs(hostTotal - targetTotal) <= State.TestSpec.rtcTimeErr); //現行ソフトの規格は　+2秒、-1秒以内だった
                    if (result)
                    {
                        State.VmTestResults.ColorRtcTimeError = Brushes.Transparent;
                        return true;
                    }
                    else
                    {
                        State.VmTestResults.ColorRtcTimeError = Brushes.HotPink;
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            });

        }


        //**************************************************************************
        //製品ソフト書き込み後に、ＲＴＣ設定コマンドを送信する
        //引数：無し
        //戻値：bool
        //**************************************************************************
        public static async Task<bool> RTC最終設定()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    //ホスト側の時間を取得
                    var dt = DateTime.Now;

                    string year = dt.ToString("yy");
                    string month = dt.ToString("MM");
                    string day = dt.ToString("dd");
                    string hour = dt.ToString("HH");
                    string min = dt.ToString("mm");
                    string sec = dt.ToString("ss");

                    //WEコマンドのチェックサム値を計算する
                    string command = "@00WE000016" + min + sec + day + hour + year + month;
                    int len = command.Length;
                    char cfcs = '\0';

                    foreach (var i in Enumerable.Range(0, len))
                    {
                        cfcs = (char)(cfcs ^ command[i]);
                    }


                    string hexCfcs = Convert.ToString((int)cfcs, 16);
                    string weCommand = command + hexCfcs + "*" + (char)0x0d;

                    Target.ClearBuff();
                    State.VmComm.DATA_TX = "";
                    State.VmComm.DATA_RX = "";

                    Target.Port_232C_3.Write(weCommand);
                    State.VmComm.DATA_TX = "RS232C-3 TX_" + weCommand;
                }
                catch
                {
                    State.VmComm.DATA_TX = "RS232C-3 TX_Error";
                    return false;
                }

                //受信処理
                try
                {
                    string RecieveData = "";

                    Target.Port_232C_3.ReadTimeout = 3000;
                    RecieveData = Target.Port_232C_3.ReadTo("\r");
                    State.VmComm.DATA_RX = "RS232C-3 RX_" + RecieveData.Substring(1);

                    //時間設定が成功したかどうかの判定
                    return (RecieveData.Contains("@00WE00"));
                }
                catch
                {
                    State.VmComm.DATA_RX = "RS232C-3 RX_Error";
                    return false;
                }
            });

        }

        //**************************************************************************
        //ＲＴＣ最終設定後に、時刻データを読み出す
        //引数：無し
        //戻値：bool
        //**************************************************************************
        public static async Task<bool> RTC最終チェック()
        {
            bool result = false;
            return await Task<bool>.Run(() =>
            {
                try
                {
                    //ターゲットから時間データを読み出す
                    string txData = "@00RE000016000353*" + (char)0x0d;
                    Target.ClearBuff();
                    State.VmComm.DATA_TX = "";
                    State.VmComm.DATA_RX = "";

                    Target.Port_232C_3.Write(txData);
                    State.VmComm.DATA_TX = "RS232C-3 TX_" + txData;
                }
                catch
                {
                    State.VmComm.DATA_TX = "RS232C-3 TX_Error";
                    return false;
                }

                DateTime dt2 = DateTime.Now;//Taregetに時刻データを要求した直後に、Host側の時刻を取得する

                string RecieveData = "";
                try
                {
                    RecieveData = Target.Port_232C_3.ReadTo("\r");
                    State.VmComm.DATA_RX = "RS232C-3 RX_" + RecieveData.Substring(1);
                }
                catch
                {
                    State.VmComm.DATA_RX = "RS232C-3 RX_Error";
                    return false;
                }

                try
                {
                    string timeData = RecieveData.Substring(7, 12);

                    string min = timeData.Substring(0, 2);
                    string sec = timeData.Substring(2, 2);
                    string day = timeData.Substring(4, 2);
                    string hour = timeData.Substring(6, 2);
                    string year = timeData.Substring(8, 2);
                    string month = timeData.Substring(10, 2);

                    string dt1 = year + "/" + month + "/" + day + "/" + hour + "/" + min + "/" + sec;
                    var TargetTime = dt1;
                    State.VmTestResults.TimeTarget2 = TargetTime;

                    string dtdata2 = dt2.ToString("yy/MM/dd/HH/mm/ss");
                    var HostTime = dtdata2;
                    State.VmTestResults.TimeHost2 = HostTime;


                    //年・月・日までがあっているかどうかの判定
                    if (dt1.Substring(0, 8) != dtdata2.Substring(0, 8)) return false;

                    //時・分・秒があっているかどうかの判定
                    //Host側の累積時間(秒)を算出
                    int hostHH = Int32.Parse(dtdata2.Substring(9, 2));
                    int hostMM = Int32.Parse(dtdata2.Substring(12, 2));
                    int hostSS = Int32.Parse(dtdata2.Substring(15, 2));
                    int hostTotal = (hostHH * 3600) + (hostMM * 60) + hostSS;

                    //target側の累積時間（秒）を算出
                    int targetHH = Int32.Parse(hour);
                    int targetMM = Int32.Parse(min);
                    int targetSS = Int32.Parse(sec);
                    int targetTotal = (targetHH * 3600) + (targetMM * 60) + targetSS;

                    //時間差を算出　誤差２秒以内ならOKとする
                    var 誤差 = Math.Abs(hostTotal - targetTotal);
                    State.VmTestResults.RtcTimeError2 = 誤差.ToString() + "秒";
                    result = (誤差 <= State.TestSpec.rtcTimeErr);
                    if (result)
                    {
                        State.VmTestResults.ColorRtcTimeError2 = Brushes.Transparent;
                        return true;
                    }
                    else
                    {
                        State.VmTestResults.ColorRtcTimeError2 = Brushes.HotPink;
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            });

        }




    }
}
