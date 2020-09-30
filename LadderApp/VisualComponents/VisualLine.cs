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

            InitializeFixedVisualInstructionsToInitialVisualLine();
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
        public enum LocalToInsertInstruction
        {
            ConditionsAtLeft,
            OutputsAtRight,
            Undefined
        }

        [XmlIgnore]
        public int tabStop = 0;

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

        public int XPosition { get; set; } = 0;
        public int YPosition { get; set; } = 0;

        public int XSize = 70;
        public int YSize = 80;

        private int XPositionToFirstOutputVisualInstruction = 0;

        public double percentageVisualInstructionReduction = 1D;

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

        public void AdjustPositioning()
        {
            VisualInstructionUserControl visualInstructionAux = null;

            int biggerX = 0;

            /// Variaveis para desenhar os simbolos de saida
            /// sempre no final da linha
            int countInstructions = 0;
            int accumulatedXAtFirstOutputInstruction = 0;
            int accumulatedXAtBiggerOutputInstruction = 0;
            int outputSize = 0;

            VisualParallelBranch parallelBranch = null;
            List<VisualParallelBranch> visualParallelBranches = new List<VisualParallelBranch>();

            /// acumula o tamanho Y (pos+tam) do ultimo
            /// VPI tratado dentro de um mesmo paralelo
            /// seu valor e usado para informar
            /// ao PF qual o seu tamanho Y
            int lastSizeOfYToFinalParallelBranch = 0;

            int sizeOfYToBackgroundDraw = 0;

            /// Variaveis auxiliares para posicionamento
            ///  dos controles
            int _posX = XPosition;  // auxiliar para posX
            int _posY = YPosition;  // auxiliar para posY
            int _tamX = this.XSize;  // auxiliar para tamX
            int _tamY = YSize;  // auxiliar para tamY
            int _acumTamX = 0;  // usado para acumular o valor de X na sequencia
            // em que os simbolos sao montados
            int _acumTamY = YSize;  // auxiliar para tamY

            _acumTamX += this.XSize;

            visualInstructions.AddRange(visualOutputInstructions);

            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                countInstructions++;

                // caso todos os paralelos abertos tenham sido
                // tratados forca paralelos tratados = 0
                if (visualParallelBranches.Count == 0)
                {
                    if (biggerX > 0)
                        _posX = biggerX;

                    biggerX = 0;
                }
                else
                {
                    if (biggerX < (_posX + _tamX))
                        biggerX = _posX + _tamX;
                }

                switch (visualInstruction.OpCode)
                {
                    case OperationCode.ParallelBranchBegin:
                        _tamY = YSize; // restaura tamanho Y base
                        _posX = _acumTamX;
                        _tamX = this.XSize / 3;

                        parallelBranch = new VisualParallelBranch();
                        visualParallelBranches.Add(parallelBranch);

                        parallelBranch._yAcum = _posY;
                        parallelBranch.maiorY = _posY;
                        parallelBranch.maiorX = biggerX;
                        parallelBranch.par = visualInstruction;

                        _acumTamX = _posX + _tamX;
                        break;
                    case OperationCode.ParallelBranchEnd:
                        _tamX = this.XSize / 3;
                        _tamY = parallelBranch.ultimoVPI.PositionXY.Y - parallelBranch.par.PositionXY.Y + parallelBranch.ultimoVPI.tamanhoXY.Height; // _ultTamY2ParaleloFinal;
                        _posY = parallelBranch.par.PositionXY.Y;

                        if (biggerX > parallelBranch.maiorX)
                            parallelBranch.maiorX = biggerX;

                        _posX = parallelBranch.maiorX;

                        // Salva (por paralelo) o maior tamanho Y para os simbolos
                        // linha inicial / final e desenho de fundo
                        if (lastSizeOfYToFinalParallelBranch > sizeOfYToBackgroundDraw)
                            sizeOfYToBackgroundDraw = lastSizeOfYToFinalParallelBranch;

                        visualInstruction.SalvaVPI2PF(parallelBranch.lstVPI);

                        /// Faz os apontamentos de paralelo
                        ///  para facilitar futuras buscas de paralelos
                        visualInstruction.Aponta2PI = parallelBranch.par;
                        parallelBranch.par.Aponta2PF = visualInstruction;
                        parallelBranch.ultimoVPI.Aponta2proxPP = visualInstruction;

                        parallelBranch.ultimoVPI.UltimoVPI = true;

                        parallelBranch = null;
                        visualParallelBranches.RemoveAt(visualParallelBranches.Count - 1);

                        if (visualParallelBranches.Count > 0)
                            parallelBranch = visualParallelBranches[visualParallelBranches.Count - 1];


                        // se tiver paralelo aberto
                        // salva nele o maior y processado dentro dele
                        if (visualParallelBranches.Count > 0)
                            if (lastSizeOfYToFinalParallelBranch > parallelBranch.maiorY)
                                parallelBranch.maiorY = lastSizeOfYToFinalParallelBranch;

                        _acumTamX = _posX + _tamX;
                        break;
                    case OperationCode.ParallelBranchNext:
                        _tamY = YSize; // restaura tamanho Y base
                        _tamX = this.XSize / 3; // tamanho X reduzido

                        if (biggerX > parallelBranch.maiorX)
                            parallelBranch.maiorX = biggerX;
                        biggerX = 0;

                        _posX = parallelBranch.par.PositionXY.X;

                        parallelBranch._yAcum = parallelBranch.maiorY;
                        _posY = parallelBranch.maiorY + (YSize * (parallelBranch.numVPITratados + 1));

                        // caso seja o primeiro VPI(NXB) atualiza o SIZE do PI(BST)
                        // caso seja o segundo VPI em diante, atualiza SIZE do VPI anterior
                        if (parallelBranch.numVPITratados > 0)
                        {
                            visualInstructionAux = parallelBranch.ultimoVPI;
                            visualInstructionAux.UltimoVPI = false;
                        }
                        else
                        {
                            visualInstructionAux = parallelBranch.par;
                        }

                        visualInstructionAux.tamanhoXY = new Size(visualInstructionAux.tamanhoXY.Width, _posY - visualInstructionAux.PositionXY.Y);

                        /// PARA FACILITAR A BUSCA DO PROXIMO PONTO DE PARALELO
                        visualInstructionAux.Aponta2proxPP = visualInstruction;

                        parallelBranch.numVPITratados += 1;
                        parallelBranch.ultimoVPI = visualInstruction;
                        lastSizeOfYToFinalParallelBranch = _posY;

                        parallelBranch.lstVPI.Add(visualInstruction);

                        _acumTamX = _posX + _tamX;
                        break;
                    default:
                        _tamY = YSize;
                        _tamX = this.XSize;
                        _posX = _acumTamX;
                        _acumTamX += _tamX;
                        break;
                }

                // posiciona e dimensiona simbolo
                visualInstruction.PositionXY = new Point(_posX, _posY);
                visualInstruction.tamanhoXY = new Size(_tamX, _tamY);

                if ((visualOutputInstructions.Count > 0) && (countInstructions >= (visualInstructions.Count - visualOutputInstructions.Count)))
                {
                    if (countInstructions == (visualInstructions.Count - visualOutputInstructions.Count))
                        accumulatedXAtFirstOutputInstruction = _acumTamX;

                    if (accumulatedXAtBiggerOutputInstruction < _acumTamX)
                        accumulatedXAtBiggerOutputInstruction = _acumTamX;
                }
            }

            outputSize = (accumulatedXAtBiggerOutputInstruction - accumulatedXAtFirstOutputInstruction);// -this.tamX;
            if (outputSize == 0)
                outputSize = XSize;

            /// Caso tenha ao menos 1 objeto de saida cria um espaco de meio simbolo
            /// entre o ultimo simbolo e o primeiro de saida
            visualInstructions.RemoveRange((visualInstructions.Count - visualOutputInstructions.Count), visualOutputInstructions.Count);

            sizeOfYToBackgroundDraw += YSize;

            if ((YPosition > YSize) && (sizeOfYToBackgroundDraw > YPosition))
                sizeOfYToBackgroundDraw -= YPosition;

            //--Inicio da linha
            LineBegin.PositionXY = new Point(XPosition, YPosition);
            LineBegin.tamanhoXY = new Size(this.XSize, sizeOfYToBackgroundDraw);
            LineBegin.Location = new Point(XPosition, YPosition);
            LineBegin.Size = new Size(this.XSize, sizeOfYToBackgroundDraw);

            //--Fim da linha
            if (_acumTamX < (ladderForm.Width - this.XSize))
                _acumTamX = (ladderForm.Width - this.XSize);

            if (PreviousLine != null)
                if (_acumTamX < PreviousLine.LineEnd.PositionXY.X)
                    _acumTamX = PreviousLine.LineEnd.PositionXY.X;

            XPositionToFirstOutputVisualInstruction = _acumTamX - accumulatedXAtFirstOutputInstruction - (outputSize);

            LineEnd.PositionXY = new Point(_acumTamX, YPosition);
            LineEnd.tamanhoXY = new Size(this.XSize, sizeOfYToBackgroundDraw);
            LineEnd.Location = new Point(_acumTamX, YPosition);
            LineEnd.Size = new Size(this.XSize, sizeOfYToBackgroundDraw);

            //--Desenho de fundo
            BackgroundLine.PositionXY = new Point(XPosition, YPosition);
            BackgroundLine.tamanhoXY = new Size(_acumTamX, sizeOfYToBackgroundDraw);
            BackgroundLine.Location = new Point(XPosition, YPosition);
            BackgroundLine.Size = new Size(_acumTamX, sizeOfYToBackgroundDraw);

            ResizeVisualInstructions();
        }

        private void ResizeVisualInstructions()
        {

            //--Inicio da linha
            tabStop++;
            LineBegin.TabIndex = tabStop;
            LineBegin.TabStop = true;

            int indiceInsereSaida = 0;
            if (visualOutputInstructions.Count > 0)
            {
                indiceInsereSaida = visualInstructions.Count;
                visualInstructions.AddRange(visualOutputInstructions);
            }

            int i = 0;
            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                tabStop++;
                visualInstruction.TabIndex = tabStop;
                visualInstruction.TabStop = true;

                visualInstruction.Size = new Size(Convert.ToInt32(Convert.ToDouble(visualInstruction.tamanhoXY.Width) * percentageVisualInstructionReduction), visualInstruction.tamanhoXY.Height);

                if (i >= indiceInsereSaida && (visualOutputInstructions.Count > 0))
                    visualInstruction.Location = new Point(visualInstruction.PositionXY.X + (visualInstruction.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(visualInstruction.tamanhoXY.Width) * percentageVisualInstructionReduction))) / 2 + XPositionToFirstOutputVisualInstruction, visualInstruction.PositionXY.Y);
                else
                    visualInstruction.Location = new Point(visualInstruction.PositionXY.X + (visualInstruction.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(visualInstruction.tamanhoXY.Width) * percentageVisualInstructionReduction))) / 2, visualInstruction.PositionXY.Y);

                visualInstruction.Visible = true;
                visualInstruction.Invalidate();
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


        private void InitializeFixedVisualInstructionsToInitialVisualLine()
        {
            LineBegin.OpCode = OperationCode.LineBegin;
            LineBegin.TabStop = true;
            LineBegin.VisualLine = this;
            LineBegin.Parent = this.ladderForm;
            LineBegin.CreateControl();
            LineBegin.BringToFront();

            LineEnd.OpCode = OperationCode.LineEnd;
            LineEnd.TabStop = false;
            LineEnd.VisualLine = this;
            LineEnd.Parent = this.ladderForm;
            LineEnd.CreateControl();
            LineEnd.BringToFront();

            InsertInstructionDirect(this.visualInstructions, this.LineBegin, line.instructions);
            InsertInstructionDirect(this.visualOutputInstructions, this.LineBegin, line.outputs);

            BackgroundLine.OpCode = OperationCode.BackgroundLine;
            BackgroundLine.TabStop = false;
            BackgroundLine.VisualLine = this;
            BackgroundLine.SendToBack();
            BackgroundLine.Parent = this.ladderForm;
            BackgroundLine.CreateControl();
            BackgroundLine.SendToBack();
            /// Desabilito o desenho de fundo para evitar que um clique do mouse
            /// sobre partes dele faca com que a tela seja movida para o primeiro
            /// controle.
            BackgroundLine.Enabled = false;

            SetFunctionsToFixedVisualInstructions();
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


        private void SetFunctionsToFixedVisualInstructions()
        {
            LineBegin.VisualInstructionSelectedEvent += new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
            LineBegin.MouseClick += new MouseEventHandler(LineBeginVisualInstruction_Click);
            LineBegin.DeleteLineEvent += new DeleteLineEventHandler(ladderForm.LineBeginVisualInstruction_DeleteLine);

            int indiceInsereSaida = 0;
            if (visualOutputInstructions.Count > 0)
            {
                indiceInsereSaida = visualInstructions.Count;
                visualInstructions.AddRange(visualOutputInstructions);
            }

            // Demais simbolos
            foreach (VisualInstructionUserControl visualInstruction in visualInstructions)
            {
                visualInstruction.VisualInstructionSelectedEvent += new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
                visualInstruction.MouseClick += new MouseEventHandler(VisualInstruction_Click);
                visualInstruction.KeyDown += new KeyEventHandler(ladderForm.VisualInstruction_KeyDown);
            }

            if (visualOutputInstructions.Count > 0)
                visualInstructions.RemoveRange(indiceInsereSaida, visualOutputInstructions.Count);

        }

        public VisualInstructionUserControl InsertInstruction(LocalToInsertInstruction localToInsertInstruction, VisualInstructionUserControl visualInstruction, params OperationCode[] opCodes)
        {
            return InsertInstruction(true, localToInsertInstruction, visualInstruction, opCodes);
        }

        public VisualInstructionUserControl InsertInstruction(bool after, LocalToInsertInstruction localToInsertInstruction, VisualInstructionUserControl visualInstruction, params OperationCode[] opCodes)
        {
            InstructionList instructions = new InstructionList();
            instructions.InsertAllWithClearBefore(opCodes);

            switch (localToInsertInstruction)
            {
                case LocalToInsertInstruction.Undefined:
                    visualInstruction = InsertInstructionAtLocalToBeDefined(after, visualInstruction, instructions);
                    break;
                case LocalToInsertInstruction.ConditionsAtLeft:
                    visualInstruction = InsertConditionInstructionAtLeft(after, visualInstruction, instructions);
                    break;
                case LocalToInsertInstruction.OutputsAtRight:
                    visualInstruction = InsertOutputInstructionAtRight(after, visualInstruction, instructions);
                    break;
            }
            return visualInstruction;
        }


        public VisualInstructionUserControl InsertInstructionAtLocalToBeDefined(bool after, VisualInstructionUserControl visualInstruction, InstructionList instructions)
        {
            instructions.ValidaOperandos(ladderForm.projectForm.program.addressing);

            if (!instructions.Contains(OperationCode.OutputCoil) &&
                !instructions.Contains(OperationCode.Timer) &&
                !instructions.Contains(OperationCode.Counter))
            {
                return InsertConditionInstructionAtLeft(after, visualInstruction, instructions);
            }
            else
            {
                return InsertOutputInstructionAtRight(after, visualInstruction, instructions);
            }
        }

        private VisualInstructionUserControl InsertConditionInstructionAtLeft(bool after, VisualInstructionUserControl visualInstruction, InstructionList instructions)
        {
            int index = VerifyPositionToInsert(after, visualInstruction, this.visualInstructions);
            foreach (Instruction instruction in instructions)
            {
                line.instructions.Insert(index, instruction);
                InsertVisualInstructionAt(index, this.visualInstructions, instruction);
                index++;
            }
            return this.visualInstructions[index - 1];
        }

        private VisualInstructionUserControl InsertOutputInstructionAtRight(bool after, VisualInstructionUserControl visualInstruction, InstructionList instructions)
        {
            int index = 0;
            int _subt2posicionaSimboloInserido = 0;

            switch (visualOutputInstructions.Count)
            {
                case 0:
                    /// case 0: Primeiro simbolo na saida, adiciona apenas um
                    /// simbolo na saida
                    index = 0;

                    if (instructions.Count > 1)
                    {
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_COMPLETO);
                        _subt2posicionaSimboloInserido = -1;
                    }

                    break;
                case 1:
                    /// case 1: Caso ja exista 1 (um) simbolo de saida, insere um 
                    /// paralelo de forma automatica

                    index = VerifyPositionToInsert(after, visualInstruction, this.visualOutputInstructions);

                    // aqui 0=antes, 1=depois
                    if (index == 0)
                    {
                        /// prepara para inserir antes do objeto atual
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_INICIADO);

                        /// insere PP antes do objeto atual na linha
                        line.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchNext));
                        InsertVisualInstructionAt(0, this.visualOutputInstructions, new Instruction(OperationCode.ParallelBranchNext));
                        /// insere PF depois do objeto atual da linha
                        line.outputs.Insert(this.visualOutputInstructions.Count, new Instruction(OperationCode.ParallelBranchEnd));
                        InsertVisualInstructionAt(this.visualOutputInstructions.Count, this.visualOutputInstructions, new Instruction(OperationCode.ParallelBranchEnd));
                    }
                    else
                    {
                        instructions.InsereParalelo(InstructionList.TipoInsercaoParalelo.PARALELO_FINALIZADO);
                        _subt2posicionaSimboloInserido = -1;

                        line.outputs.Insert(0, new Instruction(OperationCode.ParallelBranchBegin));
                        InsertVisualInstructionAt(0, this.visualOutputInstructions, new Instruction(OperationCode.ParallelBranchBegin));
                        index++;
                    }

                    break;
                default:
                    /// Caso ja haja paralelo, insere apenas PP + simbolo
                    index = VerifyPositionToInsert(false, visualInstruction, this.visualOutputInstructions);

                    switch (this.visualOutputInstructions[index].OpCode)
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
                            index++;
                            break;
                    }
                    break;
            }

            foreach (Instruction instruction in instructions)
            {
                line.outputs.Insert(index, instruction);
                InsertVisualInstructionAt(index, this.visualOutputInstructions, instruction);
                index++;
            }

            /// retorna o ultimo objeto inserido
            return this.visualOutputInstructions[index - 1 + _subt2posicionaSimboloInserido];
        }

        private static int VerifyPositionToInsert(bool after, VisualInstructionUserControl visualInstruction, List<VisualInstructionUserControl> visualInstructions)
        {
            int index = visualInstructions.IndexOf(visualInstruction);

            if (index < 0)
                index = 0;
            else
                if (after)
                index++;
            return index;
        }

        private VisualInstructionUserControl InsertVisualInstructionAt(int index, List<VisualInstructionUserControl> visualInstructions, Instruction instruction)
        {
            visualInstructions.Insert(index, new VisualInstructionUserControl(instruction));
            visualInstructions[index].VisualLine = this;
            visualInstructions[index].TabStop = true;
            visualInstructions[index].VisualInstructionSelectedEvent += new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
            visualInstructions[index].MouseClick += new MouseEventHandler(VisualInstruction_Click);
            visualInstructions[index].KeyDown += new KeyEventHandler(ladderForm.VisualInstruction_KeyDown);
            visualInstructions[index].AskToChangeAddressEvent += new AskToChangeAddressEventHandler(ladderForm.ControleSelecionado_SolicitaMudarEndereco);
            visualInstructions[index].Parent = this.ladderForm;
            visualInstructions[index].Visible = false;
            visualInstructions[index].CreateControl();
            visualInstructions[index].BringToFront();

            if (instruction.OpCode == OperationCode.Timer ||
                instruction.OpCode == OperationCode.Counter)
                visualInstructions[index].MouseHover += new EventHandler(ladderForm.OutputTypeBox_MouseHover);

            return visualInstructions[index];
        }



        public VisualInstructionUserControl InsertInstructionDirect(List<VisualInstructionUserControl> visualInstructions, VisualInstructionUserControl visualInstruction, List<Instruction> instructions)
        {
            int index = visualInstructions.IndexOf(visualInstruction);

            if (index < 0)
                index = 0;
            else
                index++;

            if (instructions.Count > 0)
            {
                foreach (Instruction instruction in instructions)
                {
                    InsertVisualInstructionAt(index, visualInstructions, instruction);
                    index++;
                }

                return visualInstructions[index - 1];
            }
            return null;
        }


        void VisualInstruction_Click(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Right))
            {
                return;
            }

            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            OperationCode opCode = visualInstruction.OpCode;
            if (opCode != OperationCode.LineBegin)
            {
                ladderForm.mnuInsertLadderLine.Enabled = false;

                ladderForm.mnuToggleBit.Enabled = false;
                if (opCode == OperationCode.ParallelBranchBegin ||
                    opCode == OperationCode.ParallelBranchEnd ||
                    opCode == OperationCode.ParallelBranchNext)
                {
                    ladderForm.mnuAddressing.Enabled = false;
                    ladderForm.mnuAddressing.Visible = false;

                    /// Extensao de paralelo - acima/abaixo
                    ///    somente sobre simbolos de paralelo
                    ladderForm.mnuExtendParallelBranchAbove.Enabled = true;
                    ladderForm.mnuExtendParallelBranchAbove.Visible = true;
                    ladderForm.mnuExtendParallelBranchBelow.Enabled = true;
                    ladderForm.mnuExtendParallelBranchBelow.Visible = true;

                }
                else
                {
                    ladderForm.mnuAddressing.Enabled = true;
                    ladderForm.mnuAddressing.Visible = true;

                    if (visualInstruction.IsAllOperandsOk())
                        ladderForm.mnuToggleBit.Enabled = true;
                    else
                        ladderForm.mnuToggleBit.Enabled = false;

                    ProjectForm projectForm = ladderForm.projectForm;
                    TreeNode addressingNode = projectForm.tvnProjectTree.Nodes["tvnProjectNode"].Nodes["tvnAddressingNode"];
                    foreach (TreeNode eachAddressTypeNode in addressingNode.Nodes)
                    {
                        ToolStripMenuItem menu = null;
                        switch (eachAddressTypeNode.Text)
                        {
                            case "Memories":
                                menu = ladderForm.mnuMemory;
                                break;
                            case "Timer":
                                menu = ladderForm.mnuTimer;
                                break;
                            case "Counter":
                                menu = ladderForm.mnuCounter;
                                break;
                            case "Input":
                                menu = ladderForm.mnuInput;
                                break;
                            case "Output":
                                menu = ladderForm.mnuOutput;
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
            ladderForm.mnuContextAtInstruction.Show(visualInstruction.PointToScreen(e.Location));
        }

        void LinhaCompletaLivre_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedMenuItem = (ToolStripMenuItem)sender;
            ladderForm.projectForm.InsertAddressAtInstruction(ladderForm.VisualInstruction, (Address)clickedMenuItem.Tag);
        }

        void LineBeginVisualInstruction_Click(object sender, MouseEventArgs e)
        {
            if (!e.Button.Equals(MouseButtons.Right))
            {
                return;
            }

            VisualInstructionUserControl visualInstruction = (VisualInstructionUserControl)sender;
            if (visualInstruction.OpCode == OperationCode.LineBegin)
            {
                ladderForm.mnuAddressing.Enabled = false;
                ladderForm.mnuInsertLadderLine.Enabled = true;

                /// Extensao de paralelo - acima/abaixo
                ///    somente sobre simbolos de paralelo
                ladderForm.mnuExtendParallelBranchAbove.Enabled = false;
                ladderForm.mnuExtendParallelBranchAbove.Visible = false;
                ladderForm.mnuExtendParallelBranchBelow.Enabled = false;
                ladderForm.mnuExtendParallelBranchBelow.Visible = false;

                ladderForm.mnuContextAtInstruction.Show(visualInstruction.PointToScreen(e.Location));
            }
        }

        public bool RemoveVisualInstruction(VisualInstructionUserControl visualInstructionToBeRemoved)
        {
            int initialPositionIndex = 0;
            int finalPositionIndex = 0;
            int outputIndex = 0;
            List<VisualInstructionUserControl> visualInstructionsToRemove = new List<VisualInstructionUserControl>();
            List<VisualInstructionUserControl> visualInstructionsAux = null;
            List<Instruction> instructions = null;
            VisualInstructionUserControl visualInstructionToChangeOpCode = null;

            if (!visualInstructions.Contains(visualInstructionToBeRemoved))
            {
                if (!visualOutputInstructions.Contains(visualInstructionToBeRemoved))
                {
                    return false;
                }
                else
                {
                    visualInstructionsAux = visualOutputInstructions;
                    instructions = line.outputs;

                    /// caso haja um paralelo na saida
                    /// deleta a linha do paralelo
                    switch (visualInstructionToBeRemoved.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            break;
                        default:
                            if (this.visualOutputInstructions.Count > 1)
                            {
                                outputIndex = this.visualOutputInstructions.IndexOf(visualInstructionToBeRemoved);
                                visualInstructionToBeRemoved = this.visualOutputInstructions[outputIndex - 1];
                            }
                            break;
                    }
                }
            }
            else
            {
                visualInstructionsAux = visualInstructions;
                instructions = line.instructions;
            }

            switch (visualInstructionToBeRemoved.OpCode)
            {
                case OperationCode.ParallelBranchBegin:
                case OperationCode.ParallelBranchNext:
                    initialPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved);
                    finalPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved.Aponta2proxPP);

                    finalPositionIndex--;

                    switch (visualInstructionToBeRemoved.OpCode)
                    {
                        case OperationCode.ParallelBranchBegin:
                            if (visualInstructionToBeRemoved.Aponta2proxPP.Aponta2proxPP.Aponta2PI != null)
                            {
                                visualInstructionsToRemove.Add(visualInstructionToBeRemoved.Aponta2proxPP.Aponta2proxPP);
                                finalPositionIndex++;
                            }
                            else
                                visualInstructionToChangeOpCode = visualInstructionToBeRemoved.Aponta2proxPP;
                            break;
                        case OperationCode.ParallelBranchNext:
                            if (visualInstructionToBeRemoved.Aponta2proxPP.Aponta2PI != null)
                            {
                                if (visualInstructionToBeRemoved.Aponta2proxPP.Aponta2PI.Aponta2proxPP.Equals(visualInstructionToBeRemoved))
                                {
                                    visualInstructionsToRemove.Add(visualInstructionToBeRemoved.Aponta2proxPP.Aponta2PI);
                                    finalPositionIndex++;
                                }
                            }
                            break;
                    }
                    break;
                case OperationCode.ParallelBranchEnd:
                    finalPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved);
                    initialPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved.Aponta2PI);
                    break;
                default:
                    initialPositionIndex = visualInstructionsAux.IndexOf(visualInstructionToBeRemoved);
                    finalPositionIndex = initialPositionIndex;
                    break;
            }

            /// levanta os controles a serem deletados
            for (int i = initialPositionIndex; i <= finalPositionIndex; i++)
                visualInstructionsToRemove.Add(visualInstructionsAux[i]);

            /// deleta um a um
            foreach (VisualInstructionUserControl _cLADeletar in visualInstructionsToRemove)
                RemoveInstruction(visualInstructionsAux, instructions, _cLADeletar);

            if (visualInstructionToChangeOpCode != null)
                visualInstructionToChangeOpCode.OpCode = OperationCode.ParallelBranchBegin;

            return true;
        }

        public bool RemoveInstruction(List<VisualInstructionUserControl> visualInstructions, List<Instruction> instructions, VisualInstructionUserControl visulInstructionToBeRemoved)
        {
            int index = visualInstructions.IndexOf(visulInstructionToBeRemoved);
            visualInstructions.RemoveAt(index);

            instructions.RemoveAt(index);

            visulInstructionToBeRemoved.VisualInstructionSelectedEvent -= new VisualInstructionSelectedEventHandler(ladderForm.VisualInstruction_Selected);
            visulInstructionToBeRemoved.MouseClick -= new MouseEventHandler(VisualInstruction_Click);
            visulInstructionToBeRemoved.KeyDown -= new KeyEventHandler(ladderForm.VisualInstruction_KeyDown);
            visulInstructionToBeRemoved.Instruction.Dispose();
            visulInstructionToBeRemoved.Dispose();

            return true;
        }

    }
}
