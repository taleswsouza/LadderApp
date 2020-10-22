using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    class DeviceFactory
    {
        public static Device CreateNewDevice()
        {
            Device device = new Device()
            {
                Id = 1,
                Manufacturer = "Texas Instruments",
                Series = "MSP430",
                Model = "MSP430F2013",
                NumberOfPorts = 2,
                NumberBitsByPort = 8
            };

            DefaultInitialization(device);

            return device;
        }

        private static void DefaultInitialization(Device device)
        {
            for (int i = 1; i <= (device.NumberOfPorts * device.NumberBitsByPort); i++)
            {
                Pin pin = new Pin();
                if ((i <= 8) || (i >= 15 && i <= 16))
                    pin.PinType = PinTypeEnum.IODigitalInputOrOutput;
                else
                    pin.PinType = PinTypeEnum.None;
                device.Pins.Add(pin);

                device.PinAddresses.Add(new Address(pin.Type, i, device.NumberBitsByPort));
            }
        }

    }
}
