﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Os303Tester
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
                            continue;
                        }

                        if (!Flags.SetOpecode)
                        {
                            State.VmTestStatus.Message = Constants.MessOpecode;
                            continue;
                        }

                        if (!Flags.AllOk周辺機器接続)
                        {
                            State.VmTestStatus.Message = Constants.MessCheckConnectMachine;
                            continue;
                        }

                        if (!Flags.StateRL1)
                        {
                            State.VmTestStatus.Message = "試験機RL1が破損しています!!!";
                            continue;
                        }


                        State.VmTestStatus.Message = Constants.MessSet;

                        while (true)
                        {
                            if (Flags.OtherPage || !General.CheckPressOpen()) return;
                            if (!Flags.SetOperator || !Flags.SetOpecode) goto RETRY;
                        }
                    }

                });

                if (Flags.OtherPage) return;

                State.VmMainWindow.EnableOtherButton = false;
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

            General.SetMetalMode();
            State.VmTestStatus.Message = Constants.MessWait;

            //現在のテーマ透過度の保存
            State.CurrentThemeOpacity = State.VmMainWindow.ThemeOpacity;
            //テーマ透過度を最小にする
            General.Show2(false);
            
            General.cam.ImageOpacity = 1.0;

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
                    テスト項目最新.Add(new TestSpecs(p.Key, p.Value));
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


                    switch (d.s.Key)
                    {
                        case 100://コネクタ実装チェック
                            if (await コネクタチェック.CheckCn()) break;
                            goto case 1000;

                        case 200://電源電圧チェック ac100
                            if (await 電圧チェック.CheckVoltAsync(電圧チェック.CH.AC100)) break;
                            goto case 1000;

                        case 201://電源電圧チェック VDD
                            if (await 電圧チェック.CheckVoltAsync(電圧チェック.CH.VDD)) break;
                            goto case 1000;

                        case 202://電源電圧チェック VCC1
                            if (await 電圧チェック.CheckVoltAsync(電圧チェック.CH.VCC1)) break;
                            goto case 1000;

                        case 300://製品プログラム書き込み
                            if (State.VmTestStatus.CheckWriteFw == true) break;
                            if (await 書き込み.WriteFw()) break;
                            goto case 1000;

                        case 500://電源電圧チェック VCC2
                            General.SetRL5(true);
                            if (await 電圧チェック.CheckVoltAsync(電圧チェック.CH.VCC2_RELAY_ON)) break;
                            goto case 1000;

                        case 600://シーケンスチェック
                            if (await シーケンスチェック.CheckDout()) break;
                            goto case 1000;

                        case 1000://NGだっときの処理
                            if (Flags.AddDecision) SetTestLog("---- FAIL\r\n");
                            FailStepNo = d.s.Key;
                            FailTitle = d.s.Value;

                            General.PowSupply(false);
                            await Task.Delay(500);


                            if (RetryCnt++ != Constants.RetryCount)
                            {
                                //リトライ履歴リスト更新
                                State.RetryLogList.Add(FailStepNo.ToString() + "," + FailTitle + "," + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                                Flags.Retry = true;
                                General.cam.ResetFlag();//LEDテストでNGになったときは、カメラのフラグを初期化しないとNG枠が出たままになる
                                goto Retry;

                            }
                            goto FAIL;//自動リトライ後の作業者への確認はしない


                    }
                    //↓↓各ステップが合格した時の処理です↓↓
                    if (Flags.AddDecision) SetTestLog("---- PASS\r\n");

                    State.VmTestStatus.IsActiveRing = false;

                    //リトライステータスをリセットする
                    RetryCnt = 0;
                    Flags.Retry = false;


                    State.VmTestStatus.進捗度 = (int)(((d.i + 1) / (double)テスト項目最新.Count()) * 100);
                }


                //↓↓すべての項目が合格した時の処理です↓↓
                General.ResetIo();
                await Task.Delay(500);

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

                }



                FlagTestTime = false;

                State.VmTestStatus.Colorlabel判定 = Brushes.DeepSkyBlue;
                State.VmTestStatus.Decision = Flags.MetalMode ? "WIN" : "PASS";
                State.VmTestStatus.ColorDecision = effect判定表示PASS;

                ResetRing();
                SetDecision();
                SbPass();

                General.PlaySound(General.soundPassLong);

                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (General.CheckPressOpen()) break;
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

                FlagTestTime = false;
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

                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (General.CheckPressOpen()) break;
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

                //次の検査の前に、試験機のリレーRL1の接点が溶着していないかチェックしておく
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
            State.VmTestStatus.FailInfo = "エラーコード " + stepNo.ToString("00") + "   " + title + "異常";
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
