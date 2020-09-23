using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace LadderApp
{
    [XmlInclude(typeof(Address))]
    [XmlType(TypeName = "Simbolo")]
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
        [XmlElement(Order = 1, ElementName = "Instrucao")]
        public OperationCode OpCode
        {
            get => opCode;
            set
            {
                this.opCode = value;
                InitializeOperands();
            }
        }

        private Object[] operands;
        [XmlElement(Order = 2, IsNullable = true, ElementName = "Operando")]
        public object[] Operands
        {
            get => operands;
            set
            {
                operands = value;
            }
        }

        public int GetNumberOfOperands()
        {
            if (operands == null)
            {
                return 0;
            }

            return operands.Length;
        }

        public bool IsAllOperandsOk()
        {
            if (operands is null)
            {
                return false;
            }
            foreach (Object operand in operands)
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
            return operands[position];
        }

        public void SetOperand(int position, Object value)
        {
            if (operands == null)
            {
                InitializeOperands();
            }

            if (position > operands.Length)
            {
                throw new Exception("Invalid operand position: " + position);
            }

            if (ValidaEndereco(position, value))
                operands[position] = value;
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
                    operands = null;
                    //INumOperandos = 0;
                    break;
                case OperationCode.LineBegin:
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                case OperationCode.OutputCoil:
                case OperationCode.Reset:
                    operands = new Object[1];
                    //INumOperandos = 1;
                    break;
                case OperationCode.Counter:
                    operands = new Object[4];
                    //INumOperandos = 4;
                    SetOperand(1, (Int32)0); // tipo
                    SetOperand(2, (Int32)0); // preset
                    SetOperand(3, (Int32)0); // acum

                    break;
                case OperationCode.Timer:
                    operands = new Object[5];
                    //INumOperandos = 5;

                    SetOperand(1, (Int32)0); // tipo
                    SetOperand(2, (Int32)0); // preset
                    SetOperand(3, (Int32)0); // acum
                    SetOperand(4, (Int32)0); // Base tempo

                    break;
            }
            return GetNumberOfOperands();
        }

        private Size TamanhoXY;
        [XmlIgnore]
        public Size tamanhoXY
        {
            get { return TamanhoXY; }
            set { TamanhoXY = new Size(0, 0); }
        }

        private Point PosicaoXY;
        [XmlIgnore]
        public Point posicaoXY
        {
            get { return PosicaoXY; }
            set { PosicaoXY = new Point(0, 0); }
        }

        [XmlIgnore]
        public Point XYConexao
        {
            get { return new Point(0, 0); }
        }

        public void Dispose()
        {
            OpCode = OperationCode.None;
        }

        /// <summary>
        /// Valida o operando de acordo com o codigo interpretavel
        /// </summary>
        /// <param name="_novoOperando">Caso operando ainda nao atribuido verifica antes</param>
        /// <returns></returns>
        private bool ValidaEndereco(int _indice, Object _novoOperando)
        {
            Address _end = null;
            Address _atualOperando = null;
            bool _bValido = true;
            bool _bEndereco = false;

            if (_novoOperando != null)
            {
                if (_novoOperando.GetType().Name == Address.ClassName())
                {
                    if (_novoOperando == null)
                        _end = (Address)GetOperand(0);
                    else
                        _end = (Address)_novoOperando;
                }
                else if (_novoOperando.GetType().Name == "Int32")
                {
                }

            }

            switch (OpCode)
            {
                case OperationCode.None:
                case OperationCode.LineEnd:
                    _bValido = false;
                    break;
                case OperationCode.LineBegin:
                    if (_novoOperando != null)
                        if (_novoOperando.GetType().ToString() != "System.Int32")
                            _bValido = false;
                    break;
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                    _bEndereco = true;
                    if (_end != null)
                    {
                        switch (_end.TpEnderecamento)
                        {
                            case AddressTypeEnum.DIGITAL_MEMORIA:
                            case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                            case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                            case AddressTypeEnum.DIGITAL_ENTRADA:
                            case AddressTypeEnum.DIGITAL_SAIDA:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                case OperationCode.OutputCoil:
                    _bEndereco = true;
                    if (_end != null)
                    {
                        switch (_end.TpEnderecamento)
                        {
                            case AddressTypeEnum.DIGITAL_MEMORIA:
                            case AddressTypeEnum.DIGITAL_SAIDA:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                case OperationCode.Timer:
                    if (_end != null && _indice == 0)
                    {
                        _bEndereco = true;
                        switch (_end.TpEnderecamento)
                        {
                            case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else if (_indice > 0)
                    {
                        switch (_indice)
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
                        _bValido = false;
                    break;
                case OperationCode.Counter:
                    if (_end != null && _indice == 0)
                    {
                        _bEndereco = true;
                        switch (_end.TpEnderecamento)
                        {
                            case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else if (_indice > 0)
                    {
                        switch (_indice)
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
                        _bValido = false;
                    break;
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchEnd:
                case OperationCode.ParallelBranchNext:
                    _bValido = false;
                    break;
                case OperationCode.Reset:
                    _bEndereco = true;
                    if (_end != null)
                    {
                        switch (_end.TpEnderecamento)
                        {
                            case AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR:
                            case AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR:
                                _bValido = true;
                                break;
                            default:
                                _bValido = false;
                                break;
                        }
                    }
                    else
                        _bValido = false;
                    break;
                default:
                    _bValido = false;
                    break;
            }

            if (_bEndereco)
            {
                if (IsAllOperandsOk())
                    _atualOperando = (Address)GetOperand(0);

                if (_bValido)
                {
                    if (_atualOperando != null)
                        _atualOperando.MudouOperando -= new MudouOperandoEventHandler(Ocorreu_MudouOperando);

                    _end.MudouOperando += new MudouOperandoEventHandler(Ocorreu_MudouOperando);
                }
                else
                    if (_novoOperando == null)
                {
                    if (_atualOperando != null)
                    {
                        _atualOperando.MudouOperando -= new MudouOperandoEventHandler(Ocorreu_MudouOperando);
                        operands[0] = null;
                    }
                }
            }

            return _bValido;
        }

        void Ocorreu_MudouOperando(object sender)
        {
            ValidaEndereco(0, null);
        }

        public void ValidaOperandosSimbolo(Addressing _EndDisp)
        {
            for (int i = 0; i < GetNumberOfOperands(); i++)
                if (operands[i] != null)
                    if (operands[i].GetType().Name == Address.ClassName())
                    {
                        /// Verifica se o endereco atual existe na lista de enderecos
                        /// do programa atual, se existir recupera o opontamento corrigido
                        /// para o endereco
                        Object oper = _EndDisp.Find(((Address)operands[i]));
                        /// recebido o endereco corrigido valida o endereco a faz as atribui
                        /// coes necessarias
                        if (oper != null)
                        {
                            if (ValidaEndereco(i, oper))
                                operands[i] = oper;
                            else
                            {
                                operands[i] = null;
                            }
                        }
                        else
                        {
                            operands[i] = null;
                        }
                    }
        }
    }
}
