using System.IO.Ports;
using System.Threading;

namespace NewPC81Tester
{
    public static class Target
    {
        //列挙型の宣言
        public enum Port { Rs232C_1, Rs232C_2, Rs232C_3, Rs232C_4, Rs422_1, Rs422_2 }


        //静的メンバの宣言
        public static SerialPort Port_232C_1;    //Port1 RS232C
        public static SerialPort Port_232C_2;    //Port1 RS232C
        public static SerialPort Port_232C_3;    //Port1 RS232C
        public static SerialPort Port_232C_4;    //Port1 RS232C

        public static SerialPort Port_422_1;     //Port1 RS422
        public static SerialPort Port_422_2;     //Port1 RS422
        public static string RecieveData { get; private set; }  //Targetから受信した生データ

        public static bool 通信ステータス { get; set; }

        //コンストラクタ
        static Target()
        {
            Port_232C_1 = new SerialPort();
            Port_232C_2 = new SerialPort();
            Port_232C_3 = new SerialPort();
            Port_232C_4 = new SerialPort();

            Port_422_1 = new SerialPort();
            Port_422_2 = new SerialPort();
        }

        //イニシャライズ
        public static bool Init(Port port)
        {
            SerialPort ActivePort = null;
            string ComNum = "";

            try
            {
                switch (port)
                {
                    case Port.Rs232C_1:
                        ActivePort = Port_232C_1;
                        ComNum = State.Setting.Com232_1;
                        break;

                    case Port.Rs232C_2:
                        ActivePort = Port_232C_2;
                        ComNum = State.Setting.Com232_2;
                        break;

                    case Port.Rs232C_3:
                        ActivePort = Port_232C_3;
                        ComNum = State.Setting.Com232_3;
                        break;

                    case Port.Rs232C_4:
                        ActivePort = Port_232C_4;
                        ComNum = State.Setting.Com232_4;
                        break;

                    case Port.Rs422_1:
                        ActivePort = Port_422_1;
                        ComNum = State.Setting.Com422_1;
                        break;

                    case Port.Rs422_2:
                        ActivePort = Port_422_2;
                        ComNum = State.Setting.Com422_2;
                        break;
                }

                if (ActivePort.IsOpen) return true;//既に開いているポートを開くと例外が発生するのでここでチェックする

                //RS232C_1の設定
                ActivePort.PortName = ComNum;
                ActivePort.BaudRate = 9600;
                ActivePort.DataBits = 7;
                ActivePort.Parity = System.IO.Ports.Parity.Even;
                ActivePort.StopBits = System.IO.Ports.StopBits.Two;
                ActivePort.NewLine = "\r\n" + (char)0x03;
                ActivePort.ReadTimeout = 2000;

                //通信ポートを開く               
                ActivePort.Open();
                return true;
            }
            catch
            {
                ActivePort.Close();
                return false;
            }

        }


        //**************************************************************************
        //Targetに文字列を送信する
        //引数：コマンド
        //戻値：bool
        //＜名前付き引数＞
        //C# 4.0 では、さたにメソッド呼び出しする時に名前を指定することにより、引数の順番から解放されます。
        //**************************************************************************
        public static bool SendData(Port pName, string Data, int Wait = 3000, bool setLog = true, bool doAnalysisData = true)
        {
            string header = pName.ToString() + "_";
            SerialPort ActivePort = null;

            try
            {
                switch (pName)
                {
                    case Port.Rs232C_1:
                        ActivePort = Port_232C_1;
                        break;

                    case Port.Rs232C_2:
                        ActivePort = Port_232C_2;
                        break;

                    case Port.Rs232C_3:
                        ActivePort = Port_232C_3;
                        break;

                    case Port.Rs232C_4:
                        ActivePort = Port_232C_4;
                        break;

                    case Port.Rs422_1:
                        ActivePort = Port_422_1;
                        break;

                    case Port.Rs422_2:
                        ActivePort = Port_422_2;
                        break;
                }

                State.VmComm.DATA_TX = "";
                State.VmComm.DATA_RX = "";

                string sendData = (char)0x02 + Data;

                ClearBuff();//受信バッファのクリア

                ActivePort.WriteLine(sendData);//"\r\n" + (char)0x03は付加されている

                if (setLog) State.VmComm.DATA_TX = header + Data;

                if (!doAnalysisData) return true;//LED全点灯用 送信コマンドに対して返信がないため

            }
            catch
            {
                State.VmComm.DATA_TX = header + "Error";
                return false;
            }

            //受信処理 
            try
            {
                RecieveData = "";//初期化
                ActivePort.ReadTimeout = Wait;
                var RxData = ActivePort.ReadLine();
                return AnalysisData(header, RxData, setLog);
            }
            catch
            {
                if (setLog) State.VmComm.DATA_RX = header + "TimeoutErr";
                return false;
            }
        }

        public static bool AnalysisData(string header, string data, bool setLog = true)
        {
            bool result = false;

            try
            {
                var stx = ((char)0x02).ToString();
                //受信データのフレームが正しいかチェックする（先頭STX）
                if (!data.StartsWith(stx))
                {
                    RecieveData = "FrameError";
                    return result = false;
                }

                //先頭のSTXを取り除いた文字列を抽出する
                RecieveData = data.Trim((char)0x02);
                return result = true;
            }
            catch
            {
                RecieveData = "Error例外";
                return result = false;
            }
            finally
            {
                if (setLog) State.VmComm.DATA_RX = header + RecieveData; Thread.Sleep(40);
            }
        }

        //**************************************************************************
        //COMポートを閉じる処理
        //**************************************************************************   
        public static void ClosePort()
        {
            if (Port_232C_1.IsOpen) Port_232C_1.Close();
            if (Port_232C_2.IsOpen) Port_232C_2.Close();
            if (Port_232C_3.IsOpen) Port_232C_3.Close();
            if (Port_232C_4.IsOpen) Port_232C_4.Close();

            if (Port_422_1.IsOpen) Port_422_1.Close();
            if (Port_422_2.IsOpen) Port_422_2.Close();

        }

        //**************************************************************************
        //各ＣＯＭポートの受信バッファ内にある受信データのバイト数を取得する
        //**************************************************************************
        public static int GetByteData(Port pName)
        {
            try
            {
                switch (pName)
                {
                    case Port.Rs232C_1:
                        return Port_232C_1.BytesToRead;

                    case Port.Rs232C_2:
                        return Port_232C_2.BytesToRead;

                    case Port.Rs232C_3:
                        return Port_232C_3.BytesToRead;

                    case Port.Rs232C_4:
                        return Port_232C_4.BytesToRead;

                    case Port.Rs422_1:
                        return Port_422_1.BytesToRead;

                    case Port.Rs422_2:
                        return Port_422_2.BytesToRead;

                    default:
                        return 0;
                }
            }
            catch
            {
                return 0;
            }

        }

        //**************************************************************************
        //受信バッファをクリアする
        //**************************************************************************
        public static void ClearBuff()
        {
            Port_232C_1.DiscardInBuffer();
            Port_232C_2.DiscardInBuffer();
            Port_232C_3.DiscardInBuffer();
            Port_232C_4.DiscardInBuffer();

            Port_422_1.DiscardInBuffer();
            Port_422_2.DiscardInBuffer();
        }

    }
}
