using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public static class コネクタチェック
    {
        public enum NAME { CN1, CN201, CN202, CN203, CN204, PORT1, PORT2, PORT3, PORT4, AN1, AN2, AN3, AN4 }

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
            byte dataIC3 = 0;
            byte dataIC4 = 0;

            try
            {
                return await Task<bool>.Run(() =>
                {
                    try
                    {
                        InitList();


                        //IC3からの出力を取り込む
                        dataIC3 = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC3);

                        //IC4からの出力を取り込む
                        dataIC4 = TC74HC54x.GetP7Data(TC74HC54x.InputName.IC4);

                        General.ResetInputPort();
                        Thread.Sleep(200);

                        ListCnSpec.ForEach(l =>
                        {
                            bool re = false;
                            switch (l.name)
                            {
                                //IC3~5はTC74HC540 出力は反転します

                                //IC3からの取り込み
                                case NAME.CN1:
                                    re = (dataIC3 & 0x01) != 0;
                                    break;
                                case NAME.CN201:
                                    re = (dataIC3 & 0x02) != 0;
                                    break;
                                case NAME.CN202:
                                    re = (dataIC3 & 0x04) != 0;
                                    break;
                                case NAME.CN203:
                                    re = (dataIC3 & 0x08) != 0;
                                    break;
                                case NAME.CN204:
                                    re = (dataIC3 & 0x10) != 0;
                                    break;
                                case NAME.PORT1:
                                    re = (dataIC3 & 0x20) != 0;
                                    break;
                                case NAME.PORT2:
                                    re = (dataIC3 & 0x40) != 0;
                                    break;
                                case NAME.PORT3:
                                    re = (dataIC3 & 0x80) != 0;
                                    break;

                                //IC4からの取り込み
                                case NAME.PORT4:
                                    re = (dataIC4 & 0x01) != 0;
                                    break;
                                case NAME.AN1:
                                    re = (dataIC4 & 0x02) != 0;
                                    break;
                                case NAME.AN2:
                                    re = (dataIC4 & 0x04) != 0;
                                    break;
                                case NAME.AN3:
                                    re = (dataIC4 & 0x08) != 0;
                                    break;
                                case NAME.AN4:
                                    re = (dataIC4 & 0x10) != 0;
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
