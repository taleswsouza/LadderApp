namespace LadderApp.Model
{
    internal class InternalCounter : Address, ICounterInstructionParameter
    {
        public InternalCounter(int index, int numberOfBitsByPort) : base(AddressTypeEnum.DigitalMemoryCounter, index, numberOfBitsByPort)
        {

        }

        public int Type { get; set; }
        public int Preset { get; set; }
        public int Accumulated { get; set; }
        public bool Enable { get; set; }
        public bool Pulse { get; set; }
        public bool Done { get; set; }
        public bool Reset { get; set; }
    }
}