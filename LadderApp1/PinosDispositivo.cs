using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp1
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
        private TiposPinosDispositivo tipoPino = TiposPinosDispositivo.NENHUM;
        public TiposPinosDispositivo TipoPino
        {
            get { return tipoPino; }
            set
            {
                if (value == TiposPinosDispositivo.IO_DIGITAL_ENTRADA ||
                   value == TiposPinosDispositivo.IO_DIGITAL_SAIDA ||
                   value == TiposPinosDispositivo.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case TiposPinosDispositivo.IO_DIGITAL_ENTRADA:
                            tipoDefinido = TipoEnderecamentoDispositivo.DIGITAL_ENTRADA;
                            tipoPino = value;
                            break;
                        case TiposPinosDispositivo.IO_DIGITAL_SAIDA:
                            tipoDefinido = TipoEnderecamentoDispositivo.DIGITAL_SAIDA;
                            tipoPino = value;
                            break;
                        case TiposPinosDispositivo.IO_DIGITAL_ENTRADA_OU_SAIDA:
                            tipoDefinido = TipoEnderecamentoDispositivo.NENHUM;
                            tipoPino = value;
                            break;
                        default:
                            tipoDefinido = TipoEnderecamentoDispositivo.NENHUM;
                            tipoPino = TiposPinosDispositivo.NENHUM;
                            break;
                    }
                }
                else
                    tipoPino = TiposPinosDispositivo.NENHUM;

            }
        }

        /// <summary>
        /// Tipo da pino do dispositivo
        /// </summary>
        private TipoEnderecamentoDispositivo tipoDefinido = TipoEnderecamentoDispositivo.NENHUM;
        public TipoEnderecamentoDispositivo TipoDefinido
        {
            get { return tipoDefinido; }
            set
            {
                if (tipoPino == TiposPinosDispositivo.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                            tipoDefinido = value;
                            break;
                        default:
                            tipoDefinido = TipoEnderecamentoDispositivo.NENHUM;
                            break;
                    }
                }
                else if (tipoPino == TiposPinosDispositivo.IO_DIGITAL_ENTRADA)
                    tipoDefinido = TipoEnderecamentoDispositivo.DIGITAL_ENTRADA;
                else if (tipoPino == TiposPinosDispositivo.IO_DIGITAL_SAIDA)
                    tipoDefinido = TipoEnderecamentoDispositivo.DIGITAL_SAIDA;
                else
                    tipoDefinido = TipoEnderecamentoDispositivo.NENHUM;
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
