using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LadderApp
{
    [XmlInclude(typeof(Address))]
    [Serializable]
    public class Addressing
    {
        public List<Address> ListMemoryAddress = new List<Address>();
        public List<Address> ListTimerAddress = new List<Address>();
        public List<Address> ListCounterAddress = new List<Address>();
        public List<Address> ListInputAddress = new List<Address>();
        public List<Address> ListOutputAddress = new List<Address>();

        public Addressing()
        {
        }

        public Address Find(Address address)
        {
            return Find(address.AddressType, address.Id);
        }

        public Address Find(AddressTypeEnum addressType, Int32 index)
        {
            List<Address> addresses;
            switch (addressType)
            {
                case AddressTypeEnum.DigitalMemory:
                    addresses = ListMemoryAddress;
                    break;
                case AddressTypeEnum.DigitalMemoryCounter:
                    addresses = ListCounterAddress;
                    break;
                case AddressTypeEnum.DigitalMemoryTimer:
                    addresses = ListTimerAddress;
                    break;
                case AddressTypeEnum.DigitalInput:
                    addresses = ListInputAddress;
                    break;
                case AddressTypeEnum.DigitalOutput:
                    addresses = ListOutputAddress;
                    break;
                default:
                    return null;
            }

            foreach (Address address in addresses)
            {
                if (address.AddressType == addressType &&
                    address.Id == index)
                {
                    return address;
                }
            }
            return null;
        }

        public void CleanUsedIndication()
        {
            List<AddressTypeEnum> addressTypes = new List<AddressTypeEnum>();
            addressTypes.Add(AddressTypeEnum.DigitalMemory);
            addressTypes.Add(AddressTypeEnum.DigitalMemoryCounter);
            addressTypes.Add(AddressTypeEnum.DigitalMemoryTimer);
            addressTypes.Add(AddressTypeEnum.DigitalInput);
            addressTypes.Add(AddressTypeEnum.DigitalOutput);

            List<Address> generalList;
            foreach (AddressTypeEnum addressType in addressTypes)
            {
                switch (addressType)
                {
                    case AddressTypeEnum.DigitalMemory:
                        generalList = ListMemoryAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryCounter:
                        generalList = ListCounterAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryTimer:
                        generalList = ListTimerAddress;
                        break;
                    case AddressTypeEnum.DigitalInput:
                        generalList = ListInputAddress;
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        generalList = ListOutputAddress;
                        break;
                    default:
                        return;
                }

                foreach (Address address in generalList)
                    address.Used = false;
            }
        }

         public void AlocateIOAddressing(Device device)
        {
            this.ListInputAddress.Clear();
            this.ListOutputAddress.Clear();
            foreach (Address address in device.PinAddresses)
            {
                address.SetDevice(device);
                switch (address.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        this.ListInputAddress.Add(address);
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        this.ListOutputAddress.Add(address);
                        break;
                }
            }

        }

        public int AlocateMemoryAddressing(Device device, List<Address> addresses, AddressTypeEnum addressType, int numberOfAddress)
        {
            int currentNumberOfAddress = addresses.Count;
            if ((currentNumberOfAddress == 0) || (currentNumberOfAddress < numberOfAddress))
            {
                for (int i = currentNumberOfAddress + 1; i <= numberOfAddress; i++)
                    addresses.Add(new Address(addressType, i, device));
            }
            else if (currentNumberOfAddress > numberOfAddress)
            {
                for (int i = (currentNumberOfAddress - 1); i >= numberOfAddress; i--)
                {
                    if (!addresses[i].Used)
                    {
                        addresses[i] = null;
                        addresses.RemoveAt(i);
                    }
                    else
                        break;
                }
            }
            return 0;
        }
    }
}
