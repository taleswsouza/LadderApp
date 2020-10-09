using System;
using System.Xml.Serialization;

namespace LadderApp
{
    public delegate void EditedCommentEventHandler(Address sender);

    [Serializable]
    [XmlType(TypeName = "address")]
    public class Address : IOperand
    {
        public event ChangedOperandEventHandler ChangedOperandEvent;
        public event EditedCommentEventHandler EditedCommentEvent;

        public Address()
        {
        }

        public Address(AddressTypeEnum addressType, int index, Device device)
        {
            AddressType = addressType;
            Id = index;
            this.device = device;
            NumberOfBitsByPort = device.NumberBitsByPort;
        }

        [XmlElement(Order = 1, ElementName = "id")]
        public int Id { get; set; } = 0;

        private AddressTypeEnum addressType = AddressTypeEnum.None;
        [XmlElement(Order = 5, ElementName = "type")]
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
                            Counter = new Counter();
                            break;
                        default:
                            break;
                    }
                    addressType = value;

                    if (ChangedOperandEvent != null)
                        ChangedOperandEvent(this);
                }
            }
        }

        private String name = "";
        [XmlElement(Order = 2)]
        public String Name
        {
            get
            {
                switch (addressType)
                {
                    case AddressTypeEnum.DigitalInput:
                    case AddressTypeEnum.DigitalOutput:
                        return addressType.GetPrefix() + Id.ToString() + "(P" + (((Id - 1) / device.NumberBitsByPort) + 1) + "." + ((Id - 1) - ((Int16)((Id - 1) / device.NumberBitsByPort) * device.NumberBitsByPort)) + ")";
                    default:
                        return addressType.GetPrefix() + Id.ToString();
                }
            }
            set { name = value; }
        }

        private String comment = "";
        [XmlElement(ElementName = "comment", Order = 6, IsNullable = true, Type = typeof(String))]
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

        [XmlElement(ElementName = "value", Order = 4, IsNullable = false, Type = typeof(Boolean))]
        public bool Value { get; set; } = false;
        [XmlIgnore]
        public bool Used { get; set; } = false;

        [XmlElement(ElementName = "BitsPorta", Order = 3, IsNullable = false, Type = typeof(int))]
        public int NumberOfBitsByPort { get; set; } = 0;

        private Device device = null;
        public void SetDevice(Device device) => this.device = device;

        [XmlIgnore]
        public Counter Counter { get; set; }
        [XmlIgnore]
        public Timer Timer { get; set; }

        public String GetNameAndComment()
        {
            return $"{Name}{(Comment == "" ? "" : " - " + Comment)}";
        }

        public string GetPortParameterization()
        {
            switch (this.addressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return "P" + (((Id - 1) / device.NumberBitsByPort) + 1) + "_DIR.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.NumberBitsByPort) * device.NumberBitsByPort)) + " = 0";
                case AddressTypeEnum.DigitalOutput:
                    return "P" + (((Id - 1) / device.NumberBitsByPort) + 1) + "_DIR.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.NumberBitsByPort) * device.NumberBitsByPort)) + " = 1";
                default:
                    return "ERROR";
            }
        }

        public string GetVariableName()
        {
            switch (this.addressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return "P" + (((Id - 1) / device.NumberBitsByPort) + 1);
                case AddressTypeEnum.DigitalOutput:
                    return "P" + (((Id - 1) / device.NumberBitsByPort) + 1);
                case AddressTypeEnum.DigitalMemory:
                    return "M" + ((Id / NumberOfBitsByPort) + 1);
                case AddressTypeEnum.DigitalMemoryTimer:
                    return "T" + Id.ToString();
                case AddressTypeEnum.DigitalMemoryCounter:
                    return "C" + Id.ToString();
                default:
                    return "ERROR";
            }
        }

        public string GetEnableBit()
        {
            return $"{GetVariableName()}.Enable";
        }

        public string GetVariableBitValueName()
        {
            switch (this.addressType)
            {
                case AddressTypeEnum.DigitalInput:
                    return "P" + (((Id - 1) / device.NumberBitsByPort) + 1) + "_IN.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.NumberBitsByPort) * device.NumberBitsByPort));
                case AddressTypeEnum.DigitalOutput:
                    return "P" + (((Id - 1) / device.NumberBitsByPort) + 1) + "_OUT.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.NumberBitsByPort) * device.NumberBitsByPort));
                case AddressTypeEnum.DigitalMemory:
                    return "M" + ((Id / NumberOfBitsByPort) + 1) + ".Bit" + (Id - (Int16)(Id / NumberOfBitsByPort) * NumberOfBitsByPort);
                case AddressTypeEnum.DigitalMemoryTimer:
                    return $"{GetVariableName()}.Done";
                case AddressTypeEnum.DigitalMemoryCounter:
                    return $"{GetVariableName()}.Done";
                default:
                    return "ERROR";
            }
        }

        public override string ToString()
        {
            return this.name;
        }

    }
}
