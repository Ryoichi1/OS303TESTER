using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Os303Tester
{
    /// <summary>
    /// EditOpeList.xaml の相互作用ロジック
    /// </summary>
    public partial class EditOpeList
    {
        private ViewModelEdit vmEdit;

        public EditOpeList()
        {
            this.InitializeComponent();
            vmEdit = new ViewModelEdit();

            this.DataContext = vmEdit;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (vmEdit.Name == "") return;
            // 入力された名前を追加
            vmEdit.ListOperator.Add(vmEdit.Name);
            vmEdit.ListOperator = new List<string>(vmEdit.ListOperator);
            vmEdit.Name = "";
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (vmEdit.SelectIndex == -1) return;
            // 選択された項目を削除
            vmEdit.ListOperator.RemoveAt(vmEdit.SelectIndex);
            vmEdit.ListOperator = new List<string>(vmEdit.ListOperator);
        }


        private async void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            buttonSave.Background = Brushes.DodgerBlue;

            //保存する処理
            State.VmMainWindow.ListOperator = new List<string>(vmEdit.ListOperator);
            General.PlaySound(General.soundSave);
            await Task.Delay(100);
            buttonSave.Background = Brushes.Transparent;
        }
    }
}
