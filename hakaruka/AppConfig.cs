namespace hakaruka
{
    /// <summary>
    /// アプリケーション設定クラス
 /// </summary>
    public class AppConfig
    {
        public int Threshold1 { get; set; }  // 段ボール1箱
        public int Threshold2 { get; set; }// 1シート
        public int Threshold3 { get; set; }  // 部品1つ

     public string ComPort { get; set; } = "COM1";
        public int BaudRate { get; set; } = 9600;
        public string ConnectionString { get; set; } = "";

        /// <summary>
     /// INIファイルから設定を読み込む
        /// </summary>
        public static AppConfig LoadFromIni(string iniPath)
{
    IniFileReader ini = new IniFileReader(iniPath);
            AppConfig config = new AppConfig();

            config.Threshold1 = int.Parse(ini.Read("Settings", "Threshold1", "15"));
            config.Threshold2 = int.Parse(ini.Read("Settings", "Threshold2", "400"));
            config.Threshold3 = int.Parse(ini.Read("Settings", "Threshold3", "416"));
            
    config.ComPort = ini.Read("Serial", "ComPort", "COM1");
 config.BaudRate = int.Parse(ini.Read("Serial", "BaudRate", "9600"));
          
 config.ConnectionString = ini.Read("Database", "ConnectionString", "");

      return config;
        }
    }
}
