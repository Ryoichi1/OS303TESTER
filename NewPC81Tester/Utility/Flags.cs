using System.Windows.Media;

namespace NewPC81Tester
{
    public static class Flags
    {
        public static bool OtherPage { get; set; }
        public static bool ReturnFromOtherPage { get; set; }

        //試験開始時に初期化が必要なフラグ
        public static bool StopInit周辺機器 { get; set; }
        public static bool Initializing周辺機器 { get; set; }
        public static bool EnableTestStart { get; set; }
        public static bool StopUserInputCheck { get; set; }
        public static bool Testing { get; set; }
        public static bool PowOn { get; set; }//メイン電源ON/OFF
        public static bool ShowErrInfo { get; set; }
        public static bool AddDecision { get; set; }
        public static bool MetalMode { get; set; }
        public static bool BgmOn { get; set; }

        public static bool ClickStopButton { get; set; }
        public static bool Click確認Button { get; set; }

        public static bool Flag電池セット { get; set; }

        private static SolidColorBrush RetryPanelBrush = new SolidColorBrush();
        private static SolidColorBrush StatePanelOkBrush = new SolidColorBrush();
        private static SolidColorBrush StatePanelNgBrush = new SolidColorBrush();
        private const double StatePanelOpacity = 0.5;

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

        private static bool _StateSS7012;
        public static bool StateSS7012
        {
            get { return _StateSS7012; }
            set
            {
                _StateSS7012 = value;
                State.VmTestStatus.ColorHIOKI7012 = value ? StatePanelOkBrush : StatePanelNgBrush;

            }
        }

        private static bool _StateVOAC7602;
        public static bool StateVOAC7602
        {
            get { return _StateVOAC7602; }
            set
            {
                _StateVOAC7602 = value;
                State.VmTestStatus.ColorVOAC7602 = value ? StatePanelOkBrush : StatePanelNgBrush;
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

        private static bool _State温度計;
        public static bool State温度計
        {
            get { return _State温度計; }
            set
            {
                _State温度計 = value;
                State.VmTestStatus.Color温度計 = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateLTM2882A;
        public static bool StateLTM2882A
        {
            get { return _StateLTM2882A; }
            set
            {
                _StateLTM2882A = value;
                State.VmTestStatus.ColorLTM2882A = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateLTM2882B;
        public static bool StateLTM2882B
        {
            get { return _StateLTM2882B; }
            set
            {
                _StateLTM2882B = value;
                State.VmTestStatus.ColorLTM2882B = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateLTM2882C;
        public static bool StateLTM2882C
        {
            get { return _StateLTM2882C; }
            set
            {
                _StateLTM2882C = value;
                State.VmTestStatus.ColorLTM2882C = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _StateLTM2882D;
        public static bool StateLTM2882D
        {
            get { return _StateLTM2882D; }
            set
            {
                _StateLTM2882D = value;
                State.VmTestStatus.ColorLTM2882D = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }


        private static bool _State1150A;
        public static bool State1150A
        {
            get { return _State1150A; }
            set
            {
                _State1150A = value;
                State.VmTestStatus.Color1150A = value ? StatePanelOkBrush : StatePanelNgBrush;
            }
        }

        private static bool _State1150B;
        public static bool State1150B
        {
            get { return _State1150B; }
            set
            {
                _State1150B = value;
                State.VmTestStatus.Color1150B = value ? StatePanelOkBrush : StatePanelNgBrush;
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
                    State.VmMainWindow.SerialNumber = "";
                }

            }
        }

    }
}
