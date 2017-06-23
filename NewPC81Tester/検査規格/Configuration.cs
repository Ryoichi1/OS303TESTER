using System.Collections.Generic;

namespace NewPC81Tester
{
    public class Configuration
    {

        public string 日付 { get; set; }
        public int TotalTestCount { get; set; }
        public int TodayOkCount { get; set; }
        public int TodayNgCount { get; set; }
        public string PathTheme { get; set; }
        public double OpacityTheme { get; set; }
        public List<string> 作業者リスト { get; set; }

        //USB-シリアルアダプタのCOMナンバー
        public string Com232_1 { get; set; }
        public string Com232_2 { get; set; }
        public string Com232_3 { get; set; }
        public string Com232_4 { get; set; }
        public string Com422_1 { get; set; }
        public string Com422_2 { get; set; }

        public string ComCa100 { get; set; }


    }
}
