using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace LadderApp
{
    [XmlInclude(typeof(Address))]
    [Serializable]
    public class Instruction : IInstruction
    {
        public Instruction()
        {
        }

        public Instruction(OperationCode opCode)
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
                InitializeOperands();
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

        public virtual bool IsAllOperandsOk()
        {
            if (Operands is null)
            {
                return false;
            }
            foreach (Object operand in Operands)
            {
                if (operand is null)
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

        public void SetOperand(int position, Object value)
        {
            if (Operands == null)
            {
                InitializeOperands();
            }

            if (position > Operands.Length)
            {
                throw new Exception("Invalid operand position: " + position);
            }

            if (ValidateAddress(position, value))
                Operands[position] = value;
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
                case OperationCode.LineBegin:
                //case OperationCode.NormallyOpenContact:
                //case OperationCode.NormallyClosedContact:
                //case OperationCode.OutputCoil:
                //case OperationCode.Reset:
                //    Operands = new Object[1];
                //    break;
                //case OperationCode.Counter:
                //    Operands = new Object[4];
                //    SetOperand(1, (Int32)0); // type
                //    SetOperand(2, (Int32)0); // preset
                //    SetOperand(3, (Int32)0); // accum
                //    break;
                case OperationCode.Timer:
                    Operands = new Object[5];
                    SetOperand(1, (Int32)0); // type
                    SetOperand(2, (Int32)0); // preset
                    SetOperand(3, (Int32)0); // accum
                    SetOperand(4, (Int32)0); // time base
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
                case OperationCode.LineBegin:
                    if (newOperand != null)
                        if (newOperand is Int32 == false)
                            isValid = false;
                    break;
                //case OperationCode.NormallyOpenContact:
                //case OperationCode.NormallyClosedContact:
                //    //isAddress = true;
                //    if (address != null)
                //    {
                //        switch (address.AddressType)
                //        {
                //            case AddressTypeEnum.DigitalMemory:
                //            case AddressTypeEnum.DigitalMemoryCounter:
                //            case AddressTypeEnum.DigitalMemoryTimer:
                //            case AddressTypeEnum.DigitalInput:
                //            case AddressTypeEnum.DigitalOutput:
                //                isValid = true;
                //                break;
                //            default:
                //                isValid = false;
                //                break;
                //        }
                //    }
                //    else
                //        isValid = false;
                //    break;
                //case OperationCode.OutputCoil:
                //    //isAddress = true;
                //    if (address != null)
                //    {
                //        switch (address.AddressType)
                //        {
                //            case AddressTypeEnum.DigitalMemory:
                //            case AddressTypeEnum.DigitalOutput:
                //                isValid = true;
                //                break;
                //            default:
                //                isValid = false;
                //                break;
                //        }
                //    }
                //    else
                //        isValid = false;
                //    break;
                case OperationCode.Timer:
                    if (address != null && index == 0)
                    {
                        //isAddress = true;
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
                        //isAddress = true;
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
                    //isAddress = true;
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

            //if (isAddress)
            //{
            //    Address currentOperand = null;
            //    if (IsAllOperandsOk())
            //    {
            //        currentOperand = (Address)GetOperand(0);
            //    }

            //    //if (isValid)
            //    //{
            //    //    if (currentOperand != null)
            //    //        currentOperand.ChangedOperandEvent -= new ChangedOperandEventHandler(Instruction_ChangedOperand);

            //    //    address.ChangedOperandEvent += new ChangedOperandEventHandler(Instruction_ChangedOperand);
            //    //}
            //    //else
            //    //    if (newOperand == null)
            //    //{
            //    //    if (currentOperand != null)
            //    //    {
            //    //        currentOperand.ChangedOperandEvent -= new ChangedOperandEventHandler(Instruction_ChangedOperand);
            //    //        Operands[0] = null;
            //    //    }
            //    //}
            //}

            return isValid;
        }

        //void Instruction_ChangedOperand(object sender)
        //{
        //    ValidateAddress(0, null);
        //}

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

        public Address GetAddress()
        {
            return (Address)GetOperand(0);
        }

        public void SetUsed()
        {
            GetAddress().Used = true;
        }
    }
}
