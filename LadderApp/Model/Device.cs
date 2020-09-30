using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace LadderApp
{
    public class Device
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
        public List<Pin> pins = new List<Pin>();
        [XmlElement(ElementName = "EnderecoPinos")]
        public List<Address> addressesToEachPinList = new List<Address>();

        public Device()
        {
        }

        public Device(int idDispositivo)
        {
            InicialiazaPadrao();
        }

        void InicialiazaPadrao()
        {
            dispositivoId = 1;
            dispositivoFabricante = "Texas Instruments";
            dispositivoSerie = "MSP430";
            dispositivoModelo = "MSP430F2013";
            dispositivoQtdPortas = 2;
            dispositivoQtdBitsPorta = 8;

            for (int i = 1; i <= (dispositivoQtdPortas * dispositivoQtdBitsPorta); i++)
            {
                Pin pin = new Pin();

                //Apenas para criar alternancia dos tipos de pino
                if ((i <= 8) || (i >= 15 && i <= 16) )
                    pin.PinType = PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA;
                else
                    pin.PinType = PinTypeEnum.NENHUM;

                PinTypeEnum pinType = pin.PinType;

                addressesToEachPinList.Add(new Address(pin.Type, i, this));

                pins.Add(pin);
            }
        }

        public void RealocaEnderecoDispositivo()
        {
            for (int i = 0; i < (dispositivoQtdPortas * dispositivoQtdBitsPorta); i++)
            {
                addressesToEachPinList[i].AddressType = pins[i].Type;
            }
        }
    }
}
