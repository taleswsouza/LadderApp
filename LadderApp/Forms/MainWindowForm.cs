using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Threading;
using LadderApp.Formularios;
using System.Security;
using LadderApp.Exceções;
using LadderApp.CodigoInterpretavel;
using System.Diagnostics;
using LadderApp.Resources;

namespace LadderApp
{
    public partial class MainWindowForm : Form
    {
        private Thread newThread;

        private ProjectForm projectForm;


        public delegate void InvalidateDiagrama();
        public InvalidateDiagrama myDelegateInvalidateDiagrama;


        public delegate void UncheckBtnSimularType();
        public UncheckBtnSimularType myDelegateUncheckBtnSimular;

        public MainWindowForm()
        {
            this.AutoScroll = false;
            this.VScroll = false;
            this.HScroll = false;
            InitializeComponent();

            myDelegateInvalidateDiagrama = new InvalidateDiagrama(InvalidateLadderForm);

            myDelegateUncheckBtnSimular = new UncheckBtnSimularType(UncheckBtnSimularMethod);

            InitializePrintPreviewDialog();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
            {
                projectForm = new ProjectForm();
                projectForm.MdiParent = this;
                projectForm.Show();
                projectForm.SetText();
            }
            else
            {
                DialogResult result = MessageBox.Show(VisualResources.STR_QUESTIONA_SALVAR_PROJETO.Replace("%%", projectForm.Text.Trim()).Trim(), "LadderApp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    projectForm.Close();
                }
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "LadderApp files (*.xml;*.a43)|*.xml;*.a43|XML files (*.xml)|*.xml|MSP430 Executable files (*.a43)|*.a43";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                switch (Path.GetExtension(fileName).ToLower())
                {
                    case ".xml":

                        try
                        {
                            XmlReader fileReader = new XmlTextReader(new FileStream(fileName, FileMode.Open));
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LadderProgram));
                            if (xmlSerializer.CanDeserialize(fileReader))
                            {
                                LadderProgram ladderProgram = (LadderProgram)xmlSerializer.Deserialize(fileReader);
                                projectForm = new ProjectForm(ladderProgram);
                                projectForm.Status = ProjectForm.ProgramStatus.Open;
                                projectForm.PathFile = fileName;
                                projectForm.Program.Name = Path.GetFileNameWithoutExtension(fileName);
                                projectForm.MdiParent = this;
                                projectForm.Show();
                                projectForm.SetText();
                            }
                            fileReader.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error reading file! " + ex.InnerException.Message, "Open files ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        break;
                    case ".a43":
                        try
                        {
                            MSP430IntegrationServices p = new MSP430IntegrationServices();
                            String strLido = p.ConvertHex2String(fileName);
                            if (VerifyPassword(strLido))
                                ReadExecutable(strLido, fileName.Substring(fileName.LastIndexOf(@"\") + 1, fileName.Length - fileName.LastIndexOf(@"\") - 1));
                        }
                        catch
                        {
                            MessageBox.Show("Unknown file format!", "Open files ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    default:
                        MessageBox.Show("Unknown file format!", "Open files ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsProjectFormOpen())
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Arquivos XML (*.xml)|*.xml";
            saveFileDialog.FileName = projectForm.Program.Name + ".xml";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Save(saveFileDialog.FileName);
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Instruction> instructions = null;
            if (IsLadderFormOpen())
            {
                if (projectForm.LadderForm.VisualInstruction != null)
                    if (!projectForm.LadderForm.VisualInstruction.IsDisposed)
                    {
                        OperationCode _cI = projectForm.LadderForm.VisualInstruction.OpCode;
                        switch (_cI)
                        {
                            case OperationCode.ParallelBranchBegin:
                            case OperationCode.ParallelBranchNext:
                            case OperationCode.ParallelBranchEnd:
                                instructions = projectForm.LadderForm.VariosSelecionados(projectForm.LadderForm.VisualInstruction, projectForm.LadderForm.SelectedVisualLine);
                                break;
                            default:
                                instructions = projectForm.LadderForm.VariosSelecionados(projectForm.LadderForm.VisualInstruction, projectForm.LadderForm.SelectedVisualLine);
                                break;
                        }

                        DataFormats.Format myFormat = DataFormats.GetFormat("List<SimboloBasico>");

                        try
                        {
                            // Insert code to set properties and fields of the object.
                            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Instruction>));
                            XmlSerializer mySerializer2 = new XmlSerializer(typeof(Device));
                            // To write to a file, create a StreamWriter object.
                            StreamWriter myWriter = new StreamWriter("myFileName.xml");
                            StreamWriter myWriter2 = new StreamWriter("myDevice.xml");
                            mySerializer.Serialize(myWriter, instructions);
                            mySerializer2.Serialize(myWriter2, projectForm.Program.device);
                            myWriter.Close();
                            myWriter2.Close();

                            Clipboard.SetData(myFormat.Name, instructions);
                        }
                        catch (InvalidOperationException ex)
                        {
                            MessageBox.Show(ex.InnerException.Message);
                        }

                    }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsLadderFormOpen())
            {
                if (projectForm.LadderForm.VisualInstruction != null)
                    if (!projectForm.LadderForm.VisualInstruction.IsDisposed)
                    {
                        DataFormats.Format myFormat = DataFormats.GetFormat("List<SimboloBasico>");
                        Object returnObject = null;
                        List<Instruction> instructionsSource = new List<Instruction>();
                        InstructionList instructionsDestination = new InstructionList();

                        IDataObject iData = Clipboard.GetDataObject();

                        // Determines whether the data is in a format you can use.
                        if (iData.GetDataPresent(myFormat.Name))
                        {
                            try
                            {
                                returnObject = iData.GetData(myFormat.Name);
                            }
                            catch
                            {
                                MessageBox.Show("Error");
                            }
                        }

                        instructionsSource = (List<Instruction>)returnObject;

                        instructionsDestination.InsertAllWithClearBefore(instructionsSource);

                        VisualInstructionUserControl _controle = projectForm.LadderForm.SelectedVisualLine.InsertInstructionAtLocalToBeDefined(true, projectForm.LadderForm.VisualInstruction, instructionsDestination);
                        projectForm.LadderForm.ReorganizeLines();
                        _controle.Select();
                    }
            }
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        public void ResetWindowLayout()
        {
            if (!IsProjectFormOpen())
            {
                return;
            }

            if (!IsLadderFormOpen() || projectForm.LadderForm.IsDisposed)
            {
                projectForm.OpenLadderForm();
            }

            this.LayoutMdi(MdiLayout.TileVertical);
            int quarterHorizontalWidth = projectForm.Width / 2 - 1;

            projectForm.Width = quarterHorizontalWidth;
            projectForm.Location = new Point(0, 0);
            projectForm.LadderForm.Width = 3 * quarterHorizontalWidth + 1;
            projectForm.LadderForm.Location = new Point(projectForm.Width, projectForm.LadderForm.Location.Y);
            projectForm.LadderForm.Activate();

            projectForm.LadderForm.ReorganizeLines();
        }

        private void mnuWindowResetLayout_Click(object sender, EventArgs e)
        {
            ResetWindowLayout();
        }

        private void InsertInstruction(VisualLine.LocalToInsertInstruction localToInsertInstruction, params OperationCode[] opCodes)
        {
            if (!IsLadderFormOpen())
                return;

            if (projectForm.LadderForm.VisualInstruction.IsDisposed)
                return;

            if (btnSimulateLadder.Checked)
            {
                btnSimulateLadder.Checked = false;
                Thread.Sleep(100);
            }

            VisualInstructionUserControl visualInstruction = projectForm.LadderForm.VisualInstruction;
            VisualLine visualLine = projectForm.LadderForm.SelectedVisualLine;
            visualInstruction = visualLine.InsertInstruction(localToInsertInstruction, visualInstruction, opCodes);

            projectForm.LadderForm.ReorganizeLines();

            visualInstruction.Select();
        }

        private void btnLadderNormallyOpenContact_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.ConditionsAtLeft, OperationCode.NormallyOpenContact);
        }

        private void btnLadderNormallyClosedContact_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.ConditionsAtLeft, OperationCode.NormallyClosedContact);
        }

        private void btnLadderOutputCoil_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.OutputsAtRight, OperationCode.OutputCoil);
        }

        private void btnLadderPararellBranch_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.ConditionsAtLeft, OperationCode.ParallelBranchBegin, OperationCode.ParallelBranchNext, OperationCode.ParallelBranchEnd);
        }

        private void btnLadderLine_Click(object sender, EventArgs e)
        {
            if (IsLadderFormOpen())
            {
                projectForm.LadderForm.InsertLine();
            }
        }

        private bool IsProjectFormOpen()
        {
            foreach (var form in MdiChildren)
            {
                if (form is ProjectForm)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsLadderFormOpen()
        {
            foreach (var form in MdiChildren)
            {
                if (form is LadderForm)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnLadderOutputTimer_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.OutputsAtRight, OperationCode.Timer);
        }

        private void btnLadderOutputCounter_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.OutputsAtRight, OperationCode.Counter);
        }

        private void EditorLadder_FormClosed(object sender, FormClosedEventArgs e)
        {
            /// para garantir que a thread não estará executando qnd a aplicação fechar
            if (btnSimulateLadder.Checked)
            {
                btnSimulateLadder.Checked = false;
                Thread.Sleep(200);
            }
            /// fecha a aplicação
            Application.Exit();
        }


        // Declare a PrintDocument object named document.
        private System.Drawing.Printing.PrintDocument document =
            new System.Drawing.Printing.PrintDocument();

        // Initalize the dialog.
        private void InitializePrintPreviewDialog()
        {

            // Create a new PrintPreviewDialog using constructor.
            this.PrintPreviewDialog1 = new PrintPreviewDialog();

            //Set the size, location, and name.
            this.PrintPreviewDialog1.ClientSize =
                new System.Drawing.Size(400, 300);
            this.PrintPreviewDialog1.Location =
                new System.Drawing.Point(29, 29);
            this.PrintPreviewDialog1.Name = "PrintPreviewDialog1";

            // Associate the event-handling method with the 
            // document's PrintPage event.
            this.document.PrintPage +=
                new System.Drawing.Printing.PrintPageEventHandler
                (document_PrintPage);

            // Set the minimum size the dialog can be resized to.
            this.PrintPreviewDialog1.MinimumSize =
                new System.Drawing.Size(375, 250);

            // Set the UseAntiAlias property to true, which will allow the 
            // operating system to smooth fonts.
            this.PrintPreviewDialog1.UseAntiAlias = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            if (IsLadderFormOpen())

            // Set the PrintDocument object's name to the selectedNode
            // object's  tag, which in this case contains the 
            // fully-qualified name of the document. This value will 
            // show when the dialog reports progress.
            {
                document.DocumentName = projectForm.LadderForm.Name;
            }

            // Set the PrintPreviewDialog.Document property to
            // the PrintDocument object selected by the user.
            PrintPreviewDialog1.Document = document;

            // Call the ShowDialog method. This will trigger the document's
            //  PrintPage event.
            PrintPreviewDialog1.ShowDialog();
        }

        private void document_PrintPage(object sender,
            System.Drawing.Printing.PrintPageEventArgs e)
        {

            // Insert code to render the page here.
            // This code will be called when the PrintPreviewDialog.Show 
            // method is called.
            //Bitmap bmp = new Bitmap("c:\\img.bmp");

            // Create image.
            Image newImage = Image.FromFile("c:\\img.bmp");


            // Create rectangle for displaying image.
            Rectangle destRect = new Rectangle(100, 100, 450, 150);

            // Draw image to screen.
            e.Graphics.DrawImage(newImage, destRect);


            // The following code will render a simple
            // message on the document in the dialog.
            string text = "In document_PrintPage method.";
            System.Drawing.Font printFont =
                new System.Drawing.Font("Arial", 35,
                System.Drawing.FontStyle.Regular);

            e.Graphics.DrawString(text, printFont,
                System.Drawing.Brushes.Black, 0, 0);

        }

        private void btnSimular_Click(object sender, EventArgs e)
        {
            if (btnSimulateLadder.Checked == true || mnuLadderSimulate.Checked == true)
            {
                btnSimulateLadder.Checked = true;
                mnuLadderSimulate.Checked = true;
                newThread = new Thread(new ThreadStart(this.ContinuousSimulationExecution));
                newThread.Start();
            }
        }

        public void ContinuousSimulationExecution()
        {
            while (btnSimulateLadder.Checked)
            {
                if (!IsLadderFormOpen())
                {
                    UncheckBtnSimulateLadder(false);
                    return;
                }

                if (!projectForm.Program.VerifyProgram())
                {
                    UncheckBtnSimulateLadder(false);
                    return;
                }

                projectForm.Program.SimulateTimers();

                if (!projectForm.Program.SimulateLadder())
                {
                    UncheckBtnSimulateLadder(false);
                    return;
                }

                this.InvalidateForm(true);

                Thread.Sleep(100);
            }
        }


        private void btnLadderOutputReset_Click(object sender, EventArgs e)
        {
            InsertInstruction(VisualLine.LocalToInsertInstruction.OutputsAtRight, OperationCode.Reset);
        }

        public void InvalidateForm(bool state)
        {
            if (projectForm.LadderForm.InvokeRequired)
            {
                this.Invoke(this.myDelegateInvalidateDiagrama);
            }
            else
            {
                this.projectForm.LadderForm.Invalidate(state);
            }
        }

        public void UncheckBtnSimulateLadder(bool state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(this.myDelegateUncheckBtnSimular);
            }
            else
            {
                btnSimulateLadder.Checked = false;
                mnuLadderSimulate.Checked = false;
            }
        }

        public void InvalidateLadderForm()
        {
            projectForm.LadderForm.Invalidate(true);
        }

        public void UncheckBtnSimularMethod()
        {
            btnSimulateLadder.Checked = false;
            mnuLadderSimulate.Checked = false;
        }


        private void ReadExecutable(String DadosConvertidosChar, String strNomeProjeto)
        {
            List<int> lstCodigosLidos = new List<int>();
            OperationCode guarda = OperationCode.None;

            if (DadosConvertidosChar.IndexOf("@laddermic.com") != -1)
            {
                Int32 intContaFim = 0;
                Int32 intIndiceLinha = 0;
                Address _endLido;
                AddressTypeEnum _tpEndLido;
                Int32 _iIndiceEndLido = 0;


                /// Cria um programa novo vazio
                LadderProgram programa = new LadderProgram();
                //programa.Status = LadderProgram.ProgramStatus.New;
                programa.Name = strNomeProjeto;
                programa.device = new Device(1);
                programa.addressing.AlocaEnderecamentoIO(programa.device);
                programa.addressing.AlocaEnderecamentoMemoria(programa.device, programa.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
                programa.addressing.AlocaEnderecamentoMemoria(programa.device, programa.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
                programa.addressing.AlocaEnderecamentoMemoria(programa.device, programa.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
                intIndiceLinha = programa.InsertLineAtEnd(new Line());

                for (int i = DadosConvertidosChar.IndexOf("@laddermic.com") + 15; i < DadosConvertidosChar.Length; i++)
                {
                    guarda = (OperationCode)Convert.ToChar(DadosConvertidosChar.Substring(i, 1));

                    switch (guarda)
                    {
                        case OperationCode.None:
                            intContaFim++;
                            break;
                        case OperationCode.LineEnd:
                            intContaFim++;
                            if ((OperationCode)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1)) != OperationCode.None)
                                intIndiceLinha = programa.InsertLineAtEnd(new Line());
                            break;
                        case OperationCode.NormallyOpenContact:
                        case OperationCode.NormallyClosedContact:
                            intContaFim = 0;
                            {
                                Instruction instruction = new Instruction((OperationCode)guarda);

                                _tpEndLido = (AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1));
                                _iIndiceEndLido = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
                                if (_endLido == null)
                                {
                                    programa.device.pins[_iIndiceEndLido - 1].Type = _tpEndLido;
                                    programa.device.RealocaEnderecoDispositivo();
                                    programa.addressing.AlocaEnderecamentoIO(programa.device);
                                    _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
                                }
                                instruction.SetOperand(0, _endLido);

                                i += 2;
                                programa.Lines[intIndiceLinha].instructions.Add(instruction);
                            }
                            break;
                        case OperationCode.OutputCoil:
                        case OperationCode.Reset:
                            intContaFim = 0;
                            {
                                InstructionList _lstSB = new InstructionList();
                                _lstSB.Add(new Instruction((OperationCode)guarda));
                                _tpEndLido = (AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1));
                                _iIndiceEndLido = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
                                if (_endLido == null)
                                {
                                    programa.device.pins[_iIndiceEndLido - 1].Type = _tpEndLido;
                                    programa.device.RealocaEnderecoDispositivo();
                                    programa.addressing.AlocaEnderecamentoIO(programa.device);
                                    _endLido = programa.addressing.Find(_tpEndLido, _iIndiceEndLido);
                                }
                                _lstSB[_lstSB.Count - 1].SetOperand(0, _endLido);
                                i += 2;
                                programa.Lines[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            intContaFim = 0;
                            programa.Lines[intIndiceLinha].instructions.Add(new Instruction((OperationCode)guarda));
                            break;
                        case OperationCode.Counter:
                            intContaFim = 0;
                            {
                                InstructionList _lstSB = new InstructionList();
                                _lstSB.Add(new Instruction((OperationCode)guarda));
                                _lstSB[_lstSB.Count - 1].SetOperand(0, programa.addressing.Find(AddressTypeEnum.DigitalMemoryCounter, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));

                                _lstSB[_lstSB.Count - 1].SetOperand(1, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Tipo);
                                _lstSB[_lstSB.Count - 1].SetOperand(2, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Counter.Preset);
                                i += 3;
                                programa.Lines[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case OperationCode.Timer:
                            intContaFim = 0;
                            {
                                InstructionList _lstSB = new InstructionList();
                                _lstSB.Add(new Instruction((OperationCode)guarda));
                                _lstSB[_lstSB.Count - 1].SetOperand(0, programa.addressing.Find(AddressTypeEnum.DigitalMemoryTimer, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.BaseTempo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 4, 1));

                                _lstSB[_lstSB.Count - 1].SetOperand(1, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Tipo);
                                _lstSB[_lstSB.Count - 1].SetOperand(2, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.Preset);
                                _lstSB[_lstSB.Count - 1].SetOperand(4, ((Address)_lstSB[_lstSB.Count - 1].GetOperand(0)).Timer.BaseTempo);

                                i += 4;
                                programa.Lines[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                    }

                    /// fim dos códigos
                    if (intContaFim >= 2)
                    {
                        /// grava os dados lidos do codigo intepretavel
                        MSP430IntegrationServices p = new MSP430IntegrationServices();
                        p.CreateFile("codigosinterpretaveis.txt", DadosConvertidosChar.Substring(DadosConvertidosChar.IndexOf("@laddermic.com"), i - DadosConvertidosChar.IndexOf("@laddermic.com") + 1));

                        /// força saída do loop
                        i = DadosConvertidosChar.Length;
                    }
                }
                projectForm = new ProjectForm(programa);
                projectForm.MdiParent = this;
                projectForm.Show();
                projectForm.SetText();

            }
            else
                MessageBox.Show("Unknown file format!", "Open files ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog();
        }

        private void mnuEditComment_Click(object sender, EventArgs e)
        {
            if (IsLadderFormOpen())
                if (projectForm.LadderForm.VisualInstruction != null)
                    if (!projectForm.LadderForm.VisualInstruction.IsDisposed)
                    {
                        Instruction instruction = projectForm.LadderForm.VisualInstruction.Instruction;
                        if (instruction.IsAllOperandsOk())
                            if (instruction.GetOperand(0) is Address)
                            {
                                Address address = (Address)instruction.GetOperand(0);
                                EditCommentForm changeCommentForm = new EditCommentForm(address);
                                DialogResult result = changeCommentForm.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    address.Comment = changeCommentForm.txtComment.Text.Trim();
                                    projectForm.LadderForm.Invalidate(true);
                                }
                            }
                    }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            if (!IsProjectFormOpen())
                return;

            switch (projectForm.Status)
            {
                case ProjectForm.ProgramStatus.Open:
                case ProjectForm.ProgramStatus.Saved:
                    Save(projectForm.PathFile);
                    break;
                default:
                    SaveAsToolStripMenuItem_Click(sender, e);
                    break;
            }
        }

        private void Save(String FileName)
        {
            try
            {
                // TODO: Add code here to save the current contents of the form to a file.
                XmlSerializer mySerializer = new XmlSerializer(typeof(LadderProgram));
                //teste XmlSerializer mySerializer = new XmlSerializer(typeof(DispositivoLadder));
                //XmlSerializer mySerializer = new XmlSerializer(typeof(DispositivoLadder));
                // To write to a file, create a StreamWriter object.
                StreamWriter myWriter = new StreamWriter(FileName);
                mySerializer.Serialize(myWriter, projectForm.Program);
                //teste mySerializer.Serialize(myWriter, frmProj.programa.dispositivo);
                //mySerializer.Serialize(myWriter, frmProj.programa.dispositivo);
                myWriter.Close();
                projectForm.PathFile = FileName;
                projectForm.Program.Name = Path.GetFileNameWithoutExtension(FileName);
                projectForm.Status = ProjectForm.ProgramStatus.Saved;
                projectForm.SetText();
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show("The file cannot be saved!" + ex.InnerException.Message, "Salve As ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch
                {
                    MessageBox.Show("The file cannot be saved!" + ex.Message, "Salvar As ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void mnuGravarLadderNoExecutavel_Click_1(object sender, EventArgs e)
        {
            mnuMicrocontrollerLadderSaveInsideMic.Checked = ((mnuMicrocontrollerLadderSaveInsideMic.Checked == true) ? false : true);
        }

        private void mnuMicrocontrollerOptionsJtagUsb_Click(object sender, EventArgs e)
        {
            mnuMicrocontrollerOptionsJtagUsb.Checked = true;
            mnuMicrocontrollerOptionsJtagParallel.Checked = false;
        }

        private void mnuMicrocontrollerOptionsJtagParallel_Click(object sender, EventArgs e)
        {
            mnuMicrocontrollerOptionsJtagUsb.Checked = false;
            mnuMicrocontrollerOptionsJtagParallel.Checked = true;
        }

        private void mnuMicrocontrollerCommunicationDownload_Click(object sender, EventArgs e)
        {
            if (!IsProjectFormOpen())
                return;

            try
            {
                projectForm.Program.GeraExecutavel(mnuMicrocontrollerLadderSaveInsideMic.Checked, mnuMicrocontrollerLadderAskPasswordToRead.Checked, true);

                File.Delete(Application.StartupPath + @"\" + projectForm.Program.Name.Replace(' ', '_') + ".a43");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Write Executable");
            }
        }

        private void mnuLadderVerify_Click(object sender, EventArgs e)
        {
            if (IsLadderFormOpen())
            {
                Boolean _bResult = projectForm.Program.VerifyProgram();

                if (_bResult)
                    MessageBox.Show("OK");
                else
                    MessageBox.Show("Error");
            }
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void mnuMicrocontrollerLadderSaveInsideMic_CheckedChanged(object sender, EventArgs e)
        {
            if (!mnuMicrocontrollerLadderSaveInsideMic.Checked)
            {
                mnuMicrocontrollerLadderAskPasswordToRead.Checked = false;
                mnuMicrocontrollerLadderAskPasswordToRead.Enabled = false;
            }
            else
                mnuMicrocontrollerLadderAskPasswordToRead.Enabled = true;
        }

        private bool bSimulacao = false;
        private void btnSimulateLadder_Click(object sender, EventArgs e)
        {
            /// inverte condição da simulação - habilitada / desabilitada
            bSimulacao = (bSimulacao == true ? false : true);

            if (bSimulacao)
            {
                btnSimulateLadder.Checked = true;
                mnuLadderSimulate.Checked = true;
                newThread = new Thread(new ThreadStart(this.ContinuousSimulationExecution));
                newThread.Start();
            }
            else
            {
                btnSimulateLadder.Checked = false;
                mnuLadderSimulate.Checked = false;
            }
        }

        private void mnuMicrocontrollerBuild_Click(object sender, EventArgs e)
        {
            if (!IsProjectFormOpen())
                return;

            try
            {
                projectForm.Program.GeraExecutavel(mnuMicrocontrollerLadderSaveInsideMic.Checked, mnuMicrocontrollerLadderAskPasswordToRead.Checked, false);
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + projectForm.Program.Name.Replace(' ', '_') + ".a43"))
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + projectForm.Program.Name.Replace(' ', '_') + ".a43");

                File.Move(Application.StartupPath + @"\" + projectForm.Program.Name.Replace(' ', '_') + ".a43", Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + projectForm.Program.Name.Replace(' ', '_') + ".a43");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Write Executable");
            }
        }

        private void mnuMicrocontrollerCommunicationUpload_Click(object sender, EventArgs e)
        {
            MSP430IntegrationServices p = new MSP430IntegrationServices();
            try
            {
                String strLido = p.ReadsViaUSB();
                if (VerifyPassword(strLido))
                    ReadExecutable(strLido, "No Name");
            }
            catch (CouldNotInitializeTIUSBException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool VerifyPassword(String strLido)
        {
            Text2OpCodeServices txt2CI = null;

            txt2CI = new Text2OpCodeServices(strLido);

            if (txt2CI.ExisteCabecalho())
            {
                txt2CI.ObtemInformacoesCabecalho();
                if (txt2CI.bSolicitarSenha)
                {
                    DialogResult _result;
                    //String _strSenha = "";
                    bool _bSenhaOK = false;
                    PasswordForm _frmSenha = new PasswordForm();

                    _frmSenha.Text = "Digite a senha (1/2):";
                    _frmSenha.lblPassword.Text = "Senha:";

                    for (int i = 0; i < 2; i++)
                    {
                        _result = _frmSenha.ShowDialog();

                        if (_result == DialogResult.Cancel)
                        {
                            MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else
                        {
                            if (txt2CI.strSenha != _frmSenha.txtSenha.Text)
                            {
                                _frmSenha.txtSenha.Text = "";
                                _frmSenha.Text = "Digite a senha (1/2):";
                            }
                            else
                            {
                                _bSenhaOK = true;
                                i = 5; //sai
                            }
                        }
                    }
                    if (!_bSenhaOK)
                    {
                        MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            }

            return true;
        }

        private void indexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("AcroRd32.exe", Application.StartupPath + @"\MANUALSPVMSP430.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LadderApp");
            }
        }
    }

}
