namespace hakaruka
{
    /// <summary>
  /// 重量データのモデル
    /// </summary>
    public class WeightData
    {
        public string Serial { get; set; } = "";
        public string Unit { get; set; } = "";
     public string Result { get; set; } = "";
public decimal Weight { get; set; }
      public DateTime RegTime { get; set; }
    }
}
