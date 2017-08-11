using System.Windows.Media;

namespace Os303Tester
{
    public static class Flags
    {
        public static bool OtherPage { get; set; }

        //試験開始時に初期化が必要なフラグ
        public static bool StopInit周辺機器 { get; set; }
        public static bool Initializing周辺機器 { get; set; }
        public static bool StopUserInputCheck { get; set; }
        public static bool Testing { get; set; }
        public static bool PowOn { get; set; }//メイン電源ON/OFF
        public static bool ShowErrInfo { get; set; }
        public static bool AddDecision { get; set; }

        public static bool PressOpenCheckBeforeTest { get; set; }


        private static SolidColorBrush RetryPanelBrush = new SolidColorBrush();
        private static SolidColorBrush StatePanelOkBrush = new SolidColorBrush();
        private static SolidColorBrush StatePanelNgBrush = new SolidColorBrush();
        private const double StatePanelOpacity = 0.4;

        static Flags()
        {
            RetryPanelBrush.Color = Colors.DodgerBlue;
            RetryPanelBrush.Opacity = StatePanelOpacity;

            StatePanelOkBrush.Color = Colors.DodgerBlue;
            StatePanelOkBrush.Opacity = StatePanelOpacity;
            StatePanelNgBrush.Color = Colors.DeepPink;
            StatePanelNgBrush.Opacity = StatePanelOpacity;
        }

        //周辺機器ステータス
        private static bool _StateRL1;
        public static bool StateRL1
        {
            get { return _StateRL1; }
            set
            {
                _StateRL1 = value;
                State.VmTestStatus.ColorRL1 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateEpx64;
        public static bool StateEpx64
        {
            get { return _StateEpx64; }
            set
            {
                _StateEpx64 = value;
                State.VmTestStatus.ColorEpx64s = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }


        private static bool _StateVOAC7502;
        public static bool StateVOAC7502
        {
            get { return _StateVOAC7502; }
            set
            {
                _StateVOAC7502 = value;
                State.VmTestStatus.ColorVOAC7502 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateCamera;
        public static bool StateCamera
        {
            get { return _StateCamera; }
            set
            {
                _StateCamera = value;
                State.VmTestStatus.ColorCAMERA = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }



        private static bool _Retry;
        public static bool Retry
        {
            get { return _Retry; }
            set
            {
                _Retry = value;
                State.VmTestStatus.ColorRetry = value ? RetryPanelBrush : Brushes.Transparent;
            }
        }

        public static bool AllOk周辺機器接続 { get; set; }

        //フラグ
        private static bool _SetOperator;
        public static bool SetOperator
        {
            get { return _SetOperator; }
            set
            {
                _SetOperator = value;
                if (value)
                {
                    if (State.VmMainWindow.Operator == "畔上")
                    {
                        State.VmTestStatus.EnableUnitTest = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        State.VmTestStatus.EnableUnitTest = System.Windows.Visibility.Hidden;
                    }
                }
                else
                {
                    State.VmMainWindow.Operator = "";
                    State.VmTestStatus.EnableUnitTest = System.Windows.Visibility.Hidden;
                    State.VmMainWindow.SelectIndex = -1;


                }
            }
        }


        private static bool _SetOpecode;
        public static bool SetOpecode
        {
            get { return _SetOpecode; }

            set
            {
                _SetOpecode = value;

                if (value)
                {
                    State.VmMainWindow.ReadOnlyOpecode = true;
                }
                else
                {
                    State.VmMainWindow.ReadOnlyOpecode = false;
                    State.VmMainWindow.Opecode = "";
                }

            }
        }

    }
}
