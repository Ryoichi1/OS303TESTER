
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace NewPC81Tester
{
    public class TestCommand
    {

        //デリゲートの宣言
        public Action RefreshDataContext;//Test.Xaml内でテスト結果をクリアするために使用すする
        public Action SbRingLoad;
        public Action SbPass;
        public Action SbFail;

        private bool FlagTestTime;

        DropShadowEffect effect判定表示PASS;
        DropShadowEffect effect判定表示FAIL;

        public TestCommand()
        {
            effect判定表示PASS = new DropShadowEffect();
            effect判定表示PASS.Color = Colors.Aqua;
            effect判定表示PASS.Direction = 0;
            effect判定表示PASS.ShadowDepth = 0;
            effect判定表示PASS.Opacity = 1.0;
            effect判定表示PASS.BlurRadius = 80;

            effect判定表示FAIL = new DropShadowEffect();
            effect判定表示FAIL.Color = Colors.HotPink; ;
            effect判定表示FAIL.Direction = 0;
            effect判定表示FAIL.ShadowDepth = 0;
            effect判定表示FAIL.Opacity = 1.0;
            effect判定表示FAIL.BlurRadius = 40;

        }

        public async Task StartCheck()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                RETRY:
                    while (true)
                    {
                        if (Flags.OtherPage) break;
                        Thread.Sleep(200);

                        //作業者名、工番が正しく入力されているかの判定
                        if (!Flags.SetOperator)
                        {
                            State.VmTestStatus.Message = Constants.MessOperator;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        if (!Flags.SetOpecode)
                        {
                            State.VmTestStatus.Message = Constants.MessOpecode;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        if (!Flags.AllOk周辺機器接続)
                        {
                            State.VmTestStatus.Message = Constants.MessCheckConnectMachine;
                            Flags.EnableTestStart = false;
                            continue;
                        }

                        State.VmTestStatus.Message = Constants.MessSet;
                        Flags.EnableTestStart = true;
                        Flags.Click確認Button = false;

                        while (true)
                        {
                            if (Flags.OtherPage || Flags.Click確認Button) break;
                            if (!Flags.SetOperator || !Flags.SetOpecode) goto RETRY;
                        }
                        break;
                    }

                });

                if (Flags.OtherPage) return;

                State.VmMainWindow.EnableOtherButton = false;
                State.VmTestStatus.StartButtonContent = Constants.停止;
                State.VmTestStatus.TestSettingEnable = false;
                State.VmMainWindow.OperatorEnable = false;
                await Test();//メインルーチンへ


                //試験合格後、ラベル貼り付けページを表示する場合は下記のステップを追加すること
                //if (Flags.ShowLabelPage) return;

                //日常点検合格、一項目試験合格、試験NGの場合は、Whileループを繰り返す
                //通常試験合格の場合は、ラベル貼り付けフォームがロードされた時点で、一旦StartCheckメソッドを終了します
                //その後、ラベル貼り付けフォームが閉じられた後に、Test.xamlがリロードされ、そのフォームロードイベントでStartCheckメソッドがコールされます

            }

        }

        private void Timer()
        {
            var t = Task.Run(() =>
            {
                //Stopwatchオブジェクトを作成する
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                while (FlagTestTime)
                {
                    Thread.Sleep(200);
                    State.VmTestStatus.TestTime = sw.Elapsed.ToString().Substring(3, 5);
                }
                sw.Stop();
            });
        }

        //メインルーチン
        public async Task Test()
        {
            Flags.Testing = true;
            Flags.Flag電池セット = true;

            General.SetMetalMode();
            General.SetBgm();
            State.VmTestStatus.Message = Constants.MessWait;

            //現在のテーマ透過度の保存
            State.CurrentThemeOpacity = State.VmMainWindow.ThemeOpacity;
            State.VmMainWindow.ThemeOpacity = Constants.OpacityMin;

            await Task.Delay(500);

            FlagTestTime = true;
            Timer();

            int FailStepNo = 0;
            int RetryCnt = 0;//リトライ用に使用する
            string FailTitle = "";




            var テスト項目最新 = new List<TestSpecs>();
            if (State.VmTestStatus.CheckUnitTest == true)
            {
                //チェックしてある項目の百の桁の解析
                var re = Int32.Parse(State.VmTestStatus.UnitTestName.Split('_').ToArray()[0]);
                int 上位桁 = Int32.Parse(State.VmTestStatus.UnitTestName.Substring(0, (re >= 1000) ? 2 : 1));
                var 抽出データ = State.テスト項目.Where(p => (p.Key / 100) == 上位桁);
                foreach (var p in 抽出データ)
                {
                    テスト項目最新.Add(new TestSpecs(p.Key, p.Value, p.PowSw));
                }
            }
            else
            {
                テスト項目最新 = State.テスト項目;
            }



            try
            {
                //IO初期化
                General.ResetIo();
                Thread.Sleep(400);


                foreach (var d in テスト項目最新.Select((s, i) => new { i, s }))
                {
                Retry:
                    State.VmTestStatus.Spec = "規格値 : ---";
                    State.VmTestStatus.MeasValue = "計測値 : ---";
                    Flags.AddDecision = true;

                    SetTestLog(d.s.Key.ToString() + "_" + d.s.Value);

                    if (d.s.PowSw)
                    {
                        if (!Flags.PowOn)
                        {
                            Target.ClearBuff();
                            General.PowSupply(true);
                            if (!General.CheckComm())
                            {
                                Flags.AddDecision = false;
                                SetTestLog("\r\n REQ DATA 受信異常！！！");
                                goto FAIL;
                            }
                            await Task.Delay(500);
                        }
                    }
                    else
                    {
                        General.PowSupply(false);
                        await Task.Delay(200);
                    }

                    switch (d.s.Key)
                    {
                        case 000://リチウム電池有無確認
                            if (await 電圧チェック.CheckVolt(電圧チェック.CH.BATTERY_ARI_NASHI)) break;
                            goto case 5000;

                        case 100://コネクタ実装チェック
                            if (await コネクタチェック.CheckCn()) break;
                            goto case 5000;

                        case 200://テストプログラム書き込み
                            if (State.VmTestStatus.CheckWriteFw == true) break;
                            if (await 書き込み.WriteFw(書き込み.WriteMode.TEST)) break;
                            goto case 5000;

                        case 300://消費電流チェック 24Vライン
                            if (await 消費電流チェック.CheckCurrent24V()) break;
                            goto case 5000;

                        case 400://消費電流チェック 3Vライン
                            if (await 消費電流チェック.CheckCurrent3V()) break;
                            goto case 5000;

                        case 500://電源電圧チェック VCC
                            if (await 電圧チェック.CheckVolt(電圧チェック.CH.VCC)) break;
                            goto case 5000;
                        case 501://電源電圧チェック -5V
                            if (await 電圧チェック.CheckVolt(電圧チェック.CH.MINUS5V)) break;
                            goto case 5000;

                        case 502://電源電圧チェック Vref
                            if (await 電圧チェック.CheckVolt(電圧チェック.CH.VREF)) break;
                            goto case 5000;

                        case 600://RS422 PORT1チェック
                            if (await シリアル通信チェック.Check422(シリアル通信チェック.CH.Port1)) break;
                            goto case 5000;
                        case 601://RS422 PORT2チェック
                            if (await シリアル通信チェック.Check422(シリアル通信チェック.CH.Port2)) break;
                            goto case 5000;
                        case 602://RS422 PORT3チェック
                            if (await シリアル通信チェック.Check422(シリアル通信チェック.CH.Port3)) break;
                            goto case 5000;
                        case 603://RS422 PORT4チェック
                            if (await シリアル通信チェック.Check422(シリアル通信チェック.CH.Port4)) break;
                            goto case 5000;
                        case 604://RS422 CN202チェック
                            if (await シリアル通信チェック.Check422(シリアル通信チェック.CH.CN202)) break;
                            goto case 5000;

                        case 700://RTCセット
                            if (await RTCチェック.SetTime()) break;
                            goto case 5000;

                        case 800://LED点灯チェック
                            if (await Test_Led.CheckLed(true)) break;
                            goto case 5000;

                        case 801://LED消灯チェック
                            if (await Test_Led.CheckLed(false)) break;
                            goto case 5000;
                        case 900://リセットスイッチ動作チェック
                            Target.ClearBuff();
                            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.H);
                            await Task.Delay(300);
                            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.L);
                            if (General.CheckComm()) break;
                            goto case 5000;

                        case 1000://リレー動作チェック
                            if (await リレーチェック.CheckRelay()) break;
                            goto case 5000;

                        case 1100://デジタル出力チェック
                            if (await デジタル出力チェック.CheckDout()) break;
                            goto case 5000;

                        case 1200://デジタル入力チェック
                            if (await デジタル入力チェック.CheckDinput()) break;
                            goto case 5000;
                        case 1300://AN１ アナログ入力チェック
                            if (await アナログ入力チェック.CheckAnIn(アナログ入力チェック.NAME.AN0)) break;
                            goto case 5000;
                        case 1301://AN2 アナログ入力チェック
                            if (await アナログ入力チェック.CheckAnIn(アナログ入力チェック.NAME.AN1)) break;
                            goto case 5000;
                        case 1302://AN3 アナログ入力チェック
                            if (await アナログ入力チェック.CheckAnIn(アナログ入力チェック.NAME.AN2)) break;
                            goto case 5000;
                        case 1303://AN4 アナログ入力チェック
                            if (await アナログ入力チェック.CheckAnIn(アナログ入力チェック.NAME.AN3)) break;
                            goto case 5000;

                        case 1400://EEPROMチェック
                            State.VmTestStatus.IsActiveRing = true;
                            if (await EEPROMチェック.CheckEeprom())
                            {
                                State.VmTestStatus.IsActiveRing = false;
                                break;
                            }
                            goto case 5000;

                        case 1401://EEPROM初期化
                            if (await EEPROMチェック.InitEeprom()) break;
                            goto case 5000;

                        case 1402://シリアルナンバー書き込み
                            if (await EEPROMチェック.SetSerial(State.VmMainWindow.SerialNumber)) break;
                            goto case 5000;

                        case 1500://熱電対 補正値取得
                            General.SetDcFun(true);
                            State.VmTestStatus.IsActiveRing = true;
                            if (!await 熱電対チェック.InitLoop()) goto case 5000;
                            State.VmTestStatus.IsActiveRing = false;
                            if (!await 熱電対チェック.CheckU6()) goto case 5000;
                            if (!await 熱電対チェック.CheckTemp(熱電対チェック.NAME.AN4)) goto case 5000;
                            if (!await 熱電対チェック.CheckTemp(熱電対チェック.NAME.AN5)) goto case 5000;
                            General.SetDcFun(false);
                            break;

                        case 1600://リアルタイムクロック チェック
                            if (await RTCチェック.CheckTime()) break;
                            goto case 5000;

                        case 1700://RS232C PORT1チェック
                            if (await シリアル通信チェック.Check232C(シリアル通信チェック.CH.Port1)) break;
                            goto case 5000;
                        case 1701://RS232C PORT2チェック
                            if (await シリアル通信チェック.Check232C(シリアル通信チェック.CH.Port2)) break;
                            goto case 5000;
                        case 1702://RS232C PORT3チェック
                            if (await シリアル通信チェック.Check232C(シリアル通信チェック.CH.Port3)) break;
                            goto case 5000;
                        case 1703://RS232C PORT4チェック
                            if (await シリアル通信チェック.Check232C(シリアル通信チェック.CH.Port4)) break;
                            goto case 5000;

                        case 1800://S1チェック(1,3ON  2,4OFF)
                            if (await スイッチチェック.CheckS1(スイッチチェック.S1検査.ON_13))
                            {
                                break;
                            }
                            else
                            {
                                General.PowSupply(false);
                                await Task.Delay(500);
                                State.VmTestStatus.Message = "プレス治具を開けてください！！！";
                                General.PlaySoundLoop(General.soundAlarm);
                                await Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        if (Flags.ClickStopButton || General.CheckPressOpen()) break;
                                    }

                                });

                                General.StopSound();
                                if (Flags.ClickStopButton) goto case 5000;


                                State.VmTestStatus.Message = "Ｓ１の１番と３番をＯＮして、プレス閉じてください！！！";
                                await Task.Run(() =>
                                {
                                    while (true)
                                    {
                                        if (Flags.ClickStopButton || !General.CheckPressOpen()) break;
                                    }

                                });

                                if (Flags.ClickStopButton) goto case 5000;


                                Target.ClearBuff();
                                General.PowSupply(true);
                                if (!General.CheckComm())
                                {
                                    Flags.AddDecision = false;
                                    SetTestLog("\r\n REQ DATA 受信異常！！！");
                                    goto case 5000;
                                }
                                await Task.Delay(500);
                                if (await スイッチチェック.CheckS1(スイッチチェック.S1検査.ON_13)) break;
                                goto case 5000;
                            }

                        case 1801://S1チェック(1,3OFF  2,4ON)
                            General.PowSupply(false);
                            await Task.Delay(500);
                            State.VmTestStatus.Message = "プレス治具を開けてください！！！";
                            General.PlaySoundLoop(General.soundAlarm);
                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    if (Flags.ClickStopButton || General.CheckPressOpen()) break;
                                }

                            });

                            General.StopSound();
                            if (Flags.ClickStopButton) goto case 5000;

                            State.VmTestStatus.Message = "Ｓ１の２番と４番をＯＮして、プレス閉じてください！！！";
                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    if (Flags.ClickStopButton || !General.CheckPressOpen()) break;
                                }

                            });

                            if (Flags.ClickStopButton) goto case 5000;

                            Target.ClearBuff();
                            General.PowSupply(true);
                            if (!General.CheckComm())
                            {
                                Flags.AddDecision = false;
                                SetTestLog("\r\n REQ DATA 受信異常！！！");
                                goto case 5000;
                            }
                            await Task.Delay(500);
                            if (await スイッチチェック.CheckS1(スイッチチェック.S1検査.ON_24)) break;
                            goto case 5000;

                        case 1802://S1チェック(1のみON ※出荷設定)
                            General.PowSupply(false);
                            await Task.Delay(500);
                            State.VmTestStatus.Message = "プレス治具を開けてください！！！";
                            General.PlaySoundLoop(General.soundAlarm);
                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    if (Flags.ClickStopButton || General.CheckPressOpen()) break;
                                }

                            });

                            General.StopSound();
                            if (Flags.ClickStopButton) goto case 5000;

                            State.VmTestStatus.Message = "Ｓ１の１番だけをＯＮして、プレスを閉じてください！！！";
                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    if (Flags.ClickStopButton || !General.CheckPressOpen()) break;
                                }

                            });

                            if (Flags.ClickStopButton) goto case 5000;


                            Target.ClearBuff();
                            General.PowSupply(true);
                            if (!General.CheckComm())
                            {
                                Flags.AddDecision = false;
                                SetTestLog("\r\n REQ DATA 受信異常！！！");
                                goto case 5000;
                            }
                            await Task.Delay(500);
                            if (await スイッチチェック.CheckS1(スイッチチェック.S1検査.ON_1))
                            {
                                State.VmTestStatus.Message = Constants.MessWait;
                                //メタルモード時はBGMが消えているのでここで再生する
                                General.SetBgm();
                                break;
                            }
                            goto case 5000;

                        case 1900://製品プログラム書き込み
                            if (await 書き込み.WriteFw(書き込み.WriteMode.PRODUCT)) break;
                            goto case 5000;

                        case 2000://電池組み付け
                            General.ResetIo();
                            await Task.Delay(200);
                            General.PowSupply(true);
                            await Task.Delay(500);
                            State.VmTestStatus.Message = "プレス治具を開けてください！！！";
                            General.PlaySoundLoop(General.soundAlarm);
                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    if (Flags.ClickStopButton || General.CheckPressOpen()) break;
                                }

                            });

                            General.StopSound();
                            if (Flags.ClickStopButton) goto case 5000;

                            General.PowSupply(false);
                            General.StopSound();

                            State.VmTestStatus.Message = "1分以内に電池をｾｯﾄしてプレスを閉めてください";

                            bool Flag1MinutePassed = false;
                            int countDown = 60; //4分30秒で3V以下になるので、余裕を持って1分以内に電池をセットできたらOKとする
                            var tm = new System.Timers.Timer();
                            tm.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                            {
                                countDown = countDown - 1;
                                if (countDown == 0)
                                {
                                    tm.Stop();
                                    Flag1MinutePassed = true;
                                }
                            };
                            tm.Interval = 1000;
                            tm.Start();

                            General.PlaySoundLoop(General.soundContinue);
                            //プレスが閉じるまで待つ
                            await Task.Run(() =>
                            {
                                while (true)
                                {
                                    State.VmTestStatus.Message = "1分以内に電池をセットしてプレスを閉めてください  残り " + countDown.ToString() + "秒";
                                    if (Flags.ClickStopButton || !General.CheckPressOpen() || Flag1MinutePassed) break;
                                }

                            });
                            tm.Stop();
                            State.VmTestStatus.Message = "1分以内に電池をセットしてプレスを閉めてください  残り " + countDown.ToString() + "秒";
                            General.StopSound();
                            if (Flags.ClickStopButton) goto case 5000;

                            if (Flag1MinutePassed)
                            {
                                MessageBox.Show("リチウム電池が消耗しています\r\n電池を取り外して責任者に渡してください");
                                Flags.Flag電池セット = false;
                                goto case 5000;
                            }
                            await Task.Delay(500);
                            General.PlaySound(General.soundBattery);
                            await Task.Delay(700);
                            State.VmTestStatus.Message = Constants.MessWait;
                            break;
                        case 2100: //リチウム電池電圧チェック
                            if (await 電圧チェック.CheckVolt(電圧チェック.CH.BATTERY)) break;
                            goto case 5000;

                        case 2200: //D4両端電圧チェック
                            if (await 電圧チェック.CheckVolt(電圧チェック.CH.D4)) break;
                            goto case 5000;

                        case 2300: //RTC最終設定
                            General.ResetIo();
                            await Task.Delay(300);
                            //電源ON/OFFを3回繰り返し、正常に通信できるかチェックする（2016.11.9 樫山工業 富岡さんからの指示は5回だが、根拠がないので3回にしておく）
                            foreach (var i in Enumerable.Range(0, 5))
                            {
                                General.PowSupply(true);
                                await Task.Delay(900);
                                General.PowSupply(false);
                                await Task.Delay(300);
                            }

                            ////////////////////////////////////////////////////////
                            General.SetCutIn();//メタルモード時はカットイン予告を入れる
                            ////////////////////////////////////////////////////////

                            //電源ON
                            General.PowSupply(true);
                            await Task.Delay(4000);
                            if (!await RTCチェック.RTC最終設定()) goto case 5000;
                            if (!await RTCチェック.RTC最終チェック()) goto case 5000;

                            //S2リセットスイッチ押し
                            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.H);
                            await Task.Delay(300);
                            General.io.OutBit(EPX64S.PORT.P3, EPX64S.BIT.b1, EPX64S.OUT.L);
                            await Task.Delay(100);

                            break;

                        case 5000://NGだっときの処理
                            if (Flags.AddDecision) SetTestLog("---- FAIL\r\n");
                            FailStepNo = d.s.Key;
                            FailTitle = d.s.Value;

                            General.PowSupply(false);
                            await Task.Delay(500);

                            if (Flags.ClickStopButton || !Flags.Flag電池セット) goto FAIL;

                            if (RetryCnt++ != Constants.RetryCount)
                            {
                                //リトライ履歴リスト更新
                                State.RetryLogList.Add(FailStepNo.ToString() + "," + FailTitle + "," + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                Flags.Retry = true;
                                goto Retry;

                            }
                            goto FAIL;//自動リトライ後の作業者への確認はしない

                        //General.PlaySoundLoop(General.soundAlarm);
                        //var YesNoResult = System.Windows.Forms.MessageBox.Show("この項目はＮＧですがリトライしますか？", "", MessageBoxButtons.YesNo);
                        //General.StopSound();

                        ////何が選択されたか調べる
                        //if (YesNoResult == DialogResult.Yes)
                        //{
                        //    RetryCnt = 0;
                        //    //メタルモード時はBGMが消えているので、ここで再生する
                        //    General.SetBgm();
                        //    goto Retry;
                        //}
                        //else
                        //{
                        //    goto FAIL; //作業者がリトライ処理をキャンセルしたのでFAIL終了処理へ
                        //}
                    }
                    //↓↓各ステップが合格した時の処理です↓↓
                    if (Flags.AddDecision) SetTestLog("---- PASS\r\n");

                    State.VmTestStatus.IsActiveRing = false;

                    //リトライステータスをリセットする
                    RetryCnt = 0;
                    Flags.Retry = false;


                    State.VmTestStatus.進捗度 = (int)(((d.i + 1) / (double)テスト項目最新.Count()) * 100);
                    if (Flags.ClickStopButton) goto FAIL;
                }


                //↓↓すべての項目が合格した時の処理です↓↓
                General.ResetIo();
                await Task.Delay(500);

                State.VmTestStatus.StartButtonContent = Constants.確認;
                State.VmTestStatus.Message = Constants.MessRemove;

                //通しで試験が合格したときの処理です(検査データを保存して、シリアルナンバーをインクリメントする)
                if (State.VmTestStatus.CheckUnitTest != true) //null or False アプリ立ち上げ時はnullになっている！
                {
                    if (!General.SaveTestData())
                    {
                        FailStepNo = 5000;
                        FailTitle = "検査データ保存";
                        goto FAIL_DATA_SAVE;
                    }

                    General.StampOn();//合格印押し

                    //当日試験合格数をインクリメント ビューモデルはまだ更新しない
                    State.Setting.TodayOkCount++;

                    //これ重要！！！ シリアルナンバーをインクリメントし、次の試験に備える ビューモデルはまだ更新しない
                    State.NewSerial++;
                }



                FlagTestTime = false;

                State.VmTestStatus.Colorlabel判定 = Brushes.DeepSkyBlue;
                State.VmTestStatus.Decision = Flags.MetalMode ? "WIN" : "PASS";
                State.VmTestStatus.ColorDecision = effect判定表示PASS;

                ResetRing();
                SetDecision();
                SbPass();

                General.PlaySound(General.soundPassLong);

                Flags.Click確認Button = false;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (Flags.Click確認Button) break;
                        if (General.CheckPressOpen())
                        {
                            Flags.Click確認Button = true;
                            State.VmTestStatus.StartButtonContent = Constants.開始;
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    General.player.Stop();
                });

                return;

                //不合格時の処理
            FAIL:
                General.ResetIo();
                await Task.Delay(500);
            FAIL_DATA_SAVE:


                bool FlagPressOpen = General.CheckPressOpen();
                FlagTestTime = false;
                State.VmTestStatus.StartButtonContent = Constants.確認;
                State.VmTestStatus.StartButtonEnable = true;
                State.VmTestStatus.Message = Constants.MessRemove;


                //当日試験不合格数をインクリメント ビューモデルはまだ更新しない
                State.Setting.TodayNgCount++;
                await Task.Delay(100);

                State.VmTestStatus.Colorlabel判定 = Brushes.AliceBlue;
                State.VmTestStatus.Decision = "FAIL";
                State.VmTestStatus.ColorDecision = effect判定表示FAIL;

                SetErrorMessage(FailStepNo, FailTitle);

                var NgDataList = new List<string>()
                                    {
                                        State.VmMainWindow.Opecode,
                                        State.VmMainWindow.Operator,
                                        System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                        State.VmTestStatus.FailInfo,
                                        State.VmTestStatus.Spec,
                                        State.VmTestStatus.MeasValue
                                    };

                General.SaveNgData(NgDataList);


                ResetRing();
                SetDecision();
                SetErrInfo();
                SbFail();

                General.PlaySound(General.soundFail);

                Flags.Click確認Button = false;
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (Flags.Click確認Button) break;
                        if (!FlagPressOpen)
                        {
                            if (General.CheckPressOpen())
                            {
                                Flags.Click確認Button = true;
                                State.VmTestStatus.StartButtonContent = Constants.開始;
                                break;
                            }
                        }
                        Thread.Sleep(100);
                    }
                });


                return;

            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("想定外の例外発生DEATH！！！\r\n申し訳ありませんが再起動してください");
                Environment.Exit(0);

            }
            finally
            {
                State.Setting.TotalTestCount++;//トータルテスト回数は内部的に保持するだけでViewには表示しない
                General.ResetIo();
                SbRingLoad();

                General.ResetViewModel();
                RefreshDataContext();

                General.cam.ResetFlag();

                //次の検査の前に、K100とK101が溶着していないかチェックしておく
                General.CheckPowRelay();
            }

        }

        //フォームきれいにする処理いろいろ
        private void ClearForm()
        {
            SbRingLoad();
            RefreshDataContext();
        }


        private void SetErrorMessage(int stepNo, string title)
        {
            if (Flags.ClickStopButton)
            {
                State.VmTestStatus.FailInfo = "エラーコード ---     強制停止";
            }
            else
            {
                State.VmTestStatus.FailInfo = "エラーコード " + stepNo.ToString("00") + "   " + title + "異常";
            }
        }


        //テストログの更新
        private void SetTestLog(string addData)
        {
            State.VmTestStatus.TestLog += addData;
        }




        private void ResetRing()
        {
            State.VmTestStatus.RingVisibility = System.Windows.Visibility.Hidden;

        }

        private void SetDecision()
        {
            State.VmTestStatus.DecisionVisibility = System.Windows.Visibility.Visible;
        }
        private void SetErrInfo()
        {
            State.VmTestStatus.ErrInfoVisibility = System.Windows.Visibility.Visible;
        }



    }
}
