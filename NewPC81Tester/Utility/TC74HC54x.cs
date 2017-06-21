using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public static class TC74HC54x
    {
        public enum InputName { IC1, IC2, IC3, IC4, IC5 }

        public static byte GetP7Data(InputName name)
        {

            //IC1～IC5の出力をZにする
            General.ResetInputPort();
            Thread.Sleep(50);

            switch (name)
            {
                case InputName.IC1:
                    General.io.OutBit(EPX64S.PORT.P6, EPX64S.BIT.b0, EPX64S.OUT.L);
                    break;

                case InputName.IC2:
                    General.io.OutBit(EPX64S.PORT.P6, EPX64S.BIT.b1, EPX64S.OUT.L);
                    break;

                case InputName.IC3:
                    General.io.OutBit(EPX64S.PORT.P6, EPX64S.BIT.b2, EPX64S.OUT.L);
                    break;

                case InputName.IC4:
                    General.io.OutBit(EPX64S.PORT.P6, EPX64S.BIT.b3, EPX64S.OUT.L);
                    break;

                case InputName.IC5:
                    General.io.OutBit(EPX64S.PORT.P6, EPX64S.BIT.b4, EPX64S.OUT.L);
                    break;

            }

            Thread.Sleep(50);

            General.io.ReadInputData(EPX64S.PORT.P7);
            return General.io.P7InputData;
        }

    }
}
