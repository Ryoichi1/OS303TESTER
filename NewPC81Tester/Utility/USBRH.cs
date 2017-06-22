using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NewPC81Tester
{
    public class USBRH
    {
        // Function definitions
        [DllImport("USBMeter.dll")]
        public static extern IntPtr FindUSB(ref int index);//戻り値をstring →　IntPtrに変更

        [DllImport("USBMeter.dll")]
        public static extern string GetVers(string dev);

        [DllImport("USBMeter.dll")]
        public static extern int GetTempHumid(string dev, ref double temp, ref double humid);

        [DllImport("USBMeter.dll")]
        public static extern int ControlIO(string dev, int port, int val);

        [DllImport("USBMeter.dll")]
        public static extern int SetHeater(string dev, int val);

        [DllImport("USBMeter.dll")]
        public static extern int GetTempHumidTrue(IntPtr dev, ref double temp, ref double humid);//devの型をstring →　IntPtrに変更

        //インスタンス変数

        public IntPtr device;//デバイス名
        private bool FlagStop;
        private bool FlagMeasuring;

        //private int led1;
        //private int led2;
        //private int heater;
        //private int idx = 0;

        private double? _temp = null;
        private double? _humid = null;

        public int idx = 0;

        public double? temp
        {
            get
            {
                return _temp;
            }

            private set
            {
                _temp = value;
                State.VmTestStatus.Temp = (_temp != null) ? ((double)_temp).ToString("F2") + "℃" : "---℃";

            }
        }

        public double? humid
        {
            get
            {
                return _humid;
            }

            private set
            {
                _humid = value;

            }
        }

        //インスタンスコンストラクタ
        public USBRH()
        {
            State.VmTestStatus.Temp = "----℃";
        }

        public bool Init()
        {
            this.device = FindUSB(ref idx);

            double temp初期値 = 999;
            double humid初期値 = 999;
            long d = 0;

            d = GetTempHumidTrue(device, ref temp初期値, ref humid初期値);

            if (d == 0)
            {
                return true;
            }
            else
            {
                State.VmTestStatus.Temp = "----℃";
                return false;
            }
            //return (temp初期値 != 999 && humid初期値 != 999);

        }


        public void MeasTemp()
        {
            FlagStop = false;
            FlagMeasuring = true;
            var t = Task.Run(() =>
            {
                while (!FlagStop)
                {
                    Thread.Sleep(1000);
                    double temp1 = 0;
                    double humid1 = 0;
                    long d = 0;

                    d = GetTempHumidTrue(device, ref temp1, ref humid1);
                    temp = temp1;
                    _humid = humid1;

                }

            });
            FlagMeasuring = false;
        }

        private void Stop()
        {
            FlagStop = false;
        }

        public void Close()
        {
            Stop();
            while (FlagMeasuring) { }

        }

    }
}
