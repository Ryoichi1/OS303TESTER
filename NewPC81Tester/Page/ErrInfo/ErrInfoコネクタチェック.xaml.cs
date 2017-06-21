using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using Microsoft.Practices.Prism.Mvvm;
using System.Linq;

namespace NewPC81Tester
{
    /// <summary>
    /// Interaction logic for BasicPage1.xaml
    /// </summary>
    public partial class ErrInfoコネクタチェック
    {
        public class vm : BindableBase
        {
            private Visibility _VisCN1 = Visibility.Hidden;
            public Visibility VisCN1 { get { return _VisCN1; } set { SetProperty(ref _VisCN1, value); } }

            private Visibility _VisCN201 = Visibility.Hidden;
            public Visibility VisCN201 { get { return _VisCN201; } set { SetProperty(ref _VisCN201, value); } }


            private Visibility _VisCN202 = Visibility.Hidden;
            public Visibility VisCN202 { get { return _VisCN202; } set { SetProperty(ref _VisCN202, value); } }


            private Visibility _VisCN203 = Visibility.Hidden;
            public Visibility VisCN203 { get { return _VisCN203; } set { SetProperty(ref _VisCN203, value); } }


            private Visibility _VisCN204 = Visibility.Hidden;
            public Visibility VisCN204 { get { return _VisCN204; } set { SetProperty(ref _VisCN204, value); } }


            private Visibility _VisPORT1 = Visibility.Hidden;
            public Visibility VisPORT1 { get { return _VisPORT1; } set { SetProperty(ref _VisPORT1, value); } }


            private Visibility _VisPORT2 = Visibility.Hidden;
            public Visibility VisPORT2 { get { return _VisPORT2; } set { SetProperty(ref _VisPORT2, value); } }


            private Visibility _VisPORT3 = Visibility.Hidden;
            public Visibility VisPORT3 { get { return _VisPORT3; } set { SetProperty(ref _VisPORT3, value); } }


            private Visibility _VisPORT4 = Visibility.Hidden;
            public Visibility VisPORT4 { get { return _VisPORT4; } set { SetProperty(ref _VisPORT4, value); } }

            private Visibility _VisAN1 = Visibility.Hidden;
            public Visibility VisAN1 { get { return _VisAN1; } set { SetProperty(ref _VisAN1, value); } }


            private Visibility _VisAN2 = Visibility.Hidden;
            public Visibility VisAN2 { get { return _VisAN2; } set { SetProperty(ref _VisAN2, value); } }


            private Visibility _VisAN3 = Visibility.Hidden;
            public Visibility VisAN3 { get { return _VisAN3; } set { SetProperty(ref _VisAN3, value); } }


            private Visibility _VisAN4 = Visibility.Hidden;
            public Visibility VisAN4 { get { return _VisAN4; } set { SetProperty(ref _VisAN4, value); } }


            private string _NgList;
            public string NgList { get { return _NgList; } set { SetProperty(ref _NgList, value); } }

        }

        private vm viewmodel;

        public ErrInfoコネクタチェック()
        {
            InitializeComponent();
            viewmodel = new vm();
            this.DataContext = viewmodel;
            SetErrInfo();
        }

        private void SetErrInfo()
        {
            if (コネクタチェック.ListCnSpec == null) return;
            var NgList = コネクタチェック.ListCnSpec.Where(cn => !cn.result);

            foreach (var cn in NgList)
            {
                switch (cn.name)
                {
                    case コネクタチェック.NAME.CN1:
                        viewmodel.VisCN1 = Visibility.Visible;
                        viewmodel.NgList += "CN1\r\n";
                        break;
                    case コネクタチェック.NAME.CN201:
                        viewmodel.VisCN201 = Visibility.Visible;
                        viewmodel.NgList += "CN201\r\n";
                        break;
                    case コネクタチェック.NAME.CN202:
                        viewmodel.VisCN202 = Visibility.Visible;
                        viewmodel.NgList += "CN202\r\n";
                        break;
                    case コネクタチェック.NAME.CN203:
                        viewmodel.VisCN203 = Visibility.Visible;
                        viewmodel.NgList += "CN203\r\n";
                        break;
                    case コネクタチェック.NAME.CN204:
                        viewmodel.VisCN204 = Visibility.Visible;
                        viewmodel.NgList += "CN204\r\n";
                        break;
                    case コネクタチェック.NAME.PORT1:
                        viewmodel.VisPORT1 = Visibility.Visible;
                        viewmodel.NgList += "PORT1\r\n";
                        break;
                    case コネクタチェック.NAME.PORT2:
                        viewmodel.VisPORT2 = Visibility.Visible;
                        viewmodel.NgList += "PORT2\r\n";
                        break;
                    case コネクタチェック.NAME.PORT3:
                        viewmodel.VisPORT3 = Visibility.Visible;
                        viewmodel.NgList += "PORT3\r\n";
                        break;
                    case コネクタチェック.NAME.PORT4:
                        viewmodel.VisPORT4 = Visibility.Visible;
                        viewmodel.NgList += "PORT4\r\n";
                        break;
                    case コネクタチェック.NAME.AN1:
                        viewmodel.VisAN1 = Visibility.Visible;
                        viewmodel.NgList += "AN1\r\n";
                        break;
                    case コネクタチェック.NAME.AN2:
                        viewmodel.VisAN2 = Visibility.Visible;
                        viewmodel.NgList += "AN2\r\n";
                        break;
                    case コネクタチェック.NAME.AN3:
                        viewmodel.VisAN3 = Visibility.Visible;
                        viewmodel.NgList += "AN3\r\n";
                        break;
                    case コネクタチェック.NAME.AN4:
                        viewmodel.VisAN4 = Visibility.Visible;
                        viewmodel.NgList += "AN4\r\n";
                        break;
                }

            }
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            State.VmMainWindow.TabIndex = 0;
            
        }
    }
}
