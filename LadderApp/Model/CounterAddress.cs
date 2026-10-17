using System;
using System.Xml.Serialization;

namespace LadderApp.Model
{
    [Serializable]
    [XmlType(TypeName = "counter")]
    public class CounterAddress : OutputBoxAddress
    {
        private CounterAddress() : base()
        {

        }
        public CounterAddress(int index, int numberOfBitsByPort) : base(AddressTypeEnum.DigitalMemoryCounter, index, numberOfBitsByPort)
        {

        }
    }
}