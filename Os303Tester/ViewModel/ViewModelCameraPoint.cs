using Microsoft.Practices.Prism.Mvvm;

namespace Os303Tester
{
    public class ViewModelCameraPoint : BindableBase
    {
        private string _LED1;
        public string LED1 { get { return _LED1; } set { SetProperty(ref _LED1, value); } }

        private string _LED2;
        public string LED2 { get { return _LED2; } set { SetProperty(ref _LED2, value); } }

        private string _LED3;
        public string LED3 { get { return _LED3; } set { SetProperty(ref _LED3, value); } }

    }
}
