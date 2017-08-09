using System;
using System.Threading;
using System.Threading.Tasks;

namespace Os303Tester
{
    public static class 書き込み
    {
        public static async Task<bool> WriteFw()
        {
            try
            {
                General.ResetIo();
                await Task.Delay(500);

                //試験機のK10,K11をONする処理
                General.SetK10_11(true);
                await Task.Delay(500);

                //電源ON
                General.PowSupply(true);
                await Task.Delay(500);

                return await FlashProgrammer.WriteFirmware(Constants.RwsPath_Product, State.TestSpec.FwSum);

            }
            catch
            {
                return false;
            }
            finally
            {
                //電源OFF
                await Task.Delay(300);
                General.PowSupply(false);
                await Task.Delay(300);
                //試験機のK1～K4をOFFする処理
                General.SetK10_11(false);
            }
        }
    }
}
