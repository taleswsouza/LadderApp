using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LadderApp
{
    public delegate void MudouComentarioEventHandler(Address sender);
    [Serializable]
    [XmlType(TypeName = "Endereco")]
    public class Address : IOperand
    {
        public event MudouOperandoEventHandler MudouOperando;
        public event MudouComentarioEventHandler MudouComentario;

        public Address()
        {
        }

        /// <summary>
        /// Indice do endereco
        /// </summary>
        private int id = 0;
        [XmlElement(Order = 1, ElementName = "Id")]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Tipo do endereco
        /// </summary>
        private AddressTypeEnum addressType = AddressTypeEnum.None;
        [XmlElement(Order = 5, ElementName = "Tipo")]
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
                            Acesso2 = "T" + id.ToString() + ".EN";
                            timer = new Timer();
                            break;
                        case AddressTypeEnum.DigitalMemoryCounter:
                            Acesso2 = "C" + id.ToString() + ".EN";
                            counter = new Counter();
                            break;
                        default:
                            break;
                    }
                    addressType = value;

                    if (MudouOperando != null)
                        MudouOperando(this);
                }
            }
        }

        private String name = "";
        [XmlElement(Order = 2)]
        public String Name
        {
            get
            {
                String NomeStr = "";
                switch (this.addressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        NomeStr = "E" + id.ToString() + "(P" + (((id - 1) / device.QtdBitsPorta) + 1) + "." + ((id - 1) - ((Int16)((id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + ")";
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        NomeStr = "S" + id.ToString() + "(P" + (((id - 1) / device.QtdBitsPorta) + 1) + "." + ((id - 1) - ((Int16)((id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + ")";
                        break;
                    case AddressTypeEnum.DigitalMemory:
                        NomeStr = "M" + id.ToString();
                        break;
                    case AddressTypeEnum.DigitalMemoryTimer:
                        NomeStr = "T" + id.ToString();
                        break;
                    case AddressTypeEnum.DigitalMemoryCounter:
                        NomeStr = "C" + id.ToString();
                        break;
                    default:
                        NomeStr = "ERROR";
                        break;
                }
                return NomeStr;
            }
            set { name = value; }
        }

        private String comment = "";
        [XmlElement(ElementName = "Apelido", Order = 6, IsNullable = true, Type = typeof(String))]
        public String Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                if (MudouComentario != null)
                    MudouComentario(this);
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
                        return "P" + (((id - 1) / device.QtdBitsPorta) + 1);
                    case AddressTypeEnum.DigitalOutput:
                        return "P" + (((id - 1) / device.QtdBitsPorta) + 1);
                    case AddressTypeEnum.DigitalMemory:
                        return "M" + ((id / BitsPorta) + 1);
                    case AddressTypeEnum.DigitalMemoryTimer:
                        return "T" + id.ToString();
                    case AddressTypeEnum.DigitalMemoryCounter:
                        return "C" + id.ToString();
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
                        return "P" + (((id - 1) / device.QtdBitsPorta) + 1) + "_DIR.Bit" + ((id - 1) - ((Int16)((id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + " = 0";
                    case AddressTypeEnum.DigitalOutput:
                        return "P" + (((id - 1) / device.QtdBitsPorta) + 1) + "_DIR.Bit" + ((id - 1) - ((Int16)((id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta)) + " = 1";
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
                        return "P" + (((id - 1) / device.QtdBitsPorta) + 1) + "_IN.Bit" + ((id - 1) - ((Int16)((id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta));
                    case AddressTypeEnum.DigitalOutput:
                        return "P" + (((id - 1) / device.QtdBitsPorta) + 1) + "_OUT.Bit" + ((id - 1) - ((Int16)((id - 1) / device.QtdBitsPorta) * device.QtdBitsPorta));
                    case AddressTypeEnum.DigitalMemory:
                        return "M" + ((id / BitsPorta) + 1) + ".Bit" + (id - (Int16)(id / BitsPorta) * BitsPorta);
                    case AddressTypeEnum.DigitalMemoryTimer:
                        return "T" + id.ToString() + ".DN";
                    case AddressTypeEnum.DigitalMemoryCounter:
                        return "C" + id.ToString() + ".DN";
                    default:
                        return "ERROR";
                }
            }
            //set { acesso = value; }
        }

        /// <summary>
        /// Para realizar o acesso ALTERNATIVO do endereco
        /// </summary>
        String acesso2 = "";
        [XmlElement(ElementName = "Acesso2", Order = 8, IsNullable = false, Type = typeof(String))]
        public String Acesso2
        {
            get { return acesso2; }
            set { acesso2 = value; }
        }

        /// <summary>
        /// Valor do endereco
        /// </summary>
        Boolean valor = false;
        [XmlElement(ElementName = "Valor", Order = 4, IsNullable = false, Type = typeof(Boolean))]
        public Boolean Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        /// <summary>
        /// Indica que o endereco esta em uso no programa
        /// </summary>
        Boolean bEmUso = false;
        [XmlIgnore]
        public Boolean EmUso
        {
            get { return bEmUso; }
            set { bEmUso = value; }
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="addressType">Tipo da endereco</param>
        /// <param name="_indicePosInicial">Indice identificador do endereco no tipo</param>
        public Address(AddressTypeEnum addressType, int index, Device device)
        {
            AddressType = addressType;
            this.id = index;
            this.device = device;
            BitsPorta = device.QtdBitsPorta;
        }

        public override string ToString()
        {
            return this.name;
        }

        private int bitsPorta = 0;
        [XmlElement(ElementName = "BitsPorta", Order = 3, IsNullable = false, Type = typeof(int))]
        public int BitsPorta
        {
            get { return bitsPorta; }
            set { bitsPorta = value; }
        }

        /// <summary>
        /// Aponta para o dispositivo do endereço
        /// </summary>
        private Device device = null;
        public void ApontaDispositivo(Device dispositivo)
        {
            this.device = dispositivo;
        }

        private Counter counter;
        [XmlIgnore]
        public Counter Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        private Timer timer;
        [XmlIgnore]
        public Timer Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        public static String ClassName()
        {
            return (new Address()).GetType().Name;
        }
    }
}
