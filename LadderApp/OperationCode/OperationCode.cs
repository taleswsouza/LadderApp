using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public enum OperationCode
    {
        None = 0,
        LineBegin = 1,
        LineEnd = 2,
        BackgroundLine = 3,
        NormallyOpenContact = 10,
        NormallyClosedContact = 11,
        OutputCoil = 12,
        Timer = 13,
        Counter = 14,
        ParallelBranchBegin = 15,
        ParallelBranchEnd = 16,
        ParallelBranchNext = 17,
        Reset = 18,
        HeadLenght = 100,
        HeadDeviceId = 105,
        HeadPassword0 = 110,
        HeadAddressingRecords = 120,
    }
}
