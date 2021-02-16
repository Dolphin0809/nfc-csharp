using System;
using System.Linq;
using System.Text;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// 暗号化なし書き込み応答
    /// </summary>
    public class WriteWithoutEncryptionResponse
    {
        /// <summary>
        /// 要求データ
        /// </summary>
        public WriteWithoutEncryptionRequest Request { get; private set; } = null;
        /// <summary>
        /// 生データ
        /// </summary>
        public byte[] RawData { get; private set; } = null;
        /// <summary>
        /// レスポンスコード
        /// </summary>
        public int ResponseCode
        {
            get
            {
                return 0x09;
            }
        }
        /// <summary>
        /// IDm
        /// </summary>
        public byte[] IDm { get; private set; } = null;
        /// <summary>
        /// ステータスフラグ1
        /// </summary>
        public int StatusFlag1 { get; private set; } = 0;
        /// <summary>
        /// ステータスフラグ2
        /// </summary>
        public int StatusFlag2 { get; private set; } = 0;

        /// <summary>
        /// 応答パケットから暗号化なし書き込み応答を生成します。
        /// </summary>
        /// <param name="request">要求データ</param>
        /// <param name="data">応答データ</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public WriteWithoutEncryptionResponse(WriteWithoutEncryptionRequest request, byte[] data)
        {
            Request = request ?? throw new ArgumentNullException("request");
            RawData = data ?? throw new ArgumentNullException("data");

            if (data.Length < 11)
            {
                throw new ArgumentException("データが不足しています。", "data");
            }

            if (data[0] != ResponseCode)
            {
                throw new ArgumentException("データの形式が正しくありません。", "data");
            }

            IDm = data.Where((val, idx) => 1 <= idx && idx <= 8).ToArray();
            StatusFlag1 = data[9];
            StatusFlag2 = data[10];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine();
            sb.AppendLine($"\tResponseCode:\t{ResponseCode:x02}");
            sb.AppendLine($"\tIDm:\t\t{string.Join("", IDm.Select(val => $"{val:x02}"))}");
            sb.AppendLine($"\tStatusFlag1:\t{StatusFlag1:x02}");
            sb.Append($"\tStatusFlag2:\t{StatusFlag2:x02}");

            return sb.ToString();
        }

    }
}