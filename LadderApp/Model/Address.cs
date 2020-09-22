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
        /// <summary>
        /// Indice do endereco
        /// </summary>
        private int indice = 0;
        [XmlElement(Order=1, ElementName="Id")]
        public int Indice
        {
            get { return indice; }
            set { indice = value; }
        }

        /// <summary>
        /// Tipo do endereco
        /// </summary>
        private AddressTypeEnum tpEnderecamento = AddressTypeEnum.NENHUM;
        [XmlElement(Order = 5, ElementName = "Tipo")]
        public AddressTypeEnum TpEnderecamento
        {
            get { return tpEnderecamento; }
            set {
                if (tpEnderecamento != value)
                {
                    switch (value)
                    {
                        case AddressTypeEnum.DIGITAL_ENTRADA:
                           break;
                        case AddressTypeEnum.DIGITAL_SAIDA:
                            break;
                        case AddressTypeEnum.DIGITAL_MEMORIA:
                            break;
                        case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                            Acesso2 = "T" + indice.ToString() + ".EN";
                            temporizador = new Timer();
                            break;
                        case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                            Acesso2 = "C" + indice.ToString() + ".EN";
                            contador = new Counter();
                            break;
                        default:
                            break;
                    }
                    tpEnderecamento = value;

                    if (MudouOperando != null)
                        MudouOperando(this);
                }
            }
        }

        /// <summary>
        /// Nome do endereco
        /// </summary>
        String nomeEndereco = "";
        [XmlElement(Order = 2)]
        //[XmlIgnore]
        public String Nome
        {
            get {
                String NomeStr = "";
                switch (this.tpEnderecamento)
                {
                    case AddressTypeEnum.DIGITAL_ENTRADA:
                        NomeStr = "E" + indice.ToString() + "(P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "." + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + ")";
                        break;
                    case AddressTypeEnum.DIGITAL_SAIDA:
                        NomeStr = "S" + indice.ToString() + "(P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "." + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + ")";
                        break;
                    case AddressTypeEnum.DIGITAL_MEMORIA:
                        NomeStr = "M" + indice.ToString();
                        break;
                    case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                        NomeStr = "T" + indice.ToString();
                        break;
                    case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                        NomeStr = "C" + indice.ToString();
                        break;
                    default:
                        NomeStr = "ERROR";
                        break;
                }
                return NomeStr; 
            }
            set { nomeEndereco = value; }
        }

        /// <summary>
        /// Apelido do endereco
        /// </summary>
        String apelido = "";
        [XmlElement(ElementName = "Apelido", Order = 6, IsNullable = true, Type = typeof(String))]
        public String Apelido
        {
            get { return apelido; }
            set {
                apelido = value;

                if (MudouComentario != null)
                    MudouComentario(this);
            }
        }

        /// <summary>
        /// Endereço raiz ou endereço PAI
        /// </summary>
//        String enderecoRaiz = "";
        [XmlIgnore]
        public String EnderecoRaiz
        {
            get {
                switch (this.tpEnderecamento)
                {
                    case AddressTypeEnum.DIGITAL_ENTRADA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1);
                    case AddressTypeEnum.DIGITAL_SAIDA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1);
                    case AddressTypeEnum.DIGITAL_MEMORIA:
                        return "M" + ((indice / BitsPorta) + 1);
                    case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                        return "T" + indice.ToString();
                    case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                        return "C" + indice.ToString();
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
            get {
                switch (this.tpEnderecamento)
                {
                    case AddressTypeEnum.DIGITAL_ENTRADA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_DIR.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + " = 0";
                    case AddressTypeEnum.DIGITAL_SAIDA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_DIR.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + " = 1";
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
            get {
                switch (this.tpEnderecamento)
                {
                    case AddressTypeEnum.DIGITAL_ENTRADA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_IN.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta));
                    case AddressTypeEnum.DIGITAL_SAIDA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_OUT.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta));
                    case AddressTypeEnum.DIGITAL_MEMORIA:
                        return "M" + ((indice / BitsPorta) + 1) + ".Bit" + (indice - (Int16)(indice / BitsPorta) * BitsPorta);
                    case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                        return "T" + indice.ToString() + ".DN";
                    case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                        return "C" + indice.ToString() + ".DN";
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
        [XmlElement(ElementName="Valor", Order = 4, IsNullable=false, Type=typeof(Boolean))]
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
        /// <param name="_tpE">Tipo da endereco</param>
        /// <param name="_indicePosInicial">Indice identificador do endereco no tipo</param>
        public Address(AddressTypeEnum _tpE, int _indice, Device dispositivo)
        {
            this.dispositivo = dispositivo;
            indice = _indice;
            BitsPorta = dispositivo.QtdBitsPorta;
            TpEnderecamento = _tpE;
        }

        public Address()
        {
        }


        public override string ToString()
        {
            return this.nomeEndereco;
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
        private Device dispositivo = null;
        public void ApontaDispositivo(Device dispositivo)
        {
            this.dispositivo = dispositivo;
        }

        /// <summary>
        /// Permite simulado de contadores
        /// </summary>
        Counter contador = null;
        [XmlIgnore]
        public Counter Contador
        {
            get { return contador; }
            set { contador = value; }
        }

        /// <summary>
        /// Permite simulado de temporizadores
        /// </summary>
        Timer temporizador = null;
        [XmlIgnore]
        public Timer Temporizador
        {
            get { return temporizador; }
            set { temporizador = value; }
        }

        public static String ClassName()
        {
            return (new Address()).GetType().Name;
        }
    }
}
