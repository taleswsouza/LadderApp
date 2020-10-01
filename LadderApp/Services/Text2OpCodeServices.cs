using System;
using System.Collections.Generic;
using System.Text;
using LadderApp.Exceções;

namespace LadderApp.CodigoInterpretavel
{
    public class Text2OpCodeServices
    {
        private String charConvertedData = "";

        private bool validatedHeader = false;
        public bool askPassword = false;
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
                        initialPosition = initialPosition + HeaderLenght + 2;

                    return initialPosition;
                }
                else
                    return -1;
            }
        }

        public bool ExistsOpCode()
        {
            if (charConvertedData.IndexOf(CodeId) != -1)
                return true;
            else
                return false;
        }

        public bool ExistsHeader()
        {
            if (ExistsOpCode())
                if (ReadOperationCode(InitialPosition) == OperationCode.HeadLenght)
                {
                    HeaderLenght = (Int32)Convert.ToChar(charConvertedData.Substring(InitialPosition + 1, 1));
                    return true;
                }

            return false;
        }

        public void GetHeaderInformation()
        {
            int passwordLenght = 0;
            if (ExistsOpCode())
                if (ExistsHeader())
                {
                    for (int i = InitialPosition + 2; i <= InitialPosition + HeaderLenght + 2; i++)
                        switch (ReadOperationCode(i))
                        {
                            case OperationCode.HeadPassword0:
                                askPassword = true;
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
                throw new NotValidOpCodeException();
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

        public Address ReadAddress(ref Int32 position, Addressing addressing)
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
                    return addressing.Find(addressType, addressIndex); ;
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

        //public int NumberOfOperands(OperationCode opCode)
        //{
        //    int iNumOperandos = -1;
        //    switch (opCode)
        //    {
        //        case OperationCode.None:
        //            iNumOperandos = 0;
        //            break;
        //        case OperationCode.LineEnd:
        //            iNumOperandos = 0;
        //            break;
        //        case OperationCode.NormallyOpenContact:
        //        case OperationCode.NormallyClosedContact:
        //            iNumOperandos = 2;
        //            break;
        //        case OperationCode.OutputCoil:
        //        case OperationCode.Reset:
        //            iNumOperandos = 2;
        //            break;
        //        case OperationCode.ParallelBranchBegin:
        //        case OperationCode.ParallelBranchEnd:
        //        case OperationCode.ParallelBranchNext:
        //            iNumOperandos = 0;
        //            break;
        //        case OperationCode.Counter:
        //            iNumOperandos = 3;
        //            break;
        //        case OperationCode.Timer:
        //            iNumOperandos = 4;
        //            break;
        //        default:
        //            iNumOperandos = 0;
        //            break;
        //    }

        //    return iNumOperandos;
        //}

        //private LadderProgram LerExecutavel(String strNomeProjeto)
        //{
        //    List<int> lstCodigosLidos = new List<int>();
        //    OperationCode guarda = OperationCode.None;

        //    if (this.ExisteCodigoInterpretavel())
        //    {
        //        Int32 intContaFim = 0;
        //        Int32 intIndiceLinha = 0;
        //        Address _endLido;
        //        AddressTypeEnum _tpEndLido;
        //        Int32 _iIndiceEndLido = 0;


        //        /// Cria um programa novo vazio
        //        LadderProgram programa = new LadderProgram();
        //        programa.Name = strNomeProjeto;
        //        programa.device = new Device(1);
        //        programa.addressing.AlocaEnderecamentoIO(programa.device);
        //        programa.addressing.AlocaEnderecamentoMemoria(programa.device, programa.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
        //        programa.addressing.AlocaEnderecamentoMemoria(programa.device, programa.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
        //        programa.addressing.AlocaEnderecamentoMemoria(programa.device, programa.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
        //        intIndiceLinha = programa.InsertLineAtEnd(new Line());

        //        for (int i = this.PosInicial; i < DadosConvertidosChar.Length; i++)
        //        {
        //            guarda = ReadOperationCode(i); i++;

        //            switch (guarda)
        //            {
        //                case OperationCode.None:
        //                    intContaFim++;
        //                    break;
        //                case OperationCode.LineEnd:
        //                    intContaFim++;
        //                    if (ReadOperationCode(i + 1) != OperationCode.None)
        //                        intIndiceLinha = programa.InsertLineAtEnd(new Line());
        //                    break;
        //                case OperationCode.NormallyOpenContact:
        //                case OperationCode.NormallyClosedContact:
        //                    intContaFim = 0;
        //                    {
        //                        Instruction instruction = new Instruction((OperationCode)guarda);
        //                        _tpEndLido = LeTipoEnderecamento(i); i++;
        //                        _iIndiceEndLido = LeInteiro(i); i++;
        //                        _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
        //                        if (_endLido == null)
        //                        {
        //                            programa.device.pins[_iIndiceEndLido - 1].Type = _tpEndLido;
        //                            programa.device.RealocaEnderecoDispositivo();
        //                            programa.addressing.AlocaEnderecamentoIO(programa.device);
        //                            _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
        //                        }
        //                        instruction.SetOperand(0, _endLido);

        //                        //i += 2;
        //                        programa.Lines[intIndiceLinha].instructions.Add(instruction);
        //                    }
        //                    break;
        //                case OperationCode.OutputCoil:
        //                case OperationCode.Reset:
        //                    intContaFim = 0;
        //                    {
        //                        InstructionList _lstSB = new InstructionList();
        //                        _lstSB.Add(new Instruction((OperationCode)guarda));
        //                        _tpEndLido = (AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1));
        //                        _iIndiceEndLido = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
        //                        _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
        //                        if (_endLido == null)
        //                        {
        //                            programa.device.pins[_iIndiceEndLido - 1].Type = _tpEndLido;
        //                            programa.device.RealocaEnderecoDispositivo();
        //                            programa.addressing.AlocaEnderecamentoIO(programa.device);
        //                            _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
        //                        }
        //                        _lstSB[_lstSB.Count - 1].SetOperand(0, _endLido);
        //                        i += 2;
        //                        programa.Lines[intIndiceLinha].Insere2Saida(_lstSB);
        //                        _lstSB.Clear();
        //                    }
        //                    break;
        //                case OperationCode.ParallelBranchBegin:
        //                case OperationCode.ParallelBranchEnd:
        //                case OperationCode.ParallelBranchNext:
        //                    intContaFim = 0;
        //                    programa.Lines[intIndiceLinha].instructions.Add(new Instruction((OperationCode)guarda));
        //                    break;
        //                case OperationCode.Counter:
        //                    intContaFim = 0;
        //                    {
        //                        InstructionList _lstSB = new InstructionList();
        //                        _lstSB.Add(new Instruction((OperationCode)guarda));
        //                        _lstSB[_lstSB.Count - 1].SetOperand(0, programa.addressing.Find(AddressTypeEnum.DigitalMemoryCounter, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
        //                        ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
        //                        ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));

        //                        _lstSB[_lstSB.Count - 1].SetOperand(1, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Tipo);
        //                        _lstSB[_lstSB.Count - 1].SetOperand(2, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Preset);
        //                        i += 3;
        //                        programa.Lines[intIndiceLinha].Insere2Saida(_lstSB);
        //                        _lstSB.Clear();
        //                    }
        //                    break;
        //                case OperationCode.Timer:
        //                    intContaFim = 0;
        //                    {
        //                        InstructionList _lstSB = new InstructionList();
        //                        _lstSB.Add(new Instruction((OperationCode)guarda));
        //                        _lstSB[_lstSB.Count - 1].SetOperand(0, programa.addressing.Find(AddressTypeEnum.DigitalMemoryTimer, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
        //                        ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
        //                        ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.BaseTempo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));
        //                        ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 4, 1));

        //                        _lstSB[_lstSB.Count - 1].SetOperand(1, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Tipo);
        //                        _lstSB[_lstSB.Count - 1].SetOperand(2, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Preset);
        //                        _lstSB[_lstSB.Count - 1].SetOperand(4, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.BaseTempo);

        //                        i += 4;
        //                        programa.Lines[intIndiceLinha].Insere2Saida(_lstSB);
        //                        _lstSB.Clear();
        //                    }
        //                    break;
        //            }

        //            /// fim dos códigos
        //            if (intContaFim >= 2)
        //            {
        //                /// grava os dados lidos do codigo intepretavel
        //                MSP430IntegrationServices p = new MSP430IntegrationServices();
        //                p.CriaArquivo("codigosinterpretaveis.txt", DadosConvertidosChar.Substring(DadosConvertidosChar.IndexOf("@laddermic.com"), i - DadosConvertidosChar.IndexOf("@laddermic.com") + 1));

        //                /// força saída do loop
        //                i = DadosConvertidosChar.Length;
        //            }
        //        }
        //        return programa;

        //    }
        //    return null;
        //}

    }
}
