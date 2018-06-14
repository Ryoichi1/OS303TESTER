using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using static System.Threading.Thread;

namespace Os303Tester
{

    public class Agilent34401A : IMultimeter
    {
        //列挙型の宣言
        public enum ErrorCode { NORMAL, CMD_ERR, TIMEOUT_ERR, OPEN_ERR, SEND_ERR, OTHER }
        public enum MeasMode { DEFAULT, DCV, ACV, DCA, RES, FREQ }

        public ErrorCode state { get; set; }
        public MeasMode mode { get; set; }

        ////private const string ComName = "通信ポート";
        //private const string ComName = "USB Serial Port ";
        private const string ID_34401A = "HEWLETT-PACKARD,34401A";

        //変数の宣言（インスタンスメンバーになります）
        private static SerialPort port;

        private string RecieveData;//34401Aから受信した生データ

        private double _VoltData;//計測したDC/AC電圧値
        public double VoltData
        {
            get { return _VoltData; }
        }

        private double _CurrData;//計測したDC/AC電流値
        public double CurrData
        {
            get { return _CurrData; }
        }



        //コンストラクタ
        public Agilent34401A()
        {
            port = new SerialPort();
            mode = MeasMode.DEFAULT;
        }


        //**************************************************************************
        //34401Aの初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        bool IMultimeter.Init()
        {
            try
            {
                ////Comポートリストの取得
                //FindSerialPort.GetDeviceNames();
                //var portName = FindSerialPort.GetComNo(ComName);
                //if (portName == null) return false;

                if (!port.IsOpen)
                {
                    //Agilent34401A用のシリアルポート設定
                    port.PortName = "COM1"; //この時点で既にポートが開いている場合COM番号は設定できず例外となる（イニシャライズは１回のみ有効）
                    //port.PortName = portName; //この時点で既にポートが開いている場合COM番号は設定できず例外となる（イニシャライズは１回のみ有効）
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.DtrEnable = true;//これ設定しないとコマンド送るたびにErrorになります！
                    port.NewLine = ("\r\n");
                    port.Open();
                }

                //クエリ送信
                SetRemote();
                port.WriteLine("*IDN?");
                ReadRecieveData(1000);
                if (RecieveData.Contains(ID_34401A))
                {
                    return true;
                }
                else
                {
                    //開いたポートが間違っているのでいったん閉じる
                    port.Close();
                    state = ErrorCode.OPEN_ERR;
                    return false;
                }
            }
            catch
            {
                port.Close();
                state = ErrorCode.OPEN_ERR;
                return false;
            }
        }

        private void SetRemote()
        {
            //コマンド送信
            port.WriteLine("SYST:REM");
            ReadRecieveData(1000);
            port.WriteLine("*CLS");
            ReadRecieveData(1000);
            port.WriteLine("*RST");
            ReadRecieveData(1000);

        }

        //**************************************************************************
        //34401Aからの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ErrorCode
        //**************************************************************************
        private bool ReadRecieveData(int time)
        {

            RecieveData = null;//念のため初期化
            string buffer = null;
            port.ReadTimeout = time;
            try
            {
                buffer = port.ReadTo("\r\n");
            }
            catch (TimeoutException)
            {
                return false;
            }

            RecieveData = buffer;
            return true;
        }


        //**************************************************************************
        //COMポートを閉じる
        //引数：なし
        //戻値：bool
        //**************************************************************************   

        bool IMultimeter.ClosePort()
        {
            try
            {
                if (port.IsOpen)
                {
                    port.WriteLine("*RST");
                    port.WriteLine("SYST:LOC");//ローカル設定に戻してからCOMポート閉じる
                    port.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        bool IMultimeter.SetVoltDc()
        {
            try
            {
                mode = MeasMode.DCV;
                Sleep(400);
                return true;
            }
            catch
            {
                mode = MeasMode.DEFAULT;
                return false;
            }
        }

        bool IMultimeter.SetVoltAc()
        {
            try
            {
                mode = MeasMode.ACV;
                Sleep(400);
                return true;
            }
            catch
            {
                mode = MeasMode.DEFAULT;
                return false;
            }

        }

        bool IMultimeter.SetCurrDc()
        {
            try
            {
                mode = MeasMode.DCA;
                Sleep(400);
                return true;
            }
            catch
            {
                mode = MeasMode.DEFAULT;
                return false;
            }
        }

        bool IMultimeter.Measure()
        {
            try
            {
                switch (mode)
                {
                    case MeasMode.DCV:
                        port.WriteLine(":MEAS:VOLT:DC?");
                        break;
                    case MeasMode.ACV:
                        port.WriteLine(":MEAS:VOLT:AC?");
                        break;
                }
                Sleep(500);//必ずこのウェイトを入れること
                if (!ReadRecieveData(1000))
                    return false;

                switch (mode)
                {
                    case MeasMode.DCV:
                    case MeasMode.ACV:
                        return (Double.TryParse(RecieveData, out _VoltData));
                    case MeasMode.DCA:
                        return (Double.TryParse(RecieveData, out _CurrData));
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

    }


}