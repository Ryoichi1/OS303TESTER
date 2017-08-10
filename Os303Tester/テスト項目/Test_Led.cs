using OpenCvSharp;
//using OpenCvSharp.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Os303Tester
{

    public static class Test_Led
    {
        public enum NAME { LED1, LED2, LED3 }

        const int WIDTH = 640;
        const int HEIGHT = 360;
        public const int TEST_FRAME = 1600;
        //public const int TEST_FRAME_COLOR = 400;

        public static double H_Ave = 0;
        public static double S_Ave = 0;
        public static double V_Ave = 0;

        private static IplImage source = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);

        static readonly double LedOkCount = State.TestSpec.LedOnCount;//（白ドットの数 / チェックした総ドット数）*100がLedOn閾値以上だったら合格とする

        public static List<LedSpec> ListLedSpec;


        public class LedSpec
        {
            public NAME name;
            public int x;
            public int y;
            public double OnCount;
            public bool result;
        }

        private static void InitList()
        {
            ListLedSpec = new List<LedSpec>();
            foreach (var n in Enum.GetValues(typeof(NAME)))
            {
                ListLedSpec.Add(new LedSpec { name = (NAME)n });
            }

            ListLedSpec.ForEach(l =>
            {
                switch (l.name)
                {
                    case NAME.LED1:
                        l.x = Int32.Parse(State.camProp.LED1.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.LED1.Split('/').ToArray()[1]);
                        break;

                    case NAME.LED2:
                        l.x = Int32.Parse(State.camProp.LED2.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.LED2.Split('/').ToArray()[1]);
                        break;

                    case NAME.LED3:
                        l.x = Int32.Parse(State.camProp.LED3.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.LED3.Split('/').ToArray()[1]);
                        break;
                }
            });


        }

        public static bool CheckColor(NAME name)
        {
            State.SetCamProp();
            General.cam.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
            //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

            int X = 0;
            int Y = 0;
            int HueMax = 0;
            int HueMin = 0;



            var ListH = new List<int>();
            var ListS = new List<int>();
            var ListV = new List<int>();

            int side = (int)Math.Sqrt(TEST_FRAME);//検査枠の１辺の長さ
            try
            {
                switch (name)
                {
                    case NAME.LED1:
                        X = Int32.Parse(State.camProp.LED1.Split('/').ToArray()[0]);
                        Y = Int32.Parse(State.camProp.LED1.Split('/').ToArray()[1]);
                        HueMax = State.TestSpec.OrangeHueMax;
                        HueMin = State.TestSpec.OrangeHueMin;
                        break;

                    case NAME.LED2:
                        X = Int32.Parse(State.camProp.LED2.Split('/').ToArray()[0]);
                        Y = Int32.Parse(State.camProp.LED2.Split('/').ToArray()[1]);
                        HueMax = State.TestSpec.GreenHueMax;
                        HueMin = State.TestSpec.GreenHueMin;
                        break;

                    case NAME.LED3:
                        X = Int32.Parse(State.camProp.LED3.Split('/').ToArray()[0]);
                        Y = Int32.Parse(State.camProp.LED3.Split('/').ToArray()[1]);
                        HueMax = State.TestSpec.RedHueMax;
                        HueMin = State.TestSpec.RedHueMin;
                        break;
                }

                //cam0の画像を取得する処理
                General.cam.FlagTestPic = true;
                while (General.cam.FlagTestPic)
                {

                }

                source = General.cam.imageForTest;

                //デバッグ用コード（下記コメントを外すと画像を保存します）
                //source.SaveImage(@"C:\OS303\ColorPic.bmp");

                using (IplImage hsv = new IplImage(640, 360, BitDepth.U8, 3)) // グレースケール画像格納用の変数
                {
                    //RGBからHSVに変換
                    Cv.CvtColor(source, hsv, ColorConversion.BgrToHsv);

                    OpenCvSharp.CPlusPlus.Mat mat = new OpenCvSharp.CPlusPlus.Mat(hsv, true);


                    foreach (var i in Enumerable.Range(0, side))
                    {
                        foreach (var j in Enumerable.Range(0, side))
                        {
                            var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>((int)Y - (side / 2) + i, (int)X - (side / 2) + j);
                            if (re[1] == 255 && re[2] == 255)
                            {
                                ListH.Add(re[0]);
                            }
                        }
                    }

                    H_Ave = ListH.Average();
                    return H_Ave > HueMin && H_Ave < HueMax;

                }


            }
            finally
            {

                string hsvValue = H_Ave.ToString("F0");

                ColorHSV hsv = new ColorHSV((float)Test_Led.H_Ave / 180, 1, 1);
                var rgb = ColorConv.HSV2RGB(hsv);
                var color = new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B));
                color.Freeze();//これ重要！！！  

                switch (name)
                {
                    case NAME.LED1:
                        State.VmTestResults.HueLED1 = hsvValue;
                        State.VmTestResults.ColorLED1 = color;
                        break;
                    case NAME.LED2:
                        State.VmTestResults.HueLED2 = hsvValue;
                        State.VmTestResults.ColorLED2 = color;
                        break;
                    case NAME.LED3:
                        State.VmTestResults.HueLED3 = hsvValue;
                        State.VmTestResults.ColorLED3 = color;
                        break;
                }
            }

        }


        public static void CheckLed()//引数には点灯するLEDを指定する
        {
            InitList();
            State.SetCamProp();
            General.cam.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                    //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

            int side = (int)Math.Sqrt(TEST_FRAME);//検査枠の１辺の長さ

            try
            {
                //cam0の画像を取得する処理
                General.cam.FlagTestPic = true;
                while (General.cam.FlagTestPic)
                {

                }

                source = General.cam.imageForTest;

                using (IplImage gray = Cv.CreateImage(new CvSize(WIDTH, HEIGHT), BitDepth.U8, 1)) // グレースケール画像格納用の変数
                {
                    Cv.CvtColor(source, gray, ColorConversion.BgrToGray);   // グレースケール変換
                    Cv.Threshold(gray, gray, State.camProp.BinLevel, 255, ThresholdType.Binary);   // グレースケール画像を2値化
                                                                                                   //ノイズ除去
                    if (State.camProp.Opening)
                    {
                        Cv.Erode(gray, gray, null, State.camProp.CloseCnt);//収縮処理 
                        Cv.Dilate(gray, gray, null, State.camProp.OpenCnt);//膨張処理 
                    }
                    {
                        Cv.Dilate(gray, gray, null, State.camProp.OpenCnt);//膨張処理 
                        Cv.Erode(gray, gray, null, State.camProp.CloseCnt);//収縮処理 
                    }


                    //デバッグ用コード（下記コメントを外すと画像を保存します）
                    //gray.SaveImage(@"C:\OS303\BinPic.bmp");

                    var mat = new OpenCvSharp.CPlusPlus.Mat(gray, true);

                    ListLedSpec.ForEach(l =>
                    {
                        int onCount = 0;
                        foreach (var i in Enumerable.Range(0, side))
                        {
                            foreach (var j in Enumerable.Range(0, side))
                            {
                                var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>((int)l.y - (side / 2) + i, (int)l.x - (side / 2) + j);
                                //点灯確認
                                if (re[0] == 255) //HSV=0,0,255 => 白
                                {
                                    onCount++;
                                }
                            }
                        }

                        l.OnCount = onCount;

                        //ビューモデル更新
                        switch (l.name)
                        {
                            case NAME.LED1:
                                State.VmTestResults.LED1Value = l.OnCount.ToString("F0");
                                break;

                            case NAME.LED2:
                                State.VmTestResults.LED2Value = l.OnCount.ToString("F0");
                                break;

                            case NAME.LED3:
                                State.VmTestResults.LED3Value = l.OnCount.ToString("F0");
                                break;
                        }

                    });

                }
            }
            catch
            {

            }
            finally
            {
                General.cam.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
            }
        }


    }

}









