using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp
{
    public class Pin
    {
        public Pin()
        {
        }
        public Pin(int _indice)
        {
            this.parametrizacao_C = this.parametrizacao_C.Replace("#", _indice.ToString());
        }

        /// <summary>
        /// Tipo da pino do dispositivo
        /// </summary>
        private PinTypeEnum tipoPino = PinTypeEnum.NENHUM;
        public PinTypeEnum TipoPino
        {
            get { return tipoPino; }
            set
            {
                if (value == PinTypeEnum.IO_DIGITAL_ENTRADA ||
                   value == PinTypeEnum.IO_DIGITAL_SAIDA ||
                   value == PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case PinTypeEnum.IO_DIGITAL_ENTRADA:
                            tipoDefinido = AddressTypeEnum.DigitalInput;
                            tipoPino = value;
                            break;
                        case PinTypeEnum.IO_DIGITAL_SAIDA:
                            tipoDefinido = AddressTypeEnum.DigitalOutput;
                            tipoPino = value;
                            break;
                        case PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA:
                            tipoDefinido = AddressTypeEnum.None;
                            tipoPino = value;
                            break;
                        default:
                            tipoDefinido = AddressTypeEnum.None;
                            tipoPino = PinTypeEnum.NENHUM;
                            break;
                    }
                }
                else
                    tipoPino = PinTypeEnum.NENHUM;

            }
        }

        /// <summary>
        /// Tipo da pino do dispositivo
        /// </summary>
        private AddressTypeEnum tipoDefinido = AddressTypeEnum.None;
        public AddressTypeEnum TipoDefinido
        {
            get { return tipoDefinido; }
            set
            {
                if (tipoPino == PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case AddressTypeEnum.DigitalInput:
                        case AddressTypeEnum.DigitalOutput:
                            tipoDefinido = value;
                            break;
                        default:
                            tipoDefinido = AddressTypeEnum.None;
                            break;
                    }
                }
                else if (tipoPino == PinTypeEnum.IO_DIGITAL_ENTRADA)
                    tipoDefinido = AddressTypeEnum.DigitalInput;
                else if (tipoPino == PinTypeEnum.IO_DIGITAL_SAIDA)
                    tipoDefinido = AddressTypeEnum.DigitalOutput;
                else
                    tipoDefinido = AddressTypeEnum.None;
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
