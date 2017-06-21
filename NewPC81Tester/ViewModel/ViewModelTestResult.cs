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


namespace NewPC81Tester
{
    public class ViewModelTestResult : BindableBase
    {
        //24V系 消費電流
        private string _Curr24V;
        public string Curr24V { get { return _Curr24V; } set { SetProperty(ref _Curr24V, value); } }

        //3V系 消費電流
        private string _Curr3V;
        public string Curr3V { get { return _Curr3V; } set { SetProperty(ref _Curr3V, value); } }

        //Vcc電圧
        private string _VolVcc;
        public string VolVcc { get { return _VolVcc; } set { SetProperty(ref _VolVcc, value); } }

        //-5V電圧
        private string _VolMinus5V;
        public string VolMinus5V { get { return _VolMinus5V; } set { SetProperty(ref _VolMinus5V, value); } }

        //Vref電圧
        private string _VolVref;
        public string VolVref { get { return _VolVref; } set { SetProperty(ref _VolVref, value); } }

        //リチウム電池電圧
        private string _VolBatt;
        public string VolBatt { get { return _VolBatt; } set { SetProperty(ref _VolBatt, value); } }

        //D4両端電圧
        private string _VolD4;
        public string VolD4 { get { return _VolD4; } set { SetProperty(ref _VolD4, value); } }

        //K1コイル電流
        private string _CurrK1;
        public string CurrK1 { get { return _CurrK1; } set { SetProperty(ref _CurrK1, value); } }

        //K2コイル電流
        private string _CurrK2;
        public string CurrK2 { get { return _CurrK2; } set { SetProperty(ref _CurrK2, value); } }

        //K3コイル電流
        private string _CurrK3;
        public string CurrK3 { get { return _CurrK3; } set { SetProperty(ref _CurrK3, value); } }

        //K4コイル電流
        private string _CurrK4;
        public string CurrK4 { get { return _CurrK4; } set { SetProperty(ref _CurrK4, value); } }

        //K5コイル電流
        private string _CurrK5;
        public string CurrK5 { get { return _CurrK5; } set { SetProperty(ref _CurrK5, value); } }

        //AN0 0V
        private string _An0_00V;
        public string An0_00V { get { return _An0_00V; } set { SetProperty(ref _An0_00V, value); } }

        //AN0 2.5V
        private string _An0_25V;
        public string An0_25V { get { return _An0_25V; } set { SetProperty(ref _An0_25V, value); } }

        //AN0 5.0V
        private string _An0_50V;
        public string An0_50V { get { return _An0_50V; } set { SetProperty(ref _An0_50V, value); } }


        //AN1 0V
        private string _An1_00V;
        public string An1_00V { get { return _An1_00V; } set { SetProperty(ref _An1_00V, value); } }

        //AN1 2.5V
        private string _An1_25V;
        public string An1_25V { get { return _An1_25V; } set { SetProperty(ref _An1_25V, value); } }

        //AN1 5.0V
        private string _An1_50V;
        public string An1_50V { get { return _An1_50V; } set { SetProperty(ref _An1_50V, value); } }

        //AN2 0V
        private string _An2_00V;
        public string An2_00V { get { return _An2_00V; } set { SetProperty(ref _An2_00V, value); } }

        //AN2 2.5V
        private string _An2_25V;
        public string An2_25V { get { return _An2_25V; } set { SetProperty(ref _An2_25V, value); } }

        //AN2 5.0V
        private string _An2_50V;
        public string An2_50V { get { return _An2_50V; } set { SetProperty(ref _An2_50V, value); } }

        //AN3 0V
        private string _An3_00V;
        public string An3_00V { get { return _An3_00V; } set { SetProperty(ref _An3_00V, value); } }

        //AN3 5.0V
        private string _An3_50V;
        public string An3_50V { get { return _An3_50V; } set { SetProperty(ref _An3_50V, value); } }

        //AN3 10.0V
        private string _An3_100V;
        public string An3_100V { get { return _An3_100V; } set { SetProperty(ref _An3_100V, value); } }

        //RTC HOST側時刻
        private string _TimeHost;
        public string TimeHost { get { return _TimeHost; } set { SetProperty(ref _TimeHost, value); } }

        //RTC 製品側時刻
        private string _TimeTarget;
        public string TimeTarget { get { return _TimeTarget; } set { SetProperty(ref _TimeTarget, value); } }

        //RTC Hostと製品の時刻誤差
        private string _RtcTimeError;
        public string RtcTimeError { get { return _RtcTimeError; } set { SetProperty(ref _RtcTimeError, value); } }

        //RTC HOST側時刻
        private string _TimeHost2;
        public string TimeHost2 { get { return _TimeHost2; } set { SetProperty(ref _TimeHost2, value); } }

        //RTC 製品側時刻
        private string _TimeTarget2;
        public string TimeTarget2 { get { return _TimeTarget2; } set { SetProperty(ref _TimeTarget2, value); } }

        //RTC Hostと製品の時刻誤差
        private string _RtcTimeError2;
        public string RtcTimeError2 { get { return _RtcTimeError2; } set { SetProperty(ref _RtcTimeError2, value); } }

        //温度計表示値
        private string _TempUSBRH;
        public string TempUSBRH { get { return _TempUSBRH; } set { SetProperty(ref _TempUSBRH, value); } }

        //製品温度補償ICの表示値
        private string _TempTarget;
        public string TempTarget { get { return _TempTarget; } set { SetProperty(ref _TempTarget, value); } }

        //Loop1補正値
        private string _Loop1Ad;
        public string Loop1Ad { get { return _Loop1Ad; } set { SetProperty(ref _Loop1Ad, value); } }

        //Loop2補正値
        private string _Loop2Ad;
        public string Loop2Ad { get { return _Loop2Ad; } set { SetProperty(ref _Loop2Ad, value); } }

        //CH1 25℃
        private string _Ch1_25;
        public string Ch1_25 { get { return _Ch1_25; } set { SetProperty(ref _Ch1_25, value); } }

        //CH1 100℃
        private string _Ch1_100;
        public string Ch1_100 { get { return _Ch1_100; } set { SetProperty(ref _Ch1_100, value); } }

        //CH1 400℃
        private string _Ch1_400;
        public string Ch1_400 { get { return _Ch1_400; } set { SetProperty(ref _Ch1_400, value); } }


        //CH2 25℃
        private string _Ch2_25;
        public string Ch2_25 { get { return _Ch2_25; } set { SetProperty(ref _Ch2_25, value); } }

        //CH2 100℃
        private string _Ch2_100;
        public string Ch2_100 { get { return _Ch2_100; } set { SetProperty(ref _Ch2_100, value); } }

        //CH2 400℃
        private string _Ch2_400;
        public string Ch2_400 { get { return _Ch2_400; } set { SetProperty(ref _Ch2_400, value); } }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //24V系 消費電流
        private Brush _ColorCurr24V;
        public Brush ColorCurr24V { get { return _ColorCurr24V; } set { SetProperty(ref _ColorCurr24V, value); } }

        //3V系 消費電流
        private Brush _ColorCurr3V;
        public Brush ColorCurr3V { get { return _ColorCurr3V; } set { SetProperty(ref _ColorCurr3V, value); } }

        //Vcc電圧
        private Brush _ColorVolVcc;
        public Brush ColorVolVcc { get { return _ColorVolVcc; } set { SetProperty(ref _ColorVolVcc, value); } }

        //-5V電圧
        private Brush _ColorVolMinus5V;
        public Brush ColorVolMinus5V { get { return _ColorVolMinus5V; } set { SetProperty(ref _ColorVolMinus5V, value); } }

        //Vref電圧
        private Brush _ColorVolVref;
        public Brush ColorVolVref { get { return _ColorVolVref; } set { SetProperty(ref _ColorVolVref, value); } }

        //リチウム電池電圧
        private Brush _ColorVolBatt;
        public Brush ColorVolBatt { get { return _ColorVolBatt; } set { SetProperty(ref _ColorVolBatt, value); } }

        //D4両端電圧
        private Brush _ColorVolD4;
        public Brush ColorVolD4 { get { return _ColorVolD4; } set { SetProperty(ref _ColorVolD4, value); } }

        //K1コイル電流
        private Brush _ColorCurrK1;
        public Brush ColorCurrK1 { get { return _ColorCurrK1; } set { SetProperty(ref _ColorCurrK1, value); } }

        //K2コイル電流
        private Brush _ColorCurrK2;
        public Brush ColorCurrK2 { get { return _ColorCurrK2; } set { SetProperty(ref _ColorCurrK2, value); } }

        //K3コイル電流
        private Brush _ColorCurrK3;
        public Brush ColorCurrK3 { get { return _ColorCurrK3; } set { SetProperty(ref _ColorCurrK3, value); } }

        //K4コイル電流
        private Brush _ColorCurrK4;
        public Brush ColorCurrK4 { get { return _ColorCurrK4; } set { SetProperty(ref _ColorCurrK4, value); } }

        //K5コイル電流
        private Brush _ColorCurrK5;
        public Brush ColorCurrK5 { get { return _ColorCurrK5; } set { SetProperty(ref _ColorCurrK5, value); } }

        //AN0 0V
        private Brush _ColorAn0_00V;
        public Brush ColorAn0_00V { get { return _ColorAn0_00V; } set { SetProperty(ref _ColorAn0_00V, value); } }

        //AN0 2.5V
        private Brush _ColorAn0_25V;
        public Brush ColorAn0_25V { get { return _ColorAn0_25V; } set { SetProperty(ref _ColorAn0_25V, value); } }

        //AN0 5.0V
        private Brush _ColorAn0_50V;
        public Brush ColorAn0_50V { get { return _ColorAn0_50V; } set { SetProperty(ref _ColorAn0_50V, value); } }


        //AN1 0V
        private Brush _ColorAn1_00V;
        public Brush ColorAn1_00V { get { return _ColorAn1_00V; } set { SetProperty(ref _ColorAn1_00V, value); } }

        //AN1 2.5V
        private Brush _ColorAn1_25V;
        public Brush ColorAn1_25V { get { return _ColorAn1_25V; } set { SetProperty(ref _ColorAn1_25V, value); } }

        //AN1 5.0V
        private Brush _ColorAn1_50V;
        public Brush ColorAn1_50V { get { return _ColorAn1_50V; } set { SetProperty(ref _ColorAn1_50V, value); } }

        //AN2 0V
        private Brush _ColorAn2_00V;
        public Brush ColorAn2_00V { get { return _ColorAn2_00V; } set { SetProperty(ref _ColorAn2_00V, value); } }

        //AN2 2.5V
        private Brush _ColorAn2_25V;
        public Brush ColorAn2_25V { get { return _ColorAn2_25V; } set { SetProperty(ref _ColorAn2_25V, value); } }

        //AN2 5.0V
        private Brush _ColorAn2_50V;
        public Brush ColorAn2_50V { get { return _ColorAn2_50V; } set { SetProperty(ref _ColorAn2_50V, value); } }

        //AN3 0V
        private Brush _ColorAn3_00V;
        public Brush ColorAn3_00V { get { return _ColorAn3_00V; } set { SetProperty(ref _ColorAn3_00V, value); } }

        //AN3 5.0V
        private Brush _ColorAn3_50V;
        public Brush ColorAn3_50V { get { return _ColorAn3_50V; } set { SetProperty(ref _ColorAn3_50V, value); } }

        //AN3 10.0V
        private Brush _ColorAn3_100V;
        public Brush ColorAn3_100V { get { return _ColorAn3_100V; } set { SetProperty(ref _ColorAn3_100V, value); } }

        //RTC HOST側時刻
        private Brush _ColorTimeHost;
        public Brush ColorTimeHost { get { return _ColorTimeHost; } set { SetProperty(ref _ColorTimeHost, value); } }

        //RTC 製品側時刻
        private Brush _ColorTimeTarget;
        public Brush ColorTimeTarget { get { return _ColorTimeTarget; } set { SetProperty(ref _ColorTimeTarget, value); } }

        //RTC Hostと製品の時刻誤差
        private Brush _ColorRtcTimeError;
        public Brush ColorRtcTimeError { get { return _ColorRtcTimeError; } set { SetProperty(ref _ColorRtcTimeError, value); } }

        //RTC HOST側時刻(最終設定)
        private Brush _ColorTimeHost2;
        public Brush ColorTimeHost2 { get { return _ColorTimeHost2; } set { SetProperty(ref _ColorTimeHost2, value); } }

        //RTC 製品側時刻(最終設定)
        private Brush _ColorTimeTarget2;
        public Brush ColorTimeTarget2 { get { return _ColorTimeTarget2; } set { SetProperty(ref _ColorTimeTarget2, value); } }

        //RTC Hostと製品の時刻誤差(最終設定)
        private Brush _ColorRtcTimeError2;
        public Brush ColorRtcTimeError2 { get { return _ColorRtcTimeError2; } set { SetProperty(ref _ColorRtcTimeError2, value); } }



        //温度計表示値
        private Brush _ColorTempUSBRH;
        public Brush ColorTempUSBRH { get { return _ColorTempUSBRH; } set { SetProperty(ref _ColorTempUSBRH, value); } }

        //製品温度補償ICの表示値
        private Brush _ColorTempTarget;
        public Brush ColorTempTarget { get { return _ColorTempTarget; } set { SetProperty(ref _ColorTempTarget, value); } }

        //Loop1補正値
        private Brush _ColorLoop1Ad;
        public Brush ColorLoop1Ad { get { return _ColorLoop1Ad; } set { SetProperty(ref _ColorLoop1Ad, value); } }

        //Loop2補正値
        private Brush _ColorLoop2Ad;
        public Brush ColorLoop2Ad { get { return _ColorLoop2Ad; } set { SetProperty(ref _ColorLoop2Ad, value); } }

        //CH1 25℃
        private Brush _ColorCh1_25;
        public Brush ColorCh1_25 { get { return _ColorCh1_25; } set { SetProperty(ref _ColorCh1_25, value); } }

        //CH1 100℃
        private Brush _ColorCh1_100;
        public Brush ColorCh1_100 { get { return _ColorCh1_100; } set { SetProperty(ref _ColorCh1_100, value); } }

        //CH1 400℃
        private Brush _ColorCh1_400;
        public Brush ColorCh1_400 { get { return _ColorCh1_400; } set { SetProperty(ref _ColorCh1_400, value); } }


        //CH2 25℃
        private Brush _ColorCh2_25;
        public Brush ColorCh2_25 { get { return _ColorCh2_25; } set { SetProperty(ref _ColorCh2_25, value); } }

        //CH2 100℃
        private Brush _ColorCh2_100;
        public Brush ColorCh2_100 { get { return _ColorCh2_100; } set { SetProperty(ref _ColorCh2_100, value); } }

        //CH2 400℃
        private Brush _ColorCh2_400;
        public Brush ColorCh2_400 { get { return _ColorCh2_400; } set { SetProperty(ref _ColorCh2_400, value); } }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //K1 ON/OFF 期待値
        private Brush _ColorK1Exp;
        public Brush ColorK1Exp { get { return _ColorK1Exp; } set { SetProperty(ref _ColorK1Exp, value); } }

        //K2 ON/OFF 期待値
        private Brush _ColorK2Exp;
        public Brush ColorK2Exp { get { return _ColorK2Exp; } set { SetProperty(ref _ColorK2Exp, value); } }

        //K3 ON/OFF 期待値
        private Brush _ColorK3Exp;
        public Brush ColorK3Exp { get { return _ColorK3Exp; } set { SetProperty(ref _ColorK3Exp, value); } }

        //K4 ON/OFF 期待値
        private Brush _ColorK4Exp;
        public Brush ColorK4Exp { get { return _ColorK4Exp; } set { SetProperty(ref _ColorK4Exp, value); } }

        //K5 ON/OFF 期待値
        private Brush _ColorK5Exp;
        public Brush ColorK5Exp { get { return _ColorK5Exp; } set { SetProperty(ref _ColorK5Exp, value); } }

        //K1 ON/OFF 出力値
        private Brush _ColorK1Out;
        public Brush ColorK1Out { get { return _ColorK1Out; } set { SetProperty(ref _ColorK1Out, value); } }

        //K2 ON/OFF 出力値
        private Brush _ColorK2Out;
        public Brush ColorK2Out { get { return _ColorK2Out; } set { SetProperty(ref _ColorK2Out, value); } }

        //K3 ON/OFF 出力値
        private Brush _ColorK3Out;
        public Brush ColorK3Out { get { return _ColorK3Out; } set { SetProperty(ref _ColorK3Out, value); } }

        //K4 ON/OFF 出力値
        private Brush _ColorK4Out;
        public Brush ColorK4Out { get { return _ColorK4Out; } set { SetProperty(ref _ColorK4Out, value); } }

        //K5 ON/OFF 出力値
        private Brush _ColorK5Out;
        public Brush ColorK5Out { get { return _ColorK5Out; } set { SetProperty(ref _ColorK5Out, value); } }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //デジタル出力 000 期待値
        private Brush _ColorDo000Exp;
        public Brush ColorDo000Exp { get { return _ColorDo000Exp; } set { SetProperty(ref _ColorDo000Exp, value); } }

        //デジタル出力 001 期待値
        private Brush _ColorDo001Exp;
        public Brush ColorDo001Exp { get { return _ColorDo001Exp; } set { SetProperty(ref _ColorDo001Exp, value); } }

        //デジタル出力 002 期待値
        private Brush _ColorDo002Exp;
        public Brush ColorDo002Exp { get { return _ColorDo002Exp; } set { SetProperty(ref _ColorDo002Exp, value); } }

        //デジタル出力 003 期待値
        private Brush _ColorDo003Exp;
        public Brush ColorDo003Exp { get { return _ColorDo003Exp; } set { SetProperty(ref _ColorDo003Exp, value); } }

        //デジタル出力 004 期待値
        private Brush _ColorDo004Exp;
        public Brush ColorDo004Exp { get { return _ColorDo004Exp; } set { SetProperty(ref _ColorDo004Exp, value); } }

        //デジタル出力 005 期待値
        private Brush _ColorDo005Exp;
        public Brush ColorDo005Exp { get { return _ColorDo005Exp; } set { SetProperty(ref _ColorDo005Exp, value); } }

        //デジタル出力 006 期待値
        private Brush _ColorDo006Exp;
        public Brush ColorDo006Exp { get { return _ColorDo006Exp; } set { SetProperty(ref _ColorDo006Exp, value); } }

        //デジタル出力 007 期待値
        private Brush _ColorDo007Exp;
        public Brush ColorDo007Exp { get { return _ColorDo007Exp; } set { SetProperty(ref _ColorDo007Exp, value); } }

        //デジタル出力 008 期待値
        private Brush _ColorDo008Exp;
        public Brush ColorDo008Exp { get { return _ColorDo008Exp; } set { SetProperty(ref _ColorDo008Exp, value); } }

        //デジタル出力 009 期待値
        private Brush _ColorDo009Exp;
        public Brush ColorDo009Exp { get { return _ColorDo009Exp; } set { SetProperty(ref _ColorDo009Exp, value); } }

        //デジタル出力 010 期待値
        private Brush _ColorDo010Exp;
        public Brush ColorDo010Exp { get { return _ColorDo010Exp; } set { SetProperty(ref _ColorDo010Exp, value); } }

        //デジタル出力 011 期待値
        private Brush _ColorDo011Exp;
        public Brush ColorDo011Exp { get { return _ColorDo011Exp; } set { SetProperty(ref _ColorDo011Exp, value); } }

        //デジタル出力 400 期待値
        private Brush _ColorDo400Exp;
        public Brush ColorDo400Exp { get { return _ColorDo400Exp; } set { SetProperty(ref _ColorDo400Exp, value); } }

        //デジタル出力 401 期待値
        private Brush _ColorDo401Exp;
        public Brush ColorDo401Exp { get { return _ColorDo401Exp; } set { SetProperty(ref _ColorDo401Exp, value); } }

        //デジタル出力 000 期待値
        private Brush _ColorDo000Out;
        public Brush ColorDo000Out { get { return _ColorDo000Out; } set { SetProperty(ref _ColorDo000Out, value); } }

        //デジタル出力 001 期待値
        private Brush _ColorDo001Out;
        public Brush ColorDo001Out { get { return _ColorDo001Out; } set { SetProperty(ref _ColorDo001Out, value); } }

        //デジタル出力 002 期待値
        private Brush _ColorDo002Out;
        public Brush ColorDo002Out { get { return _ColorDo002Out; } set { SetProperty(ref _ColorDo002Out, value); } }

        //デジタル出力 003 期待値
        private Brush _ColorDo003Out;
        public Brush ColorDo003Out { get { return _ColorDo003Out; } set { SetProperty(ref _ColorDo003Out, value); } }

        //デジタル出力 004 期待値
        private Brush _ColorDo004Out;
        public Brush ColorDo004Out { get { return _ColorDo004Out; } set { SetProperty(ref _ColorDo004Out, value); } }

        //デジタル出力 005 期待値
        private Brush _ColorDo005Out;
        public Brush ColorDo005Out { get { return _ColorDo005Out; } set { SetProperty(ref _ColorDo005Out, value); } }

        //デジタル出力 006 期待値
        private Brush _ColorDo006Out;
        public Brush ColorDo006Out { get { return _ColorDo006Out; } set { SetProperty(ref _ColorDo006Out, value); } }

        //デジタル出力 007 期待値
        private Brush _ColorDo007Out;
        public Brush ColorDo007Out { get { return _ColorDo007Out; } set { SetProperty(ref _ColorDo007Out, value); } }

        //デジタル出力 008 期待値
        private Brush _ColorDo008Out;
        public Brush ColorDo008Out { get { return _ColorDo008Out; } set { SetProperty(ref _ColorDo008Out, value); } }

        //デジタル出力 009 期待値
        private Brush _ColorDo009Out;
        public Brush ColorDo009Out { get { return _ColorDo009Out; } set { SetProperty(ref _ColorDo009Out, value); } }

        //デジタル出力 010 期待値
        private Brush _ColorDo010Out;
        public Brush ColorDo010Out { get { return _ColorDo010Out; } set { SetProperty(ref _ColorDo010Out, value); } }

        //デジタル出力 011 期待値
        private Brush _ColorDo011Out;
        public Brush ColorDo011Out { get { return _ColorDo011Out; } set { SetProperty(ref _ColorDo011Out, value); } }

        //デジタル出力 400 期待値
        private Brush _ColorDo400Out;
        public Brush ColorDo400Out { get { return _ColorDo400Out; } set { SetProperty(ref _ColorDo400Out, value); } }

        //デジタル出力 401 期待値
        private Brush _ColorDo401Out;
        public Brush ColorDo401Out { get { return _ColorDo401Out; } set { SetProperty(ref _ColorDo401Out, value); } }

        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //デジタル入力 001 期待値
        private Brush _ColorDi000Exp;
        public Brush ColorDi000Exp { get { return _ColorDi000Exp; } set { SetProperty(ref _ColorDi000Exp, value); } }

        //デジタル入力 001 期待値
        private Brush _ColorDi001Exp;
        public Brush ColorDi001Exp { get { return _ColorDi001Exp; } set { SetProperty(ref _ColorDi001Exp, value); } }

        //デジタル入力 002 期待値
        private Brush _ColorDi002Exp;
        public Brush ColorDi002Exp { get { return _ColorDi002Exp; } set { SetProperty(ref _ColorDi002Exp, value); } }

        //デジタル入力 003 期待値
        private Brush _ColorDi003Exp;
        public Brush ColorDi003Exp { get { return _ColorDi003Exp; } set { SetProperty(ref _ColorDi003Exp, value); } }

        //デジタル入力 004 期待値
        private Brush _ColorDi004Exp;
        public Brush ColorDi004Exp { get { return _ColorDi004Exp; } set { SetProperty(ref _ColorDi004Exp, value); } }

        //デジタル入力 005 期待値
        private Brush _ColorDi005Exp;
        public Brush ColorDi005Exp { get { return _ColorDi005Exp; } set { SetProperty(ref _ColorDi005Exp, value); } }

        //デジタル入力 006 期待値
        private Brush _ColorDi006Exp;
        public Brush ColorDi006Exp { get { return _ColorDi006Exp; } set { SetProperty(ref _ColorDi006Exp, value); } }

        //デジタル入力 007 期待値
        private Brush _ColorDi007Exp;
        public Brush ColorDi007Exp { get { return _ColorDi007Exp; } set { SetProperty(ref _ColorDi007Exp, value); } }

        //デジタル入力 008 期待値
        private Brush _ColorDi008Exp;
        public Brush ColorDi008Exp { get { return _ColorDi008Exp; } set { SetProperty(ref _ColorDi008Exp, value); } }

        //デジタル入力 009 期待値
        private Brush _ColorDi009Exp;
        public Brush ColorDi009Exp { get { return _ColorDi009Exp; } set { SetProperty(ref _ColorDi009Exp, value); } }

        //デジタル入力 010 期待値
        private Brush _ColorDi010Exp;
        public Brush ColorDi010Exp { get { return _ColorDi010Exp; } set { SetProperty(ref _ColorDi010Exp, value); } }

        //デジタル入力 011 期待値
        private Brush _ColorDi011Exp;
        public Brush ColorDi011Exp { get { return _ColorDi011Exp; } set { SetProperty(ref _ColorDi011Exp, value); } }

        //デジタル入力 012 期待値
        private Brush _ColorDi012Exp;
        public Brush ColorDi012Exp { get { return _ColorDi012Exp; } set { SetProperty(ref _ColorDi012Exp, value); } }

        //デジタル入力 013 期待値
        private Brush _ColorDi013Exp;
        public Brush ColorDi013Exp { get { return _ColorDi013Exp; } set { SetProperty(ref _ColorDi013Exp, value); } }

        //デジタル入力 014 期待値
        private Brush _ColorDi014Exp;
        public Brush ColorDi014Exp { get { return _ColorDi014Exp; } set { SetProperty(ref _ColorDi014Exp, value); } }

        //デジタル入力 015 期待値
        private Brush _ColorDi015Exp;
        public Brush ColorDi015Exp { get { return _ColorDi015Exp; } set { SetProperty(ref _ColorDi015Exp, value); } }

        //デジタル入力 400 期待値
        private Brush _ColorDi400Exp;
        public Brush ColorDi400Exp { get { return _ColorDi400Exp; } set { SetProperty(ref _ColorDi400Exp, value); } }

        //デジタル入力 000 入力値
        private Brush _ColorDi000In;
        public Brush ColorDi000In { get { return _ColorDi000In; } set { SetProperty(ref _ColorDi000In, value); } }
        //デジタル入力 001 入力値
        private Brush _ColorDi001In;
        public Brush ColorDi001In { get { return _ColorDi001In; } set { SetProperty(ref _ColorDi001In, value); } }

        //デジタル入力 002 入力値
        private Brush _ColorDi002In;
        public Brush ColorDi002In { get { return _ColorDi002In; } set { SetProperty(ref _ColorDi002In, value); } }

        //デジタル入力 003 入力値
        private Brush _ColorDi003In;
        public Brush ColorDi003In { get { return _ColorDi003In; } set { SetProperty(ref _ColorDi003In, value); } }

        //デジタル入力 004 入力値
        private Brush _ColorDi004In;
        public Brush ColorDi004In { get { return _ColorDi004In; } set { SetProperty(ref _ColorDi004In, value); } }

        //デジタル入力 005 入力値
        private Brush _ColorDi005In;
        public Brush ColorDi005In { get { return _ColorDi005In; } set { SetProperty(ref _ColorDi005In, value); } }

        //デジタル入力 006 入力値
        private Brush _ColorDi006In;
        public Brush ColorDi006In { get { return _ColorDi006In; } set { SetProperty(ref _ColorDi006In, value); } }

        //デジタル入力 007 入力値
        private Brush _ColorDi007In;
        public Brush ColorDi007In { get { return _ColorDi007In; } set { SetProperty(ref _ColorDi007In, value); } }

        //デジタル入力 008 入力値
        private Brush _ColorDi008In;
        public Brush ColorDi008In { get { return _ColorDi008In; } set { SetProperty(ref _ColorDi008In, value); } }

        //デジタル入力 009 入力値
        private Brush _ColorDi009In;
        public Brush ColorDi009In { get { return _ColorDi009In; } set { SetProperty(ref _ColorDi009In, value); } }

        //デジタル入力 010 入力値
        private Brush _ColorDi010In;
        public Brush ColorDi010In { get { return _ColorDi010In; } set { SetProperty(ref _ColorDi010In, value); } }

        //デジタル入力 011 入力値
        private Brush _ColorDi011In;
        public Brush ColorDi011In { get { return _ColorDi011In; } set { SetProperty(ref _ColorDi011In, value); } }

        //デジタル入力 012 入力値
        private Brush _ColorDi012In;
        public Brush ColorDi012In { get { return _ColorDi012In; } set { SetProperty(ref _ColorDi012In, value); } }

        //デジタル入力 013 入力値
        private Brush _ColorDi013In;
        public Brush ColorDi013In { get { return _ColorDi013In; } set { SetProperty(ref _ColorDi013In, value); } }

        //デジタル入力 014 入力値
        private Brush _ColorDi014In;
        public Brush ColorDi014In { get { return _ColorDi014In; } set { SetProperty(ref _ColorDi014In, value); } }

        //デジタル入力 015 入力値
        private Brush _ColorDi015In;
        public Brush ColorDi015In { get { return _ColorDi015In; } set { SetProperty(ref _ColorDi015In, value); } }

        //デジタル入力 400 入力値
        private Brush _ColorDi400In;
        public Brush ColorDi400In { get { return _ColorDi400In; } set { SetProperty(ref _ColorDi400In, value); } }
    }
}








