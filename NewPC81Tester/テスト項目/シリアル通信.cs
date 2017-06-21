using System.Threading;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public static class シリアル通信チェック
    {
        public static bool IsCommSet { get; set; }
        public enum CH { Port1, Port2, Port3, Port4, CN202 }
        public enum TEST_CASE_422 { CASE1, CASE2, CASE3 }

        public static async Task<bool> Check422(CH ch)
        {
            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        Target.Port port;
                        switch (ch)
                        {
                            case CH.Port1:
                            case CH.Port2:
                                port = Target.Port.Rs422_1;
                                break;

                            case CH.Port3:
                            case CH.Port4:
                            case CH.CN202:
                                port = Target.Port.Rs422_2;
                                break;
                            default:
                                port = Target.Port.Rs422_1;
                                break;
                        }

                        //通信経路の選択

                        //JP1～4 の5-6、7-8を短絡する処理
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.H);
                        Thread.Sleep(200);

                        //MOXA_1150A or MOXA_1150B の接続
                        switch (ch)
                        {
                            case CH.Port1:
                            case CH.Port3:
                                //K10,11,12,13をOFFする処理
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.L);
                                //K14,15をOFFする処理
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                                break;
                            case CH.Port2:
                            case CH.Port4:
                                //K10,11,12,13をONする処理
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.H);
                                //K14,15をOFFする処理
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                                break;
                            case CH.CN202:
                                //K14,15をONする処理
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.H);
                                break;

                            default:
                                break;
                        }

                        Thread.Sleep(100);
                        Target.ClearBuff();
                        ////電源ON
                        //General.PowSupply(true);
                        //if (!General.CheckComm()) return false;

                        var header = ch.ToString() + "@"; //Port1なら "Port1@"
                        if (!Target.SendData(port, "ComCheck")) return false;
                        if (!Target.RecieveData.Contains(header + "1234567890")) return false;
                        Thread.Sleep(400);
                        if (!Target.SendData(port, "ABCDEFGHIJ")) return false;
                        if (!Target.RecieveData.Contains(header + "KLMNOPQRST")) return false;

                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                });
            }
            finally
            {

            }
        }

        public static async Task<bool> Check232C(CH ch)
        {

            return await Task<bool>.Run(() =>
            {
                Target.ClearBuff();
                try
                {
                    Target.Port port;
                    switch (ch)
                    {
                        case CH.Port1:
                            port = Target.Port.Rs232C_1;
                            break;
                        case CH.Port2:
                            port = Target.Port.Rs232C_2;
                            break;
                        case CH.Port3:
                            port = Target.Port.Rs232C_3;
                            break;
                        case CH.Port4:
                            port = Target.Port.Rs232C_4;
                            break;
                        default:
                            port = Target.Port.Rs232C_1;
                            break;
                    }

                    var header = ch.ToString() + "@"; //Port1なら "Port1@"

                        if (!Target.SendData(port, "ComCheck")) return false;
                    if (!Target.RecieveData.Contains(header + "1234567890")) return false;
                    Thread.Sleep(500);
                    if (!Target.SendData(port, "ABCDEFGHIJ")) return false;
                    if (!Target.RecieveData.Contains(header + "KLMNOPQRST")) return false;

                    return true;
                }
                catch
                {
                    return false;
                }

            });

        }

    }
}
