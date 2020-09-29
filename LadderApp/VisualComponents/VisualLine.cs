using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LadderApp
{
    public partial class VisualLine
    {
        public VisualLine()
        {
        }

        public VisualLine(LadderForm ladderForm, Line line)
        {
            this.ladderForm = ladderForm;
            this.line = line;

            InicializaSimbolosFixosDaLinha();
        }

        private Line line;

        /// <summary>
        /// Link para o formulario DiagramaLadder aonde sera
        /// desenhada os controles da linha
        /// </summary>
        public LadderForm ladderForm;
        public VisualLine PreviousLine { get; set; }
        public VisualLine NextLine { get; set; }

        /// <summary>
        /// objetos fixos na linha
        /// </summary>
        [XmlIgnore]
        public VisualInstructionUserControl LineBegin = new VisualInstructionUserControl(OperationCode.LineBegin);
        [XmlIgnore]
        public VisualInstructionUserControl LineEnd = new VisualInstructionUserControl(OperationCode.LineEnd);
        [XmlIgnore]
        public VisualInstructionUserControl BackgroundLine = new VisualInstructionUserControl(OperationCode.BackgroundLine);

        /// <summary>
        /// lista de Objetos dinamicos na linha
        /// </summary>
        public List<VisualInstructionUserControl> visualInstructions = new List<VisualInstructionUserControl>();
        public List<VisualInstructionUserControl> visualOutputInstructions = new List<VisualInstructionUserControl>();

        /// <summary>
        /// enum para indicar em qual das listas de objetos dinamicos da linha
        /// a funcao de insere simbolo ira inserir (simbolos ou saida)
        /// </summary>
        public enum LocalInsereSimbolo
        {
            SIMBOLOS,
            SAIDA,
            INDEFINIDO
        }

        [XmlIgnore]
        public int iTabStop = 0;

        private int iNumLinha = 0;
        public int NumLinha
        {
            get { return iNumLinha; }
            set
            {
                LineBegin.SetOperand(0, value);
                iNumLinha = value;
            }
        }

        public int posX = 0;
        public int posY = 0;

        public int tamX = 70;
        public int tamY = 80;

        private int posX2primeiroSimbSaida = 0;

        public double percReducaoSimbolo = 1D;

        private int xTotalHorizontal = 0;
        public int xTotal
        {
            get { return xTotalHorizontal; }
            set { xTotalHorizontal = value - 10; }
        }


        private int yTotalVertical = 0;
        public int yTotal
        {
            get { return yTotalVertical; }
            set { yTotalVertical = value - 10; }
        }

        public void AjustaPosicionamento()
        {
            VisualInstructionUserControl _ctrlLivreAux = null;

            int _maiorX = 0;

            /// Variaveis para desenhar os simbolos de saida
            /// sempre no final da linha
            int _contaNumSimbolos = 0;
            int _guardaAcumXnoPrimeiroSimbSaida = 0;
            int _guardaAcumXnoMaiorSimbSaida = 0;
            int _guardaTamSaida = 0;

            VisualParallelBranch _par = null;
            List<VisualParallelBranch> _lst_par = new List<VisualParallelBranch>();

            /// acumula o tamanho Y (pos+tam) do ultimo
            /// VPI tratado dentro de um mesmo paralelo
            /// seu valor e usado para informar
            /// ao PF qual o seu tamanho Y
            int _ultTamY2ParaleloFinal = 0;

            int _tamY2DesenhoFundo = 0;

            /// Variaveis auxiliares para posicionamento
            ///  dos controles
            int _posX = posX;  // auxiliar para posX
            int _posY = posY;  // auxiliar para posY
            int _tamX = this.tamX;  // auxiliar para tamX
            int _tamY = tamY;  // auxiliar para tamY
            int _acumTamX = 0;  // usado para acumular o valor de X na sequencia
            // em que os simbolos sao montados
            int _acumTamY = tamY;  // auxiliar para tamY

            _acumTamX += this.tamX;

            visualInstructions.AddRange(visualOutputInstructions);

            foreach (VisualInstructionUserControl simbAux in visualInstructions)
            {
                _contaNumSimbolos++;

                // caso todos os paralelos abertos tenham sido
                // tratados forca paralelos tratados = 0
                if (_lst_par.Count == 0)
                {
                    if (_maiorX > 0)
                        _posX = _maiorX;

                    _maiorX = 0;
                }
                else
                {
                    if (_maiorX < (_posX + _tamX))
                        _maiorX = _posX + _tamX;
                }

                switch (simbAux.OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                        _tamY = tamY; // restaura tamanho Y base
                        _posX = _acumTamX;
                        _tamX = this.tamX / 3;

                        _par = new VisualParallelBranch();
                        _lst_par.Add(_par);

                        _par._yAcum = _posY;
                        _par.maiorY = _posY;
                        _par.maiorX = _maiorX;
                        _par.par = simbAux;

                        _acumTamX = _posX + _tamX;
                        break;
                    case OperationCode.ParallelBranchEnd:
                        _tamX = this.tamX / 3;
                        _tamY = _par.ultimoVPI.PositionXY.Y - _par.par.PositionXY.Y + _par.ultimoVPI.tamanhoXY.Height; // _ultTamY2ParaleloFinal;
                        _posY = _par.par.PositionXY.Y;

                        if (_maiorX > _par.maiorX)
                            _par.maiorX = _maiorX;

                        _posX = _par.maiorX;

                        // Salva (por paralelo) o maior tamanho Y para os simbolos
                        // linha inicial / final e desenho de fundo
                        if (_ultTamY2ParaleloFinal > _tamY2DesenhoFundo)
                            _tamY2DesenhoFundo = _ultTamY2ParaleloFinal;

                        simbAux.SalvaVPI2PF(_par.lstVPI);

                        /// Faz os apontamentos de paralelo
                        ///  para facilitar futuras buscas de paralelos
                        simbAux.Aponta2PI = _par.par;
                        _par.par.Aponta2PF = simbAux;
                        _par.ultimoVPI.Aponta2proxPP = simbAux;

                        _par.ultimoVPI.UltimoVPI = true;

                        _par = null;
                        _lst_par.RemoveAt(_lst_par.Count - 1);

                        if (_lst_par.Count > 0)
                            _par = _lst_par[_lst_par.Count - 1];


                        // se tiver paralelo aberto
                        // salva nele o maior y processado dentro dele
                        if (_lst_par.Count > 0)
                            if (_ultTamY2ParaleloFinal > _par.maiorY)
                                _par.maiorY = _ultTamY2ParaleloFinal;

                        _acumTamX = _posX + _tamX;
                        break;
                    case OperationCode.ParallelBranchNext:
                        _tamY = tamY; // restaura tamanho Y base
                        _tamX = this.tamX / 3; // tamanho X reduzido

                        if (_maiorX > _par.maiorX)
                            _par.maiorX = _maiorX;
                        _maiorX = 0;

                        _posX = _par.par.PositionXY.X;

                        _par._yAcum = _par.maiorY;
                        _posY = _par.maiorY + (tamY * (_par.numVPITratados + 1));

                        // caso seja o primeiro VPI(NXB) atualiza o SIZE do PI(BST)
                        // caso seja o segundo VPI em diante, atualiza SIZE do VPI anterior
                        if (_par.numVPITratados > 0)
                        {
                            _ctrlLivreAux = _par.ultimoVPI;
                            _ctrlLivreAux.UltimoVPI = false;
                        }
                        else
                        {
                            _ctrlLivreAux = _par.par;
                        }

                        _ctrlLivreAux.tamanhoXY = new Size(_ctrlLivreAux.tamanhoXY.Width, _posY - _ctrlLivreAux.PositionXY.Y);

                        /// PARA FACILITAR A BUSCA DO PROXIMO PONTO DE PARALELO
                        _ctrlLivreAux.Aponta2proxPP = simbAux;

                        _par.numVPITratados += 1;
                        _par.ultimoVPI = simbAux;
                        _ultTamY2ParaleloFinal = _posY;

                        _par.lstVPI.Add(simbAux);

                        _acumTamX = _posX + _tamX;
                        break;
                    default:
                        _tamY = tamY;
                        _tamX = this.tamX;
                        _posX = _acumTamX;
                        _acumTamX += _tamX;
                        break;
                }

                // posiciona e dimensiona simbolo
                simbAux.PositionXY = new Point(_posX, _posY);
                simbAux.tamanhoXY = new Size(_tamX, _tamY);

                if ((visualOutputInstructions.Count > 0) && (_contaNumSimbolos >= (visualInstructions.Count - visualOutputInstructions.Count)))
                {
                    if (_contaNumSimbolos == (visualInstructions.Count - visualOutputInstructions.Count))
                        _guardaAcumXnoPrimeiroSimbSaida = _acumTamX;

                    if (_guardaAcumXnoMaiorSimbSaida < _acumTamX)
                        _guardaAcumXnoMaiorSimbSaida = _acumTamX;
                }
            }

            _guardaTamSaida = (_guardaAcumXnoMaiorSimbSaida - _guardaAcumXnoPrimeiroSimbSaida);// -this.tamX;
            if (_guardaTamSaida == 0)
                _guardaTamSaida = tamX;

            /// Caso tenha ao menos 1 objeto de saida cria um espaco de meio simbolo
            /// entre o ultimo simbolo e o primeiro de saida
            visualInstructions.RemoveRange((visualInstructions.Count - visualOutputInstructions.Count), visualOutputInstructions.Count);

            _tamY2DesenhoFundo += tamY;

            if ((posY > tamY) && (_tamY2DesenhoFundo > posY))
                _tamY2DesenhoFundo -= posY;

            //--Inicio da linha
            LineBegin.PositionXY = new Point(posX, posY);
            LineBegin.tamanhoXY = new Size(this.tamX, _tamY2DesenhoFundo);
            LineBegin.Location = new Point(posX, posY);
            LineBegin.Size = new Size(this.tamX, _tamY2DesenhoFundo);

            //--Fim da linha
            if (_acumTamX < (ladderForm.Width - this.tamX))
                _acumTamX = (ladderForm.Width - this.tamX);

            if (PreviousLine != null)
                if (_acumTamX < PreviousLine.LineEnd.PositionXY.X)
                    _acumTamX = PreviousLine.LineEnd.PositionXY.X;

            posX2primeiroSimbSaida = _acumTamX - _guardaAcumXnoPrimeiroSimbSaida - (_guardaTamSaida);

            LineEnd.PositionXY = new Point(_acumTamX, posY);
            LineEnd.tamanhoXY = new Size(this.tamX, _tamY2DesenhoFundo);
            LineEnd.Location = new Point(_acumTamX, posY);
            LineEnd.Size = new Size(this.tamX, _tamY2DesenhoFundo);

            //--Desenho de fundo
            BackgroundLine.PositionXY = new Point(posX, posY);
            BackgroundLine.tamanhoXY = new Size(_acumTamX, _tamY2DesenhoFundo);
            BackgroundLine.Location = new Point(posX, posY);
            BackgroundLine.Size = new Size(_acumTamX, _tamY2DesenhoFundo);

            RedimensionaSimbolos();
        }

        private void RedimensionaSimbolos()
        {

            //--Inicio da linha
            iTabStop++;
            LineBegin.TabIndex = iTabStop;
            LineBegin.TabStop = true;

            int indiceInsereSaida = 0;
            if (visualOutputInstructions.Count > 0)
            {
                indiceInsereSaida = visualInstructions.Count;
                visualInstructions.AddRange(visualOutputInstructions);
            }

            int i = 0;
            foreach (VisualInstructionUserControl _simbAux in visualInstructions)
            {
                iTabStop++;
                _simbAux.TabIndex = iTabStop;
                _simbAux.TabStop = true;

                _simbAux.Size = new Size(Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * percReducaoSimbolo), _simbAux.tamanhoXY.Height);

                if (i >= indiceInsereSaida && (visualOutputInstructions.Count > 0))
                    _simbAux.Location = new Point(_simbAux.PositionXY.X + (_simbAux.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * percReducaoSimbolo))) / 2 + posX2primeiroSimbSaida, _simbAux.PositionXY.Y);
                else
                    _simbAux.Location = new Point(_simbAux.PositionXY.X + (_simbAux.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * percReducaoSimbolo))) / 2, _simbAux.PositionXY.Y);

                _simbAux.Visible = true;
                _simbAux.Invalidate();
                i++;
            }

            if (visualOutputInstructions.Count > 0)
            {
                visualInstructions.RemoveRange(indiceInsereSaida, visualOutputInstructions.Count);
            }

            //--Fim da linha
            //iTabStop = 0;
            LineEnd.TabIndex = 0;
            LineEnd.TabStop = false;

            //--Desenho de fundo
            //iTabStop = 0;
            BackgroundLine.TabIndex = 0;
            BackgroundLine.TabStop = false;
            BackgroundLine.Invalidate();

        }


        private void InicializaSimbolosFixosDaLinha()
        {
            // Inicio de Linha
            LineBegin.OpCode = OperationCode.LineBegin;
            LineBegin.TabStop = true;

            // Fim de Linha
            LineEnd.OpCode = OperationCode.LineEnd;
            LineEnd.TabStop = false;

            // Desenho de fundo da Linha
            BackgroundLine.OpCode = OperationCode.BackgroundLine;
            BackgroundLine.TabStop = false;


            ///////////////////////////////////////

            // Inicio de linha
            LineBegin.VisualLine = this;
            LineBegin.Parent = this.ladderForm;
            LineBegin.CreateControl();
            LineBegin.BringToFront();

            // Fim de linha de linha
            LineEnd.VisualLine = this;
            LineEnd.Parent = this.ladderForm;
            LineEnd.CreateControl();
            LineEnd.BringToFront();



            InsereSimboloDireto(this.visualInstructions, this.LineBegin, line.instructions);
            InsereSimboloDireto(this.visualOutputInstructions, this.LineBegin, line.outputs);

            // Desenho de fundo da linha
            BackgroundLine.VisualLine = this;
            BackgroundLine.SendToBack();
            BackgroundLine.Parent = this.ladderForm;
            BackgroundLine.CreateControl();
            BackgroundLine.SendToBack();
            /// Desabilito o desenho de fundo para evitar que um clique do mouse
            /// sobre partes dele faca com que a tela seja movida para o primeiro
            /// controle.
            BackgroundLine.Enabled = false;

            AtribuiFuncoesAosControles();
        }

        public void ApagaLinha()
        {
            visualOutputInstructions.Reverse();
            foreach (VisualInstructionUserControl cl in visualOutputInstructions)
            {
                cl.Dispose();
            }
            visualOutputInstructions.Clear();

            visualInstructions.Reverse();
            foreach (VisualInstructionUserControl cl in visualInstructions)
            {
                cl.Dispose();
            }
            visualInstructions.Clear();

            LineBegin.Dispose();
            LineEnd.Dispose();
            BackgroundLine.Dispose();
        }


        private void AtribuiFuncoesAosControles()
        {
            LineBegin.ControleSelecionado += new ControleSelecionadoEventHandler(ladderForm.Simbolo_ControleSelecionado);
            LineBegin.MouseClick += new MouseEventHandler(simboloInicioLinha_Click);
            LineBegin.DeletaLinha += new DeletaLinhaEventHandler(ladderForm.DeletaLinha);

            int indiceInsereSaida = 0;
            if (visualOutputInstructions.Count > 0)
            {
                indiceInsereSaida = visualInstructions.Count;
                visualInstructions.AddRange(visualOutputInstructions);
            }

            // Demais simbolos
            foreach (VisualInstructionUserControl simbAux in visualInstructions)
            {
                simbAux.ControleSelecionado += new ControleSelecionadoEventHandler(ladderForm.Simbolo_ControleSelecionado);
                simbAux.MouseClick += new MouseEventHandler(Simbolo_Click);
                simbAux.KeyDown += new KeyEventHandler(ladderForm.Simbolo_KeyDown);
            }

            if (visualOutputInstructions.Count > 0)
                visualInstructions.RemoveRange(indiceInsereSaida, visualOutputInstructions.Count);

        }

        public VisualInstructionUserControl InsereSimbolo(LocalInsereSimbolo _lIS, VisualInstructionUserControl _controle, params OperationCode[] _arrayCI)
        {
            return InsereSimbolo(true, _lIS, _controle, _arrayCI);
        }

        public VisualInstructionUserControl InsereSimbolo(bool _bApos, LocalInsereSimbolo _lIS, VisualInstructionUserControl _controle, params OperationCode[] _arrayCI)
        {
            InstructionList _lstSB = new InstructionList();

            /// Insere o array de CodigosInterpretaveis em uma lista de simbolos para facilitar a manipulacao
            _lstSB.InsertAllWithClearBefore(_arrayCI);

            switch (_lIS)
            {
                case LocalInsereSimbolo.INDEFINIDO:
                    _controle = InsereSimboloIndefinido(_bApos, _controle, _lstSB);
                    break;
                case LocalInsereSimbolo.SIMBOLOS:
                    _controle = Insere2Simbolo(_bApos, _controle, _lstSB);
                    break;
                case LocalInsereSimbolo.SAIDA:
                    _controle = Insere2Saida(_bApos, _controle, _lstSB);
                    break;
            }
            return _controle;
        }


        public VisualInstructionUserControl InsereSimboloIndefinido(bool _bApos, VisualInstructionUserControl _controle, InstructionList instructions)
        {

            instructions.ValidaOperandos(ladderForm.projectForm.program.addressing);

            /// Verifica se a insercao sera no lista simbolos ou saida
            if (!instructions.Contains(OperationCode.OutputCoil) &&
                !instructions.Contains(OperationCode.Timer) &&
                !instructions.Contains(OperationCode.Counter))
            {
                return Insere2Simbolo(_bApos, _controle, instructions);
            }
            else
            {
                return Insere2Saida(_bApos, _controle, instructions);
            }
        }

        private VisualInstructionUserControl Insere2Simbolo(bool _bApos, VisualInstructionUserControl _controle, InstructionList instructions)
        {
            int _indiceSimbolo = VerificaPosicaoDeInserirSimbolo(_bApos, _controle, this.visualInstructions);

            foreach (Instruction instruction in instructions)
            {
                line.instructions.Insert(_indiceSimbolo, instruction);
                InsereSimboloUnicoVisual(_indiceSimbolo, this.visualInstructions, instruction);
                _indiceSimbolo++;
            }

            return this.visualInstructions[_indiceSimbolo - 1];
        }

        private VisualInstructionUserControl Insere2Saida(bool _bApos, VisualInstructionUserControl _controle, InstructionList instructions)
        {
            int _indiceSimbolo = 0;
            int _subt2posicionaSimboloInserido = 0;

            switch (visualOutputInstructions.Count)
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

                    _indiceSimbolo = VerificaPosicaoDeInserirSimbolo(_bApos, _controle, this.visualOutputInstructions);

                    // aqui 0=antes, 1=depois
                    if (_indiceSimbolo == 0)
                    {
                        /// prepara para inserir antes do objeto atual
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                        /// insere PP antes do objeto atual na linha
                        line.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        InsereSimboloUnicoVisual(0, this.visualOutputInstructions, new Instruction(OperationCode.ParallelBranchNext));
                        /// insere PF depois do objeto atual da linha
                        line.outputs.Insert(this.visualOutputInstructions.Count, new Instruction(OperationCode.ParallelBranchEnd));
                        InsereSimboloUnicoVisual(this.visualOutputInstructions.Count, this.visualOutputInstructions, new Instruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_FINALIZADO);
                        _subt2posicionaSimboloInserido = -1;

                        line.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchBegin));
                        InsereSimboloUnicoVisual(0, this.visualOutputInstructions, new Instruction(OperationCode.ParallelBranchBegin));
                        _indiceSimbolo++;
                    }

                    break;
                default:
                    /// Caso ja haja paralelo, insere apenas PP + simbolo
                    _indiceSimbolo = VerificaPosicaoDeInserirSimbolo(false, _controle, this.visualOutputInstructions);

                    switch (this.visualOutputInstructions[_indiceSimbolo].OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                            line.outputs[0].OpCode = OperationCode.ParallelBranchNext;
                            this.visualOutputInstructions[0].OpCode = OperationCode.ParallelBranchNext;
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

            foreach (Instruction instruction in instructions)
            {
                line.outputs.Insert(_indiceSimbolo, instruction);
                InsereSimboloUnicoVisual(_indiceSimbolo, this.visualOutputInstructions, instruction);
                _indiceSimbolo++;
            }

            /// retorna o ultimo objeto inserido
            return this.visualOutputInstructions[_indiceSimbolo - 1 + _subt2posicionaSimboloInserido];
        }

        private static int VerificaPosicaoDeInserirSimbolo(bool _bApos, VisualInstructionUserControl _controle, List<VisualInstructionUserControl> _lstFinal)
        {
            int _indiceSimbolo = _lstFinal.IndexOf(_controle);

            if (_indiceSimbolo < 0)
                _indiceSimbolo = 0;
            else
                if (_bApos)
                _indiceSimbolo++;

            return _indiceSimbolo;
        }

        private VisualInstructionUserControl InsereSimboloUnicoVisual(int _indiceSimbolo, List<VisualInstructionUserControl> _lstCL, Instruction instruction)
        {
            /// Visual
            _lstCL.Insert(_indiceSimbolo, new VisualInstructionUserControl(instruction));
            _lstCL[_indiceSimbolo].VisualLine = this;
            _lstCL[_indiceSimbolo].TabStop = true;
            _lstCL[_indiceSimbolo].ControleSelecionado += new ControleSelecionadoEventHandler(ladderForm.Simbolo_ControleSelecionado);
            _lstCL[_indiceSimbolo].MouseClick += new MouseEventHandler(Simbolo_Click);
            _lstCL[_indiceSimbolo].KeyDown += new KeyEventHandler(ladderForm.Simbolo_KeyDown);
            _lstCL[_indiceSimbolo].SolicitaMudarEndereco += new SolicitaMudarEnderecoEventHandler(ladderForm.ControleSelecionado_SolicitaMudarEndereco);
            _lstCL[_indiceSimbolo].Parent = this.ladderForm;
            _lstCL[_indiceSimbolo].Visible = false;
            _lstCL[_indiceSimbolo].CreateControl();
            _lstCL[_indiceSimbolo].BringToFront();

            if (instruction.OpCode == OperationCode.Timer ||
                instruction.OpCode == OperationCode.Counter)
                _lstCL[_indiceSimbolo].MouseHover += new EventHandler(ladderForm.SimboloQuadroSaida_MouseHover);

            return _lstCL[_indiceSimbolo];
        }



        public VisualInstructionUserControl InsereSimboloDireto(List<VisualInstructionUserControl> _lstCL, VisualInstructionUserControl _AposCL, List<Instruction> instructions)
        {
            int _indiceSimbolo = _lstCL.IndexOf(_AposCL);

            if (_indiceSimbolo < 0)
                _indiceSimbolo = 0;
            else
                _indiceSimbolo++;

            if (instructions.Count > 0)
            {
                foreach (Instruction instruction in instructions)
                {
                    InsereSimboloUnicoVisual(_indiceSimbolo, _lstCL, instruction);
                    _indiceSimbolo++;
                }

                return _lstCL[_indiceSimbolo - 1];
            }
            return null;
        }


        void Simbolo_Click(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Right))
            {
                return;
            }

            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            OperationCode opCode = visualInstruction.OpCode;
            if (opCode != OperationCode.LineBegin)
            {
                ladderForm.menuInsereLinha.Enabled = false;

                ladderForm.menuToggleBit.Enabled = false;
                if (opCode == OperationCode.ParallelBranchBegin ||
                    opCode == OperationCode.ParallelBranchEnd ||
                    opCode == OperationCode.ParallelBranchNext)
                {
                    ladderForm.menuEnderecamento.Enabled = false;
                    ladderForm.menuEnderecamento.Visible = false;

                    /// Extensao de paralelo - acima/abaixo
                    ///    somente sobre simbolos de paralelo
                    ladderForm.menuEstenderParaleloAcima.Enabled = true;
                    ladderForm.menuEstenderParaleloAcima.Visible = true;
                    ladderForm.menuEstenderParaleloAbaixo.Enabled = true;
                    ladderForm.menuEstenderParaleloAbaixo.Visible = true;

                }
                else
                {
                    ladderForm.menuEnderecamento.Enabled = true;
                    ladderForm.menuEnderecamento.Visible = true;

                    if (visualInstruction.IsAllOperandsOk())
                        ladderForm.menuToggleBit.Enabled = true;
                    else
                        ladderForm.menuToggleBit.Enabled = false;

                    ProjectForm projectForm = ladderForm.projectForm;
                    TreeNode addressingNode = projectForm.tvnProjectTree.Nodes["tvnProjectNode"].Nodes["tvnAddressingNode"];
                    foreach (TreeNode eachAddressTypeNode in addressingNode.Nodes)
                    {
                        ToolStripMenuItem menu = null;
                        switch (eachAddressTypeNode.Text)
                        {
                            case "Memories":
                                menu = ladderForm.menuMemoria;
                                break;
                            case "Timer":
                                menu = ladderForm.menuTemporizador;
                                break;
                            case "Counter":
                                menu = ladderForm.menuContador;
                                break;
                            case "Input":
                                menu = ladderForm.menuEntrada;
                                break;
                            case "Output":
                                menu = ladderForm.menuSaida;
                                break;
                        }

                        Address address = null;
                        if (visualInstruction.IsAllOperandsOk())
                        {
                            Object obj = visualInstruction.GetOperand(0);
                            if (obj is Address)
                            {
                                address = (Address)obj;
                            }
                        }

                        menu.DropDownItems.Clear();
                        foreach (TreeNode eachAddressNode in eachAddressTypeNode.Nodes)
                        {
                            menu.DropDownItems.Add(eachAddressNode.Text);

                            if (address != null)
                                if (address.Name == eachAddressNode.Text)
                                    menu.DropDownItems[menu.DropDownItems.Count - 1].Select();

                            menu.DropDownItems[menu.DropDownItems.Count - 1].Name = eachAddressNode.Text;
                            menu.DropDownItems[menu.DropDownItems.Count - 1].Tag = eachAddressNode.Tag;
                            menu.DropDownItems[menu.DropDownItems.Count - 1].Click += new EventHandler(LinhaCompletaLivre_Click);
                        }
                    }

                }
            }
            ladderForm.menuControle.Show(visualInstruction.PointToScreen(e.Location));
        }

        void LinhaCompletaLivre_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedMenuItem = (ToolStripMenuItem)sender;
            ladderForm.projectForm.InsereEnderecoNoSimbolo(ladderForm.ControleSelecionado, (Address)clickedMenuItem.Tag);
        }

        void simboloInicioLinha_Click(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Right))
            {
                return;
            }

            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            if (visualInstruction.OpCode == OperationCode.LineBegin)
            {
                ladderForm.menuEnderecamento.Enabled = false;
                ladderForm.menuInsereLinha.Enabled = true;

                /// Extensao de paralelo - acima/abaixo
                ///    somente sobre simbolos de paralelo
                ladderForm.menuEstenderParaleloAcima.Enabled = false;
                ladderForm.menuEstenderParaleloAcima.Visible = false;
                ladderForm.menuEstenderParaleloAbaixo.Enabled = false;
                ladderForm.menuEstenderParaleloAbaixo.Visible = false;

                ladderForm.menuControle.Show(visualInstruction.PointToScreen(e.Location));
            }
        }

        public bool ApagaSimbolos(VisualInstructionUserControl _aSerApagado)
        {
            int _indicePosInicial = 0;
            int _indicePosFinal = 0;
            int _auxSaida = 0;
            List<VisualInstructionUserControl> _lstCLDeletar = new List<VisualInstructionUserControl>();
            List<VisualInstructionUserControl> _lstCL = null;
            List<Instruction> instructions = null;
            VisualInstructionUserControl _cLAMudarCI = null;

            if (!visualInstructions.Contains(_aSerApagado))
            {
                if (!visualOutputInstructions.Contains(_aSerApagado))
                {
                    return false;
                }
                else
                {
                    _lstCL = this.visualOutputInstructions;
                    instructions = line.outputs;

                    /// caso haja um paralelo na saida
                    /// deleta a linha do paralelo
                    switch (_aSerApagado.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            break;
                        default:
                            if (this.visualOutputInstructions.Count > 1)
                            {
                                _auxSaida = this.visualOutputInstructions.IndexOf(_aSerApagado);
                                _aSerApagado = this.visualOutputInstructions[_auxSaida - 1];
                            }
                            break;
                    }
                }
            }
            else
            {
                _lstCL = this.visualInstructions;
                instructions = line.instructions;
            }

            switch (_aSerApagado.OpCode)
            {
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchNext:
                    _indicePosInicial = _lstCL.IndexOf(_aSerApagado);
                    _indicePosFinal = _lstCL.IndexOf(_aSerApagado.Aponta2proxPP);

                    _indicePosFinal--;

                    switch (_aSerApagado.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            if (_aSerApagado.Aponta2proxPP.Aponta2proxPP.Aponta2PI != null)
                            {
                                _lstCLDeletar.Add(_aSerApagado.Aponta2proxPP.Aponta2proxPP);
                                _indicePosFinal++;
                            }
                            else
                                _cLAMudarCI = _aSerApagado.Aponta2proxPP;
                            break;
                        case OperationCode.ParallelBranchNext:
                            if (_aSerApagado.Aponta2proxPP.Aponta2PI != null)
                            {
                                if (_aSerApagado.Aponta2proxPP.Aponta2PI.Aponta2proxPP.Equals(_aSerApagado))
                                {
                                    _lstCLDeletar.Add(_aSerApagado.Aponta2proxPP.Aponta2PI);
                                    _indicePosFinal++;
                                }
                                //else
                                //_aSerApagado.Aponta2proxPP.setCI(CodigosInterpretaveis.PARALELO_FINAL);
                            }
                            break;
                    }
                    break;
                case OperationCode.ParallelBranchEnd:
                    _indicePosFinal = _lstCL.IndexOf(_aSerApagado);
                    _indicePosInicial = _lstCL.IndexOf(_aSerApagado.Aponta2PI);
                    break;
                default:
                    _indicePosInicial = _lstCL.IndexOf(_aSerApagado);
                    _indicePosFinal = _indicePosInicial;
                    break;
            }

            /// levanta os controles a serem deletados
            for (int i = _indicePosInicial; i <= _indicePosFinal; i++)
                _lstCLDeletar.Add(_lstCL[i]);

            /// deleta um a um
            foreach (VisualInstructionUserControl _cLADeletar in _lstCLDeletar)
                ApagaSimbolo(_lstCL, instructions, _cLADeletar);

            if (_cLAMudarCI != null)
                _cLAMudarCI.OpCode = OperationCode.ParallelBranchBegin;

            return true;
        }

        public bool ApagaSimbolo(List<VisualInstructionUserControl> _lstCL, List<Instruction> instructions, VisualInstructionUserControl _aSerApagado)
        {
            int _indice = 0;

            _indice = _lstCL.IndexOf(_aSerApagado);
            _lstCL.RemoveAt(_indice);

            instructions.RemoveAt(_indice);

            _aSerApagado.ControleSelecionado -= new ControleSelecionadoEventHandler(ladderForm.Simbolo_ControleSelecionado);
            _aSerApagado.MouseClick -= new MouseEventHandler(Simbolo_Click);
            _aSerApagado.KeyDown += new KeyEventHandler(ladderForm.Simbolo_KeyDown);
            _aSerApagado.Instruction.Dispose();
            _aSerApagado.Dispose();

            return true;
        }

    }
}
