using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewPC81Tester
{
    /// <summary>
    /// Theme.xaml の相互作用ロジック
    /// </summary>
    public partial class Theme
    {
        Storyboard storyboard = new Storyboard();
        DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();

        public Theme()
        {
            InitializeComponent();
            this.DataContext = State.VmMainWindow;
            SliderOpacity.Value = State.Setting.OpacityTheme;
        }

        private async void Pic1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/moon.jpg";
            await Show();
        }

        private async void Pic2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/baby2.jpg";
            await Show();
        }

        private async void Pic3_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/baby3.jpg";
            await Show();
        }

        private async void Pic4_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            State.VmMainWindow.Theme = "Resources/baby6.jpg";
            await Show();
        }

        private async Task Show()
        {
            int time1 = 650;//0.4s
            int time2 = 10;//0.01s
            int time3 = time1 / time2;

            State.Setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;

            State.VmMainWindow.ThemeOpacity = 0;
            await Task.Run(() =>
            {
                foreach(var i in Enumerable.Range(0, time3))
                {

                    State.VmMainWindow.ThemeOpacity += State.Setting.OpacityTheme/(double)time3;
                    Thread.Sleep(10);

                }
            });
        }

        private void SliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            State.Setting.OpacityTheme = State.VmMainWindow.ThemeOpacity;
        }
    }
}
