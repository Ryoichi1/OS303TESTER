using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using System.Windows.Input;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using AForge.Video.DirectShow;

namespace Os303Tester
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

            toggleSw.IsChecked = General.cam.Opening;

            SetRect();

        }

        bool FlagSetLed;

        private async void SetLed()
        {
            General.SetRL1(false);
            General.SetRL2(false);
            General.SetRL3(false);
            General.SetRL4(false);
            General.SetRL5(true);
            General.SetRL6(true);
            await Task.Delay(500);

            General.PowSupply(true);//RL1
            Thread.Sleep(2000);
            General.S1On();

            FlagSetLed = true;

        }




        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;
            SaveCameraProp();
            await Task.Delay(150);
            General.PlaySound(General.soundSave);
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
            State.camProp.Whitebalance = General.cam.Wb;
            State.camProp.Theta = General.cam.Theta;
            State.camProp.BinLevel = General.cam.BinLevel;

            State.camProp.Opening = General.cam.Opening;
            State.camProp.OpenCnt = General.cam.openCnt;
            State.camProp.CloseCnt = General.cam.closeCnt;


            State.camProp.LED1 = State.VmCameraPoint.LED1;
            State.camProp.LED2 = State.VmCameraPoint.LED2;
            State.camProp.LED3 = State.VmCameraPoint.LED3;


        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            General.cam.ResetFlag();

            //TODO:
            //LEDを全消灯させる処理
            General.ResetIo();
            State.SetCamProp();
        }


        private void rbLED1_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.LED1);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.LED1);
            General.cam.FlagCross = true;
        }

        private void rbLED2_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.LED2);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.LED2);
            General.cam.FlagCross = true;
        }

        private void rbLED3_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.CrossX = GetPointX(State.VmCameraPoint.LED3);
            General.cam.CrossY = GetPointY(State.VmCameraPoint.LED3);
            General.cam.FlagCross = true;
        }



        private void rbLED1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.LED1 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbLED2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.LED2 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
        }

        private void rbLED3_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            ChangePoint(e.Key);
            State.VmCameraPoint.LED3 = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();
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

        bool GridSw = false;
        private void buttonGrid_Click(object sender, RoutedEventArgs e)
        {
            ResetRb();
            General.cam.ResetFlag();
            GridSw = !GridSw;
            General.cam.FlagGrid = GridSw;
            buttonGrid.Background = GridSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonBin.IsEnabled = !GridSw;
            canvasLdPoint.IsEnabled = !GridSw;
        }

        bool BinSw = false;
        private void buttonBin_Click(object sender, RoutedEventArgs e)
        {
            ResetRb();
            General.cam.ResetFlag();
            BinSw = !BinSw;
            General.cam.FlagBin = BinSw;
            buttonBin.Background = BinSw ? Brushes.DodgerBlue : Brushes.Transparent;

            buttonGrid.IsEnabled = !BinSw;
            canvasLdPoint.IsEnabled = !BinSw;

            SetRect();
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

            var newX = GetPointX(State.VmCameraPoint.LED1) + X;
            var newY = GetPointY(State.VmCameraPoint.LED1) + Y;
            State.VmCameraPoint.LED1 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.LED2) + X;
            newY = GetPointY(State.VmCameraPoint.LED2) + Y;
            State.VmCameraPoint.LED2 = newX.ToString() + "/" + newY.ToString();

            newX = GetPointX(State.VmCameraPoint.LED3) + X;
            newY = GetPointY(State.VmCameraPoint.LED3) + Y;
            State.VmCameraPoint.LED3 = newX.ToString() + "/" + newY.ToString();



        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            if (General.cam.FlagFrame)
            {
                //全座標をY方向に-1シフトする
                Offset(DIRECTION.UP);
            }
            else if (General.cam.FlagCross)
            {
                ChangePoint(Key.Up);
                SetPointViewModel();
            }
        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (General.cam.FlagFrame)
            {
                //全座標をY方向に-1シフトする
                Offset(DIRECTION.LEFT);
            }
            else if (General.cam.FlagCross)
            {
                ChangePoint(Key.Left);
                SetPointViewModel();
            }

        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            if (General.cam.FlagFrame)
            {
                //全座標をY方向に-1シフトする
                Offset(DIRECTION.DOWN);
            }
            else if (General.cam.FlagCross)
            {
                ChangePoint(Key.Down);
                SetPointViewModel();
            }
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            if (General.cam.FlagFrame)
            {
                //全座標をY方向に-1シフトする
                Offset(DIRECTION.RIGHT);
            }
            else if (General.cam.FlagCross)
            {
                ChangePoint(Key.Right);
                SetPointViewModel();
            }
        }

        private void SetRect()
        {
            General.cam.MakeFrame = (img) =>
            {
                //四角を表示
                var TatenagaList = new List<CvPoint>();
                TatenagaList.Add(GetCenter(State.VmCameraPoint.LED1));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.LED2));
                TatenagaList.Add(GetCenter(State.VmCameraPoint.LED3));

                int side = (int)Math.Sqrt(Test_Led.TEST_FRAME);//検査枠の１辺の長さ

                foreach (var p in TatenagaList)
                {
                    img.Rectangle(new CvRect(p.X - side / 2, p.Y - side / 2, side, side), CvColor.HotPink, 1);
                }
            };

        }

        private void rbFrame_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.ResetFlag();
            General.cam.FlagFrame = true;
        }

        private void ResetRb()
        {
            rbLED1.IsChecked = false;
            rbLED2.IsChecked = false;
            rbLED3.IsChecked = false;
            rbFrame.IsChecked = false;

        }

        private void SetPointViewModel()
        {
            string point = General.cam.CrossX.ToString() + "/" + General.cam.CrossY.ToString();

            if (rbLED1.IsChecked == true) { State.VmCameraPoint.LED1 = point; }
            if (rbLED2.IsChecked == true) { State.VmCameraPoint.LED2 = point; }
            if (rbLED3.IsChecked == true) { State.VmCameraPoint.LED3 = point; }


        }


        private void rbFrame_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void rbFrame_Unchecked(object sender, RoutedEventArgs e)
        {
            General.cam.FlagFrame = false;
        }

        private void toggleSw_Checked(object sender, RoutedEventArgs e)
        {
            General.cam.Opening = true;
        }

        private void toggleSw_Unchecked(object sender, RoutedEventArgs e)
        {
            General.cam.Opening = false;
        }

        private async void buttonLed1On_Click(object sender, RoutedEventArgs e)
        {
            buttonLed1On.Background = Brushes.DodgerBlue;
            buttonLed2On.Background = Brushes.Transparent;
            buttonLed3On.Background = Brushes.Transparent;
            await Task.Run(() =>
            {

                if (!FlagSetLed) SetLed();
                General.SetRL2(true);
                General.SetRL3(false);
                General.SetRL4(false);
            });

        }

        private async void buttonLed2On_Click(object sender, RoutedEventArgs e)
        {
            buttonLed1On.Background = Brushes.Transparent;
            buttonLed2On.Background = Brushes.DodgerBlue;
            buttonLed3On.Background = Brushes.Transparent;
            await Task.Run(() =>
            {

                if (!FlagSetLed) SetLed();
                General.SetRL2(false);
                General.SetRL3(true);
                General.SetRL4(false);
            });
        }

        private async void buttonLed3On_Click(object sender, RoutedEventArgs e)
        {
            buttonLed1On.Background = Brushes.Transparent;
            buttonLed2On.Background = Brushes.Transparent;
            buttonLed3On.Background = Brushes.DodgerBlue;
            await Task.Run(() =>
            {

                if (!FlagSetLed) SetLed();
                General.SetRL2(false);
                General.SetRL3(false);
                General.SetRL4(true);
            });
        }


    }
}
