using System.Diagnostics;
using System.IO.Ports;
using System.IO;
using System.Linq;

namespace hakaruka
{
    /// <summary>
    /// 重量計との通信を管理するクラス
    /// comPlcSImpF.exeを使用してシリアル通信を行う
    /// </summary>
    public class WeightScaleCommunicator : IDisposable
    {
        private SerialPort? serialPort;
        private Process? comPlcProcess;
        private string comPort;
        private int baudRate;
        private bool useExternalExe = false; // comPlcSImpF.exeを使用するかどうか
        private string? externalExePath;

        public event EventHandler<decimal>? WeightReceived;

        public WeightScaleCommunicator(string comPort, int baudRate, AppConfig? config = null)
        {
            this.comPort = comPort;
            this.baudRate = baudRate;

            if (config != null)
            {
                useExternalExe = config.UseExternalTool;
                if (useExternalExe)
                {
                    // ツールフォルダとパターンを使って最初のマッチを探す
                    string folder = config.ToolFolder;
                    string pattern = config.ToolPattern ?? "comserial*.exe";
                    try
                    {
                        if (Directory.Exists(folder))
                        {
                            var files = Directory.GetFiles(folder, pattern);
                            if (files.Length > 0)
                            {
                                externalExePath = files[0];
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // 探索失敗は無視
                    }
                }
            }
        }

        /// <summary>
        /// シリアルポートを開く（直接通信版）
        /// </summary>
        public bool OpenSerialPort()
        {
            try
            {
                if (useExternalExe && !string.IsNullOrEmpty(externalExePath) && File.Exists(externalExePath))
                {
                    // external exeを起動
                    comPlcProcess = new Process();
                    comPlcProcess.StartInfo.FileName = externalExePath;
                    comPlcProcess.StartInfo.UseShellExecute = false;
                    comPlcProcess.StartInfo.RedirectStandardOutput = true;
                    comPlcProcess.Start();
                    return true;
                }

                // 直接シリアルポートを使用
                serialPort = new SerialPort(comPort, baudRate);
                serialPort.Parity = Parity.None;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;
                serialPort.ReadTimeout = 3000;
                serialPort.WriteTimeout = 3000;
                serialPort.DataReceived += SerialPort_DataReceived;

                serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"シリアルポートエラー: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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
                        WeightReceived?.Invoke(this, weight);
                    }
                }
            }
            catch (Exception)
            {
                // データ受信エラーは無視
            }
        }

        /// <summary>
        /// 重量計から重量を読み取る
        /// </summary>
        public decimal? ReadWeight()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    // 重量計にコマンドを送信（機種によって異なる）
                    serialPort.WriteLine("S");
                    Thread.Sleep(100);

                    string response = serialPort.ReadLine().Trim();
                    if (decimal.TryParse(response, out decimal weight))
                    {
                        return weight;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// シリアルポートを閉じる
        /// </summary>
        public void Close()
        {
            serialPort?.Close();
            try { if (comPlcProcess != null && !comPlcProcess.HasExited) comPlcProcess.Kill(); } catch { }
        }

        public void Dispose()
        {
            Close();
            serialPort?.Dispose();
            try { comPlcProcess?.Dispose(); } catch { }
        }
    }
}
