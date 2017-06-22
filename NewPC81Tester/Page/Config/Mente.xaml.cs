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
        private SolidColorBrush ButtonOnBrush = new SolidColorBrush();
        private SolidColorBrush ButtonOffBrush = new SolidColorBrush();
        private const double ButtonOpacity = 0.4;

        public Mente()
        {
            InitializeComponent();
            this.DataContext = State.VmComm;

            tbCommand.Text = "";

            ButtonOnBrush.Color = Colors.DodgerBlue;
            ButtonOffBrush.Color = Colors.White;
            ButtonOnBrush.Opacity = ButtonOpacity;
            ButtonOffBrush.Opacity = ButtonOpacity;

            buttonPow.Background = ButtonOffBrush;
            buttonFun.Background = ButtonOffBrush;
            buttonS1.Background = ButtonOffBrush;
            buttonStamp.Background = ButtonOffBrush;
            buttonSetE1.Background = ButtonOffBrush;

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
            buttonStamp.Background = ButtonOnBrush;
            await Task.Delay(400);
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b0, EPX64S.OUT.L);
            buttonStamp.Background = ButtonOffBrush;

        }

        bool FlagFun;
        private void buttonFun_Click(object sender, RoutedEventArgs e)
        {
            FlagFun = !FlagFun;
            General.SetDcFun(FlagFun);
            buttonFun.Background = FlagFun ? ButtonOnBrush : ButtonOffBrush;

        }

        private async void buttonS1_Click(object sender, RoutedEventArgs e)
        {
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.H);
            buttonS1.Background = ButtonOnBrush;
            await Task.Delay(400);
            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.L);
            buttonS1.Background = ButtonOffBrush;
        }

        bool FlagSetE1;
        private void buttonSetE1_Click(object sender, RoutedEventArgs e)
        {
            FlagSetE1 = !FlagSetE1;
            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, FlagSetE1 ? EPX64S.OUT.H : EPX64S.OUT.L);
            buttonSetE1.Background = FlagSetE1? ButtonOnBrush : ButtonOffBrush;
        }

        private Target.Port SetComm()
        {

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

            return port;
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            if (!Flags.PowOn)
            {
                General.PowSupply(true);
                var re = General.CheckComm();
                if (!re)
                {
                    General.PowSupply(false);
                    return;
                }
            }

            var port = SetComm();

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

        private void buttonCheckSerial_Click(object sender, RoutedEventArgs e)
        {
            if (!Flags.PowOn)
            {
                General.PowSupply(true);
                var re = General.CheckComm();
                if (!re)
                {
                    General.PowSupply(false);
                    return;
                }
            }
            var port = SetComm();

            Target.SendData(port, "SerialRead");
        }
    }
}
