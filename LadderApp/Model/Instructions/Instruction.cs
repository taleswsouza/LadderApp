using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace LadderApp.Model.Instructions
{
    [XmlInclude(typeof(Address))]
    [XmlInclude(typeof(OutputBoxAddress))]
    [XmlInclude(typeof(CounterAddress))]
    [XmlInclude(typeof(TimerAddress))]
    [Serializable]
    public class Instruction : IInstruction
    {
        private Instruction()
        {
        }

        internal Instruction(OperationCode opCode)
        {
            this.OpCode = opCode;
        }

        private OperationCode opCode = OperationCode.None;
        [XmlElement(Order = 1, ElementName = "opcode")]
        public OperationCode OpCode
        {
            get => opCode;
            set
            {
                this.opCode = value;
            }
        }
        [XmlElement(Order = 2, IsNullable = true, ElementName = "operand")]
        public object[] Operands { get; set; }

        public int GetNumberOfOperands()
        {
            if (Operands == null)
            {
                return 0;
            }
            return Operands.Length;
        }

        bool IsIndexBoundOk(int index)
        {
            return index >= 0 && index < Operands.Length;
        }

        public virtual bool IsAllOperandsOk()
        {
            if (Operands is null)
            {
                return true;
            }
            for (int index = 0; index < Operands.Length; index++)
            {
                if (!IsOperandOk(index, GetOperand(index)))
                {
                    return false;
                }
            }
            return true;
        }

        public Object GetOperand(int position)
        {
            if (Operands == null || position > Operands.Length)
            {
                throw new Exception("Invalid operand position: " + position);
            }
            return Operands[position];
        }

        public void SetOperand(int index, Object value)
        {
            if (index > Operands.Length)
            {
                throw new IndexOutOfRangeException($"Invalid set operand with index={index}, value={value}.");
            }

            if (IsOperandOk(index, value))
            {
                Operands[index] = value;
            }
        }

        protected virtual bool IsOperandOk(int index, object value)
        {
            if (!IsIndexBoundOk(index))
            {
                return false;
            }
            return !(value is null);
        }

        private int InitializeOperands()
        {
            switch (OpCode)
            {
                case OperationCode.None:
                case OperationCode.LineEnd:
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchEnd:
                case OperationCode.ParallelBranchNext:
                    Operands = null;
                    break;
            }
            return GetNumberOfOperands();
        }

        public void Dispose()
        {
            OpCode = OperationCode.None;
        }


        protected virtual bool ValidateAddress(int index, Object newOperand)
        {
            Address address = null;
            if (newOperand is Address)
            {
                address = (Address)newOperand;
            }

            bool isValid = true;
            switch (OpCode)
            {
                case OperationCode.None:
                case OperationCode.LineEnd:
                    isValid = false;
                    break;
                 case OperationCode.Timer:
                    if (address != null && index == 0)
                    {
                        switch (address.AddressType)
                        {
                            case AddressTypeEnum.DigitalMemoryTimer:
                                isValid = true;
                                break;
                            default:
                                isValid = false;
                                break;
                        }
                    }
                    else if (index > 0)
                    {
                        switch (index)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        isValid = false;
                    break;
                case OperationCode.Counter:
                    if (address != null && index == 0)
                    {
                        switch (address.AddressType)
                        {
                            case AddressTypeEnum.DigitalMemoryCounter:
                                isValid = true;
                                break;
                            default:
                                isValid = false;
                                break;
                        }
                    }
                    else if (index > 0)
                    {
                        switch (index)
                        {
                            case 1:
                                break;
                            case 2:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        isValid = false;
                    break;
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchEnd:
                case OperationCode.ParallelBranchNext:
                    isValid = false;
                    break;
                case OperationCode.Reset:
                     if (address != null)
                    {
                        switch (address.AddressType)
                        {
                            case AddressTypeEnum.DigitalMemoryCounter:
                            case AddressTypeEnum.DigitalMemoryTimer:
                                isValid = true;
                                break;
                            default:
                                isValid = false;
                                break;
                        }
                    }
                    else
                        isValid = false;
                    break;
                default:
                    isValid = false;
                    break;
            }


            return isValid;
        }

        public void ValidateInstructionOperands()
        {
            for (int i = 0; i < GetNumberOfOperands(); i++)
                if (Operands[i] != null)
                    if (Operands[i] is Address)
                    {
                        Object operand = (Address)Operands[i];
                        if (operand != null)
                        {
                            if (ValidateAddress(i, operand))
                                Operands[i] = operand;
                            else
                            {
                                Operands[i] = null;
                            }
                        }
                        else
                        {
                            Operands[i] = null;
                        }
                    }
        }

        public virtual bool GetValue()
        {
            throw new InvalidOperationException($"Method not implemented. OpCode={opCode}");
        }
    }
}
