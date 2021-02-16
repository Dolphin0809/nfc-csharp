using System;
using System.Linq;
using System.Text;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// ポーリング応答
    /// </summary>
    public class PollingResponse
    {
        /// <summary>
        /// 要求データ
        /// </summary>
        public PollingRequest Request { get; private set; } = null;
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
                return 0x01;
            }
        }
        /// <summary>
        /// IDm
        /// </summary>
        public byte[] IDm { get; private set; } = null;
        /// <summary>
        /// PMm
        /// </summary>
        public byte[] PMm { get; private set; } = null;
        /// <summary>
        /// リクエスデータ
        /// </summary>
        public int? RequestData { get; private set; } = null;

        /// <summary>
        /// 応答パケットからポーリング応答を生成します。
        /// </summary>
        /// <param name="request">要求データ</param>
        /// <param name="data">応答データ</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public PollingResponse(PollingRequest request, byte[] data)
        {
            Request = request ?? throw new ArgumentNullException("request");
            RawData = data ?? throw new ArgumentNullException("data");
            
            if (data.Length < 17)
            {
                throw new ArgumentException("データが不足しています。", "data");
            }

            if (data[0] != ResponseCode)
            {
                throw new ArgumentException("データの形式が正しくありません。", "data");
            }

            IDm = data.Where((val, idx) => 1 <= idx && idx <= 8 ).ToArray();
            PMm = data.Where((val, idx) => 9 <= idx && idx <= 16 ).ToArray();

            if (request.RequestCode != 0)
            {
                if (data.Length < 19)
                {
                    throw new ArgumentException("データが不足しています。", "data");
                }

                RequestData = data[17];
                RequestData <<= 8;
                RequestData += data[18];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine();
            sb.AppendLine($"\tResponseCode:\t{ResponseCode:x02}");
            sb.AppendLine($"\tIDm:\t\t{string.Join("", IDm.Select(val => $"{val:x02}"))}");
            sb.AppendLine($"\tPMm:\t\t{string.Join("", PMm.Select(val => $"{val:x02}"))}");
            sb.Append($"\tRequestData:\t{(RequestData.HasValue ? $"{RequestData:x04}" : "null")}");

            return sb.ToString();
        }
    }
}