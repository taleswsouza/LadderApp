using System;
using System.Xml.Serialization;

namespace LadderApp.Model
{
    [Serializable]
    [XmlType(TypeName = "timer")]
    public class TimerAddress : OutputBoxAddress
    {
        private TimerAddress() : base()
        {

        }
        public TimerAddress(int index, int numberOfBitsByPort) : base(AddressTypeEnum.DigitalMemoryTimer, index, numberOfBitsByPort)
        {

        }
        public int TimeBase { get; set; }
        [XmlIgnore]
        public int ParcialAccumulated { get; set; }
        [XmlIgnore]
        public int ParcialPreset
        {
            get
            {
                int parcialPreset = 0;

                switch (this.TimeBase)
                {
                    case 1: /// 100 ms
                        parcialPreset = 1;
                        break;
                    case 2: /// 1 second
                        parcialPreset = 1 * 10;
                        break;
                    case 3: /// 1 minute
                        parcialPreset = 1 * 10 * 60;
                        break;
                    default:
                        parcialPreset = 0;
                        break;
                }
                return parcialPreset;
            }
        }
    }
}