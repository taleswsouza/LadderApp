using LadderApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class Text2OpCodeServices
    {
        private AddressingServices addressingServices = AddressingServices.Instance;

        private String charConvertedData = "";

        private bool validatedHeader = false;
        public string password = "";
        public int CurrentPosition { get; } = -1;

        private int HeaderLenght = -1;

        const string CodeId = "@laddermic.com";

        public Text2OpCodeServices(string charConvertedData)
        {
            this.charConvertedData = charConvertedData;
        }

        public int InitialPosition
        {
            get
            {
                if (this.ExistsOpCode())
                {
                    int initialPosition = charConvertedData.IndexOf(CodeId) + CodeId.Length;

                    if (validatedHeader)
                    {
                        initialPosition = initialPosition + HeaderLenght + 2;
                    }

                    return initialPosition;
                }
                else
                {
                    return -1;
                }
            }
        }

        public bool AskPassword { get; set; } = false;

        public bool ExistsOpCode()
        {
            if (charConvertedData.IndexOf(CodeId) != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExistsHeader()
        {
            if (ExistsOpCode())
            {
                if (ReadOperationCode(InitialPosition) == OperationCode.HeadLenght)
                {
                    HeaderLenght = (int)Convert.ToChar(charConvertedData.Substring(InitialPosition + 1, 1));
                    return true;
                }
            }

            return false;
        }

        public void GetHeaderInformation()
        {
            int passwordLenght = 0;
            if (ExistsOpCode())
            {
                if (ExistsHeader())
                {
                    for (int i = InitialPosition + 2; i <= InitialPosition + HeaderLenght + 2; i++)
                        switch (ReadOperationCode(i))
                        {
                            case OperationCode.HeadPassword0:
                                AskPassword = true;
                                i++;
                                passwordLenght = ReadInteger(i);
                                i++;

                                password = charConvertedData.Substring(i, passwordLenght);
                                i += passwordLenght;
                                break;
                            default:
                                break;
                        }
                }
            }
            validatedHeader = true;
        }

        public OperationCode ReadOperationCode(int position)
        {
            try
            {
                return (OperationCode)Convert.ToChar(charConvertedData.Substring(position, 1));
            }
            catch
            {
                throw new Exception("It's not an valid OpCode!");
            }
        }

        public AddressTypeEnum GetAddressingType(int position)
        {
            if (ExistsOpCode())
            {
                try
                {
                    return (AddressTypeEnum)Convert.ToChar(charConvertedData.Substring(position, 1));
                }
                catch
                {
                    throw new Exception();
                }
            }
            return AddressTypeEnum.None;
        }

        public int ReadInteger(Int32 position)
        {
            if (ExistsOpCode())
            {
                try
                {
                    return (Int32)Convert.ToChar(charConvertedData.Substring(position, 1));
                }
                catch
                {
                    throw new Exception();
                }
            }
            return -1;
        }

        public Address ReadAddress(ref Int32 position, LadderAddressing addressing)
        {
            Instruction instruction = new Instruction(ReadOperationCode(position));
            switch (instruction.OpCode)
            {
                case OperationCode.None:
                    break;
                case OperationCode.LineEnd:
                    break;
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                    AddressTypeEnum addressType = GetAddressingType(position);
                    position++;
                    Int32 addressIndex = ReadInteger(position);
                    position++;
                    return addressingServices.Find(addressType, addressIndex); ;
                case OperationCode.OutputCoil:
                case OperationCode.Reset:
                    break;
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchEnd:
                case OperationCode.ParallelBranchNext:
                    break;
                case OperationCode.Counter:
                    break;
                case OperationCode.Timer:
                    break;
            }
            return null;
        }
    }
}
