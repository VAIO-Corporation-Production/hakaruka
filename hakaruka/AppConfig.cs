namespace hakaruka
{
    /// <summary>
    /// アプリケーション設定クラス
 /// </summary>
    public class AppConfig
    {
        //旧単一閾値（互換性のため残すが、実際の判定はMin/Maxを使う）
        public int Threshold1 { get; set; }  // 段ボール1箱（deprecated）
        public int Threshold2 { get; set; }// 1シート（deprecated）
        public int Threshold3 { get; set; }  // 部品1つ（deprecated）

        // 新：閾値の下限・上限
        public int Threshold1Min { get; set; }
        public int Threshold1Max { get; set; }
        public int Threshold2Min { get; set; }
        public int Threshold2Max { get; set; }
        public int Threshold3Min { get; set; }
        public int Threshold3Max { get; set; }

     public string ComPort { get; set; } = "COM1";
        public int BaudRate { get; set; } = 9600;
        public string ConnectionString { get; set; } = "";

        // 外部ツール利用設定
        public bool UseExternalTool { get; set; } = false;
        // ツールが置かれるフォルダ（アプリと同じフォルダ内の相対パスも可）
        public string ToolFolder { get; set; } = "tools";
        // ツールのファイル名パターン（ワイルドカード可）
        public string ToolPattern { get; set; } = "comserial*.exe";

        /// <summary>
     /// INIファイルから設定を読み込む
        /// </summary>
        public static AppConfig LoadFromIni(string iniPath)
{
    IniFileReader ini = new IniFileReader(iniPath);
            AppConfig config = new AppConfig();

            //互換用の単一閾値読み込み（旧設定が残っている場合のため）
            config.Threshold1 = int.Parse(ini.Read("Settings", "Threshold1", "15"));
            config.Threshold2 = int.Parse(ini.Read("Settings", "Threshold2", "400"));
            config.Threshold3 = int.Parse(ini.Read("Settings", "Threshold3", "416"));

            // 新しいMin/Max閾値を読み込む（存在しない場合は旧閾値を基にデフォルト設定）
            config.Threshold1Min = int.Parse(ini.Read("Settings", "Threshold1Min", (config.Threshold1 -5).ToString()));
            config.Threshold1Max = int.Parse(ini.Read("Settings", "Threshold1Max", config.Threshold1.ToString()));

            config.Threshold2Min = int.Parse(ini.Read("Settings", "Threshold2Min", (config.Threshold2 -50).ToString()));
            config.Threshold2Max = int.Parse(ini.Read("Settings", "Threshold2Max", config.Threshold2.ToString()));

            config.Threshold3Min = int.Parse(ini.Read("Settings", "Threshold3Min", (config.Threshold3 -50).ToString()));
            config.Threshold3Max = int.Parse(ini.Read("Settings", "Threshold3Max", config.Threshold3.ToString()));

    config.ComPort = ini.Read("Serial", "ComPort", "COM1");
 config.BaudRate = int.Parse(ini.Read("Serial", "BaudRate", "9600"));
          
 config.ConnectionString = ini.Read("Database", "ConnectionString", "");

      // 外部ツール関連
 config.UseExternalTool = bool.Parse(ini.Read("Tool", "UseExternalTool", "false"));
 config.ToolFolder = ini.Read("Tool", "ToolFolder", "tools");
 config.ToolPattern = ini.Read("Tool", "ToolPattern", "comserial*.exe");

 // ToolFolderが相対パスならアプリケーションディレクトリに基づいて絶対パスへ
 if (!Path.IsPathRooted(config.ToolFolder))
 {
 config.ToolFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.ToolFolder);
 }

      return config;
        }
    }
}
