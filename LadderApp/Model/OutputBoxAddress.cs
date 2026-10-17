using System.Xml.Serialization;

namespace LadderApp.Model
{
    public class OutputBoxAddress : Address
    {
        protected OutputBoxAddress() : base()
        {

        }
        public OutputBoxAddress(AddressTypeEnum type, int index, int numberOfBitsByPort) : base(type, index, numberOfBitsByPort)
        {

        }

        public override bool Value { get => Done; set => Done = value; }

        public int Type { get; set; }

        public int Preset { get; set; }

        [XmlIgnore]
        public int Accumulated { get; set; }

        [XmlIgnore]
        public bool Done { get; set; }

        [XmlIgnore]
        public bool Enable { get; set; }

        [XmlIgnore]
        public bool Pulse { get; set; }

        [XmlIgnore]
        public bool Reset { get; set; }
    }
}