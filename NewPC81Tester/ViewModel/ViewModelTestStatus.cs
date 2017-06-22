using Microsoft.Practices.Prism.Mvvm;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace NewPC81Tester
{
    public class ViewModelTestStatus : BindableBase
    {

        //スタートボタンのプロパティ/////////////////////////////////////////////////////////////////////////////////////

        private bool _StartButtonEnable;
        public bool StartButtonEnable
        {

            get { return _StartButtonEnable; }
            set { SetProperty(ref _StartButtonEnable, value); }

        }





        private string _StartButtonContent;
        public string StartButtonContent
        {
            get { return _StartButtonContent; }
            set { SetProperty(ref _StartButtonContent, value); }
        }

        private string _FwVer;
        public string FwVer
        {
            get { return _FwVer; }
            set { SetProperty(ref _FwVer, value); }
        }


        private string _FwSum;
        public string FwSum
        {
            get { return _FwSum; }
            set { SetProperty(ref _FwSum, value); }
        }




        //テスト設定パネルのプロパティ（FW書き込みパス、単体試験選択）■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        //試験中はテスト設定パネルを操作できないようにするためのプロパティ
        private bool _TestSettingEnable = true;
        public bool TestSettingEnable
        {

            get { return _TestSettingEnable; }
            set { SetProperty(ref _TestSettingEnable, value); }
        }

        //FW書き込みパス チェックボックスがチェックされているかどうかの判定
        private bool? _CheckWriteFw = false;
        public bool? CheckWriteFw
        {
            get { return _CheckWriteFw; }
            set { SetProperty(ref _CheckWriteFw, value); }
        }


        //単体試験チェックボックスとコンボボックスの可視切り替え
        //これ重要！！！ 
        //EnableUnitTestをhiddenにした時点で、CheckUnitTestは必ずfalseになる
        //畔上以外の作業者を選択時は、EnableUnitTestがhiddenになるため、
        //絶対に一項目試験はできなくなり、通しで試験をするようになる

        private Visibility _EnableUnitTest;
        public Visibility EnableUnitTest
        {
            get { return _EnableUnitTest; }

            set
            {
                SetProperty(ref _EnableUnitTest, value);
            }
        }

        //単体試験チェックボックスがチェックされているかどうかの判定
        private bool? _CheckUnitTest = false;
        public bool? CheckUnitTest
        {
            get { return _CheckUnitTest; }
            set { SetProperty(ref _CheckUnitTest, value); }
        }

        //単体試験コンボボックスのアイテムソース
        private List<string> _UnitTestItems;
        public List<string> UnitTestItems
        {

            get { return _UnitTestItems; }
            set { SetProperty(ref _UnitTestItems, value); }

        }

        //単体試験コンボボックスの選択されたアイテム
        private string _UnitTestName;
        public string UnitTestName
        {

            get { return _UnitTestName; }
            set { SetProperty(ref _UnitTestName, value); }

        }


        //判定表示のプロパティ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        //判定表示　PASS or FAIL
        private string _Decision;
        public string Decision
        {
            get { return _Decision; }
            set { SetProperty(ref _Decision, value); }
        }

        //label判定Color
        private Brush _Colorlabel判定;
        public Brush Colorlabel判定
        {
            get { return _Colorlabel判定; }
            set { SetProperty(ref _Colorlabel判定, value); }
        }

        //FAIL判定時に表示するエラー情報
        private string _FailInfo;
        public string FailInfo
        {
            get { return _FailInfo; }
            set { SetProperty(ref _FailInfo, value); }
        }

        //FAIL判定時に表示する試験スペック
        private string _Spec;
        public string Spec
        {
            get { return _Spec; }
            set { SetProperty(ref _Spec, value); }
        }

        //FAIL判定時に表示する計測値
        private string _MeasValue;
        public string MeasValue
        {
            get { return _MeasValue; }
            set { SetProperty(ref _MeasValue, value); }
        }

        //判定表示の可視性
        private Visibility _DecisionVisibility;
        public Visibility DecisionVisibility
        {
            get { return _DecisionVisibility; }
            set { SetProperty(ref _DecisionVisibility, value); }
        }

        //規格値、計測値の可視性
        private Visibility _ErrInfoVisibility;
        public Visibility ErrInfoVisibility
        {
            get { return _ErrInfoVisibility; }
            set { SetProperty(ref _ErrInfoVisibility, value); }
        }

        //エラー詳細表示ボタンの可視切り替え
        private Visibility _EnableButtonErrInfo = Visibility.Hidden;
        public Visibility EnableButtonErrInfo
        {
            get { return _EnableButtonErrInfo; }

            set
            {
                SetProperty(ref _EnableButtonErrInfo, value);
            }
        }


        //判定表示の色
        private DropShadowEffect _ColorDecision;
        public DropShadowEffect ColorDecision
        {
            get { return _ColorDecision; }
            set { SetProperty(ref _ColorDecision, value); }
        }



        //ステータス表示部のプロパティ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        private string _OkCount;
        public string OkCount
        {
            get { return _OkCount; }
            set { SetProperty(ref _OkCount, value); }
        }

        private string _NgCount;
        public string NgCount
        {
            get { return _NgCount; }
            set { SetProperty(ref _NgCount, value); }
        }

        private string _TotalCount;
        public string TotalCount
        {
            get { return _TotalCount; }
            set { SetProperty(ref _TotalCount, value); }
        }



        //プログレスリングのプロパティ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        //プログレスリングのEndAngle
        private int _進捗度;
        public int 進捗度
        {
            get { return _進捗度; }
            set { SetProperty(ref _進捗度, value); }
        }

        //プログレスリングの可視性
        private Visibility _RingVisibility;
        public Visibility RingVisibility
        {
            get { return _RingVisibility; }
            set { SetProperty(ref _RingVisibility, value); }
        }


        //テストログ系のプロパティ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        //テスト時間
        private string _TestTime;
        public string TestTime
        {
            get { return _TestTime; }
            set { SetProperty(ref _TestTime, value); }
        }

        //作業者へのメッセージ
        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

        private string _TestLog;
        public string TestLog
        {
            get { return _TestLog; }
            set { SetProperty(ref _TestLog, value); }
        }

        //EEPROMチェック、熱電対補正値取得時のリング表示
        private bool _IsActiveRing;
        public bool IsActiveRing { get { return _IsActiveRing; } set { SetProperty(ref _IsActiveRing, value); } }



        //ステータスパネルのプロパティ///////////////////////////////////////////////////////////////////////////////
        //現在温度
        private string _Temp;
        public string Temp { get { return _Temp; } set { SetProperty(ref _Temp, value); } }

        private Brush _ColorRetry;
        public Brush ColorRetry
        {
            get { return _ColorRetry; }
            set { SetProperty(ref _ColorRetry, value); }
        }


        private Brush _ColorK100;
        public Brush ColorK100
        {
            get { return _ColorK100; }
            set { SetProperty(ref _ColorK100, value); }
        }

        private Brush _ColorK101;
        public Brush ColorK101
        {
            get { return _ColorK101; }
            set { SetProperty(ref _ColorK101, value); }
        }

        private Brush _ColorEPX64S;
        public Brush ColorEpx64s
        {
            get { return _ColorEPX64S; }
            set { SetProperty(ref _ColorEPX64S, value); }
        }

        private Brush _ColorHIOKI7012;
        public Brush ColorHIOKI7012
        {
            get { return _ColorHIOKI7012; }
            set { SetProperty(ref _ColorHIOKI7012, value); }
        }

        private Brush _ColorVOAC7602;
        public Brush ColorVOAC7602
        {
            get { return _ColorVOAC7602; }
            set { SetProperty(ref _ColorVOAC7602, value); }
        }

        private Brush _ColorCAMERA;
        public Brush ColorCAMERA
        {
            get { return _ColorCAMERA; }
            set { SetProperty(ref _ColorCAMERA, value); }
        }

        private Brush _Color温度計;
        public Brush Color温度計
        {
            get { return _Color温度計; }
            set { SetProperty(ref _Color温度計, value); }
        }

        private Brush _ColorLTM2882A;
        public Brush ColorLTM2882A
        {
            get { return _ColorLTM2882A; }
            set { SetProperty(ref _ColorLTM2882A, value); }
        }

        private Brush _ColorLTM2882B;
        public Brush ColorLTM2882B
        {
            get { return _ColorLTM2882B; }
            set { SetProperty(ref _ColorLTM2882B, value); }
        }

        private Brush _ColorLTM2882C;
        public Brush ColorLTM2882C
        {
            get { return _ColorLTM2882C; }
            set { SetProperty(ref _ColorLTM2882C, value); }
        }

        private Brush _ColorLTM2882D;
        public Brush ColorLTM2882D
        {
            get { return _ColorLTM2882D; }
            set { SetProperty(ref _ColorLTM2882D, value); }
        }

        private Brush _Color1150A;
        public Brush Color1150A
        {
            get { return _Color1150A; }
            set { SetProperty(ref _Color1150A, value); }
        }

        private Brush _Color1150B;
        public Brush Color1150B
        {
            get { return _Color1150B; }
            set { SetProperty(ref _Color1150B, value); }
        }





    }
}
