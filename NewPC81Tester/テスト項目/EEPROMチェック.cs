using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public static class EEPROMチェック
    {

        public static async Task<bool> CheckEeprom()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    if (!Target.SendData(Target.Port.Rs232C_1, "EEPCheck", Wait: 35000)) return false;//EEPROMチェックコマンド送信
                    return Target.RecieveData.Contains("EEPOK");
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Thread.Sleep(500);
                }
            });
        }

        public static async Task<bool> InitEeprom()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
                    if (!Target.SendData(Target.Port.Rs232C_1, "SetEEP", Wait: 15000)) return false;//EEPROMチェックコマンド送信
                    return Target.RecieveData.Contains("INIOK");
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Thread.Sleep(500);
                }
            });
        }

        public static async Task<bool> SetSerial(string 工番シリアル)
        {
            return await Task<bool>.Run(() =>
            {
                try
                { 
                    if (!Target.SendData(Target.Port.Rs232C_1, "SetSerial-" + 工番シリアル, Wait: 5000)) return false;//EEPROMチェックコマンド送信
                    return Target.RecieveData.Contains("WRITEOK");
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Thread.Sleep(500);
                }
            });
        }


    }
}
