using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace NewPC81Tester
{

    public static class Test_Led
    {
        public enum NAME
        {
            TX1, TX2, TX3, TX4, TX5,
            RX1, RX2, RX3, RX4, RX5,
            CPU, DEB, VCC
        }

        const int WIDTH = 640;
        const int HEIGHT = 360;
        const int TEST_FRAME = 49;

        private static IplImage source = new IplImage(WIDTH, HEIGHT, BitDepth.U8, 3);

        static readonly double LedOkPercentage = State.TestSpec.ledOnPercentage;//（白ドットの数 / チェックした総ドット数）*100がLedOn閾値以上だったら合格とする

        public static List<LedSpec> ListLedSpec;

        public class LedSpec
        {
            public NAME name;
            public int x;
            public int y;
            public double OnOffPercentage;
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
                    case NAME.CPU:
                        l.x = Int32.Parse(State.camProp.CPU.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.CPU.Split('/').ToArray()[1]);
                        break;

                    case NAME.DEB:
                        l.x = Int32.Parse(State.camProp.DEB.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.DEB.Split('/').ToArray()[1]);
                        break;

                    case NAME.VCC:
                        l.x = Int32.Parse(State.camProp.VCC.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.VCC.Split('/').ToArray()[1]);
                        break;

                    case NAME.TX1:
                        l.x = Int32.Parse(State.camProp.TX1.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.TX1.Split('/').ToArray()[1]);
                        break;

                    case NAME.TX2:
                        l.x = Int32.Parse(State.camProp.TX2.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.TX2.Split('/').ToArray()[1]);
                        break;

                    case NAME.TX3:
                        l.x = Int32.Parse(State.camProp.TX3.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.TX3.Split('/').ToArray()[1]);
                        break;

                    case NAME.TX4:
                        l.x = Int32.Parse(State.camProp.TX4.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.TX4.Split('/').ToArray()[1]);
                        break;

                    case NAME.TX5:
                        l.x = Int32.Parse(State.camProp.TX5.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.TX5.Split('/').ToArray()[1]);
                        break;

                    case NAME.RX1:
                        l.x = Int32.Parse(State.camProp.RX1.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.RX1.Split('/').ToArray()[1]);
                        break;

                    case NAME.RX2:
                        l.x = Int32.Parse(State.camProp.RX2.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.RX2.Split('/').ToArray()[1]);
                        break;

                    case NAME.RX3:
                        l.x = Int32.Parse(State.camProp.RX3.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.RX3.Split('/').ToArray()[1]);
                        break;

                    case NAME.RX4:
                        l.x = Int32.Parse(State.camProp.RX4.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.RX4.Split('/').ToArray()[1]);
                        break;

                    case NAME.RX5:
                        l.x = Int32.Parse(State.camProp.RX5.Split('/').ToArray()[0]);
                        l.y = Int32.Parse(State.camProp.RX5.Split('/').ToArray()[1]);
                        break;
                }
            });


        }

        public static async Task<bool> CheckLed(bool sw)
        {
            General.cam.ImageOpacity = 1.0;
            bool result = false;
            InitList();
            State.SetCamProp();
            General.cam.ResetFlag();//カメラのフラグを初期化 リトライ時にフラグが初期化できてないとだめ
                                    //例 ＮＧリトライ時は、General.cam.FlagFrame = trueになっていてNGフレーム表示の無限ループにいる

            int side = (int)Math.Sqrt(TEST_FRAME);//検査枠の１辺の長さ

            try
            {
                return result = await Task<bool>.Run(() =>
                {
                    try
                    {
                        //LEDを全点灯/消灯 させる処理
                        if (sw)
                        {
                            Target.SendData(Target.Port.Rs232C_1, "LEDCheck", doAnalysisData: false);
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }

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
                            Cv.Erode(gray, gray, null, 2);//収縮処理３回　ノイズ除去 
                            Cv.Dilate(gray, gray, null, 2);//拡張処理2回　ノイズ除去 

                            //デバッグ用コード（下記コメントを外すと画像を保存します）
                            //gray.SaveImage(@"C:\Users\TSDP00059\Desktop\BinPic.bmp");

                            var mat = new OpenCvSharp.CPlusPlus.Mat(gray, true);

                            ListLedSpec.ForEach(l =>
                            {
                                int okCount = 0;
                                foreach (var i in Enumerable.Range(0, side))
                                {
                                    foreach (var j in Enumerable.Range(0, side))
                                    {
                                        var re = mat.At<OpenCvSharp.CPlusPlus.Vec3b>((int)l.y - (side / 2) + i, (int)l.x - (side / 2) + j);
                                        if (sw)
                                        {//点灯確認
                                            if (re[0] == 255) //HSV=0,0,255 => 白
                                            {
                                                okCount++;
                                            }
                                        }
                                        else
                                        {//消灯確認
                                            if (re[0] == 0) //HSV=0,0,0 => 黒
                                            {
                                                okCount++;
                                            }
                                        }
                                    }
                                }

                                l.OnOffPercentage = ((double)okCount / TEST_FRAME) * 100;
                                if (sw)
                                {
                                    l.result = l.OnOffPercentage >= State.TestSpec.ledOnPercentage;
                                }
                                else
                                {
                                    l.result = l.OnOffPercentage == 100.0;
                                }
                            });

                            return ListLedSpec.All(l => l.result);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            finally
            {
                if (!result)
                {
                    General.cam.MakeFrame = (img) =>
                    {
                        //リストからNGの座標を抽出する
                        var NgList = ListLedSpec.Where(l => !l.result).ToList();
                        NgList.ForEach(n =>
                        {
                            img.Rectangle(new CvRect(n.x - (side / 2), n.y - (side / 2), side, side), CvColor.Red, 2);
                        });
                    };
                    General.cam.FlagFrame = true;
                }

                General.PowSupply(false);
                General.cam.ImageOpacity = Constants.OpacityImgMin;
            }
        }
    }

}









