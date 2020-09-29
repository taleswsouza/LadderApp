using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public enum AddressTypeEnum
    {
        None = 0,
        DigitalInput = 5,
        DigitalOutput = 6,
        DigitalMemory = 7,
        DigitalMemoryTimer = 8,
        DigitalMemoryCounter = 9
    }
    public static class AddressTypeEnumExtensions
    {
        public static String GetPrefix(this AddressTypeEnum addressType)
        {
            switch (addressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return "I";
                case AddressTypeEnum.DigitalOutput:
                    return "O";
                case AddressTypeEnum.DigitalMemory:
                    return "M";
                case AddressTypeEnum.DigitalMemoryTimer:
                    return "T";
                case AddressTypeEnum.DigitalMemoryCounter:
                    return "C";
                default:
                    return "ERROR";
            }
        }
    }
}
