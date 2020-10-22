using System;
using System.Xml.Serialization;

namespace LadderApp.Model
{
    public delegate void EditedCommentEventHandler(Address sender);

    [Serializable]
    [XmlType(TypeName = "address")]
    public class Address
    {

        [XmlIgnore]
        private AddressTypeEnum addressType = AddressTypeEnum.None;

        private String comment = "";

        private int numberOfBitsByPort;

        internal Address(AddressTypeEnum addressType, int index)
        {
            AddressType = addressType;
            Id = index;
        }

        internal Address(AddressTypeEnum addressType, int index, int numberOfBitsByPort)
        {
            AddressType = addressType;
            Id = index;
            this.numberOfBitsByPort = numberOfBitsByPort;
        }

        private Address()
        {
        }

        public event EditedCommentEventHandler EditedCommentEvent;

        [XmlElement(ElementName = "type")]
        public AddressTypeEnum AddressType
        {
            get { return addressType; }
            set
            {
                if (addressType != value)
                {
                    switch (value)
                    {
                        case AddressTypeEnum.DigitalInput:
                            break;

                        case AddressTypeEnum.DigitalOutput:
                            break;

                        case AddressTypeEnum.DigitalMemory:
                            break;

                        case AddressTypeEnum.DigitalMemoryTimer:
                            Timer = new Timer();
                            break;

                        case AddressTypeEnum.DigitalMemoryCounter:
                            break;

                        default:
                            break;
                    }
                    addressType = value;
                }
            }
        }

        [XmlElement(ElementName = "comment", IsNullable = true, Type = typeof(String))]
        public String Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                if (EditedCommentEvent != null)
                    EditedCommentEvent(this);
            }
        }

        [XmlElement(ElementName = "id")]
        public int Id { get; set; } = 0;

        [XmlIgnore]
        public Timer Timer { get; set; }

        [XmlIgnore]
        public bool Used { get; set; } = false;

        [XmlElement(ElementName = "value", IsNullable = false, Type = typeof(Boolean))]
        public bool Value { get; set; } = false;

        public string GetBitVariableName()
        {
            switch (this.addressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return $"{GetStructVariable()}_IN.Bit{GetBit()}";

                case AddressTypeEnum.DigitalOutput:
                    return $"{GetStructVariable()}_OUT.Bit{GetBit()}";

                case AddressTypeEnum.DigitalMemory:
                    return $"{GetStructVariable()}.Bit{GetBit()}";

                case AddressTypeEnum.DigitalMemoryTimer:
                    return $"{GetStructVariable()}.Done";

                case AddressTypeEnum.DigitalMemoryCounter:
                    return $"{GetStructVariable()}.Done";

                default:
                    return "ERROR";
            }
        }

        public string GetEnableBit()
        {
            return $"{GetStructVariable()}.Enable";
        }

        public string GetIOParameterization()
        {
            switch (addressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return $"{GetStructVariable()}_DIR.Bit{GetBit()} = 0";

                case AddressTypeEnum.DigitalOutput:
                    return $"{GetStructVariable()}_DIR.Bit{GetBit()} = 1";

                default:
                    return "ERROR";
            }
        }

        public string GetName()
        {
            switch (addressType)
            {
                case AddressTypeEnum.DigitalInput:
                case AddressTypeEnum.DigitalOutput:
                    return $"{addressType.GetDisplayPrefix()}{Id}({GetStructVariable()}.{GetBit()})";

                default:
                    return $"{addressType.GetDisplayPrefix()}{Id}";
            }
        }

        public String GetNameAndComment()
        {
            return $"{GetName()}{(Comment == "" ? "" : " - " + Comment)}";
        }

        public string GetStructVariable()
        {
            switch (this.addressType)
            {
                case AddressTypeEnum.DigitalInput:
                case AddressTypeEnum.DigitalOutput:
                case AddressTypeEnum.DigitalMemory:
                    return $"{addressType.GetInternalPrefix()}{GetWord()}";

                case AddressTypeEnum.DigitalMemoryTimer:
                case AddressTypeEnum.DigitalMemoryCounter:
                    return $"{addressType.GetInternalPrefix()}{Id}";

                default:
                    return "ERROR";
            }
        }

        public void SetNumberOfBitsByPort(int numberOfBitsByPort) => this.numberOfBitsByPort = numberOfBitsByPort;

        // TODO - solve this constant number
        private int GetNumberOfBitsByPort()
        {
            return 8;
        }

        private int GetBaseId()
        {
            return Id - 1;
        }

        private int GetBit()
        {
            return GetBaseId() % GetNumberOfBitsByPort();
        }

        private int GetWord()
        {
            return GetBaseId() / GetNumberOfBitsByPort() + 1;
        }

        public override bool Equals(object obj)
        {
            return obj is Address address &&
                   Id == address.Id &&
                   addressType == address.addressType;
        }

        public override int GetHashCode()
        {
            int hashCode = -932076469;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + addressType.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return GetName();
        }

    }
}