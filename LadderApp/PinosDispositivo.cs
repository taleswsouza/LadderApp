using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp
{
    public class BitPortasDispositivo
    {
        public BitPortasDispositivo()
        {
        }
        public BitPortasDispositivo(int _indice)
        {
            this.parametrizacao_C = this.parametrizacao_C.Replace("#", _indice.ToString());
        }

        /// <summary>
        /// Tipo da pino do dispositivo
        /// </summary>
        private PinType tipoPino = PinType.NENHUM;
        public PinType TipoPino
        {
            get { return tipoPino; }
            set
            {
                if (value == PinType.IO_DIGITAL_ENTRADA ||
                   value == PinType.IO_DIGITAL_SAIDA ||
                   value == PinType.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case PinType.IO_DIGITAL_ENTRADA:
                            tipoDefinido = AddressType.DIGITAL_ENTRADA;
                            tipoPino = value;
                            break;
                        case PinType.IO_DIGITAL_SAIDA:
                            tipoDefinido = AddressType.DIGITAL_SAIDA;
                            tipoPino = value;
                            break;
                        case PinType.IO_DIGITAL_ENTRADA_OU_SAIDA:
                            tipoDefinido = AddressType.NENHUM;
                            tipoPino = value;
                            break;
                        default:
                            tipoDefinido = AddressType.NENHUM;
                            tipoPino = PinType.NENHUM;
                            break;
                    }
                }
                else
                    tipoPino = PinType.NENHUM;

            }
        }

        /// <summary>
        /// Tipo da pino do dispositivo
        /// </summary>
        private AddressType tipoDefinido = AddressType.NENHUM;
        public AddressType TipoDefinido
        {
            get { return tipoDefinido; }
            set
            {
                if (tipoPino == PinType.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case AddressType.DIGITAL_ENTRADA:
                        case AddressType.DIGITAL_SAIDA:
                            tipoDefinido = value;
                            break;
                        default:
                            tipoDefinido = AddressType.NENHUM;
                            break;
                    }
                }
                else if (tipoPino == PinType.IO_DIGITAL_ENTRADA)
                    tipoDefinido = AddressType.DIGITAL_ENTRADA;
                else if (tipoPino == PinType.IO_DIGITAL_SAIDA)
                    tipoDefinido = AddressType.DIGITAL_SAIDA;
                else
                    tipoDefinido = AddressType.NENHUM;
            }
        }

        /// <summary>
        /// String para acesso ao IO via codigo C
        /// </summary>
        private String acesso_C = "";
        [XmlIgnore]
        public String Acesso_C
        {
            get { return acesso_C; }
            set { acesso_C = value; }
        }

        /// <summary>
        /// String para parametrizacao do IO via codigo C
        /// </summary>
        private String parametrizacao_C = "P#DIR_bit.P1DIR_#";
        [XmlIgnore]
        public String Parametrizacao_C
        {
            get { return parametrizacao_C; }
            set { parametrizacao_C = value; }
        }
    }
}
