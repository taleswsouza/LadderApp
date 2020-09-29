using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
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
            this.Id = index;
            this.device = device;
            BitsPorta = device.QtdBitsPorta;
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
                            Acesso2 = "T" + Id.ToString() + ".EN";
                            Timer = new Timer();
                            break;
                        case AddressTypeEnum.DigitalMemoryCounter:
                            Acesso2 = "C" + Id.ToString() + ".EN";
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
                        return addressType.GetPrefix() + Id.ToString() + "(P" + (((Id - 1) / device.QtdBitsPorta) + 1) + "." + ((Id - 1) - ((Int16)((Id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + ")";
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

        [XmlIgnore]
        public String EnderecoRaiz
        {
            get
            {
                switch (this.addressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        return "P" + (((Id - 1) / device.QtdBitsPorta) + 1);
                    case AddressTypeEnum.DigitalOutput:
                        return "P" + (((Id - 1) / device.QtdBitsPorta) + 1);
                    case AddressTypeEnum.DigitalMemory:
                        return "M" + ((Id / BitsPorta) + 1);
                    case AddressTypeEnum.DigitalMemoryTimer:
                        return "T" + Id.ToString();
                    case AddressTypeEnum.DigitalMemoryCounter:
                        return "C" + Id.ToString();
                    default:
                        return "ERROR";
                }
            }
        }

        /// <summary>
        /// Para realizar a Parametrização do endereco dentro do código C
        [XmlIgnore]
        public String Parametro
        {
            get
            {
                switch (this.addressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        return "P" + (((Id - 1) / device.QtdBitsPorta) + 1) + "_DIR.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + " = 0";
                    case AddressTypeEnum.DigitalOutput:
                        return "P" + (((Id - 1) / device.QtdBitsPorta) + 1) + "_DIR.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + " = 1";
                    default:
                        return "ERROR";
                }
            }
            //set { parametro = value; }
        }

        /// <summary>
        /// Para realizar o acesso do endereco dentro do código C
        /// </summary>
        [XmlIgnore]
        public String Acesso
        {
            get
            {
                switch (this.addressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        return "P" + (((Id - 1) / device.QtdBitsPorta) + 1) + "_IN.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta));
                    case AddressTypeEnum.DigitalOutput:
                        return "P" + (((Id - 1) / device.QtdBitsPorta) + 1) + "_OUT.Bit" + ((Id - 1) - ((Int16)((Id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta));
                    case AddressTypeEnum.DigitalMemory:
                        return "M" + ((Id / BitsPorta) + 1) + ".Bit" + (Id - (Int16)(Id / BitsPorta) * BitsPorta);
                    case AddressTypeEnum.DigitalMemoryTimer:
                        return "T" + Id.ToString() + ".DN";
                    case AddressTypeEnum.DigitalMemoryCounter:
                        return "C" + Id.ToString() + ".DN";
                    default:
                        return "ERROR";
                }
            }
        }
        [XmlElement(ElementName = "Acesso2", Order = 8, IsNullable = false, Type = typeof(String))]
        public string Acesso2 { get; set; } = "";
        [XmlElement(ElementName = "value", Order = 4, IsNullable = false, Type = typeof(Boolean))]
        public bool Value { get; set; } = false;
        [XmlIgnore]
        public bool Used { get; set; } = false;

        public override string ToString()
        {
            return this.name;
        }

        [XmlElement(ElementName = "BitsPorta", Order = 3, IsNullable = false, Type = typeof(int))]
        public int BitsPorta { get; set; } = 0;

        private Device device = null;
        public void SetDevice(Device device) => this.device = device;

        [XmlIgnore]
        public Counter Counter { get; set; }
        [XmlIgnore]
        public Timer Timer { get; set; }

        //public static String ClassName()
        //{
        //    return (new Address()).GetType().Name;
        //}
    }
}
