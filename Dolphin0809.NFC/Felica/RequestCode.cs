using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// リクエストコード<br />
    /// リクエストデータの指定
    /// </summary>
    public enum RequestCode : int
    {
        /// <summary>
        /// 要求なし
        /// </summary>
        NoRequest = 0x00,
        /// <summary>
        /// システムコード要求
        /// </summary>
        SystemCode = 0x01,
        /// <summary>
        /// 通信性能要求
        /// </summary>
        CommunicationPerformance = 0x02
    }
}
