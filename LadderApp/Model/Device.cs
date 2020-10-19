using System;
using System.Collections.Generic;

namespace LadderApp
{
    [Serializable]
    public class Device
    {
        internal Device()
        {
        }

        public int Id { get; set; } = -1;
        public string Manufacturer { get; set; }
        public string Series { get; set; }
        public string Model { get; set; }
        public int NumberOfPorts { get; set; } = 0;
        public int NumberBitsByPort { get; set; } = 0;
        public List<Address> PinAddresses { get; set; } = new List<Address>();
        public List<Pin> Pins { get; set; } = new List<Pin>();
    }
}
