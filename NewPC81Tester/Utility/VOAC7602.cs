using System;
using System.IO.Ports;

namespace NewPC81Tester
{

    public class VOAC7602 : Multimeter
    {
        //列挙型の宣言
        public enum ErrorCode { Normal, CmdErr, TimeoutErr, OpenErr, SendErr, Other }
        public enum MeasMode { Volt, Current }

        public ErrorCode State7602 { get; set; }
        public MeasMode Mode { get; set; }

        //
        private const string ID_7602 = "IWATSU,VOAC7602,AA177130717,5.06";
        private const string ComName = "Iwatsu VOAC";

        //プライベートメンバ
        private SerialPort port;

        //変数の宣言（インスタンスメンバーになります）

        private string RecieveData;//34461Aから受信した生データ


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
        public VOAC7602()
        {
            port = new SerialPort();
        }


        //**************************************************************************
        //34401Aの初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public bool Init()
        {
            try
            {
                FindSerialPort.GetDeviceNames();
                var portName = FindSerialPort.GetComNo(ComName);
                if (portName == null) return false;//

                if (!port.IsOpen)
                {
                    //シリアルポート設定
                    port.PortName = portName;
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.NewLine = ("\r\n");
                    port.Open();
                }

                return (SendQuery("*IDN?") && RecieveData == ID_7602);
            }
            catch
            {
                State7602 = ErrorCode.OpenErr;
                ClosePort();
                return false;
            }
        }

        //**************************************************************************
        //SS7012に設定コマンドを送る
        //引数：なし
        //戻値：bool
        //**************************************************************************
        private bool SendCommand(string cmd)
        {
            //送信処理
            try
            {
                port.DiscardInBuffer();//COM受信バッファクリア
                port.WriteLine(cmd);
                return true;
            }
            catch
            {
                State7602 = ErrorCode.SendErr;
                return false;
            }

            //受信処理なし 
        }

        private bool SendQuery(string cmd)
        {
            //送信処理
            try
            {
                port.DiscardInBuffer();//COM受信バッファクリア
                port.WriteLine(cmd);
            }
            catch
            {
                State7602 = ErrorCode.SendErr;
                return false;
            }

            //受信処理 
            try
            {
                RecieveData = "";//初期化
                port.ReadTimeout = 2500;
                RecieveData = port.ReadLine();
                State7602 = ErrorCode.Normal;
                return true;
            }
            catch
            {
                State7602 = ErrorCode.TimeoutErr;
                return false;
            }
        }



        //**************************************************************************
        //DC電圧モードに設定する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public bool SetVoltDc()
        {
            if (!SendCommand(":CONF:VOLT:DC")) return false;
            if (!SendQuery(":CONF?")) return false;
            if (!RecieveData.Contains("VOLT ")) return false;
            Mode = MeasMode.Volt;


            if (!SendCommand(":VOLT:NPLC 0.002")) return false;
            if (!SendQuery(":VOLT:NPLC?")) return false;
            if (!RecieveData.Contains("+2E-3")) return false;

            if (!SendCommand(":VOLT:RANG:AUTO 1")) return false;
            if (!SendQuery(":VOLT:RANG:AUTO?")) return false;
            if (!RecieveData.Contains("1")) return false;

            return true;
        }

        //**************************************************************************
        //DC電流モードに設定する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public bool SetCurrDc()
        {
            if (!SendCommand(":CONF:CURR:DC")) return false;
            if (!SendQuery(":CONF?")) return false;
            if (!RecieveData.Contains("CURR ")) return false;
            Mode = MeasMode.Current;


            //if (!SendCommand(":CURR:NPLC 0.002")) return false;
            //if (!SendQuery(":CURR:NPLC?")) return false;
            //if (!RecieveData.Contains("+2E-3")) return false;

            //if (!SendCommand(":CURR:RANG:AUTO 1")) return false;
            //if (!SendQuery(":CURR:RANG:AUTO?")) return false;
            //if (!RecieveData.Contains("1")) return false;

            return true;
        }


        public bool Measure()
        {
            if (SendQuery(":READ?"))
            {
                if (Mode == MeasMode.Volt)
                {
                    return (Double.TryParse(RecieveData, out _VoltData));
                }
                else
                {
                    return (Double.TryParse(RecieveData, out _CurrData));
                }
            }
            else
            {
                return false;
            }
        }



        //**************************************************************************
        //COMポートを閉じる
        //引数：なし
        //戻値：bool
        //**************************************************************************   
        public bool ClosePort()
        {
            try
            {
                if (port.IsOpen) port.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
