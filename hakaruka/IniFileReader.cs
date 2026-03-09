using System.Runtime.InteropServices;
using System.Text;

namespace hakaruka
{
    /// <summary>
    /// INIファイルの読み書きを行うクラス
    /// </summary>
    public class IniFileReader
    {
        private string filePath;

        [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileString(
     string section,
            string key,
    string defaultValue,
       StringBuilder returnValue,
      int size,
     string filePath);

      [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(
            string section,
string key,
  string value,
    string filePath);

        public IniFileReader(string filePath)
    {
            this.filePath = filePath;
        }

  public string Read(string section, string key, string defaultValue = "")
        {
       StringBuilder sb = new StringBuilder(255);
     GetPrivateProfileString(section, key, defaultValue, sb, 255, filePath);
            return sb.ToString();
        }

        public void Write(string section, string key, string value)
      {
  WritePrivateProfileString(section, key, value, filePath);
        }
    }
}
