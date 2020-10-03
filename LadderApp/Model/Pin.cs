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

        private PinTypeEnum pinType = PinTypeEnum.None;
        public PinTypeEnum PinType
        {
            get { return pinType; }
            set
            {
                if (value == PinTypeEnum.IODigitalInput ||
                   value == PinTypeEnum.IODigitalOutput ||
                   value == PinTypeEnum.IODigitalInputOrOutput)
                {
                    switch (value)
                    {
                        case PinTypeEnum.IODigitalInput:
                            type = AddressTypeEnum.DigitalInput;
                            pinType = value;
                            break;
                        case PinTypeEnum.IODigitalOutput:
                            type = AddressTypeEnum.DigitalOutput;
                            pinType = value;
                            break;
                        case PinTypeEnum.IODigitalInputOrOutput:
                            type = AddressTypeEnum.None;
                            pinType = value;
                            break;
                        default:
                            type = AddressTypeEnum.None;
                            pinType = PinTypeEnum.None;
                            break;
                    }
                }
                else
                    pinType = PinTypeEnum.None;

            }
        }

        private AddressTypeEnum type = AddressTypeEnum.None;
        public AddressTypeEnum Type
        {
            get { return type; }
            set
            {
                if (pinType == PinTypeEnum.IODigitalInputOrOutput)
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
                else if (pinType == PinTypeEnum.IODigitalInput)
                    type = AddressTypeEnum.DigitalInput;
                else if (pinType == PinTypeEnum.IODigitalOutput)
                    type = AddressTypeEnum.DigitalOutput;
                else
                    type = AddressTypeEnum.None;
            }
        }
    }
}
