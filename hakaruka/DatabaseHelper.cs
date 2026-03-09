using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace hakaruka
{
    /// <summary>
    /// データベースアクセスクラス（MySQL用）
    /// </summary>
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 重量データをDBに挿入
        /// </summary>
        public bool InsertWeightData(WeightData data)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO `fan` (Serial, Unit, Result, Weight, reg_time) 
                                    VALUES (@Serial, @Unit, @Result, @Weight, @RegTime)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Serial", data.Serial);
                        cmd.Parameters.AddWithValue("@Unit", data.Unit);
                        cmd.Parameters.AddWithValue("@Result", data.Result);
                        cmd.Parameters.AddWithValue("@Weight", data.Weight);
                        cmd.Parameters.AddWithValue("@RegTime", data.RegTime);

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (MySqlException ex)
            {
                // エラーログをファイルに出力して原因を追跡しやすくする
                try
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db_errors.log");
                    File.AppendAllText(logPath, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\n" + ex.ToString() + "\n\n");
                }
                catch
                {
                    // ログ出力失敗は無視
                }

                // 呼び出し元で UI スレッド上で処理できるよう例外を投げる
                throw new Exception($"データベースエラー: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// DB接続テスト：接続して簡単なクエリを実行します。例外は呼び出し元へ投げます。
        /// </summary>
        public void TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT 1", conn))
                {
                    cmd.ExecuteScalar();
                }
            }
        }
    }
}
