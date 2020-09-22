using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LadderApp
{
    public delegate void MudouComentarioEventHandler(EnderecamentoLadder sender);
    [Serializable]
    [XmlType(TypeName = "Endereco")]
    public class EnderecamentoLadder : IOperando
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
        private TipoEnderecamentoDispositivo tpEnderecamento = TipoEnderecamentoDispositivo.NENHUM;
        [XmlElement(Order = 5, ElementName = "Tipo")]
        public TipoEnderecamentoDispositivo TpEnderecamento
        {
            get { return tpEnderecamento; }
            set {
                if (tpEnderecamento != value)
                {
                    switch (value)
                    {
                        case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                           break;
                        case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                            break;
                        case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                            break;
                        case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                            Acesso2 = "T" + indice.ToString() + ".EN";
                            temporizador = new SimulaTemporizador();
                            break;
                        case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                            Acesso2 = "C" + indice.ToString() + ".EN";
                            contador = new SimulaContador();
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
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        NomeStr = "E" + indice.ToString() + "(P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "." + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + ")";
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        NomeStr = "S" + indice.ToString() + "(P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "." + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + ")";
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                        NomeStr = "M" + indice.ToString();
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                        NomeStr = "T" + indice.ToString();
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
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
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1);
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1);
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                        return "M" + ((indice / BitsPorta) + 1);
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                        return "T" + indice.ToString();
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
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
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_DIR.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta)) + " = 0";
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
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
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_IN.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta));
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        return "P" + (((indice - 1) / dispositivo.QtdBitsPorta) + 1) + "_OUT.Bit" + ((indice - 1) - ((Int16)((indice - 1) / dispositivo.QtdBitsPorta) * dispositivo.QtdBitsPorta));
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                        return "M" + ((indice / BitsPorta) + 1) + ".Bit" + (indice - (Int16)(indice / BitsPorta) * BitsPorta);
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                        return "T" + indice.ToString() + ".DN";
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
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
        public EnderecamentoLadder(TipoEnderecamentoDispositivo _tpE, int _indice, DispositivoLadder dispositivo)
        {
            this.dispositivo = dispositivo;
            indice = _indice;
            BitsPorta = dispositivo.QtdBitsPorta;
            TpEnderecamento = _tpE;
        }

        public EnderecamentoLadder()
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
        private DispositivoLadder dispositivo = null;
        public void ApontaDispositivo(DispositivoLadder dispositivo)
        {
            this.dispositivo = dispositivo;
        }

        /// <summary>
        /// Permite simulado de contadores
        /// </summary>
        SimulaContador contador = null;
        [XmlIgnore]
        public SimulaContador Contador
        {
            get { return contador; }
            set { contador = value; }
        }

        /// <summary>
        /// Permite simulado de temporizadores
        /// </summary>
        SimulaTemporizador temporizador = null;
        [XmlIgnore]
        public SimulaTemporizador Temporizador
        {
            get { return temporizador; }
            set { temporizador = value; }
        }

        public static String ClassName()
        {
            return (new EnderecamentoLadder()).GetType().Name;
        }
    }
}
