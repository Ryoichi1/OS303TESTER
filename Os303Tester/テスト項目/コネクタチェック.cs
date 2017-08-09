using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Os303Tester
{
    public static class コネクタチェック
    {
        public enum NAME { CN1, CN2, CN3, CN4, CN5, CN7, CN8  }

        public static byte P7Data;

        public static List<CnSpec> ListCnSpec;

        public class CnSpec
        {
            public NAME name;
            public bool result;
        }



        private static void InitList()
        {
            ListCnSpec = new List<CnSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListCnSpec.Add(new CnSpec { name = (NAME)n });
            }
        }


        public static async Task<bool> CheckCn()
        {
            bool result = false;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        InitList();

                        //IC3からの出力を取り込む
                        if (!General.io.ReadInputData(EPX64S.PORT.P7)) return false;

                        var p7Data = General.io.P7InputData;

                        ListCnSpec.ForEach(l =>
                        {
                            bool re = false;
                            switch (l.name)
                            {
                                case NAME.CN1:
                                    re = (P7Data & 0x01) == 0;
                                    break;
                                case NAME.CN2:
                                    re = (p7Data & 0x02) == 0;
                                    break;
                                case NAME.CN3:
                                    re = (p7Data & 0x04) == 0;
                                    break;
                                case NAME.CN4:
                                    re = (p7Data & 0x08) == 0;
                                    break;
                                case NAME.CN5:
                                    re = (p7Data & 0x10) == 0;
                                    break;
                                case NAME.CN7:
                                    re = (p7Data & 0x20) == 0;
                                    break;
                                case NAME.CN8:
                                    re = (p7Data & 0x40) == 0;
                                    break;
                            }

                            l.result = re;
                        });

                        return result = ListCnSpec.All(l => l.result);

                    }
                    catch
                    {
                        return result = false;
                    }

                });
            }
            finally
            {
                if (!result)
                {
                    State.uriErrInfoPage = new Uri("Page/ErrInfo/ErrInfoコネクタチェック.xaml", UriKind.Relative);
                    State.VmTestStatus.EnableButtonErrInfo = System.Windows.Visibility.Visible;
                }
            }
        }

















    }
}
