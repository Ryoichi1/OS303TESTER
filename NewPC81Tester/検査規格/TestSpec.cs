
namespace NewPC81Tester
{
    public class TestSpec
    {
        public string TestSpecVer { get; set; }

        public string FwVer { get; set; }
        public string FwSum { get; set; }

        public string FwFileName { get; set; }
        public string TestFwFileName { get; set; }


        public double vccMax { get; set; }
        public double vccMin { get; set; }

        public double minus5vMax { get; set; }
        public double minus5vMin { get; set; }

        public double vRefMax { get; set; }
        public double vRefMin { get; set; }

        public double amp24vMax { get; set; }
        public double amp24vMin { get; set; }

        public double amp3vMax { get; set; }
        public double amp3vMin { get; set; }

        public double an1min_Max { get; set; }
        public double an1min_Min { get; set; }

        public double an1mid_Max { get; set; }
        public double an1mid_Min { get; set; }

        public double an1max_Max { get; set; }
        public double an1max_Min { get; set; }

        public double an2min_Max { get; set; }
        public double an2min_Min { get; set; }

        public double an2mid_Max { get; set; }
        public double an2mid_Min { get; set; }

        public double an2max_Max { get; set; }
        public double an2max_Min { get; set; }

        public double ErrTemp_UsbRh_U6 { get; set; }//USB温度計とU6温度補償ICの表示温度の差異

        public double temp1Max { get; set; }
        public double temp1Min { get; set; }

        public double temp2Max { get; set; }
        public double temp2Min { get; set; }

        public double temp3Max { get; set; }
        public double temp3Min { get; set; }


        public double batteryMax { get; set; }
        public double batteryMin { get; set; }
        public double d4Max { get; set; }
        public double d4Min { get; set; }
        public double coilCurrentMax { get; set; }
        public double coilCurrentMin { get; set; }
        public double ledOnPercentage { get; set; }

        public double rtcTimeErr { get; set; }

    }
}
