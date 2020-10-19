namespace LadderApp.Model.Instructions
{
    public interface IAddressable
    {
        Address GetAddress();
        bool GetValue();
        void SetUsed();
    }
}