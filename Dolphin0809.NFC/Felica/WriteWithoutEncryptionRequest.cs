using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// 暗号化なし書き込み要求
    /// </summary>
    public class WriteWithoutEncryptionRequest
    {
        private byte[] _idm = null;
        private readonly List<byte> _serviceCodeList = new List<byte>();
        private readonly List<byte> _blockList = new List<byte>();
        private readonly List<byte> _blockData = new List<byte>();

        /// <summary>
        /// コマンドコード
        /// </summary>
        public int CommandCode
        {
            get
            {
                return 0x08;
            }
        }
        /// <summary>
        /// IDm
        /// </summary>
        public byte[] IDm
        {
            get
            {
                return _idm;
            }
            set
            {
                if (value == null)
                {
                    throw new Exception("IDmの値がnullです。");
                }

                if (value.Length != 8)
                {
                    throw new Exception("IDmの長さが不正です。");
                }

                _idm = value;
            }
        }
        /// <summary>
        /// サービス数
        /// </summary>
        public int NumberOfService
        {
            get
            {
                return _serviceCodeList.Count / 2;
            }
        }
        /// <summary>
        /// サービスコードリスト
        /// </summary>
        public byte[] ServiceCodeList
        {
            get
            {
                return _serviceCodeList.ToArray();
            }
        }
        /// <summary>
        /// ブロック数
        /// </summary>
        public int NumberOfBlock { get; private set; } = 0;
        /// <summary>
        /// ブロックリスト
        /// </summary>
        public byte[] BlockList
        {
            get
            {
                return _blockList.ToArray();
            }
        }

        public byte[] BlockData
        {
            get
            {
                return _blockData.ToArray();
            }
        }

        /// <summary>
        /// コマンドパケット
        /// </summary>
        public byte[] Command
        {
            get
            {
                List<byte> cmd = new List<byte>(11 + _serviceCodeList.Count + _blockList.Count + _blockData.Count);
                cmd.Add((byte)(CommandCode & 0xFF));
                cmd.AddRange(IDm);
                cmd.Add((byte)(NumberOfService & 0xFF));
                cmd.AddRange(ServiceCodeList);
                cmd.Add((byte)(NumberOfBlock & 0xFF));
                cmd.AddRange(BlockList);
                cmd.AddRange(BlockData);

                return cmd.ToArray();
            }
        }

        /// <summary>
        /// 暗号化なし書き込み要求を生成します。
        /// </summary>
        /// <param name="idm">IDm</param>
        /// <exception cref="Exception" />
        public WriteWithoutEncryptionRequest(byte[] idm)
        {
            IDm = idm;
        }

        /// <summary>
        /// 書き込むブロックを追加<br />
        /// サービスコードは初回のみ有効、2回目以降は無視されます。<br />
        /// 有効な値の組み合わせ: [{blockIndex: 0x00 - 0xFF, blockIndexSize: 1},
        /// {blockIndex: 0x0000 - 0xFFFF, blockIndexSize: 2}]
        /// </summary>
        /// <param name="serviceCode">サービスコード</param>
        /// <param name="blockIndex">ブロック番号</param>
        /// <param name="blockIndexSize">ブロック番号サイズ</param>
        /// <param name="blockData">ブロックデータ</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="Exception" />
        public void AddWriteBlock(int serviceCode, int blockIndex, int blockIndexSize, byte[] blockData)
        {
            // 引数チェック: blockIndexSize
            if (blockIndexSize < 1 || 2 < blockIndexSize)
            {
                throw new ArgumentException("不正な値です。", "blockIndexSize");
            }

            // 引数チェック: blockData
            if (blockData == null)
            {
                throw new ArgumentNullException("blockData");
            }
            if (blockData.Length != 16)
            {
                throw new ArgumentException("不正な長さです。", "blockData");
            }

            // ブロック数チェック
            if (2 <= NumberOfBlock)
            {
                throw new Exception("同時に3ブロック以上は書き込めません。");
            }

            // サービスコードリスト
            if (_serviceCodeList.Count == 0)
            {
                _serviceCodeList.Add((byte)(serviceCode & 0xFF));
                _serviceCodeList.Add((byte)((serviceCode >> 8) & 0xFF));
            }

            // ブロック数
            NumberOfBlock++;

            // ブロックリスト
            if (blockIndexSize == 1)
            {
                _blockList.Add(0x80);
                _blockList.Add((byte)(blockIndex & 0xFF));
            }
            else if (blockIndex == 2)
            {
                _blockList.Add(0x00);
                _blockList.Add((byte)(blockIndex & 0xFF));
                _blockList.Add((byte)((blockIndex >> 8) & 0xFF));
            }

            // ブロックデータ
            _blockData.AddRange(blockData);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine();
            sb.AppendLine($"\tCommandCode:\t{CommandCode:x02}");
            sb.AppendLine($"\tIDm:\t\t{string.Join("", IDm.Select(val => $"{val:x02}"))}");
            sb.AppendLine($"\tNumberOfService:{NumberOfService:x02}");
            sb.AppendLine($"\tServiceCodeList:{string.Join("", ServiceCodeList.Select(val => $"{val:x02}"))}");
            sb.AppendLine($"\tNumberOfBlock:\t{NumberOfBlock:x02}");
            sb.AppendLine($"\tBlockList:\t{string.Join("", BlockList.Select(val => $"{val:x02}"))}");
            sb.AppendLine($"\tBlockData:\t{string.Join("", BlockData.Select(val => $"{val:x02}"))}");
            sb.Append($"\tCommand:\t{string.Join("", Command.Select(val => $"{val:x02}"))}");

            return sb.ToString();
        }
    }
}