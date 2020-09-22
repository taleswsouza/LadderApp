using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace LadderApp
{
    public class DispositivoLadder
    {
        /// <summary>
        /// id - Identificador
        /// </summary>
        private int dispositivoId = -1;
        public int Id
        {
            get { return dispositivoId; }
            set { dispositivoId = value; }
        }

        /// <summary>
        /// Fabricante - Fabricante do programa.dispositivo
        /// </summary>
        private String dispositivoFabricante;
        //[XmlIgnore]
        public String Fabricante
        { get { return dispositivoFabricante; }
          set { dispositivoFabricante = value; }
        }


        /// <summary>
        /// Serie - Serie do dispositivo
        /// </summary>
        private String dispositivoSerie;
        public String Serie
        { 
            get { return dispositivoSerie; }
            set { dispositivoSerie = value; }
        }
        
        /// <summary>
        /// Modelo - Modelo do dispositivo
        /// </summary>
        private String dispositivoModelo;
        public String Modelo
        { 
            get { return dispositivoModelo; }
            set { dispositivoModelo = value; }
        }

        /// <summary>
        /// Quantidade de pinos fisicos do dispositivo.
        /// </summary>
        private int dispositivoQtdPortas = 0;
        public int QtdPortas
        { 
            get { return dispositivoQtdPortas; }
            set { dispositivoQtdPortas = value; }
        }

        /// <summary>
        /// Quantidade de bits por porta do dispositivo.
        /// </summary>
        private int dispositivoQtdBitsPorta = 0;
        public int QtdBitsPorta
        {
            get { return dispositivoQtdBitsPorta; }
            set { dispositivoQtdBitsPorta = value; }
        }


        [XmlElement(ElementName = "Bit")]
        public List<BitPortasDispositivo> lstBitPorta = new List<BitPortasDispositivo>();
        [XmlElement(ElementName = "EnderecoPinos")]
        public List<Address> lstEndBitPorta = new List<Address>();

        public DispositivoLadder()
        {
        }

        public DispositivoLadder(int idDispositivo)
        {
            InicialiazaPadrao();
        }

        void InicialiazaPadrao()
        {
            BitPortasDispositivo _pinoAux = null;
            TiposPinosDispositivo _tpPino = TiposPinosDispositivo.IO_DIGITAL_ENTRADA;

            dispositivoId = 1;
            dispositivoFabricante = "Texas Instruments";
            dispositivoSerie = "MSP430";
            dispositivoModelo = "MSP430F2013";
            dispositivoQtdPortas = 2;
            dispositivoQtdBitsPorta = 8;

            for (int i = 1; i <= (dispositivoQtdPortas * dispositivoQtdBitsPorta); i++)
            {
                _pinoAux = new BitPortasDispositivo(i);

                //Apenas para criar alternancia dos tipos de pino
                if ((i <= 8) || (i >= 15 && i <= 16) )
                    _pinoAux.TipoPino = TiposPinosDispositivo.IO_DIGITAL_ENTRADA_OU_SAIDA;
                else
                    _pinoAux.TipoPino = TiposPinosDispositivo.NENHUM;

                _tpPino = _pinoAux.TipoPino;

                lstEndBitPorta.Add(new Address(_pinoAux.TipoDefinido, i, this));

                lstBitPorta.Add(_pinoAux);
            }
        }

        public void RealocaEnderecoDispositivo()
        {
            for (int i = 0; i < (dispositivoQtdPortas * dispositivoQtdBitsPorta); i++)
            {
                lstEndBitPorta[i].TpEnderecamento = lstBitPorta[i].TipoDefinido;
            }
        }
    }
}
