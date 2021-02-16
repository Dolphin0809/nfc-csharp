using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PCSC;
using PCSC.Exceptions;

namespace Dolphin0809.NFC.Felica
{
    public class Felica : IDisposable
    {
        private delegate byte[] CardReaderTransmit(byte[] cmd);

        private ISCardContext context = null;
        private ICardReader reader = null;
        private string _readerName = null;
        private CardReaderTransmit transmit = null;

        /// <summary>
        /// 利用可能なカードリーダ名
        /// </summary>
        public string[] ReaderNames
        {
            get
            {
                return context.GetReaders();
            }
        }

        /// <summary>
        /// 利用するカードリーダ名
        /// </summary>
        /// <exception cref="Exception" />
        public string CurrentReaderName
        {
            get
            {
                return _readerName;
            }
            set
            {
                if (value == _readerName)
                {
                    return;
                }
                try
                {
                    reader = context.ConnectReader(value, SCardShareMode.Direct, SCardProtocol.Unset);

                    _readerName = value;

                    if (Regex.IsMatch(_readerName, "acs", RegexOptions.IgnoreCase))
                    {
                        transmit = TransmitACS;
                    }
                    else if (Regex.IsMatch(_readerName, "fujifilm", RegexOptions.IgnoreCase))
                    {
                        transmit = TransmitFUJIFILM;
                    }
                    else if (Regex.IsMatch(_readerName, "sony", RegexOptions.IgnoreCase))
                    {
                        transmit = TransmitSONY;
                    }
                    else
                    {
                        throw new Exception("未対応のカードリーダが選択されました。");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("カードリーダに接続できませんでした。", ex);
                }
            }
        }

        /// <summary>
        /// PC/SCを利用してFeliCaカードを操作する
        /// </summary>
        /// <exception cref="Exception" />
        public Felica()
        {
            context = ContextFactory.Instance.Establish(SCardScope.System);

            try
            {
                CurrentReaderName = ReaderNames[0];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new Exception("有効なカードリーダがありません。", ex);
            }
        }

        ~Felica()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            reader?.Dispose();
            context?.Dispose();
        }

        /// <summary>
        /// Polling カードを捕捉する
        /// </summary>
        /// <param name="request">要求データ</param>
        /// <returns>応答データまたはnull</returns>
        /// <exception cref="ArgumentNullException" />
        public PollingResponse Polling(PollingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            byte[] recv = transmit(request.Command);

            if (recv == null)
            {
                return null;
            }

            var response = new PollingResponse(request, recv);

            return response;
        }

        /// <summary>
        /// ReadWithoutEncryption 暗号化なし読み取り
        /// </summary>
        /// <param name="request">要求データ</param>
        /// <returns>応答データまたはnull</returns>
        /// <exception cref="ArgumentNullException" />
        public ReadWithoutEncryptionResponse ReadWithoutEncryption(ReadWithoutEncryptionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            byte[] recv = transmit(request.Command);

            if (recv == null)
            {
                return null;
            }

            var response = new ReadWithoutEncryptionResponse(request, recv);

            return response;
        }

        /// <summary>
        /// WriteWithoutEncryption 暗号化なし書き込み
        /// </summary>
        /// <param name="request">要求データ</param>
        /// <returns>応答データまたはnull</returns>
        /// <exception cref="ArgumentNullException" />
        public WriteWithoutEncryptionResponse WriteWithoutEncryption(WriteWithoutEncryptionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            byte[] recv = transmit(request.Command);

            if (recv == null)
            {
                return null;
            }

            var response = new WriteWithoutEncryptionResponse(request, recv);

            return response;
        }

        private byte[] Transmit (byte[] send)
        {
            try
            {
                reader.Reconnect(SCardShareMode.Shared, SCardProtocol.Any, SCardReaderDisposition.Leave);
            }
            catch (NoSmartcardException)
            {
                return null;
            }

            byte[] recv = new byte[1024];

            int recvLength = reader.Transmit(send, recv);

            recv = recv.Where((val, idx) => idx < recvLength).ToArray();
            Console.WriteLine(string.Join("", send.Select(val => $"{val:x02}")));
            Console.WriteLine(string.Join("", recv.Select(val => $"{val:x02}")));
            if (!(recv[recv.Length - 2] == 0x90 && recv[recv.Length - 1] == 0x00))
            {
                return null;
            }

            var response = recv.Where((val, idx) => 1 <= idx && idx < recv.Length - 2).ToArray();

            return response;
        }

        private byte[] TransmitACS(byte[] cmd)
        {
            List<byte> send = new List<byte>();
            send.Add(0xFF);
            send.Add(0x00);
            send.Add(0x00);
            send.Add(0x00);
            send.Add((byte)(cmd.Length + 1));
            send.Add((byte)(cmd.Length + 1));
            send.AddRange(cmd);

            var response = Transmit(send.ToArray());

            return response;
        }

        private byte[] TransmitFUJIFILM(byte[] cmd)
        {
            List<byte> send = new List<byte>();
            send.Add((byte)(cmd.Length + 1));
            send.AddRange(cmd);

            try
            {
                reader.Reconnect(SCardShareMode.Shared, SCardProtocol.Any, SCardReaderDisposition.Leave);
            }
            catch (NoSmartcardException)
            {
                return null;
            }

            byte[] recv = new byte[1024];

            int recvLength = reader.Transmit(send.ToArray(), recv);

            recv = recv.Where((val, idx) => idx < recvLength).ToArray();

            var response = recv.Where((val, idx) => 1 <= idx).ToArray();

            return response;
        }

        private byte[] TransmitSONY(byte[] cmd)
        {
            List<byte> send = new List<byte>();
            send.Add(0xFF);
            send.Add(0xFE);
            send.Add(0x01);
            send.Add(0x00);
            send.Add((byte)(cmd.Length + 1));
            send.Add((byte)(cmd.Length + 1));
            send.AddRange(cmd);

            var response = Transmit(send.ToArray());

            return response;
        }
    }
}
