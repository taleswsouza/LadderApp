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
                            tipoDefinido = AddressTypeEnum.DIGITAL_ENTRADA;
                            tipoPino = value;
                            break;
                        case PinTypeEnum.IO_DIGITAL_SAIDA:
                            tipoDefinido = AddressTypeEnum.DIGITAL_SAIDA;
                            tipoPino = value;
                            break;
                        case PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA:
                            tipoDefinido = AddressTypeEnum.NENHUM;
                            tipoPino = value;
                            break;
                        default:
                            tipoDefinido = AddressTypeEnum.NENHUM;
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
        private AddressTypeEnum tipoDefinido = AddressTypeEnum.NENHUM;
        public AddressTypeEnum TipoDefinido
        {
            get { return tipoDefinido; }
            set
            {
                if (tipoPino == PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case AddressTypeEnum.DIGITAL_ENTRADA:
                        case AddressTypeEnum.DIGITAL_SAIDA:
                            tipoDefinido = value;
                            break;
                        default:
                            tipoDefinido = AddressTypeEnum.NENHUM;
                            break;
                    }
                }
                else if (tipoPino == PinTypeEnum.IO_DIGITAL_ENTRADA)
                    tipoDefinido = AddressTypeEnum.DIGITAL_ENTRADA;
                else if (tipoPino == PinTypeEnum.IO_DIGITAL_SAIDA)
                    tipoDefinido = AddressTypeEnum.DIGITAL_SAIDA;
                else
                    tipoDefinido = AddressTypeEnum.NENHUM;
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
