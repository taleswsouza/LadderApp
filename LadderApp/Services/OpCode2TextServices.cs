using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class OpCode2TextServices
    {
        private String internalText = "";
        private String txtInternalWithTypeCast = "";
        public OpCode2TextServices Header { get; set; } = null;

        private Int32 headerPosition2Internal = 0;
        private Int32 HeaderPosition2InternalWithTypeCast = 0;

        private const String identificationCode = "@laddermic.com";

        public OpCode2TextServices()
        {
            this.Add(identificationCode);
            headerPosition2Internal = internalText.Length;
            HeaderPosition2InternalWithTypeCast = txtInternalWithTypeCast.Length;
        }

        private bool bTxtWithTypeCast = true;
        public bool TxtWithTypeCast
        {
            get { return bTxtWithTypeCast; }
            set { bTxtWithTypeCast = value; }
        }

        public Int32 Length
        {
            get { return internalText.Length; }
        }

        public void AddHeader()
        {
            if (Header == null)
                Header = new OpCode2TextServices();
        }

        public void FinalizeHeader()
        {
            if (Header != null)
            {
                if (Header.Length > 0)
                {
                    this.Header.Insert(Header.Length);
                    this.Header.Insert(OperationCode.HeadLenght);

                    txtInternalWithTypeCast = txtInternalWithTypeCast.Insert(this.HeaderPosition2InternalWithTypeCast, Header.ToStringInternalWithTypeCast() + ", ");
                    internalText = internalText.Insert(this.headerPosition2Internal, Header.ToStringInternal());

                    Header = null;
                }
            }
        }

        public void Add(String text)
        {
            internalText += text;
            for (int i = 0; i < text.Length; i++)
            {
                txtInternalWithTypeCast += "'" + text.Substring(i, 1) + "', ";
            }
        }

        public void Add(Int32 number)
        {
            internalText += Convert.ToChar(number);
            txtInternalWithTypeCast += "(char)" + number.ToString() + ", ";
        }

        internal void Insert(int number)
        {
            internalText = Convert.ToChar(number) + internalText;
            txtInternalWithTypeCast = "(char)" + number.ToString() + ", " + txtInternalWithTypeCast;
        }


        public void Add(OperationCode opCode)
        {
            Add((int)opCode);
        }

        internal void Insert(OperationCode opCode)
        {
            Insert((int)opCode);
        }

        private void Add(Address address)
        {
            Add((int)address.AddressType);
            Add((int)address.Id);
        }

        public void Add(Instruction instruction)
        {
            if (!instruction.IsAllOperandsOk())
            {
                throw new ArgumentException($"Operands not ok {instruction}");
            }

            Add(instruction.OpCode);

            switch (instruction.OpCode)
            {
                case OperationCode.Counter:
                    if (instruction.IsAllOperandsOk())
                    {
                        if (instruction is CounterInstruction counter)
                        {
                            Add(counter.GetAddress().Id);
                            Add(counter.GetBoxType());
                            Add(counter.GetPreset());
                        }
                    }
                    break;
                case OperationCode.Timer:
                    if (instruction is TimerInstruction timer)
                    {
                        Add(timer.GetAddress().Id);
                        Add(timer.GetBoxType());
                        Add(timer.GetPreset());
                        Add(timer.GetTimeBase());
                    }
                    break;
                default:
                    for (int i = 0; i < instruction.GetNumberOfOperands(); i++)
                    {
                        if (instruction.GetOperand(i) != null)
                        {
                            if (instruction.GetOperand(i) is Address)
                            {
                                Add((Address)instruction.GetOperand(i));
                            }
                        }
                    }
                    break;
            }
        }

        public override string ToString()
        {
            if (bTxtWithTypeCast)
            {
                return ToStringInternalWithTypeCast();
            }
            else
            {
                return "\"" + internalText + "\"";
            }
        }

        internal string ToStringInternalWithTypeCast()
        {
            return txtInternalWithTypeCast.Substring(0, txtInternalWithTypeCast.Length - 2);
        }

        internal string ToStringInternal()
        {
            return internalText;
        }

    }
}
