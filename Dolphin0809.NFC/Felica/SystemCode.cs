using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// システムコード
    /// </summary>
    public enum SystemCode : int
    {
        /// <summary>
        /// 交通系ICカード Suica,PASMO,ICOCA,etc...
        /// </summary>
        TransportationICCard = 0x0003,
        /// <summary>
        /// FeliCa Lite-S
        /// </summary>
        FelicaLiteS = 0x88B4,
        /// <summary>
        /// ワイルドカード
        /// </summary>
        Wildcard = 0xFFFF
    }
}
