using Microsoft.Practices.Prism.Mvvm;


namespace NewPC81Tester
{

    public class ViewModelCommunication : BindableBase
    {
        //プロパティ
        private string _DATA_TX;
        public string DATA_TX
        {
            get { return _DATA_TX; }
            set { SetProperty(ref _DATA_TX, value); }
        }

        private string _DATA_RX;
        public string DATA_RX
        {
            get { return _DATA_RX; }
            set { SetProperty(ref _DATA_RX, value); }
        }
        
        private string _Command;
        public string Command
        {
            get { return _Command; }
            set { SetProperty(ref _Command, value); }
        }



    }
}
