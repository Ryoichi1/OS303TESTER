using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace Os303Tester
{
    public class ViewModelTestResult : BindableBase
    {

        //Vdd電圧
        private string _VolVdd;
        public string VolVdd { get { return _VolVdd; } set { SetProperty(ref _VolVdd, value); } }

        //Vcc1電圧
        private string _VolVcc1;
        public string VolVcc1 { get { return _VolVcc1; } set { SetProperty(ref _VolVcc1, value); } }

        //Vcc2電圧 製品のRY1、RY2がOFFしているときのVCC電圧
        private string _VolVcc2Off;
        public string VolVcc2Off { get { return _VolVcc2Off; } set { SetProperty(ref _VolVcc2Off, value); } }

        //Vcc2電圧 製品のRY1、RY2がONしているときのVCC電圧
        private string _VolVcc2On;
        public string VolVcc2On { get { return _VolVcc2On; } set { SetProperty(ref _VolVcc2On, value); } }

        //Vcc2電圧
        private string _VolAc100;
        public string VolAc100 { get { return _VolAc100; } set { SetProperty(ref _VolAc100, value); } }

        //Vdd電圧
        private Brush _ColorVolVdd;
        public Brush ColorVolVdd { get { return _ColorVolVdd; } set { SetProperty(ref _ColorVolVdd, value); } }

        //Vcc1電圧
        private Brush _ColorVolVcc1;
        public Brush ColorVolVcc1 { get { return _ColorVolVcc1; } set { SetProperty(ref _ColorVolVcc1, value); } }

        //Vcc2電圧 製品のRY1、RY2がOFFしているときのVCC電圧
        private Brush _ColorVolVcc2Off;
        public Brush ColorVolVcc2Off { get { return _ColorVolVcc2Off; } set { SetProperty(ref _ColorVolVcc2Off, value); } }

        //Vcc2電圧 製品のRY1、RY2がONしているときのVCC電圧
        private Brush _ColorVolVcc2On;
        public Brush ColorVolVcc2On { get { return _ColorVolVcc2On; } set { SetProperty(ref _ColorVolVcc2On, value); } }

        //Vcc1電圧
        private Brush _ColorVolAc100;
        public Brush ColorVolAc100 { get { return _ColorVolAc100; } set { SetProperty(ref _ColorVolAc100, value); } }





        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //デジタル出力 期待値
        private Brush _ColorPc1Exp;
        public Brush ColorPc1Exp { get { return _ColorPc1Exp; } set { SetProperty(ref _ColorPc1Exp, value); } }

        private Brush _ColorPc2Exp;
        public Brush ColorPc2Exp { get { return _ColorPc2Exp; } set { SetProperty(ref _ColorPc2Exp, value); } }

        private Brush _ColorPc3Exp;
        public Brush ColorPc3Exp { get { return _ColorPc3Exp; } set { SetProperty(ref _ColorPc3Exp, value); } }

        private Brush _ColorPc4Exp;
        public Brush ColorPc4Exp { get { return _ColorPc4Exp; } set { SetProperty(ref _ColorPc4Exp, value); } }

        private Brush _ColorLed1Exp;
        public Brush ColorLed1Exp { get { return _ColorLed1Exp; } set { SetProperty(ref _ColorLed1Exp, value); } }

        private Brush _ColorLed2Exp;
        public Brush ColorLed2Exp { get { return _ColorLed2Exp; } set { SetProperty(ref _ColorLed2Exp, value); } }

        private Brush _ColorLed3Exp;
        public Brush ColorLed3Exp { get { return _ColorLed3Exp; } set { SetProperty(ref _ColorLed3Exp, value); } }

        //デジタル出力 出力値
        private Brush _ColorPc1Out;
        public Brush ColorPc1Out { get { return _ColorPc1Out; } set { SetProperty(ref _ColorPc1Out, value); } }

        private Brush _ColorPc2Out;
        public Brush ColorPc2Out { get { return _ColorPc2Out; } set { SetProperty(ref _ColorPc2Out, value); } }

        private Brush _ColorPc3Out;
        public Brush ColorPc3Out { get { return _ColorPc3Out; } set { SetProperty(ref _ColorPc3Out, value); } }

        private Brush _ColorPc4Out;
        public Brush ColorPc4Out { get { return _ColorPc4Out; } set { SetProperty(ref _ColorPc4Out, value); } }

        private Brush _ColorLed1Out;
        public Brush ColorLed1Out { get { return _ColorLed1Out; } set { SetProperty(ref _ColorLed1Out, value); } }

        private Brush _ColorLed2Out;
        public Brush ColorLed2Out { get { return _ColorLed2Out; } set { SetProperty(ref _ColorLed2Out, value); } }

        private Brush _ColorLed3Out;
        public Brush ColorLed3Out { get { return _ColorLed3Out; } set { SetProperty(ref _ColorLed3Out, value); } }


        private Brush _ColorRL1;
        public Brush ColorRL1 { get { return _ColorRL1; } set { SetProperty(ref _ColorRL1, value); } }

        private Brush _ColorRL2;
        public Brush ColorRL2 { get { return _ColorRL2; } set { SetProperty(ref _ColorRL2, value); } }

        private Brush _ColorRL3;
        public Brush ColorRL3 { get { return _ColorRL3; } set { SetProperty(ref _ColorRL3, value); } }

        private Brush _ColorRL4;
        public Brush ColorRL4 { get { return _ColorRL4; } set { SetProperty(ref _ColorRL4, value); } }

        private Brush _ColorRL5;
        public Brush ColorRL5 { get { return _ColorRL5; } set { SetProperty(ref _ColorRL5, value); } }

        private Brush _ColorRL6;
        public Brush ColorRL6 { get { return _ColorRL6; } set { SetProperty(ref _ColorRL6, value); } }

        private Brush _ColorS1;
        public Brush ColorS1 { get { return _ColorS1; } set { SetProperty(ref _ColorS1, value); } }





        //LED輝度 LED1
        private string _LED1Value;
        public string LED1Value { get { return _LED1Value; } set { SetProperty(ref _LED1Value, value); } }

        //LED輝度 LED2
        private string _LED2Value;
        public string LED2Value { get { return _LED2Value; } set { SetProperty(ref _LED2Value, value); } }

        //LED輝度 LED3
        private string _LED3Value;
        public string LED3Value { get { return _LED3Value; } set { SetProperty(ref _LED3Value, value); } }

        //LED1
        private Brush _ColorLED1;
        public Brush ColorLED1 { get { return _ColorLED1; } set { SetProperty(ref _ColorLED1, value); } }

        //LED2
        private Brush _ColorLED2;
        public Brush ColorLED2 { get { return _ColorLED2; } set { SetProperty(ref _ColorLED2, value); } }

        //LED3
        private Brush _ColorLED3;
        public Brush ColorLED3 { get { return _ColorLED3; } set { SetProperty(ref _ColorLED3, value); } }

        //LED1
        private string _HueLED1;
        public string HueLED1 { get { return _HueLED1; } set { SetProperty(ref _HueLED1, value); } }

        //LED2
        private string _HueLED2;
        public string HueLED2 { get { return _HueLED2; } set { SetProperty(ref _HueLED2, value); } }

        //LED3
        private string _HueLED3;
        public string HueLED3 { get { return _HueLED3; } set { SetProperty(ref _HueLED3, value); } }


    }
}








