using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;

namespace NewPC81Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class Mente
    {
        private SolidColorBrush ButtonPowBrush = new SolidColorBrush();
        private SolidColorBrush ButtonFunBrush = new SolidColorBrush();
        private SolidColorBrush ButtonS1Brush = new SolidColorBrush();
        private SolidColorBrush ButtonStampBrush = new SolidColorBrush();
        private SolidColorBrush ButtonE1Brush = new SolidColorBrush();
        private const double ButtonOpacity = 0.4;

        public Mente()
        {
            InitializeComponent();
            this.DataContext = State.VmComm;

            tbCommand.Text = "";

            ButtonPowBrush.Color = Colors.White;
            ButtonFunBrush.Color = Colors.White;
            ButtonS1Brush.Color = Colors.White;
            ButtonStampBrush.Color = Colors.White;
            ButtonE1Brush.Color = Colors.White;

            ButtonPowBrush.Opacity = ButtonOpacity;
            ButtonFunBrush.Opacity = ButtonOpacity;
            ButtonS1Brush.Opacity = ButtonOpacity;
            ButtonStampBrush.Opacity = ButtonOpacity;
            ButtonE1Brush.Opacity = ButtonOpacity;

            buttonPow.Background = ButtonPowBrush;
            buttonFun.Background = ButtonFunBrush;
            buttonS1.Background = ButtonS1Brush;
            buttonStamp.Background = ButtonStampBrush;
            buttonSetE1.Background = ButtonE1Brush;

            rb232P1.IsChecked = true;
            ch = CH._232cPort1;

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
                ButtonPowBrush.Color = Colors.White;
                buttonPow.Background = ButtonPowBrush;

            }
            else
            {
                General.PowSupply(true);
                ButtonPowBrush.Color = Colors.DodgerBlue;
                buttonPow.Background = ButtonPowBrush;
            }

            FlagPow = !FlagPow;
        }




        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            State.VmComm.DATA_RX = "";
            State.VmComm.DATA_TX = "";
            State.VmComm.Command = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Target.SendData(Target.Port.Rs422_1, "SerialRead");// SendRs422Data("SerialRead");
            MessageBox.Show(Target.RecieveData);
        }

        private async void buttonStamp_Click(object sender, RoutedEventArgs e)
        {
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b0, EPX64S.OUT.H);
            ButtonStampBrush.Color = Colors.DodgerBlue;
            buttonStamp.Background = ButtonStampBrush;
            await Task.Delay(400);
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b0, EPX64S.OUT.L);
            ButtonStampBrush.Color = Colors.White;
            buttonStamp.Background = ButtonStampBrush;

        }

        bool FlagFun;
        private void buttonFun_Click(object sender, RoutedEventArgs e)
        {
            FlagFun = !FlagFun;
            General.SetDcFun(FlagFun);

            ButtonFunBrush.Color = FlagFun ? Colors.DodgerBlue : Colors.White;
            buttonFun.Background = ButtonFunBrush;

        }

        private async void buttonS1_Click(object sender, RoutedEventArgs e)
        {
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.H);
            ButtonS1Brush.Color = Colors.DodgerBlue;
            buttonS1.Background = ButtonS1Brush;
            await Task.Delay(400);
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.L);
            ButtonS1Brush.Color = Colors.White;
            buttonS1.Background = ButtonS1Brush;
        }

        bool FlagSetE1;
        private void buttonSetE1_Click(object sender, RoutedEventArgs e)
        {
            FlagSetE1 = !FlagSetE1;
            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, FlagSetE1 ? EPX64S.OUT.H : EPX64S.OUT.L);

            ButtonE1Brush.Color = FlagSetE1 ? Colors.DodgerBlue : Colors.White;
            buttonSetE1.Background = ButtonE1Brush;
        }



        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            if (!Flags.PowOn) return;

            Target.Port port;
            switch (ch)
            {
                case CH._232cPort1:
                    port = Target.Port.Rs232C_1;
                    break;

                case CH._232cPort2:
                    port = Target.Port.Rs232C_2;
                    break;

                case CH._232cPort3:
                    port = Target.Port.Rs232C_3;
                    break;

                case CH._232cPort4:
                    port = Target.Port.Rs232C_4;
                    break;

                case CH._422Port1:
                case CH._422Port2:
                    port = Target.Port.Rs422_1;
                    break;

                case CH._422Port3:
                case CH._422Port4:
                case CH._422CN202:
                    port = Target.Port.Rs422_2;
                    break;

                default:
                    port = Target.Port.Rs232C_1;
                    break;
            }

            //MOXA_1150A or MOXA_1150B の接続
            switch (ch)
            {
                case CH._422Port1:
                case CH._422Port3:
                    //K10,11,12,13をOFFする処理
                    General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.L);
                    //K14,15をOFFする処理
                    General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                    break;
                case CH._422Port2:
                case CH._422Port4:
                    //K10,11,12,13をONする処理
                    General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.H);
                    //K14,15をOFFする処理
                    General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                    break;
                case CH._422CN202:
                    //K14,15をONする処理
                    General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.H);
                    break;

                default:
                    break;
            }


            //JP1～4 の5-6、7-8を短絡する処理
            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.H);
            Thread.Sleep(200);

            Target.SendData(port, tbCommand.Text);


        }

        private enum CH { _232cPort1, _232cPort2, _232cPort3, _232cPort4, _422Port1, _422Port2, _422Port3, _422Port4, _422CN202 }
        private CH ch;


        private void rb232P1_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._232cPort1;
        }

        private void rb232P2_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._232cPort2;
        }

        private void rb232P3_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._232cPort3;
        }

        private void rb232P4_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._232cPort4;
        }
        private void rb422P1_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._422Port1;
        }
        private void rb422P2_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._422Port2;
        }

        private void rb422P3_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._422Port3;
        }

        private void rb422P4_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._422Port4;
        }

        private void rb422CN202_Checked(object sender, RoutedEventArgs e)
        {
            ch = CH._422CN202;
        }


    }
}
