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

        public VisualLine(LadderForm _frmDiag, Line _linhaBase)
        {
            frmDiag = _frmDiag;
            linhaBase = _linhaBase;

            InicializaSimbolosFixosDaLinha();
        }

        Line linhaBase = null;
        
        /// <summary>
        /// Link para o formulario DiagramaLadder aonde sera
        /// desenhada os controles da linha
        /// </summary>
        public LadderForm frmDiag = null;

        public VisualLine linhaAnterior = null;
        public VisualLine linhaProxima = null;

        /// <summary>
        /// objetos fixos na linha
        /// </summary>
        [XmlIgnore]
        public FreeUserControl simboloInicioLinha = new FreeUserControl(OperationCode.LineBegin);
        [XmlIgnore]
        public FreeUserControl simboloFimLinha = new FreeUserControl(OperationCode.LineEnd);
        [XmlIgnore]
        public FreeUserControl simboloDesenhoFundo = new FreeUserControl(OperationCode.BackgroundLine);
        
        /// <summary>
        /// lista de Objetos dinamicos na linha
        /// </summary>
        public List<FreeUserControl> simbolos = new List<FreeUserControl>();
        public List<FreeUserControl> saida = new List<FreeUserControl>();

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
            get { return iNumLinha;  }
            set { 
                simboloInicioLinha.SetOperand(0, value);
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
            set { xTotalHorizontal = value-10; }
        }


        private int yTotalVertical = 0;
        public int yTotal
        {
            get { return yTotalVertical; }
            set { yTotalVertical = value-10; }
        }

        public void AjustaPosicionamento()
        {
            FreeUserControl _ctrlLivreAux = null;

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

            simbolos.AddRange(saida);

            foreach (FreeUserControl simbAux in simbolos)
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
                        _tamY = _par.ultimoVPI.posicaoXY.Y - _par.par.posicaoXY.Y + _par.ultimoVPI.tamanhoXY.Height; // _ultTamY2ParaleloFinal;
                        _posY = _par.par.posicaoXY.Y;

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

                        _posX = _par.par.posicaoXY.X;

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

                        _ctrlLivreAux.tamanhoXY = new Size(_ctrlLivreAux.tamanhoXY.Width, _posY - _ctrlLivreAux.posicaoXY.Y);

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
                simbAux.posicaoXY = new Point(_posX, _posY);
                simbAux.tamanhoXY = new Size(_tamX, _tamY);

                if ((saida.Count > 0) && (_contaNumSimbolos >= (simbolos.Count - saida.Count)))
                {
                    if (_contaNumSimbolos == (simbolos.Count - saida.Count))
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
            simbolos.RemoveRange((simbolos.Count - saida.Count), saida.Count);

            _tamY2DesenhoFundo += tamY;

            if ((posY > tamY) && (_tamY2DesenhoFundo > posY))
                _tamY2DesenhoFundo -= posY;

            //--Inicio da linha
            simboloInicioLinha.posicaoXY = new Point(posX, posY);
            simboloInicioLinha.tamanhoXY = new Size(this.tamX, _tamY2DesenhoFundo);
            simboloInicioLinha.Location = new Point(posX, posY);
            simboloInicioLinha.Size = new Size(this.tamX, _tamY2DesenhoFundo);

            //--Fim da linha
            if (_acumTamX < (frmDiag.Width - this.tamX))
                _acumTamX = (frmDiag.Width - this.tamX);

            if (linhaAnterior != null)
                if (_acumTamX < linhaAnterior.simboloFimLinha.posicaoXY.X)
                    _acumTamX = linhaAnterior.simboloFimLinha.posicaoXY.X;

            posX2primeiroSimbSaida = _acumTamX - _guardaAcumXnoPrimeiroSimbSaida - (_guardaTamSaida);

            simboloFimLinha.posicaoXY = new Point(_acumTamX, posY);
            simboloFimLinha.tamanhoXY = new Size(this.tamX, _tamY2DesenhoFundo);
            simboloFimLinha.Location = new Point(_acumTamX, posY);
            simboloFimLinha.Size = new Size(this.tamX, _tamY2DesenhoFundo);

            //--Desenho de fundo
            simboloDesenhoFundo.posicaoXY = new Point(posX, posY);
            simboloDesenhoFundo.tamanhoXY = new Size(_acumTamX, _tamY2DesenhoFundo);
            simboloDesenhoFundo.Location = new Point(posX, posY);
            simboloDesenhoFundo.Size = new Size(_acumTamX, _tamY2DesenhoFundo);

            RedimensionaSimbolos();
        }

        private void RedimensionaSimbolos()
        {

            //--Inicio da linha
            iTabStop++;
            simboloInicioLinha.TabIndex = iTabStop;
            simboloInicioLinha.TabStop = true;

            int indiceInsereSaida = 0;
            if (saida.Count > 0)
            {
                indiceInsereSaida = simbolos.Count;
                simbolos.AddRange(saida);
            }

            int i = 0;
            foreach (FreeUserControl _simbAux in simbolos)
            {
                iTabStop++;
                _simbAux.TabIndex = iTabStop;
                _simbAux.TabStop = true;

                _simbAux.Size = new Size(Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * percReducaoSimbolo), _simbAux.tamanhoXY.Height);

                if (i >= indiceInsereSaida && (saida.Count > 0))
                    _simbAux.Location = new Point(_simbAux.posicaoXY.X + (_simbAux.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * percReducaoSimbolo))) / 2 + posX2primeiroSimbSaida, _simbAux.posicaoXY.Y);
                else
                    _simbAux.Location = new Point(_simbAux.posicaoXY.X + (_simbAux.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * percReducaoSimbolo))) / 2, _simbAux.posicaoXY.Y);

                _simbAux.Visible = true;
                _simbAux.Invalidate();
                i++;
            }

            if (saida.Count > 0)
            {
                simbolos.RemoveRange(indiceInsereSaida, saida.Count);
            }

            //--Fim da linha
            //iTabStop = 0;
            simboloFimLinha.TabIndex = 0;
            simboloFimLinha.TabStop = false;

            //--Desenho de fundo
            //iTabStop = 0;
            simboloDesenhoFundo.TabIndex = 0;
            simboloDesenhoFundo.TabStop = false;
            simboloDesenhoFundo.Invalidate();

        }


        private void InicializaSimbolosFixosDaLinha()
        {
            // Inicio de Linha
            simboloInicioLinha.OpCode = OperationCode.LineBegin;
            simboloInicioLinha.TabStop = true;

            // Fim de Linha
            simboloFimLinha.OpCode = OperationCode.LineEnd;
            simboloFimLinha.TabStop = false;

            // Desenho de fundo da Linha
            simboloDesenhoFundo.OpCode = OperationCode.BackgroundLine;
            simboloDesenhoFundo.TabStop = false;


            ///////////////////////////////////////

            // Inicio de linha
            simboloInicioLinha.VisualLine = this;
            simboloInicioLinha.Parent = this.frmDiag;
            simboloInicioLinha.CreateControl();
            simboloInicioLinha.BringToFront();

            // Fim de linha de linha
            simboloFimLinha.VisualLine = this;
            simboloFimLinha.Parent = this.frmDiag;
            simboloFimLinha.CreateControl();
            simboloFimLinha.BringToFront();



            InsereSimboloDireto(this.simbolos, this.simboloInicioLinha, linhaBase.instructions);
            InsereSimboloDireto(this.saida, this.simboloInicioLinha, linhaBase.outputs); 

            // Desenho de fundo da linha
            simboloDesenhoFundo.VisualLine = this;
            simboloDesenhoFundo.SendToBack();
            simboloDesenhoFundo.Parent = this.frmDiag;
            simboloDesenhoFundo.CreateControl();
            simboloDesenhoFundo.SendToBack();
            /// Desabilito o desenho de fundo para evitar que um clique do mouse
            /// sobre partes dele faca com que a tela seja movida para o primeiro
            /// controle.
            simboloDesenhoFundo.Enabled = false;

            AtribuiFuncoesAosControles();
        }

        public void ApagaLinha()
        {
            saida.Reverse();
            foreach (FreeUserControl cl in saida)
            {
                cl.Dispose();
            }
            saida.Clear();

            simbolos.Reverse();
            foreach (FreeUserControl cl in simbolos)
            {
                cl.Dispose();
            }
            simbolos.Clear();

            simboloInicioLinha.Dispose();
            simboloFimLinha.Dispose();
            simboloDesenhoFundo.Dispose();
        }


        private void AtribuiFuncoesAosControles()
        {
            simboloInicioLinha.ControleSelecionado += new ControleSelecionadoEventHandler(frmDiag.Simbolo_ControleSelecionado);
            simboloInicioLinha.MouseClick += new MouseEventHandler(simboloInicioLinha_Click);
            simboloInicioLinha.DeletaLinha += new DeletaLinhaEventHandler(frmDiag.DeletaLinha);

            int indiceInsereSaida = 0;
            if (saida.Count > 0)
            {
                indiceInsereSaida = simbolos.Count;
                simbolos.AddRange(saida);
            }

            // Demais simbolos
            foreach (FreeUserControl simbAux in simbolos)
            {
                simbAux.ControleSelecionado += new ControleSelecionadoEventHandler(frmDiag.Simbolo_ControleSelecionado);
                simbAux.MouseClick += new MouseEventHandler(Simbolo_Click);
                simbAux.KeyDown +=new KeyEventHandler(frmDiag.Simbolo_KeyDown);
            }

            if (saida.Count > 0)
                simbolos.RemoveRange(indiceInsereSaida, saida.Count);

        }

        public FreeUserControl InsereSimbolo(LocalInsereSimbolo _lIS, FreeUserControl _controle, params OperationCode[] _arrayCI)
        {
            return InsereSimbolo(true, _lIS, _controle, _arrayCI);
        }

        public FreeUserControl InsereSimbolo(bool _bApos, LocalInsereSimbolo _lIS, FreeUserControl _controle, params OperationCode[] _arrayCI)
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


        public FreeUserControl InsereSimboloIndefinido(bool _bApos, FreeUserControl _controle, InstructionList _lstSB)
        {

            _lstSB.ValidaOperandos(this.frmDiag.linkProjeto.programa.endereco);

            /// Verifica se a insercao sera no lista simbolos ou saida
            if (!_lstSB.Contains(OperationCode.OutputCoil) &&
                !_lstSB.Contains(OperationCode.Timer) &&
                !_lstSB.Contains(OperationCode.Counter))
            {
                return Insere2Simbolo(_bApos, _controle, _lstSB);
            }
            else
            {
                return Insere2Saida(_bApos, _controle, _lstSB);
            }
        }

        private FreeUserControl Insere2Simbolo(bool _bApos, FreeUserControl _controle, InstructionList instructions)
        {
            int _indiceSimbolo = VerificaPosicaoDeInserirSimbolo(_bApos, _controle, this.simbolos);

            foreach (Instruction instruction in instructions)
            {
                linhaBase.instructions.Insert(_indiceSimbolo, instruction);
                InsereSimboloUnicoVisual(_indiceSimbolo, this.simbolos, instruction);
                _indiceSimbolo++;
            }

            return this.simbolos[_indiceSimbolo - 1];
        }

        private FreeUserControl Insere2Saida(bool _bApos, FreeUserControl _controle, InstructionList instructions)
        {
            int _indiceSimbolo = 0;
            int _subt2posicionaSimboloInserido = 0;

            switch (saida.Count)
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

                    _indiceSimbolo = VerificaPosicaoDeInserirSimbolo(_bApos, _controle, this.saida);

                    // aqui 0=antes, 1=depois
                    if (_indiceSimbolo == 0)
                    {
                        /// prepara para inserir antes do objeto atual
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                        /// insere PP antes do objeto atual na linha
                        linhaBase.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        InsereSimboloUnicoVisual(0, this.saida, new Instruction(OperationCode.ParallelBranchNext));
                        /// insere PF depois do objeto atual da linha
                        linhaBase.outputs.Insert(this.saida.Count, new Instruction(OperationCode.ParallelBranchEnd));
                        InsereSimboloUnicoVisual(this.saida.Count, this.saida, new Instruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_FINALIZADO);
                        _subt2posicionaSimboloInserido = -1;

                        linhaBase.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchBegin));
                        InsereSimboloUnicoVisual(0, this.saida, new Instruction(OperationCode.ParallelBranchBegin));
                        _indiceSimbolo++;
                    }

                    break;
                default:
                    /// Caso ja haja paralelo, insere apenas PP + simbolo
                    _indiceSimbolo = VerificaPosicaoDeInserirSimbolo(false, _controle, this.saida);

                    switch (this.saida[_indiceSimbolo].OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                            linhaBase.outputs[0].OpCode = OperationCode.ParallelBranchNext;
                            this.saida[0].OpCode = OperationCode.ParallelBranchNext;
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
                linhaBase.outputs.Insert(_indiceSimbolo, instruction);
                InsereSimboloUnicoVisual(_indiceSimbolo, this.saida, instruction);
                _indiceSimbolo++;
            }

            /// retorna o ultimo objeto inserido
            return this.saida[_indiceSimbolo - 1 + _subt2posicionaSimboloInserido];
        }

        private static int VerificaPosicaoDeInserirSimbolo(bool _bApos, FreeUserControl _controle, List<FreeUserControl> _lstFinal)
        {
            int _indiceSimbolo = _lstFinal.IndexOf(_controle);

            if (_indiceSimbolo < 0)
                _indiceSimbolo = 0;
            else
                if (_bApos)
                    _indiceSimbolo++;

            return _indiceSimbolo;
        }

        private FreeUserControl InsereSimboloUnicoVisual(int _indiceSimbolo, List<FreeUserControl> _lstCL, Instruction instruction)
        {
            /// Visual
            _lstCL.Insert(_indiceSimbolo, new FreeUserControl(instruction));
            _lstCL[_indiceSimbolo].VisualLine = this;
            _lstCL[_indiceSimbolo].TabStop = true;
            _lstCL[_indiceSimbolo].ControleSelecionado += new ControleSelecionadoEventHandler(frmDiag.Simbolo_ControleSelecionado);
            _lstCL[_indiceSimbolo].MouseClick += new MouseEventHandler(Simbolo_Click);
            _lstCL[_indiceSimbolo].KeyDown += new KeyEventHandler(frmDiag.Simbolo_KeyDown);
            _lstCL[_indiceSimbolo].SolicitaMudarEndereco += new SolicitaMudarEnderecoEventHandler(frmDiag.ControleSelecionado_SolicitaMudarEndereco);
            _lstCL[_indiceSimbolo].Parent = this.frmDiag;
            _lstCL[_indiceSimbolo].Visible = false;
            _lstCL[_indiceSimbolo].CreateControl();
            _lstCL[_indiceSimbolo].BringToFront();

            if (instruction.OpCode== OperationCode.Timer ||
                instruction.OpCode== OperationCode.Counter)
                _lstCL[_indiceSimbolo].MouseHover += new EventHandler(frmDiag.SimboloQuadroSaida_MouseHover);

            return _lstCL[_indiceSimbolo];
        }



        public FreeUserControl InsereSimboloDireto(List<FreeUserControl> _lstCL, FreeUserControl _AposCL, List<Instruction> instructions)
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
            FreeUserControl _cL = (FreeUserControl)sender;
            OperationCode _cI = _cL.OpCode;

            ProjectForm _frmPL;
            _frmPL = frmDiag.linkProjeto;

            if (e.Button == MouseButtons.Right)
            {
                if (_cI != OperationCode.LineBegin)
                {
                    frmDiag.menuInsereLinha.Enabled = false;

                    frmDiag.menuToggleBit.Enabled = false;
                    if (_cI == OperationCode.ParallelBranchBegin ||
                        _cI == OperationCode.ParallelBranchEnd ||
                        _cI == OperationCode.ParallelBranchNext)
                    {
                        frmDiag.menuEnderecamento.Enabled = false;
                        frmDiag.menuEnderecamento.Visible = false;

                        /// Extensao de paralelo - acima/abaixo
                        ///    somente sobre simbolos de paralelo
                        frmDiag.menuEstenderParaleloAcima.Enabled = true;
                        frmDiag.menuEstenderParaleloAcima.Visible = true;
                        frmDiag.menuEstenderParaleloAbaixo.Enabled = true;
                        frmDiag.menuEstenderParaleloAbaixo.Visible = true;

                    }
                    else
                    {
                        frmDiag.menuEnderecamento.Enabled = true;
                        frmDiag.menuEnderecamento.Visible = true;

                        if (_cL.IsAllOperandsOk())
                            frmDiag.menuToggleBit.Enabled = true;
                        else
                            frmDiag.menuToggleBit.Enabled = false;

                        TreeNode _NoEnderecamento = _frmPL.ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];

                        ToolStripMenuItem _mnu = null;
                        //ToolStripMenuItem _mnuSub = null;

                        foreach (TreeNode _NoEnd in _NoEnderecamento.Nodes)
                        {
                            //_mnu = new ToolStripMenuItem(_end.Nome);
                            switch (_NoEnd.Text)
                            {
                                case "Memories":
                                    _mnu = frmDiag.menuMemoria;
                                    break;
                                case "Timer":
                                    _mnu = frmDiag.menuTemporizador;
                                    break;
                                case "Counter":
                                    _mnu = frmDiag.menuContador;
                                    break;
                                case "Input":
                                    _mnu = frmDiag.menuEntrada;
                                    break;
                                case "Output":
                                    _mnu = frmDiag.menuSaida;
                                    break;
                            }

                            Address _end = null;
                            if (_cL.IsAllOperandsOk())
                            {
                                Object obj = _cL.GetOperand(0);
                                if (obj.GetType().Name == Address.ClassName()) // TODO Vefiry: ==  "LadderApp1.EnderecamentoLadder")
                                {
                                    _end = (LadderApp.Address)obj;
                                }
                            }

                            _mnu.DropDownItems.Clear();
                            foreach (TreeNode _NoEndSub in _NoEnd.Nodes)
                            {
                                _mnu.DropDownItems.Add(_NoEndSub.Text);

                                if (_end != null)
                                    if (_end.Nome == _NoEndSub.Text)
                                        _mnu.DropDownItems[_mnu.DropDownItems.Count - 1].Select();

                                _mnu.DropDownItems[_mnu.DropDownItems.Count - 1].Name = _NoEndSub.Text;
                                _mnu.DropDownItems[_mnu.DropDownItems.Count - 1].Tag = _NoEndSub.Tag;
                                _mnu.DropDownItems[_mnu.DropDownItems.Count - 1].Click += new EventHandler(LinhaCompletaLivre_Click);
                            }
                        }

                    }
                }
                frmDiag.menuControle.Show(_cL.PointToScreen(e.Location));
            }
        }

        void LinhaCompletaLivre_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem _mnu = (ToolStripMenuItem)sender;

            ProjectForm _frmPL;
            _frmPL = this.frmDiag.linkProjeto;

            _frmPL.InsereEnderecoNoSimbolo(frmDiag.ControleSelecionado, (Address)_mnu.Tag);
        }

        void simboloInicioLinha_Click(object sender, MouseEventArgs e)
        {
            FreeUserControl _cL = (FreeUserControl)sender;
            OperationCode _cI = _cL.OpCode;
            LadderForm _frmDL;
            _frmDL = (LadderForm)_cL.Parent;

            if (e.Button == MouseButtons.Right)
            {
                if (_cI == OperationCode.LineBegin)
                {
                    _frmDL.menuEnderecamento.Enabled = false;
                    _frmDL.menuInsereLinha.Enabled = true;

                    /// Extensao de paralelo - acima/abaixo
                    ///    somente sobre simbolos de paralelo
                    _frmDL.menuEstenderParaleloAcima.Enabled = false;
                    _frmDL.menuEstenderParaleloAcima.Visible = false;
                    _frmDL.menuEstenderParaleloAbaixo.Enabled = false;
                    _frmDL.menuEstenderParaleloAbaixo.Visible = false;

                    _frmDL.menuControle.Show(_cL.PointToScreen(e.Location));
                }
            }
        }

        public bool ApagaSimbolos(FreeUserControl _aSerApagado)
        {
            int _indicePosInicial = 0;
            int _indicePosFinal = 0;
            int _auxSaida = 0;
            List<FreeUserControl> _lstCLDeletar = new List<FreeUserControl>();
            List<FreeUserControl> _lstCL = null;
            List<Instruction> instructions = null;
            FreeUserControl _cLAMudarCI = null;

            if (!simbolos.Contains(_aSerApagado))
            {
                if (!saida.Contains(_aSerApagado))
                {
                    return false;
                }
                else
                {
                    _lstCL = this.saida;
                    instructions = linhaBase.outputs;

                    /// caso haja um paralelo na saida
                    /// deleta a linha do paralelo
                    switch (_aSerApagado.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            break;
                        default:
                            if (this.saida.Count > 1)
                            {
                                _auxSaida = this.saida.IndexOf(_aSerApagado);
                                _aSerApagado = this.saida[_auxSaida - 1];
                            }
                            break;
                    }
                }
            }
            else
            {
                _lstCL = this.simbolos;
                instructions = linhaBase.instructions;
            }

            switch (_aSerApagado.OpCode)
            {
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchNext:
                    _indicePosInicial = _lstCL.IndexOf(_aSerApagado);
                    _indicePosFinal = _lstCL.IndexOf(_aSerApagado.Aponta2proxPP);

                    _indicePosFinal--;

                    switch(_aSerApagado.OpCode)
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
            foreach (FreeUserControl _cLADeletar in _lstCLDeletar)
                ApagaSimbolo(_lstCL, instructions, _cLADeletar);

            if (_cLAMudarCI != null)
                _cLAMudarCI.OpCode = OperationCode.ParallelBranchBegin;

            return true;
        }

        public bool ApagaSimbolo(List<FreeUserControl> _lstCL, List<Instruction> instructions, FreeUserControl _aSerApagado)
        {
            int _indice = 0;

            _indice = _lstCL.IndexOf(_aSerApagado);
            _lstCL.RemoveAt(_indice);

            instructions.RemoveAt(_indice);

            _aSerApagado.ControleSelecionado -= new ControleSelecionadoEventHandler(frmDiag.Simbolo_ControleSelecionado);
            _aSerApagado.MouseClick -= new MouseEventHandler(Simbolo_Click);
            _aSerApagado.KeyDown += new KeyEventHandler(frmDiag.Simbolo_KeyDown);
            _aSerApagado.Instruction.Dispose();
            _aSerApagado.Dispose();

            return true;
        }

    }
}
