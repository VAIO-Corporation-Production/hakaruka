using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace hakaruka
{
 public partial class Form1 : Form
 {
 private AppConfig? config;
 private DatabaseHelper? dbHelper;
 private WeightScaleCommunicator? scaleCommunicator;
 private decimal currentWeight =0;
 private string currentResult = "";
 private SerialPort? serialPort; //直接シリアルポート使用

 public Form1()
 {
 InitializeComponent();
 }

 private void Form1_Load(object sender, EventArgs e)
 {
 try
 {
 // INIファイルのパス
 string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
 
 // INIファイルが存在しない場合は作成
 if (!File.Exists(iniPath))
 {
 CreateDefaultIniFile(iniPath);
 }

 // 設定を読み込む
 config = AppConfig.LoadFromIni(iniPath);

 // データベースヘルパーを初期化
 if (!string.IsNullOrEmpty(config.ConnectionString))
 {
 dbHelper = new DatabaseHelper(config.ConnectionString);
 }

 // シリアルポートを初期化
 InitializeSerialPort();

 AddLog("アプリケーションを起動しました。");
 // 閾値は Min-Max の形式でログ表示
 AddLog($"閾値: 段ボール={config.Threshold1Min}-{config.Threshold1Max}g, シート={config.Threshold2Min}-{config.Threshold2Max}g, 部品={config.Threshold3Min}-{config.Threshold3Max}g");
 }
 catch (Exception ex)
 {
MessageBox.Show($"初期化エラー: {ex.Message}", "エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }

 private void CreateDefaultIniFile(string iniPath)
 {
 IniFileReader ini = new IniFileReader(iniPath);
 // 古い単一閾値
 ini.Write("Settings", "Threshold1", "15");
 ini.Write("Settings", "Threshold2", "400");
 ini.Write("Settings", "Threshold3", "416");
 // 新しい Min/Max 値（単位はグラム）
 ini.Write("Settings", "Threshold1Min", "10");
 ini.Write("Settings", "Threshold1Max", "15");
 ini.Write("Settings", "Threshold2Min", "350");
 ini.Write("Settings", "Threshold2Max", "400");
 ini.Write("Settings", "Threshold3Min", "366");
 ini.Write("Settings", "Threshold3Max", "416");
 ini.Write("Serial", "ComPort", "COM1");
 ini.Write("Serial", "BaudRate", "9600");
 ini.Write("Database", "ConnectionString", "Server=10.2.13.100;Database=hakaruka;Uid=ubuntu;Pwd=vaio;");
 // Tool section defaults
 ini.Write("Tool", "UseExternalTool", "false");
 ini.Write("Tool", "ToolFolder", "tools");
 ini.Write("Tool", "ToolPattern", "comserial*.exe");
 }

 private void InitializeSerialPort()
 {
 try
 {
 if (config == null) return;

 serialPort = new SerialPort(config.ComPort, config.BaudRate);
 serialPort.Parity = Parity.None;
 serialPort.DataBits =8;
 serialPort.StopBits = StopBits.One;
 serialPort.Handshake = Handshake.None;
 serialPort.ReadTimeout =3000;
 serialPort.WriteTimeout =3000;
 serialPort.DataReceived += SerialPort_DataReceived;

 // シリアルポートを開く
 if (SerialPort.GetPortNames().Contains(config.ComPort))
 {
 serialPort.Open();
 AddLog($"シリアルポート {config.ComPort} を開きました。");
 }
 else
 {
 AddLog($"警告: シリアルポート {config.ComPort} が見つかりません。");
 }
 }
 catch (Exception ex)
{
 AddLog($"シリアルポートエラー: {ex.Message}");
 }
 }

 private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
 {
try
 {
 if (serialPort != null && serialPort.IsOpen)
 {
string data = serialPort.ReadLine().Trim();
 if (decimal.TryParse(data, out decimal weight))
 {
 // UIスレッドで更新
 this.Invoke((MethodInvoker)delegate
 {
 UpdateWeight(weight);
 });
}
 }
 }
 catch (Exception)
 {
 // データ受信エラーは無視
}
 }

 private void TxtSerial_KeyDown(object? sender, KeyEventArgs e)
 {
 if (e.KeyCode == Keys.Enter)
 {
 e.SuppressKeyPress = true;
 BtnScan_Click(sender, e);
 }
 }

 private async void BtnScan_Click(object? sender, EventArgs e)
 {
 try
 {
 // シリアル番号のチェック
 if (string.IsNullOrWhiteSpace(txtSerial.Text))
 {
 MessageBox.Show("シリアル番号を入力してください。", "入力エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Warning);
 txtSerial.Focus();
 return;
 }

lblStatus.Text = "重量を測定中...";
 lblStatus.ForeColor = Color.Blue;
 AddLog($"シリアル番号: {txtSerial.Text} の測定を開始");

 // 重量を読み取る（非同期）
 await ReadWeightFromScale();
 }
 catch (Exception ex)
 {
 MessageBox.Show($"エラー: {ex.Message}", "エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }

private async Task ReadWeightFromScale()
 {
 // 手動入力モード（重量計が接続されていない場合のテスト用）
 if (serialPort == null || !serialPort.IsOpen)
 {
string input = Microsoft.VisualBasic.Interaction.InputBox(
 "重量を入力してください（グラム）:", "重量入力", "0");
 
 if (decimal.TryParse(input, out decimal weight))
 {
 UpdateWeight(weight);
 }
 return;
 }

 // シリアルポートから読み取り
 try
 {
 // 重量計にコマンドを送信（機種によって変更が必要）
 serialPort.WriteLine("S");
 // UI をブロックしないように非同期で待機
 await Task.Delay(200);

 if (serialPort.BytesToRead >0)
 {
 string response = serialPort.ReadLine().Trim();
 if (decimal.TryParse(response, out decimal weight))
 {
 UpdateWeight(weight);
 }
 }
 }
 catch (Exception ex)
 {
 AddLog($"重量読み取りエラー: {ex.Message}");
 }
 }

 private void UpdateWeight(decimal weight)
 {
 currentWeight = weight;
 txtWeight.Text = weight.ToString("F1");

 // 判定を行う
 JudgeWeight(weight);
 }

 private void JudgeWeight(decimal weight)
 {
 if (config == null) return;

 // 単位に応じた閾値範囲を取得
 (int min, int max) = cmbUnit.SelectedIndex switch
 {
0 => (config.Threshold1Min, config.Threshold1Max), // 段ボール1箱
1 => (config.Threshold2Min, config.Threshold2Max), //1シート
2 => (config.Threshold3Min, config.Threshold3Max), // 部品1つ
 _ => (0, int.MaxValue)
 };

 // 判定：閾値範囲内ならOK
 if (weight >= min && weight <= max)
 {
 currentResult = "OK";
 lblResultValue.Text = "OK";
 lblResultValue.ForeColor = Color.Black;
 lblStatus.Text = "正常です";
 lblStatus.ForeColor = Color.Green;
 }
 else
 {
 currentResult = "NG";
 lblResultValue.Text = "NG";
 lblResultValue.ForeColor = Color.Red;
 lblStatus.Text = $"重量が閾値範囲外です ({min}-{max}g)";
 lblStatus.ForeColor = Color.Red;
 }

 txtWeight.BackColor = currentResult == "OK" ? Color.White : Color.LightGray;
 
 AddLog($"測定結果: {weight}g - {currentResult} (閾値: {min}-{max}g)");
 }

 private async void BtnSave_Click(object? sender, EventArgs e)
 {
 try
 {
 // 入力チェック
 if (string.IsNullOrWhiteSpace(txtSerial.Text))
 {
 MessageBox.Show("シリアル番号を入力してください。", "入力エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Warning);
 return;
 }

 if (currentWeight ==0)
 {
 MessageBox.Show("重量を測定してください。", "入力エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Warning);
 return;
 }

 // データを作成
 WeightData data = new WeightData
 {
 Serial = txtSerial.Text.Trim(),
 Unit = cmbUnit.SelectedItem?.ToString() ?? "",
 Result = currentResult,
 Weight = currentWeight,
 RegTime = DateTime.Now
 };

 // データベースに保存（バックグラウンドで実行して UI フリーズを防止）
 if (dbHelper != null)
 {
 try
 {
 await Task.Run(() => dbHelper.InsertWeightData(data));
 AddLog($"データを保存しました: {data.Serial} - {data.Weight}g - {data.Result}");
 MessageBox.Show("データを保存しました。", "成功",
 MessageBoxButtons.OK, MessageBoxIcon.Information);
 
 // リセット
 ResetForm();
 }
 catch (Exception ex)
 {
 // DBエラーはログに出力
 AddLog($"DB保存エラー: {ex.Message}");
 MessageBox.Show($"DB保存エラー: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }
 else
 {
 MessageBox.Show("データベースが接続されていません。", "エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Warning);
 }
 }
 catch (Exception ex)
 {
 MessageBox.Show($"保存エラー: {ex.Message}", "エラー",
 MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 }

 private void BtnReset_Click(object? sender, EventArgs e)
 {
 ResetForm();
 }

 private void ResetForm()
 {
 txtSerial.Clear();
 txtWeight.Text = "0";
 lblResultValue.Text = "--";
 lblResultValue.ForeColor = Color.Black;
 txtWeight.BackColor = Color.White;
 currentWeight =0;
 currentResult = "";
 lblStatus.Text = "待機中...";
 lblStatus.ForeColor = Color.Black;
 cmbUnit.SelectedIndex =0;
 txtSerial.Focus();
 AddLog("リセットしました。");
 }

 private void AddLog(string message)
 {
 string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
 txtLog.AppendText($"[{timestamp}] {message}\r\n");
 }

protected override void OnFormClosing(FormClosingEventArgs e)
 {
 base.OnFormClosing(e);
 
 // リソースを解放
 serialPort?.Close();
 serialPort?.Dispose();
 scaleCommunicator?.Dispose();
 }
 }
}
