using System;
using System.Windows;
using System.Windows.Navigation;

namespace Os303Tester
{
    /// <summary>
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Conf
    {
        private NavigationService naviEdit;
        private NavigationService naviTheme;
        private NavigationService naviMente;
        private NavigationService naviCamera;
        Uri uriEditPage = new Uri("Page/Config/EditOpeList.xaml", UriKind.Relative);
        Uri uriThemePage = new Uri("Page/Config/Theme.xaml", UriKind.Relative);
        Uri uriMentePage = new Uri("Page/Config/Mente.xaml", UriKind.Relative);
        Uri uriCameraPage = new Uri("Page/Config/CameraConf.xaml", UriKind.Relative);

        public Conf()
        {
            InitializeComponent();
            naviEdit = FrameEdit.NavigationService;
            naviTheme = FrameTheme.NavigationService;
            naviMente = FrameMente.NavigationService;
            naviCamera = FrameCamera.NavigationService;
            FrameEdit.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameTheme.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameMente.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            FrameCamera.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            TabMenu.SelectedIndex = 0;

            // オブジェクト作成に必要なコードをこの下に挿入します。
        }
        private void TabMente_Loaded(object sender, RoutedEventArgs e)
        {
            naviMente.Navigate(uriMentePage);
        }

        private void TabOperator_Loaded(object sender, RoutedEventArgs e)
        {
            naviEdit.Navigate(uriEditPage);
        }

        private void TabTheme_Loaded(object sender, RoutedEventArgs e)
        {
            naviTheme.Navigate(uriThemePage);
        }

        private void TabCamera_Loaded(object sender, RoutedEventArgs e)
        {
            naviCamera.Navigate(uriCameraPage);
        }
    }
}
