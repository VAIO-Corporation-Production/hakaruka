using System.Data.SqlClient;

namespace hakaruka
{
    /// <summary>
    /// データベースアクセスクラス
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
     using (SqlConnection conn = new SqlConnection(connectionString))
            {
   conn.Open();
           string query = @"INSERT INTO ループ (Serial, Unit, Result, Weight, reg_time) 
        VALUES (@Serial, @Unit, @Result, @Weight, @RegTime)";

   using (SqlCommand cmd = new SqlCommand(query, conn))
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
   catch (Exception ex)
            {
     MessageBox.Show($"データベースエラー: {ex.Message}", "エラー", 
    MessageBoxButtons.OK, MessageBoxIcon.Error);
     return false;
       }
      }
    }
}
