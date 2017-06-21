

namespace NewPC81Tester
{
    public class CamProperty
    {
        //カメラナンバー
        public int CamNumber { get; set; }

        public int BinLevel { get; set; }

        //カメラプロパティ
        public double Brightness { get; set; }
        public double Contrast { get; set; }
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Sharpness { get; set; }
        public double Gamma { get; set; }
        public double Gain { get; set; }
        public double Exposure { get; set; }
        public double Theta { get; set; }

        //LEDの座標
        public string TX1 { get; set; }
        public string TX2 { get; set; }
        public string TX3 { get; set; }
        public string TX4 { get; set; }
        public string TX5 { get; set; }
        public string RX1 { get; set; }
        public string RX2 { get; set; }
        public string RX3 { get; set; }
        public string RX4 { get; set; }
        public string RX5 { get; set; }
        public string CPU { get; set; }
        public string DEB { get; set; }
        public string VCC { get; set; }

    }
}
