using System;
using System.Xml.Serialization;

namespace LadderApp.Model.Instructions
{
    public class CounterInstruction : OutputBoxInstruction
    {
        public CounterInstruction() : base(OperationCode.Counter)
        {
            Operands = new Object[4];
            setBoxType(0);
            setPreset(0);
            setAccumulated(0);
            Counter = new InternalCounter(this);
        }

        protected override bool CheckFirstOperandHasTheCorrectAddressType(Address address)
        {
            if (address.AddressType.Equals(AddressTypeEnum.DigitalMemoryCounter))
            {
                return true;
            }
            return false;
        }

        public class InternalCounter
        {
            private CounterInstruction counter;
            internal InternalCounter(CounterInstruction counter)
            {
                this.counter = counter;
            }
            public int Type { get => counter.GetBoxType(); set => counter.setBoxType(value); }
            public int Preset { get => counter.GetPreset(); set => counter.setPreset(value); }
            public int Accumulated { get => counter.GetAccumulated(); set => counter.setAccumulated(value); }
            public bool Enable { get; set; }
            public bool Pulse { get; set; }
            public bool Done { get; set; }
            public bool Reset { get; set; }
        }
        [XmlIgnore]
        public InternalCounter Counter { get; set; }
    }
}
