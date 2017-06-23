using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

namespace NewPC81Tester
{

    public class Ca100 : SignalSource
    {
        //定数の宣言(クラスメンバーになります)

        public enum ComNumber { COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, COM10 }
        public enum ErrorCode { Normal, ResponseErr, OverRange, DataErr, TimeoutErr, MeasErr, InitErr, SendErr, Other }
        public enum OutRange { R500mV, R5V, R35V, R20mA, R100mA, 省略 }
        public enum SourceRange { R100mV, R1V, R10V, typeB, typeE, typeJ, typeK, typeN, typeR, typeT, 省略 }


        //変数の宣言（インスタンスメンバーになります）
        private SerialPort port;
        private string RecieveData;
        private byte statusByte;
        public ErrorCode Ca100state { get; set; }//CA100のｽﾃｰﾀｽ


        //コンストラクタ
        public Ca100()
        {
            port = new SerialPort();
        }

        public bool Init()
        {
            try
            {
                //CA100用のシリアルポート設定
                port.PortName = State.Setting.ComCa100;
                port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.NewLine = "\r\n";
                port.Open();
                Thread.Sleep(500);
                return Reset();
            }
            catch
            {
                Ca100state = ErrorCode.InitErr;
                ClosePort();
                return false;
            }
        }

        public void ClosePort()
        {
            StopSource();
            if (port.IsOpen) port.Close();
        }

        //**************************************************************************
        //ＣＡ１００にコマンドを送信する
        //引数：コマンド
        //戻値：bool
        //**************************************************************************
        private bool SendCommand(string command, string parameter = null)
        {
            //コマンド送信前にステータスを初期化する
            Ca100state = ErrorCode.Normal;

            try
            {
                port.DiscardInBuffer();//データ送信前に受信バッファのクリア

                //送信したコマンドが正常に処理されたかチェック
                if (parameter == null)
                {
                    port.WriteLine(command);
                    return (GetRecieveData(1000));
                }
                else
                {
                    port.WriteLine(command + parameter);
                    Thread.Sleep(50);//インターバル
                    port.WriteLine(command + "?");
                    return (CheckRecieveData(command + parameter, 1000));
                }
            }
            catch
            {
                Ca100state = ErrorCode.SendErr;
                return false;
            }
        }

        //**************************************************************************
        //ＣＡ１００の初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        private bool Reset()
        {
            bool result = false;
            try
            {
                port.WriteLine("RC");
                Thread.Sleep(200);
                var ESC = ((char)0x1b).ToString();
                if (!SendCommand(ESC + "S")) return false;
                if (RecieveData == "64") return true;

                Thread.Sleep(300);
                if (!SendCommand(ESC + "S")) return false;
                return result = (RecieveData == "64");//ステータスバイトのbit6は1固定 異常がなければその他のビットが0なので64が返ってくる

            }
            catch
            {
                return false;
            }
            finally
            {
                if (result)
                {
                    Ca100state = ErrorCode.Normal;
                }
                else
                { 
                    Ca100state = ErrorCode.InitErr;
                }

            }
        }

        //**************************************************************************
        //ＣＡ１００から指定した【文字列】が、指定時間内に帰ってくるかの判定
        //引数：チェックする文字列、指定時間（ｍｓｅｃ）
        //戻値：bool
        //**************************************************************************
        private bool CheckRecieveData(string data, int time)
        {
            if (!GetRecieveData(time)) return false;

            if (!RecieveData.Contains(data))
            {
                Ca100state = ErrorCode.ResponseErr;
                return false;
            }
            return true;
        }
        //**************************************************************************
        //ＣＡ１００からの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：bool
        //**************************************************************************
        private bool GetRecieveData(int time)
        {
            try
            {
                RecieveData = "";//念のため初期化
                port.ReadTimeout = time;
                RecieveData = port.ReadTo("\r\n");
                return true;
            }
            catch
            {
                Ca100state = ErrorCode.TimeoutErr;
                return false;
            }
        }

        //**************************************************************************
        //ステータスバイトの読み出し
        //引数：なし
        //戻値：bool
        //**************************************************************************
        private bool ReadStatusByte()
        {
            //ステータスバイトをクリアする
            if (!SendCommand((char)0x1B + "S")) return false;//ClearStatus

            //ステータスバイトを読み出す
            foreach (var i in Enumerable.Range(0, 100))//コマンド送信インターバルが50msecなのでMax5秒ステータスバイトを読み出す
            {
                Application.DoEvents();
                if (!SendCommand((char)0x1B + "S")) return false;//CheckStatus
                if (!Byte.TryParse(RecieveData, out statusByte)) continue;
                if ((statusByte & 0x08) == 0x08)
                {
                    Ca100state = ErrorCode.OverRange;
                    return false;
                }
                if ((statusByte & 0x01) == 1) return true;
            }

            Ca100state = ErrorCode.MeasErr;
            return false;
        }


        //****************************************************************************************
        //直流電圧をＯＮする
        //引数:レンジ、出力値
        //戻値:CA100からのエラーコード
        //****************************************************************************************
        public bool OutDcV(double outValue)
        {
            try
            {
                if (!StopSource()) return false;

                //ファンクションの切り替え & 出力電圧の設定
                if (!SendCommand("SF", "0")) return false;//DCV発生ファンクションに設定

                if (outValue >= 0 && outValue <= 0.11)
                {
                    if (!SendCommand("SR", "0")) return false;//100mVレンジに設定
                }
                else if (outValue > 0.11 && outValue <= 1.1)
                {
                    if (!SendCommand("SR", "1")) return false;//1Vレンジに設定
                }
                else if (outValue > 1.1 && outValue <= 11)
                {
                    if (!SendCommand("SR", "2")) return false;//10Vレンジに設定
                }
                else//0～11V以外の出力値は設定させない
                {
                    return false;
                }

                if (!SendCommand("SD", outValue.ToString("F3"))) return false;//出力値を設定
                return (SendCommand("SO", "1"));//出力開始  
            }
            catch
            {
                StopSource();
                return false;
            }

        }

        //****************************************************************************************
        //熱電対をＯＮする
        //引数:レンジ、出力値
        //戻値:CA100からのエラーコード
        //****************************************************************************************
        public bool OutTc_K(double outValue)
        {
            try
            {
                if (!StopSource()) return false;

                //ファンクションの切り替え & 出力電圧の設定
                if (!SendCommand("SF", "3")) return false;//（TC）熱電対発生ファンクションに設定
                if (!SendCommand("SR", "3")) return false;//K

                if (!SendCommand("SD", outValue.ToString("F1"))) return false;//出力値を設定

                return (SendCommand("SO", "1"));//出力開始
            }
            catch
            {
                StopSource();
                return false;
            }

        }
        //****************************************************************************************
        //ソース出力をＯＦＦする
        //引数:なし
        //戻値:bool
        //****************************************************************************************
        public bool StopSource()
        {
            return (SendCommand("SO", "0"));//出力停止
        }

    }

































}








