
namespace NewPC81Tester
{
    public static class Constants
    {
                //スタートボタンの表示
        public const string 開始 = "検査開始";
        public const string 停止 = "強制停止";
        public const string 確認= "確認";

        //作業者へのメッセージ
        public const string MessOpecode = "工番を入力してください";
        public const string MessOperator = "作業者名を選択してください";
        public const string MessSet = "製品をセットして開始ボタンを押して下さい";
        public const string MessRemove = "製品を取り外してください";

        public const string MessWait = "検査中！　しばらくお待ちください・・・";
        public const string MessCheckConnectMachine = "周辺機器の接続を確認してください！";

        public static readonly string filePath_TestSpec = @"ConfigData\TestSpec.config";

        public static readonly string filePath_Configuration = @"C:\PC-81\ConfigData\Configuration.config";
        public static readonly string filePath_CameraProperty = @"C:\PC-81\ConfigData\CameraProperty.config";

        public static readonly string RwsPath_Test    = @"C:\PC-81\FW_WRITE\ForTest\PC81WRITE\PC81WRITE.rws";
        public static readonly string RwsPath_Product = @"C:\PC-81\FW_WRITE\ForProduct\PC81WRITE\PC81WRITE.rws";

        //検査データフォルダのパス
        public static readonly string PassDataFolderPath = @"C:\PC-81\検査データ\合格品データ\";
        public static readonly string FailDataFolderPath = @"C:\PC-81\検査データ\不良品データ\";
        public static readonly string fileName_RetryLog = FailDataFolderPath + "リトライ履歴.txt";


        //Imageの透明度
        public const double OpacityMax = 1.0;
        public const double OpacityMin = 0.16;
        public const double OpacityImgMin = 0.3;

        //リトライ回数
        public static readonly int RetryCount = 1;












    }
}
