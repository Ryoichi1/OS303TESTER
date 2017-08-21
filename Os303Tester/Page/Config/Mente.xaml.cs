using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

namespace Os303Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Mente
    {
        private SolidColorBrush ButtonOnBrush = new SolidColorBrush();
        private SolidColorBrush ButtonOffBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.4;

        System.Timers.Timer timerReadInput;

        public Mente()
        {
            InitializeComponent();

            ButtonOnBrush.Color = Colors.DodgerBlue;
            ButtonOffBrush.Color = Colors.White;
            ButtonOnBrush.Opacity = ButtonOpacity;
            ButtonOffBrush.Opacity = ButtonOpacity;

            canvasManual.DataContext = State.VmTestResults;
            CanvasImg.DataContext = General.cam;
            General.cam.ImageOpacity = 1.0;

            timerReadInput = new System.Timers.Timer();
            timerReadInput.Interval = 1000;
            timerReadInput.Elapsed += (sender, e) =>
            {
                AnalysisInputData();
            };
            timerReadInput.Start();


        }

        private static void AnalysisInputData()
        {
            General.io.ReadInputData(EPX64S.PORT.P3);
            var p3Data = General.io.P3InputData;

            bool outPc1 = (p3Data & 0x02) == 0x00;
            bool outPc2 = (p3Data & 0x04) == 0x00;
            bool outPc3 = (p3Data & 0x20) == 0x00;
            bool outPc4 = (p3Data & 0x40) == 0x00;

            State.VmTestResults.ColorPc1Out = outPc1 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorPc2Out = outPc2 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorPc3Out = outPc3 ? Brushes.DodgerBlue : Brushes.Transparent;
            State.VmTestResults.ColorPc4Out = outPc4 ? Brushes.DodgerBlue : Brushes.Transparent;

        }




        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            General.PowSupply(false);

            buttonPow.Background = Brushes.Transparent;

        }


        bool FlagPow;
        private void buttonPow_Click(object sender, RoutedEventArgs e)
        {


            if (FlagPow)
            {
                General.PowSupply(false);
                buttonPow.Background = ButtonOffBrush;

            }
            else
            {
                General.PowSupply(true);
                buttonPow.Background = ButtonOnBrush;
            }

            FlagPow = !FlagPow;
        }




        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private async void buttonStamp_Click(object sender, RoutedEventArgs e)
        {
            buttonStamp.Background = ButtonOnBrush;
            General.StampOn();
            await Task.Delay(400);
            buttonStamp.Background = ButtonOffBrush;

        }



        private async void buttonS1_Click(object sender, RoutedEventArgs e)
        {
            buttonS1.Background = ButtonOnBrush;
            General.S1On();
            await Task.Delay(400);
            buttonS1.Background = ButtonOffBrush;
        }

        bool FlagSetE1;
        private void buttonSetE1_Click(object sender, RoutedEventArgs e)
        {
            FlagSetE1 = !FlagSetE1;
            General.SetK10_11(FlagSetE1);
            buttonSetE1.Background = FlagSetE1 ? ButtonOnBrush : ButtonOffBrush;
        }



        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {

            buttonPow.Background = ButtonOffBrush;
            buttonS1.Background = ButtonOffBrush;
            buttonStamp.Background = ButtonOffBrush;
            buttonSetE1.Background = ButtonOffBrush;
        }

        bool RL1;
        private void buttonRL1_Click(object sender, RoutedEventArgs e)
        {
            RL1 = !RL1;
            General.PowSupply(RL1);
            buttonRL1.Background = RL1 ? ButtonOnBrush : ButtonOffBrush;
        }

        bool RL2;
        private void buttonRL2_Click(object sender, RoutedEventArgs e)
        {
            RL2 = !RL2;
            General.SetRL2(RL2);
            buttonRL2.Background = RL2 ? ButtonOnBrush : ButtonOffBrush;
        }

        bool RL3;
        private void buttonRL3_Click(object sender, RoutedEventArgs e)
        {
            RL3 = !RL3;
            General.SetRL3(RL3);
            buttonRL3.Background = RL3 ? ButtonOnBrush : ButtonOffBrush;
        }

        bool RL4;
        private void buttonRL4_Click(object sender, RoutedEventArgs e)
        {
            RL4 = !RL4;
            General.SetRL4(RL4);
            buttonRL4.Background = RL4 ? ButtonOnBrush : ButtonOffBrush;
        }

        bool RL5;
        private void buttonRL5_Click(object sender, RoutedEventArgs e)
        {
            RL5 = !RL5;
            General.SetRL5(RL5);
            buttonRL5.Background = RL5 ? ButtonOnBrush : ButtonOffBrush;
        }

        bool RL6;
        private void buttonRL6_Click(object sender, RoutedEventArgs e)
        {
            RL6 = !RL6;
            General.SetRL6(RL6);
            buttonRL6.Background = RL6 ? ButtonOnBrush : ButtonOffBrush;
        }

        private void buttonRL1_Unloaded(object sender, RoutedEventArgs e)
        {
            timerReadInput.Stop();
            timerReadInput.Dispose();
        }

        private void buttonS1On_Click(object sender, RoutedEventArgs e)
        {
            buttonS1On.Background = ButtonOnBrush;
            General.S1On();
            buttonS1On.Background = ButtonOffBrush;
        }

        private async void buttonDischarge_Click(object sender, RoutedEventArgs e)
        {
            buttonDischarge.Background = ButtonOnBrush;
            await General.Discharge();
            buttonDischarge.Background = ButtonOffBrush;

        }
    }
}
