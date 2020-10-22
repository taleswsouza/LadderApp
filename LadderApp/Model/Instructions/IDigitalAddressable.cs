namespace LadderApp.Model.Instructions
{
    public interface IDigitalAddressable
    {
        Address GetAddress();
        void SetAddress(Address address);
        void SetValue(bool value);
        bool GetValue();
        void SetUsed();
    }
}