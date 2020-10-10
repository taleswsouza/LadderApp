using System;
using System.Collections.Generic;

namespace LadderApp
{
    [Serializable]
    public class Device
    {
        public Device()
        {
        }

        public Device(bool value)
        {
            DefaultInitialization();
        }

        public int Id { get; set; } = -1;
        public string Manufacturer { get; set; }
        public string Series { get; set; }
        public string Model { get; set; }
        public int NumberOfPorts { get; set; } = 0;
        public int NumberBitsByPort { get; set; } = 0;
        public List<Address> PinAddresses { get; set; } = new List<Address>();
        public List<Pin> Pins { get; set; } = new List<Pin>();

        void DefaultInitialization()
        {
            Id = 1;
            Manufacturer = "Texas Instruments";
            Series = "MSP430";
            Model = "MSP430F2013";
            NumberOfPorts = 2;
            NumberBitsByPort = 8;

            for (int i = 1; i <= (NumberOfPorts * NumberBitsByPort); i++)
            {
                Pin pin = new Pin();
                if ((i <= 8) || (i >= 15 && i <= 16))
                    pin.PinType = PinTypeEnum.IODigitalInputOrOutput;
                else
                    pin.PinType = PinTypeEnum.None;
                Pins.Add(pin);

                PinAddresses.Add(new Address(pin.Type, i, this));
            }
        }

        public void RealocatePinAddresses()
        {
            for (int i = 0; i < (NumberOfPorts * NumberBitsByPort); i++)
            {
                PinAddresses[i].AddressType = Pins[i].Type;
            }
        }
    }
}
