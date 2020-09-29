using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp
{
    public class Line
    {
        [XmlIgnore]
        public Instruction simboloInicioLinha = new Instruction(OperationCode.LineBegin);
        [XmlElement(ElementName = "Instruction")]
        public List<Instruction> instructions = new List<Instruction>();
        public List<Instruction> outputs = new List<Instruction>();
        [XmlIgnore]
        public Instruction simboloFimLinha = new Instruction(OperationCode.LineEnd);

        [XmlIgnore]
        public Addressing endereco = null;

        //public Line(int quallinha, Addressing _endereco)
        //{
        //    endereco = _endereco;
            
        //    switch (quallinha)
        //    {
        //        case 1:
        //            InicializaDesenhosSimbolos1();
        //            break;
        //        case 2:
        //            InicializaDesenhosSimbolos2();
        //            break;
        //        case 3:
        //            InicializaDesenhosSimbolos3();
        //            break;
        //        case 4:
        //            InicializaDesenhosSimbolos4();
        //            break;
        //        case 5:
        //            InicializaDesenhosSimbolos5();
        //            break;
        //        case 6:
        //            InicializaDesenhosSimbolosVazio();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public Line()
        {
        }
        
        //private void InicializaDesenhosSimbolos1()
        //{
        //    // Inicio de Linha
        //    simboloInicioLinha.OpCode = OperationCode.LineBegin;

        //    Instruction ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    instructions.Add(ex1);
        //    //
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);
        //    //
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    //teste inicio
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    // teste fim
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    //saida
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    // Fim de Linha
        //    simboloFimLinha.OpCode = OperationCode.LineEnd;
        //}

        //private void InicializaDesenhosSimbolos2()
        //{
        //    // Inicio de Linha
        //    simboloInicioLinha.OpCode = OperationCode.LineBegin;

        //    Instruction ex1 = null;

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    instructions.Add(ex1);
        //    //
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);
        //    //
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    //ex1 = new SimboloBasico();
        //    ////ex1.TabStop = true;
        //    //ex1.setCI(CodigosInterpretaveis.CONTATO_NA);
        //    //simbolos.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    //teste inicio
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    // teste fim
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    instructions.Add(ex1);

        //    //saida
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    // Fim de Linha
        //    simboloFimLinha.OpCode = OperationCode.LineEnd;
        //}

        //private void InicializaDesenhosSimbolos3()
        //{
        //    // Inicio de Linha
        //    simboloInicioLinha.OpCode = OperationCode.LineBegin;

        //    Instruction ex1 = null;

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    ex1.SetOperand(0, endereco.lstMemoria[1]);
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyClosedContact;
        //    ex1.SetOperand(0, endereco.lstTemporizador[2]);
        //    instructions.Add(ex1);

            
        //    //saida
        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    ex1.SetOperand(0, endereco.lstMemoria[1]);
        //    outputs.Add(ex1);

        //    // Fim de Linha
        //    simboloFimLinha.OpCode = OperationCode.LineEnd;
        //}

        //private void InicializaDesenhosSimbolos4()
        //{
        //    // Inicio de Linha
        //    simboloInicioLinha.OpCode = OperationCode.LineBegin;

        //    Instruction ex1 = null;

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    outputs.Add(ex1);

        //    // Fim de Linha
        //    simboloFimLinha.OpCode = OperationCode.LineEnd;
        //}

        //private void InicializaDesenhosSimbolos5()
        //{
        //    // Inicio de Linha
        //    simboloInicioLinha.OpCode = OperationCode.LineBegin;

        //    Instruction ex1 = null;

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.NormallyOpenContact;
        //    ex1.SetOperand(0, endereco.lstMemoria[0]);
        //    endereco.lstMemoria[0].EmUso = true;
        //    instructions.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchBegin;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchNext;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.OutputCoil;
        //    outputs.Add(ex1);

        //    ex1 = new Instruction();
        //    ex1.OpCode = OperationCode.ParallelBranchEnd;
        //    outputs.Add(ex1);

        //    // Fim de Linha
        //    simboloFimLinha.OpCode = OperationCode.LineEnd;
        //    //simboloFimLinha.TabStop = false;

        //}

        //private void InicializaDesenhosSimbolosVazio()
        //{
        //    // Inicio de Linha
        //    simboloInicioLinha.OpCode = OperationCode.LineBegin;

        //    // Fim de Linha
        //    simboloFimLinha.OpCode = OperationCode.LineEnd;
        //}

        public void ApagaLinha()
        {
            outputs.Reverse();
            foreach (Instruction instruction in outputs)
            {
                instruction.Dispose();
            }
            outputs.Clear();

            instructions.Reverse();
            foreach (Instruction instruction in instructions)
            {
                instruction.Dispose();
            }
            instructions.Clear();

            simboloInicioLinha.Dispose();
            simboloFimLinha.Dispose();
        }

        public Instruction Insere2Saida(InstructionList instructions)
        {
            int _indiceSimbolo = 0;
            int _subt2posicionaSimboloInserido = 0;

            switch (outputs.Count)
            {
                case 0:
                    /// case 0: Primeiro simbolo na saida, adiciona apenas um
                    /// simbolo na saida
                    _indiceSimbolo = 0;

                    if (instructions.Count > 1)
                    {
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_COMPLETO);
                        _subt2posicionaSimboloInserido = -1;
                    }

                    break;
                case 1:
                    /// case 1: Caso ja exista 1 (um) simbolo de saida, insere um 
                    /// paralelo de forma automatica


                    // aqui 0=antes, 1=depois
                    if (_indiceSimbolo == 0)
                    {
                        /// prepara para inserir antes do objeto atual
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                        /// insere PP antes do objeto atual na linha
                        this.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        /// insere PF depois do objeto atual da linha
                        this.outputs.Insert(this.outputs.Count, new Instruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_FINALIZADO);
                        _subt2posicionaSimboloInserido = -1;

                        this.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchBegin));
                        _indiceSimbolo++;
                    }

                    break;
                default:
                    switch (this.outputs[_indiceSimbolo].OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                            this.outputs[0].OpCode = OperationCode.ParallelBranchNext;
                            break;
                        case OperationCode.ParallelBranchNext:
                            instructions.InsereParaleloProximo();
                            break;
                        case OperationCode.ParallelBranchEnd:
                            instructions.InsereParaleloProximo();
                            break;
                        default:
                            instructions.InsereParaleloProximo();
                            _indiceSimbolo++;
                            break;
                    }
                    break;
            }

            foreach (Instruction _sb in instructions)
            {
                this.outputs.Insert(_indiceSimbolo, _sb);
                _indiceSimbolo++;
            }

            /// retorna o ultimo objeto inserido
            return this.outputs[_indiceSimbolo - 1 + _subt2posicionaSimboloInserido];
        }


    }
}
