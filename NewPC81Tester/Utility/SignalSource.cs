
namespace NewPC81Tester
{
    public interface SignalSource
    {
        //メソッドの定義

        bool Init();

        bool OutDcV(double outValue);

        bool OutTc_K(double outValue);

        bool StopSource();

        void ClosePort();

    }

}
