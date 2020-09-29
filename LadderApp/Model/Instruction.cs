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

        public bool IsAllOperandsOk()
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
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                case OperationCode.OutputCoil:
                case OperationCode.Reset:
                    Operands = new Object[1];
                    break;
                case OperationCode.Counter:
                    Operands = new Object[4];
                    SetOperand(1, (Int32)0); // tipo
                    SetOperand(2, (Int32)0); // preset
                    SetOperand(3, (Int32)0); // acum
                    break;
                case OperationCode.Timer:
                    Operands = new Object[5];
                    SetOperand(1, (Int32)0); // tipo
                    SetOperand(2, (Int32)0); // preset
                    SetOperand(3, (Int32)0); // acum
                    SetOperand(4, (Int32)0); // Base tempo
                    break;
            }
            return GetNumberOfOperands();
        }

        //private Size TamanhoXY;
        //[XmlIgnore]
        //public Size tamanhoXY
        //{
        //    get { return TamanhoXY; }
        //    set { TamanhoXY = new Size(0, 0); }
        //}

        //private Point positionXY;
        //public Point PositionXY { get => positionXY; set => positionXY = new Point(0, 0); }

        //[XmlIgnore]
        //public Point XYConexao
        //{
        //    get { return new Point(0, 0); }
        //}


        public void Dispose()
        {
            OpCode = OperationCode.None;
        }


        private bool ValidateAddress(int index, Object newOperand)
        {
            bool _bEndereco = false;

            Address address = null;
            if (newOperand != null)
            {
                if (newOperand is Address)
                {
                    address = (Address)newOperand;
                }
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
                        if (newOperand.GetType().ToString() != "System.Int32")
                            isValid = false;
                    break;
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                    _bEndereco = true;
                    if (address != null)
                    {
                        switch (address.AddressType)
                        {
                            case AddressTypeEnum.DigitalMemory:
                            case AddressTypeEnum.DigitalMemoryCounter:
                            case AddressTypeEnum.DigitalMemoryTimer:
                            case AddressTypeEnum.DigitalInput:
                            case AddressTypeEnum.DigitalOutput:
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
                case OperationCode.OutputCoil:
                    _bEndereco = true;
                    if (address != null)
                    {
                        switch (address.AddressType)
                        {
                            case AddressTypeEnum.DigitalMemory:
                            case AddressTypeEnum.DigitalOutput:
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
                case OperationCode.Timer:
                    if (address != null && index == 0)
                    {
                        _bEndereco = true;
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
                        _bEndereco = true;
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
                    _bEndereco = true;
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

            if (_bEndereco)
            {
                Address currentOperand = null;
                if (IsAllOperandsOk())
                {
                    currentOperand = (Address)GetOperand(0);
                }

                if (isValid)
                {
                    if (currentOperand != null)
                        currentOperand.ChangedOperandEvent -= new ChangedOperandEventHandler(Instruction_ChangedOperand);

                    address.ChangedOperandEvent += new ChangedOperandEventHandler(Instruction_ChangedOperand);
                }
                else
                    if (newOperand == null)
                {
                    if (currentOperand != null)
                    {
                        currentOperand.ChangedOperandEvent -= new ChangedOperandEventHandler(Instruction_ChangedOperand);
                        Operands[0] = null;
                    }
                }
            }

            return isValid;
        }

        void Instruction_ChangedOperand(object sender)
        {
            ValidateAddress(0, null);
        }

        public void ValidateInstructionOperands(Addressing addressing)
        {
            for (int i = 0; i < GetNumberOfOperands(); i++)
                if (Operands[i] != null)
                    if (Operands[i] is Address)
                    {
                        /// Verifica se o endereco atual existe na lista de enderecos
                        /// do programa atual, se existir recupera o opontamento corrigido
                        /// para o endereco
                        Object oper = addressing.Find(((Address)Operands[i]));
                        /// recebido o endereco corrigido valida o endereco a faz as atribui
                        /// coes necessarias
                        if (oper != null)
                        {
                            if (ValidateAddress(i, oper))
                                Operands[i] = oper;
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
    }
}
