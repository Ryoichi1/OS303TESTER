
namespace Os303Tester
{
    public static class Constants
    {
        //スタートボタンの表示
        public const string 開始 = "検査開始";
        public const string 停止 = "強制停止";
        public const string 確認 = "確認";

        //作業者へのメッセージ
        public const string MessOpecode = "工番を入力してください";
        public const string MessOperator = "作業者名を選択してください";
        public const string MessSet = "製品をセットして開閉レバーを下げてください";
        public const string MessRemove = "製品を取り外してください";

        public const string MessWait = "検査中！　しばらくお待ちください・・・";
        public const string MessCheckConnectMachine = "周辺機器の接続を確認してください！";

        public static readonly string filePath_TestSpec = @"C:\OS303\ConfigData\TestSpec.config";
        public static readonly string filePath_Configuration = @"C:\OS303\ConfigData\Configuration.config";
        public static readonly string filePath_CameraProperty = @"C:\OS303\ConfigData\CameraProperty.config";

        public static readonly string RwsPath_Product = @"C:\OS303\FW_WRITE_LIMITED\FW_WRITE_LIMITED.rws";
        //public static readonly string RwsPath_Product = @"C:\OS303\FW_WRITE\FW_WRITE.rws";

        //検査データフォルダのパス
        public static readonly string PassDataFolderPath = @"C:\OS303\検査データ\合格品データ\";
        public static readonly string FailDataFolderPath = @"C:\OS303\検査データ\不良品データ\";
        public static readonly string fileName_RetryLog = FailDataFolderPath + "リトライ履歴.txt";

        public static readonly string Path_Manual = @"C:\OS303\OS303検査仕様書.pdf";

        //Imageの透明度
        public const double OpacityMin = 0;
        public const double OpacityImgMin = 0;

        //リトライ回数
        public static readonly int RetryCount = 0;












    }
}
