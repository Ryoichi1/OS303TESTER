
namespace Os303Tester
{
    public interface Multimeter
    {
        //プロパティの定義
        double VoltData { get; }//計測した電圧値
        double CurrData { get; }//計測した電流値


        //メソッドの定義

        //イニシャライズ
        bool Init();

        //計測モード設定
        bool SetVoltDc();
        bool SetVoltAc();
        bool SetCurrDc();

        //計測
        bool Measure();
        
        //ポートを閉じる処理
        bool ClosePort();
        
    }

}
