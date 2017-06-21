using System;
using System.IO.Ports;
using System.Threading;

namespace NewPC81Tester
{

    public class HIOKI7012 : SignalSource
    {
        //列挙型の宣言
        public enum ErrorCode { Normal, CmdErr, TimeoutErr, OpenErr, SendErr, Other }
        private enum FUNC_MODE { CV_2_5, CV_25, CC_25, TC_0 }

        //定数の宣言
        private const string SS7012_ID = "HIOKI,SS7012, Ver 1.03";
        private const string ComName = "Prolific USB-to-Serial Comm Port";

        //パブリックメンバ
        public ErrorCode ErrState { get; set; }

        //プライベートメンバ
        private SerialPort port;
        private string RecieveData;//7012から受信した生データ

        //静的コンストラクタ
        public HIOKI7012()
        {
            port = new SerialPort();
        }

        //**************************************************************************
        //COMポートのオープン
        //**************************************************************************
        public bool Init()
        {
            try
            {
                FindSerialPort.GetDeviceNames();
                var portName = FindSerialPort.GetComNo(ComName);
                if (portName == null) return false;//
                
                //シリアルポート設定
                port.PortName = portName;
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.NewLine = ("\r\n");
                port.Open();

                return (SendQuery("*IDN?") && RecieveData == SS7012_ID);
            }
            catch
            {
                ErrState = ErrorCode.OpenErr;
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
            }
            catch
            {
                ErrState = ErrorCode.SendErr;
                return false;
            }

            //受信処理 
            //設定コマンド送信時は、OK または CMD ERR が返ってきます
            try
            {
                RecieveData = "";//初期化
                port.ReadTimeout = 1500;
                RecieveData = port.ReadLine();
                if (RecieveData == "OK")
                {
                    ErrState = ErrorCode.Normal;
                    return true;
                }
                else
                {
                    ErrState = ErrorCode.CmdErr;
                    return false;
                }
            }
            catch
            {
                ErrState = ErrorCode.TimeoutErr;
                return false;
            }
        }

        //**************************************************************************
        //SS7012に設定コマンドを送る
        //引数：なし
        //戻値：bool
        //**************************************************************************
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
                ErrState = ErrorCode.SendErr;
                return false;
            }

            //受信処理 
            //設定コマンド送信時は、OK または CMD ERR が返ってきます
            try
            {
                RecieveData = "";//初期化
                port.ReadTimeout = 1500;
                RecieveData = port.ReadLine();
                if (RecieveData != "CMD ERR")
                {
                    ErrState = ErrorCode.Normal;
                    return true;
                }
                else
                {
                    ErrState = ErrorCode.CmdErr;
                    return false;
                }
            }
            catch
            {
                ErrState = ErrorCode.TimeoutErr;
                return false;
            }
        }

        //**************************************************************************
        //DC電圧を出力する
        //引数：なし
        //戻値：bool
        //**************************************************************************       
        public bool OutDcV(double outValue)
        {
            try
            {
                if (!StopSource()) return false;

                //ファンクションの切り替え & 出力電圧の設定
                if (outValue >= 0 && outValue <= 2.5)
                {
                    if (!SendCommand("FCC 0")) return false;
                    if (!SendCommand("CVV " + outValue.ToString("F4"))) return false;
                }
                else if (outValue > 2.5 && outValue <= 25)
                {
                    if (!SendCommand("FCC 1")) return false;
                    if (!SendCommand("CVV " + outValue.ToString("F3"))) return false;
                }
                else//0～25V以外の出力値は設定させない
                {
                    return false;
                }

                //出力開始
                return SendCommand("OUT 1");
            }
            catch
            {
                StopSource();
                return false;
            }
        }

        //**************************************************************************
        //K熱電対起電力を出力する
        //引数：なし
        //戻値：bool
        //**************************************************************************       
        public bool OutTc_K(double outValue)
        {
            try
            {
                if (!StopSource()) return false;

                //ファンクションの切り替え & 出力電圧の設定
                if (!SendCommand("FCC 3")) return false;
                if (!SendCommand("TCC K, " + outValue.ToString("F1"))) return false;

                //出力開始
                return SendCommand("OUT 1");
            }
            catch
            {
                StopSource();
                return false;
            }
        }

        public bool StopSource()
        {
            return SendCommand("OUT 0");
        }

        //**************************************************************************
        //COMポートを閉じる処理
        //引数：なし
        //戻値：なし
        //**************************************************************************   
        public void ClosePort()
        {
            StopSource();
            if (port.IsOpen) port.Close();
        }

    }


}