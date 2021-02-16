using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolphin0809.NFC.Felica
{
    /// <summary>
    /// タイムスロット<br />
    /// 応答可能な最大スロット数の指定
    /// </summary>
    public enum TimeSlot : int
    {
        x1 = 0x00,
        x2 = 0x01,
        x4 = 0x03,
        x8 = 0x07,
        x16 = 0x0F
    }
}
