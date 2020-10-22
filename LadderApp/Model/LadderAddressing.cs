using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LadderApp.Model
{
    [XmlInclude(typeof(Address))]
    [Serializable]
    public class LadderAddressing
    {
        public LadderAddressing()
        {
        }

        public List<Address> ListMemoryAddress { get; set; } = new List<Address>();
        public List<Address> ListTimerAddress { get; set; } = new List<Address>();
        public List<Address> ListCounterAddress { get; set; } = new List<Address>();
        public List<Address> ListInputAddress { get; set; } = new List<Address>();
        public List<Address> ListOutputAddress { get; set; } = new List<Address>();

        public List<Address> GetAllAddresses()
        {
            List<Address> allAddresses = new List<Address>();
            allAddresses.AddRange(ListMemoryAddress);
            allAddresses.AddRange(ListTimerAddress);
            allAddresses.AddRange(ListCounterAddress);
            allAddresses.AddRange(ListInputAddress);
            allAddresses.AddRange(ListOutputAddress);

            return allAddresses;
        }

        public List<Address> GetIOAddresses()
        {
            List<Address> allAddresses = new List<Address>();
            allAddresses.AddRange(ListInputAddress);
            allAddresses.AddRange(ListOutputAddress);

            return allAddresses;
        }

        public List<Address> GetAllExceptIOAddresses()
        {
            List<Address> allAddresses = new List<Address>();
            allAddresses.AddRange(ListMemoryAddress);
            allAddresses.AddRange(ListTimerAddress);
            allAddresses.AddRange(ListCounterAddress);

            return allAddresses;
        }
    }
}
