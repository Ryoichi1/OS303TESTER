using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public static class スイッチチェック
    {
        public enum S1検査 { ON_13, ON_24, ON_1 }

        public static async Task<bool> CheckS1(S1検査 s1)
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    if (!Target.SendData(Target.Port.Rs232C_1, "ReadS1")) return false;   //SendRs422Data("ReadS1");//S1状態読み出しコマンド

                    if (!Target.RecieveData.Contains("S1")) return false;
                    byte s1Data = Convert.ToByte(Target.RecieveData.Substring(2), 16);

                    switch (s1)
                    {
                        case S1検査.ON_13://1,3on 2,4offの確認
                            return (s1Data == 0x0A);
                        case S1検査.ON_24://1,3off 2,4onの確認
                            return (s1Data == 0x05);
                        case S1検査.ON_1://1on　2,3,4offの確認
                            return (s1Data == 0x0E);
                        default:
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
