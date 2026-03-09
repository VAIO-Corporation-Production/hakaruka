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
 AddLog($"閾値: 段ボール={config.Threshold1}以下, シート={config.Threshold2}以下, 部品={config.Threshold3}以下");
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
 ini.Write("Settings", "Threshold1", "15");
 ini.Write("Settings", "Threshold2", "400");
 ini.Write("Settings", "Threshold3", "416");
 ini.Write("Serial", "ComPort", "COM1");
 ini.Write("Serial", "BaudRate", "9600");
 ini.Write("Database", "ConnectionString", "Server=localhost;Database=hakaruka;Integrated Security=true;");
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

// 単位に応じた閾値を取得
 int threshold = cmbUnit.SelectedIndex switch
 {
0 => config.Threshold1, // 段ボール1箱
1 => config.Threshold2, //1シート
2 => config.Threshold3, // 部品1つ
 _ =>0
 };

 // 判定
 if (weight > threshold)
 {
 currentResult = "NG";
 lblResultValue.Text = "NG";
 lblResultValue.ForeColor = Color.Red;
 lblStatus.Text = "重量が基準を超えています";
 lblStatus.ForeColor = Color.Red;
}
 else
 {
 currentResult = "OK";
 lblResultValue.Text = "OK";
 lblResultValue.ForeColor = Color.Black;
 lblStatus.Text = "正常です";
 lblStatus.ForeColor = Color.Green;
 }

 txtWeight.BackColor = currentResult == "OK" ? Color.White : Color.LightGray;
 
 AddLog($"測定結果: {weight}g - {currentResult} (閾値: {threshold}g以下)");
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
 bool saved = await Task.Run(() => dbHelper.InsertWeightData(data));
 if (saved)
 {
 AddLog($"データを保存しました: {data.Serial} - {data.Weight}g - {data.Result}");
 MessageBox.Show("データを保存しました。", "成功",
 MessageBoxButtons.OK, MessageBoxIcon.Information);
 
 // リセット
 ResetForm();
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
