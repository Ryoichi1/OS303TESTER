using Microsoft.Practices.Prism.Mvvm;

namespace NewPC81Tester
{
    public class ViewModelCameraPoint : BindableBase
    {
        private string _RX1;
        public string RX1 { get { return _RX1; } set { SetProperty(ref _RX1, value); } }

        private string _RX2;
        public string RX2 { get { return _RX2; } set { SetProperty(ref _RX2, value); } }

        private string _RX3;
        public string RX3 { get { return _RX3; } set { SetProperty(ref _RX3, value); } }

        private string _RX4;
        public string RX4 { get { return _RX4; } set { SetProperty(ref _RX4, value); } }

        private string _RX5;
        public string RX5 { get { return _RX5; } set { SetProperty(ref _RX5, value); } }

        private string _TX1;
        public string TX1 { get { return _TX1; } set { SetProperty(ref _TX1, value); } }

        private string _TX2;
        public string TX2 { get { return _TX2; } set { SetProperty(ref _TX2, value); } }

        private string _TX3;
        public string TX3 { get { return _TX3; } set { SetProperty(ref _TX3, value); } }

        private string _TX4;
        public string TX4 { get { return _TX4; } set { SetProperty(ref _TX4, value); } }

        private string _TX5;
        public string TX5 { get { return _TX5; } set { SetProperty(ref _TX5, value); } }

        private string _CPU;
        public string CPU { get { return _CPU; } set { SetProperty(ref _CPU, value); } }

        private string _DEB;
        public string DEB { get { return _DEB; } set { SetProperty(ref _DEB, value); } }

        private string _VCC;
        public string VCC { get { return _VCC; } set { SetProperty(ref _VCC, value); } }


    }
}
