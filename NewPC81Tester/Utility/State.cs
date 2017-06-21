using System;
using System.Collections.Generic;
using System.Linq;

namespace NewPC81Tester
{
    public class TestSpecs
    {
        public int Key;
        public string Value;
        public bool PowSw;

        public TestSpecs(int key, string value, bool powSW = true)
        {
            this.Key = key;
            this.Value = value;
            this.PowSw = powSW;

        }
    }

    public static class State
    {
        //データソース（バインディング用）
        public static ViewModelMainWindow VmMainWindow = new ViewModelMainWindow();
        public static ViewModelTestStatus VmTestStatus = new ViewModelTestStatus();
        public static ViewModelTestResult VmTestResults = new ViewModelTestResult();
        public static ViewModelCommunication VmComm = new ViewModelCommunication();
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

        public static string LastSerial { get; set; }



        //リトライ履歴保存用リスト
        public static List<string> RetryLogList = new List<string>();

        public static List<TestSpecs> テスト項目 = new List<TestSpecs>()
        {
            new TestSpecs(100, "コネクタ実装チェック", false),
            new TestSpecs(200, "検査ソフト書き込み", false),
            new TestSpecs(300, "消費電流チェック 24Vライン", false),
            new TestSpecs(400, "消費電流チェック 3Vライン", false),

            new TestSpecs(500, "電源電圧チェック Vcc", false),
            new TestSpecs(501, "電源電圧チェック -5V", false),
            new TestSpecs(502, "電源電圧チェック Vref", false),

            new TestSpecs(600, "RS422チェックPORT1", true),
            new TestSpecs(601, "RS422チェックPORT2", true),
            new TestSpecs(602, "RS422チェックPORT3", true),
            new TestSpecs(603, "RS422チェックPORT4", true),
            new TestSpecs(604, "RS422チェックCN202", true),

            new TestSpecs(700, "RTCセット", true),

            new TestSpecs(800, "LED点灯チェック", true),
            new TestSpecs(801, "LED消灯チェック", false),

            new TestSpecs(900, "リセットスイッチ動作チェック", true),

            new TestSpecs(1000, "リレー動作チェック", false),

            new TestSpecs(1100, "デジタル出力チェック", true),

            new TestSpecs(1200, "デジタル入力チェック", true),

            new TestSpecs(1300, "AN1 アナログ入力チェック", true),
            new TestSpecs(1301, "AN2 アナログ入力チェック", true),
            new TestSpecs(1302, "AN3 アナログ入力チェック", true),
            new TestSpecs(1303, "AN4 アナログ入力チェック", true),

            new TestSpecs(1400, "EEPROMェック", true),
            new TestSpecs(1401, "EEPROM初期化", true),
            new TestSpecs(1402, "シリアル№書き込み", true),

            new TestSpecs(1500, "熱電対 補正値取得", false),

            new TestSpecs(1600, "RTCチェック", true),

            new TestSpecs(1700, "RS232CチェックPORT1", true),
            new TestSpecs(1701, "RS232CチェックPORT2", true),
            new TestSpecs(1702, "RS232CチェックPORT3", true),
            new TestSpecs(1703, "RS232CチェックPORT4", true),

            new TestSpecs(1800, "S1 1番、3番ONチェック", true),
            new TestSpecs(1801, "S1 2番、4番ONチェック", false),
            new TestSpecs(1802, "S1 1番ONチェック", false),

            new TestSpecs(1900, "製品ソフト書き込み", false),

            new TestSpecs(2000, "リチウム電池セット", false),

            new TestSpecs(2100, "リチウム電池電圧チェック", false),

            new TestSpecs(2200, "D4両端電圧チェック", false),

            new TestSpecs(2300, "RTC最終設定", false),
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
            General.cam.Theta = camProp.Theta;
            General.cam.BinLevel = camProp.BinLevel;

            State.VmCameraPoint.TX1 = camProp.TX1;
            State.VmCameraPoint.TX2 = camProp.TX2;
            State.VmCameraPoint.TX3 = camProp.TX3;
            State.VmCameraPoint.TX4 = camProp.TX4;
            State.VmCameraPoint.TX5 = camProp.TX5;

            State.VmCameraPoint.RX1 = camProp.RX1;
            State.VmCameraPoint.RX2 = camProp.RX2;
            State.VmCameraPoint.RX3 = camProp.RX3;
            State.VmCameraPoint.RX4 = camProp.RX4;
            State.VmCameraPoint.RX5 = camProp.RX5;

            State.VmCameraPoint.CPU = camProp.CPU;
            State.VmCameraPoint.DEB = camProp.DEB;
            State.VmCameraPoint.VCC = camProp.VCC;
        }


        public static bool LoadLastSerial(string filePath)
        {
            try
            {
                // csvファイルを開く
                using (var sr = new System.IO.StreamReader(filePath))
                {
                    var listTestResults = new List<string>();
                    // ストリームの末尾まで繰り返す
                    while (!sr.EndOfStream)
                    {
                        // ファイルから一行読み込んでリストに追加
                        listTestResults.Add(sr.ReadLine());
                    }

                    var lastData = listTestResults.Last();
                    LastSerial = lastData.Split(',')[0];

                    var IsCorrectformat = (System.Text.RegularExpressions.Regex.IsMatch(
                        LastSerial, @"\d-41-\d\d\d\d-\d\d\d-\d\d\d\d$",
                        System.Text.RegularExpressions.RegexOptions.ECMAScript));

                    var IsCorrectOpecode = LastSerial.Contains(State.VmMainWindow.Opecode);

                    return IsCorrectformat && IsCorrectOpecode;
                }
            }
            catch
            {
                return false;
                // ファイルを開くのに失敗したとき
            }
        }

    }

}
