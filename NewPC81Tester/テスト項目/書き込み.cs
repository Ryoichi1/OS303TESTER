using System;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public static class 書き込み
    {
        public enum WriteMode { TEST, PRODUCT }

        public static async Task<bool> WriteFw(WriteMode mode)
        {
            try
            {
                string RfpPath = "";
                if (mode == WriteMode.TEST)
                {
                    RfpPath = Constants.RwsPath_Test;
                }
                else
                {
                    RfpPath = Constants.RwsPath_Product;
                }

                string Sum = (mode == WriteMode.PRODUCT) ? State.TestSpec.FwSum : "";
                bool calcSum = (mode == WriteMode.PRODUCT) ? true : false;  


                //試験機のK1～K4をONする処理
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, EPX64S.OUT.H);
                await Task.Delay(500);

                //電源ON
                General.PowSupply(true);
                await Task.Delay(500);

                return await FlashProgrammer.WriteFirmware(RfpPath, Sum, calcSum);

            }
            catch
            {
                return false;
            }
            finally
            {
                //電源OFF
                General.PowSupply(false);
                await Task.Delay(300);

                //試験機のK1～K4をOFFする処理
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, EPX64S.OUT.L);
            }


        }



    }
}
