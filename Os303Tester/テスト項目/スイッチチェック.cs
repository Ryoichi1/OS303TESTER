using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Os303Tester
{
    public static class スイッチチェック
    {

        public static async Task<bool> CheckS1()
        {
            return await Task<bool>.Run(() =>
            {
                try
                {
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
