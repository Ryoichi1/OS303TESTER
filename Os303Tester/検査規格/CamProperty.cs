

namespace Os303Tester
{
    public class CamProperty
    {
        //カメラナンバー
        public int CamNumber { get; set; }

        public int BinLevel { get; set; }
        public bool Opening { get; set; }//オープニング処理 or クロージング処理
        public int OpenCnt { get; set; }//クロージング処理時の拡張回数
        public int CloseCnt { get; set; }//クロージング処理時の収縮回数


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
        public string LED1 { get; set; }
        public string LED2 { get; set; }
        public string LED3 { get; set; }


    }
}
