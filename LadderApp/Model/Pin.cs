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

        public Pin(int index)
        {
        }

        private PinTypeEnum pinType = PinTypeEnum.NENHUM;
        public PinTypeEnum PinType
        {
            get { return pinType; }
            set
            {
                if (value == PinTypeEnum.IO_DIGITAL_ENTRADA ||
                   value == PinTypeEnum.IO_DIGITAL_SAIDA ||
                   value == PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case PinTypeEnum.IO_DIGITAL_ENTRADA:
                            type = AddressTypeEnum.DigitalInput;
                            pinType = value;
                            break;
                        case PinTypeEnum.IO_DIGITAL_SAIDA:
                            type = AddressTypeEnum.DigitalOutput;
                            pinType = value;
                            break;
                        case PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA:
                            type = AddressTypeEnum.None;
                            pinType = value;
                            break;
                        default:
                            type = AddressTypeEnum.None;
                            pinType = PinTypeEnum.NENHUM;
                            break;
                    }
                }
                else
                    pinType = PinTypeEnum.NENHUM;

            }
        }

        private AddressTypeEnum type = AddressTypeEnum.None;
        public AddressTypeEnum Type
        {
            get { return type; }
            set
            {
                if (pinType == PinTypeEnum.IO_DIGITAL_ENTRADA_OU_SAIDA)
                {
                    switch (value)
                    {
                        case AddressTypeEnum.DigitalInput:
                        case AddressTypeEnum.DigitalOutput:
                            type = value;
                            break;
                        default:
                            type = AddressTypeEnum.None;
                            break;
                    }
                }
                else if (pinType == PinTypeEnum.IO_DIGITAL_ENTRADA)
                    type = AddressTypeEnum.DigitalInput;
                else if (pinType == PinTypeEnum.IO_DIGITAL_SAIDA)
                    type = AddressTypeEnum.DigitalOutput;
                else
                    type = AddressTypeEnum.None;
            }
        }
    }
}
