using System.Linq;
using System.Text;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// ポーリング要求
    /// </summary>
    public class PollingRequest
    {
        /// <summary>
        /// コマンドコード
        /// </summary>
        public int CommandCode
        {
            get
            {
                return 0x00;
            }
        }
        /// <summary>
        /// システムコード
        /// </summary>
        public int SystemCode { get; set; } = 0xFFFF;
        /// <summary>
        /// リクエストコード
        /// </summary>
        public int RequestCode { get; set; } = 0x00;
        /// <summary>
        /// タイムスロット
        /// </summary>
        public int TimeSlot { get; set; } = 0x00;
        /// <summary>
        /// コマンドパケット
        /// </summary>
        public byte[] Command
        {
            get
            {
                byte[] cmd = new byte[5];
                cmd[0] = (byte)(CommandCode & 0xFF);
                cmd[1] = (byte)((SystemCode >> 8) & 0xFF);
                cmd[2] = (byte)(SystemCode & 0xFF);
                cmd[3] = (byte)(RequestCode & 0xFF);
                cmd[4] = (byte)(TimeSlot & 0xFF);

                return cmd;
            }
        }

        /// <summary>
        /// ポーリング要求を生成します。
        /// </summary>
        /// <param name="systemCode">システムコード</param>
        /// <param name="requestCode">リクエストコード</param>
        /// <param name="timeSlot">タイムスロット</param>
        public PollingRequest(int systemCode, int requestCode = (int)NFC.Felica.RequestCode.NoRequest, int timeSlot = (int)NFC.Felica.TimeSlot.x1)
        {
            SystemCode = systemCode;
            RequestCode = requestCode;
            TimeSlot = timeSlot;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine();
            sb.AppendLine($"\tCommandCode:\t{CommandCode:x02}");
            sb.AppendLine($"\tSystemCode:\t{SystemCode:x04}");
            sb.AppendLine($"\tRequestCode:\t{RequestCode:x02}");
            sb.AppendLine($"\tTimeSlot:\t{TimeSlot:x02}");
            sb.Append($"\tCommand:\t{string.Join("", Command.Select(val => $"{val:x02}"))}");

            return sb.ToString();
        }
    }
}