using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using OpenCvSharp;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NewPC81Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class CameraConf
    {

        public CameraConf()
        {
            InitializeComponent();
            this.DataContext = General.cam;
            canvasLdPoint.DataContext = State.VmCameraPoint;

            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
        }

        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;
            SaveCameraProp();
            await Task.Delay(150);
            General.PlaySound(General.soundBattery);
            buttonSave.Background = Brushes.Transparent;
        }

        private void SaveCameraProp()
        {
            //すべてのデータを保存する
            State.camProp.Brightness = General.cam.Brightness;
            State.camProp.Contrast = General.cam.Contrast;
            State.camProp.Hue = General.cam.Hue;
            State.camProp.Saturation = General.cam.Saturation;
            State.camProp.Sharpness = General.cam.Sharpness;
            State.camProp.Gamma = General.cam.Gamma;
            State.camProp.Gain = General.cam.Gain;
            State.camProp.Exposure = General.cam.Exposure;
            State.camProp.Theta = General.cam.Theta;
            State.camProp.BinLevel = General.cam.BinLevel;

            State.camProp.TX1 = State.VmCameraPoint.TX1;
            State.camProp.TX2 = State.VmCameraPoint.TX2;
            State.camProp.TX3 = State.VmCameraPoint.TX3;
            State.camProp.TX4 = State.VmCameraPoint.TX4;
            State.camProp.TX5 = State.VmCameraPoint.TX5;

            State.camProp.RX1 = State.VmCameraPoint.RX1;
            State.camProp.RX2 = State.VmCameraPoint.RX2;
            State.camProp.RX3 = State.VmCameraPoint.RX3;
            State.camProp.RX4 = State.VmCameraPoint.RX4;
            State.camProp.RX5 = State.VmCameraPoint.RX5;

            State.camProp.CPU = State.VmCameraPoint.CPU;
            State.camProp.DEB = State.VmCameraPoint.DEB;
            State.camProp.VCC = State.VmCameraPoint.VCC;

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            General.cam.FlagCross = false;
            General.cam.FlagBin = false;
            General.cam.FlagFrame = false;
            General.cam.FlagGray = false;
            General.cam.FlagGrid = false;

            //TODO:
            //LEDを全消灯させる処理

            State.SetCamProp();
        }

        private void rbRX4_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.RX4);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.RX4);
            General.cam.FlagCross = true;
        }

        private void rbRX3_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.RX3);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.RX3);
            General.cam.FlagCross = true;
        }

        private void rbTX3_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.TX3);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.TX3);
            General.cam.FlagCross = true;
        }

        private void rbTX4_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.TX4);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.TX4);
            General.cam.FlagCross = true;
        }

        private void rbTX1_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.TX1);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.TX1);
            General.cam.FlagCross = true;
        }

        private void rbTX2_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.TX2);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.TX2);
            General.cam.FlagCross = true;
        }

        private void rbRX2_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.RX2);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.RX2);
            General.cam.FlagCross = true;
        }

        private void rbRX1_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.RX1);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.RX1);
            General.cam.FlagCross = true;
        }

        private void rbRX5_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.RX5);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.RX5);
            General.cam.FlagCross = true;
        }

        private void rbCPU_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.CPU);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.CPU);
            General.cam.FlagCross = true;
        }

        private void rbDEB_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.DEB);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.DEB);
            General.cam.FlagCross = true;
        }

        private void rbVCC_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.VCC);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.VCC);
            General.cam.FlagCross = true;
        }

        private void rbTX5_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.TX5);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.TX5);
            General.cam.FlagCross = true;
        }

        private void rbRX4_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.RX4 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbRX3_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.RX3 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbTX3_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.TX3 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbTX4_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.TX4 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbTX1_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.TX1 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbTX2_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.TX2 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbRX2_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.RX2 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbRX1_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.RX1 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbRX5_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.RX5 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbCPU_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.CPU = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbDEB_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.DEB = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbVCC_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.VCC = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbTX5_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.TX5 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }



        private CvPoint GetCenter(string data)
        {
            var re = data.Split('/').ToArray();
            return new CvPoint(Int32.Parse(re[0]), Int32.Parse(re[1]));
        }

        private void ChangePoint(Key k)
        {
            switch (k)
            {
                case Key.Right:
                    General.cam.CrossX += 1;
                    break;
                case Key.Left:
                    General.cam.CrossX -= 1;
                    break;
                case Key.Up:
                    General.cam.CrossY -= 1;
                    break;
                case Key.Down:
                    General.cam.CrossY += 1;
                    break;
                case Key.Enter:
                    if (tbPoint.Visibility == System.Windows.Visibility.Visible)
                    {
                        var re = tbPoint.Text.Split('/').ToArray();
                        General.cam.CrossX = Int32.Parse(re[0].Trim('X', 'Y', '='));
                        General.cam.CrossY = Int32.Parse(re[1]);
                    }

                    break;
            }


        }
        private int GetPointX(string xyData)
        {
            var re = xyData.Split('/').ToArray();
            return Int32.Parse(re[0]);
        }
        private int GetPointY(string xyData)
        {
            var re = xyData.Split('/').ToArray();
            return Int32.Parse(re[1]);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            State.SetCamProp();
            //TODO:LEDを全点灯させる処理
        }

        private void im_MouseLeave(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Hidden;
            tbHsv.Visibility = System.Windows.Visibility.Hidden;
            General.cam.FlagHsv = false;
        }

        private void im_MouseEnter(object sender, MouseEventArgs e)
        {
            General.cam.FlagHsv = true;
            tbHsv.Visibility = System.Windows.Visibility.Visible;
        }

        private void im_MouseMove(object sender, MouseEventArgs e)
        {
            tbPoint.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(im);
            tbPoint.Text = "XY=" + ((int)(point.X)).ToString() + "/" + ((int)(point.Y)).ToString();

            General.cam.PointX = (int)point.X;
            General.cam.PointY = (int)point.Y;

            tbHsv.Text = "HSV=" + General.cam.Hdata.ToString() + "," + General.cam.Sdata.ToString() + "," + General.cam.Vdata.ToString();
        }

        bool LedOn;
        private async void buttonLedOnOff_Click(object sender, RoutedEventArgs e)
        {
            LedOn = !LedOn;
            if (LedOn)
            {
                try
                {
                    Target.ClearBuff();
                    General.PowSupply(true);
                    buttonLedOnOff.Background = Brushes.DodgerBlue;
                    var buff = Target.Port_232C_1.ReadLine();
                    if (!buff.Contains("ReqData"))
                    {
                        MessageBox.Show("受信データ異常DEATH!!!");
                        LedOn = false;
                        General.PowSupply(false);
                        General.ResetIo();
                        return;
                    }
                    await Task.Delay(500);
                    Target.SendData(Target.Port.Rs232C_1, "LEDCheck", doAnalysisData: false);
                }
                catch
                {
                    MessageBox.Show("通信タイムアウトDEATH!!!");
                    LedOn = false;
                    General.PowSupply(false);
                    buttonLedOnOff.Background = Brushes.Transparent;
                    General.ResetIo();
                    return;

                }
            }
            else
            {
                General.PowSupply(false);
                buttonLedOnOff.Background = Brushes.Transparent;
                await Task.Delay(400);
                General.ResetIo();
            }
        }

        private void buttonGrid_Click(object sender, RoutedEventArgs e)
        {
            var re = General.cam.FlagGrid;
            General.cam.FlagGrid = !re;

            buttonGrid.Background = re ? Brushes.Transparent : Brushes.DodgerBlue;

            buttonBin.IsEnabled = re;
            buttonSave.IsEnabled = re;
            buttonFrame.IsEnabled = re;
        }

        private void buttonFrame_Click(object sender, RoutedEventArgs e)
        {
            var re = General.cam.FlagFrame;
            General.cam.FlagFrame = !re;

            buttonBin.IsEnabled = re;
            buttonGrid.IsEnabled = re;
            buttonSave.IsEnabled = re;

            buttonFrame.Background = re ? Brushes.Transparent : Brushes.DodgerBlue;

            if (!re)
            {

                General.cam.MakeFrame = (img) =>
                {
                    //四角を表示（Seg a,g,d）
                    var TatenagaList = new List<CvPoint>();
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.CPU));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.DEB));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.VCC));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.RX1));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.RX2));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.RX3));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.RX4));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.RX5));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.TX1));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.TX2));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.TX3));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.TX4));
                    TatenagaList.Add(GetCenter(State.VmCameraPoint.TX5));


                    foreach (var p in TatenagaList)
                    {
                        img.Rectangle(new CvRect(p.X - 5, p.Y - 5, 10, 10), CvColor.Blue);

                    }


                };

                General.cam.FlagCross = false;


            }
        }

        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            var re = General.cam.FlagBin;
            General.cam.FlagBin = !re;
            buttonBin.Background = re ? Brushes.Transparent : Brushes.DodgerBlue;

            buttonGrid.IsEnabled = re;
            buttonSave.IsEnabled = re;
            buttonFrame.IsEnabled = re;
        }


        private enum DIRECTION { UP, DOWN, RIGHT, LEFT }
        private void Offset(DIRECTION dir)
        {
            int X = 0;
            int Y = 0;

            switch (dir)
            {
                case DIRECTION.UP:
                    X = 0; Y = -1;
                    break;
                case DIRECTION.DOWN:
                    X = 0; Y = 1;
                    break;
                case DIRECTION.RIGHT:
                    X = 1; Y = 0;
                    break;
                case DIRECTION.LEFT:
                    X = -1; Y = 0;
                    break;
            }

            var newX = GetPointX(State.VmCameraPoint.TX1) + X;
            var newY = GetPointY(State.VmCameraPoint.TX1) + Y;
            State.VmCameraPoint.TX1 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.TX2) + X;
            newY = GetPointY(State.VmCameraPoint.TX2) + Y;
            State.VmCameraPoint.TX2 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.TX3) + X;
            newY = GetPointY(State.VmCameraPoint.TX3) + Y;
            State.VmCameraPoint.TX3 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.TX4) + X;
            newY = GetPointY(State.VmCameraPoint.TX4) + Y;
            State.VmCameraPoint.TX4 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.TX5) + X;
            newY = GetPointY(State.VmCameraPoint.TX5) + Y;
            State.VmCameraPoint.TX5 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.RX1) + X;
            newY = GetPointY(State.VmCameraPoint.RX1) + Y;
            State.VmCameraPoint.RX1 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.RX2) + X;
            newY = GetPointY(State.VmCameraPoint.RX2) + Y;
            State.VmCameraPoint.RX2 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.RX3) + X;
            newY = GetPointY(State.VmCameraPoint.RX3) + Y;
            State.VmCameraPoint.RX3 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.RX4) + X;
            newY = GetPointY(State.VmCameraPoint.RX4) + Y;
            State.VmCameraPoint.RX4 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.RX5) + X;
            newY = GetPointY(State.VmCameraPoint.RX5) + Y;
            State.VmCameraPoint.RX5 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.CPU) + X;
            newY = GetPointY(State.VmCameraPoint.CPU) + Y;
            State.VmCameraPoint.CPU = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.DEB) + X;
            newY = GetPointY(State.VmCameraPoint.DEB) + Y;
            State.VmCameraPoint.DEB = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.VCC) + X;
            newY = GetPointY(State.VmCameraPoint.VCC) + Y;
            State.VmCameraPoint.VCC = newX.ToString() + "/" + newY.ToString();

        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            if (!General.cam.FlagFrame) return;
            //全座標をY方向に-1シフトする
            Offset(DIRECTION.UP);

            General.cam.MakeFrame = (img) =>
            {
                //四角を表示（Seg a,g,d）
                var TatenagaList = new List<CvPoint>();
                TatenagaList.Add(GetCenter(State.VmCameraPoint.CPU));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.DEB));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.VCC));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX5));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX5));


                foreach (var p in TatenagaList)
                {
                    img.Rectangle(new CvRect(p.X - 5, p.Y - 5, 10, 10), CvColor.Blue);

                }


            };

        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!General.cam.FlagFrame) return;
            //全座標をY方向に-1シフトする
            Offset(DIRECTION.LEFT);

            General.cam.MakeFrame = (img) =>
            {
                //四角を表示（Seg a,g,d）
                var TatenagaList = new List<CvPoint>();
                TatenagaList.Add(GetCenter(State.VmCameraPoint.CPU));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.DEB));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.VCC));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX5));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX5));


                foreach (var p in TatenagaList)
                {
                    img.Rectangle(new CvRect(p.X - 5, p.Y - 5, 10, 10), CvColor.Blue);

                }


            };

        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            if (!General.cam.FlagFrame) return;
            //全座標をY方向に-1シフトする
            Offset(DIRECTION.DOWN);

            General.cam.MakeFrame = (img) =>
            {
                //四角を表示（Seg a,g,d）
                var TatenagaList = new List<CvPoint>();
                TatenagaList.Add(GetCenter(State.VmCameraPoint.CPU));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.DEB));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.VCC));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX5));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX5));


                foreach (var p in TatenagaList)
                {
                    img.Rectangle(new CvRect(p.X - 5, p.Y - 5, 10, 10), CvColor.Blue);

                }


            };

        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            if (!General.cam.FlagFrame) return;
            //全座標をY方向に-1シフトする
            Offset(DIRECTION.RIGHT);

            General.cam.MakeFrame = (img) =>
            {
                //四角を表示（Seg a,g,d）
                var TatenagaList = new List<CvPoint>();
                TatenagaList.Add(GetCenter(State.VmCameraPoint.CPU));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.DEB));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.VCC));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.RX5));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX3));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX4));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.TX5));


                foreach (var p in TatenagaList)
                {
                    img.Rectangle(new CvRect(p.X - 5, p.Y - 5, 10, 10), CvColor.Blue);

                }


            };

        }
    }
}
