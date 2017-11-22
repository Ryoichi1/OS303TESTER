using System;
using System.Collections.Generic;
using System.Linq;

namespace Os303Tester
{
    public class TestSpecs
    {
        public int Key;
        public string Value;
        public bool PowSw;

        public TestSpecs(int key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    public static class State
    {
        public enum PC_NAME { PC1, PC2 }

        //データソース（バインディング用）
        public static ViewModelMainWindow VmMainWindow = new ViewModelMainWindow();
        public static ViewModelTestStatus VmTestStatus = new ViewModelTestStatus();
        public static ViewModelTestResult VmTestResults = new ViewModelTestResult();
        public static TestCommand testCommand = new TestCommand();
        public static ViewModelCameraPoint VmCameraPoint = new ViewModelCameraPoint();

        //パブリックメンバ
        public static Configuration Setting { get; set; }
        public static TestSpec TestSpec { get; set; }

        public static CamProperty camProp { get; set; }
        public static string CurrDir { get; set; }

        public static string AssemblyInfo { get; set; }

        public static double CurrentThemeOpacity { get; set; }

        public static Uri uriErrInfoPage { get; set; }


        //リトライ履歴保存用リスト
        public static List<string> RetryLogList = new List<string>();

        public static List<TestSpecs> テスト項目 = new List<TestSpecs>()
        {
            new TestSpecs(100, "コネクタ実装チェック"),

            new TestSpecs(200, "AC100Vチェック"),
            new TestSpecs(201, "電源電圧チェック Vdd"),
            new TestSpecs(202, "電源電圧チェック Vcc1"),

            new TestSpecs(300, "ソフト書き込み"),
        
            new TestSpecs(500, "電源電圧チェック Vcc2(RY1,RY2 OFF)"),

            new TestSpecs(600, "シーケンスチェック"),
        };

        //個別設定のロード
        public static void LoadConfigData()
        {
            //Configファイルのロード
            Setting = Deserialize<Configuration>(Constants.filePath_Configuration);
            if (Setting.日付 != DateTime.Now.ToString("yyyyMMdd"))
            {
                Setting.日付 = DateTime.Now.ToString("yyyyMMdd");
                Setting.TodayOkCount = 0;
                Setting.TodayNgCount = 0;
            }

            VmMainWindow.ListOperator = Setting.作業者リスト;
            VmMainWindow.Theme = Setting.PathTheme;
            VmMainWindow.ThemeOpacity = Setting.OpacityTheme;
            VmTestStatus.OkCount = Setting.TodayOkCount.ToString() + "台";
            VmTestStatus.NgCount = Setting.TodayNgCount.ToString() + "台";
            VmTestStatus.TotalCount = Setting.TotalTestCount.ToString() + "台";

            VmMainWindow.Opecode = Setting.CurrentOpecode;

            //TestSpecファイルのロード
            TestSpec = Deserialize<TestSpec>(Constants.filePath_TestSpec);//TODO:

            //カメラプロパティファイルのロード
            camProp = Deserialize<CamProperty>(Constants.filePath_CameraProperty);

        }


        //インスタンスをXMLデータに変換する
        public static bool Serialization<T>(T obj, string xmlFilePath)
        {
            try
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                //書き込むファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(xmlFilePath, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, obj);
                //ファイルを閉じる
                sw.Close();

                return true;

            }
            catch
            {
                return false;
            }

        }

        //XMLデータからインスタンスを生成する
        public static T Deserialize<T>(string xmlFilePath)
        {
            System.Xml.Serialization.XmlSerializer serializer;
            using (var sr = new System.IO.StreamReader(xmlFilePath, new System.Text.UTF8Encoding(false)))
            {
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        //********************************************************
        //個別設定データの保存
        //********************************************************
        public static bool Save個別データ()
        {
            try
            {
                //Configファイルの保存
                Setting.作業者リスト = VmMainWindow.ListOperator;
                Setting.PathTheme = VmMainWindow.Theme;
                Setting.OpacityTheme = VmMainWindow.ThemeOpacity;

                if (Flags.SetOpecode)
                {
                    Setting.CurrentOpecode = VmMainWindow.Opecode;
                }

                Serialization<Configuration>(Setting, Constants.filePath_Configuration);

                //Cam0プロパティの保存
                Serialization<CamProperty>(State.camProp, Constants.filePath_CameraProperty);

                return true;
            }
            catch
            {
                return false;

            }

        }


        public static void SetCamProp()
        {
            //cam0の設定
            General.cam.Brightness = camProp.Brightness;
            General.cam.Contrast = camProp.Contrast;
            General.cam.Hue = camProp.Hue;
            General.cam.Saturation = camProp.Saturation;
            General.cam.Sharpness = camProp.Sharpness;
            General.cam.Gamma = camProp.Gamma;
            General.cam.Gain = camProp.Gain;
            General.cam.Exposure = camProp.Exposure;
            General.cam.Wb = camProp.Whitebalance;
            General.cam.Theta = camProp.Theta;
            General.cam.BinLevel = camProp.BinLevel;

            General.cam.Opening = camProp.Opening;
            General.cam.openCnt = camProp.OpenCnt;
            General.cam.closeCnt = camProp.CloseCnt;


            General.cam.Opening = camProp.Opening;
            General.cam.openCnt = camProp.OpenCnt;
            General.cam.closeCnt = camProp.CloseCnt;

            State.VmCameraPoint.LED1 = camProp.LED1;
            State.VmCameraPoint.LED2 = camProp.LED2;
            State.VmCameraPoint.LED3 = camProp.LED3;
        }


    }

}
