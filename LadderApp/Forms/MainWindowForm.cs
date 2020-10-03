using LadderApp.CodigoInterpretavel;
using LadderApp.Exceções;
using LadderApp.Formularios;
using LadderApp.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace LadderApp
{
    public partial class MainWindowForm : Form
    {
        private Thread newThread;

        private ProjectForm projectForm;


        public delegate void InvalidateLadderFormEventHandler();
        public InvalidateLadderFormEventHandler InvalidateLadderFormEvent;


        public delegate void UncheckBtnSimularType();
        public UncheckBtnSimularType myDelegateUncheckBtnSimular;

        public MainWindowForm()
        {
            this.AutoScroll = false;
            this.VScroll = false;
            this.HScroll = false;
            InitializeComponent();

            InvalidateLadderFormEvent = new InvalidateLadderFormEventHandler(MainWindowForm_InvalidateLadderForm);

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
                DialogResult result = MessageBox.Show(VisualResources.SaveTheProjectQuestion.Replace("%%", projectForm.Text.Trim()).Trim(), "LadderApp", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                            String readContent = p.ConvertHex2String(fileName);
                            if (VerifyPassword(readContent))
                                ReadExecutable(readContent, fileName.Substring(fileName.LastIndexOf(@"\") + 1, fileName.Length - fileName.LastIndexOf(@"\") - 1));
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

        private void MainWindowForm_FormClosed(object sender, FormClosedEventArgs e)
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


            Bitmap bmp = new Bitmap(projectForm.LadderForm.Width, projectForm.LadderForm.Height);
            this.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
            bmp.Save(@"C:\sample.png", ImageFormat.Png); // make sure path exists!

            // Draw image to screen.
            e.Graphics.DrawImage(bmp, destRect);


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
                this.Invoke(this.InvalidateLadderFormEvent);
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

        public void MainWindowForm_InvalidateLadderForm()
        {
            projectForm.LadderForm.Invalidate(true);
        }

        public void UncheckBtnSimularMethod()
        {
            btnSimulateLadder.Checked = false;
            mnuLadderSimulate.Checked = false;
        }


        private void ReadExecutable(string content, string projectName)
        {
            OperationCode opCode = OperationCode.None;
            if (content.IndexOf("@laddermic.com") != -1)
            {
                int countOfEnds = 0;
                int lineIndex = 0;
                Address address;
                AddressTypeEnum addressType;
                int index = 0;


                LadderProgram program = new LadderProgram();
                program.Name = projectName;
                program.device = new Device();
                program.addressing.AlocateIOAddressing(program.device);
                program.addressing.AlocateMemoryAddressing(program.device, program.addressing.ListMemoryAddress, AddressTypeEnum.DigitalMemory, 10);
                program.addressing.AlocateMemoryAddressing(program.device, program.addressing.ListTimerAddress, AddressTypeEnum.DigitalMemoryTimer, 10);
                program.addressing.AlocateMemoryAddressing(program.device, program.addressing.ListCounterAddress, AddressTypeEnum.DigitalMemoryCounter, 10);
                lineIndex = program.InsertLineAtEnd(new Line());

                for (int position = content.IndexOf("@laddermic.com") + 15; position < content.Length; position++)
                {
                    opCode = (OperationCode)Convert.ToChar(content.Substring(position, 1));

                    switch (opCode)
                    {
                        case OperationCode.None:
                            countOfEnds++;
                            break;
                        case OperationCode.LineEnd:
                            countOfEnds++;
                            if ((OperationCode)Convert.ToChar(content.Substring(position + 1, 1)) != OperationCode.None)
                                lineIndex = program.InsertLineAtEnd(new Line());
                            break;
                        case OperationCode.NormallyOpenContact:
                        case OperationCode.NormallyClosedContact:
                            countOfEnds = 0;
                            {
                                Instruction instruction = new Instruction((OperationCode)opCode);

                                addressType = (AddressTypeEnum)Convert.ToChar(content.Substring(position + 1, 1));
                                index = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                                address = program.addressing.Find(addressType, index);
                                if (address == null)
                                {
                                    program.device.Pins[index - 1].Type = addressType;
                                    program.device.RealocatePinAddresses();
                                    program.addressing.AlocateIOAddressing(program.device);
                                    address = program.addressing.Find(addressType, index);
                                }
                                instruction.SetOperand(0, address);

                                position += 2;
                                program.Lines[lineIndex].Instructions.Add(instruction);
                            }
                            break;
                        case OperationCode.OutputCoil:
                        case OperationCode.Reset:
                            countOfEnds = 0;
                            {
                                InstructionList instructions = new InstructionList();
                                instructions.Add(new Instruction((OperationCode)opCode));
                                addressType = (AddressTypeEnum)Convert.ToChar(content.Substring(position + 1, 1));
                                index = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                                address = program.addressing.Find(addressType, index);
                                if (address == null)
                                {
                                    program.device.Pins[index - 1].Type = addressType;
                                    program.device.RealocatePinAddresses();
                                    program.addressing.AlocateIOAddressing(program.device);
                                    address = program.addressing.Find(addressType, index);
                                }
                                instructions[instructions.Count - 1].SetOperand(0, address);
                                position += 2;
                                program.Lines[lineIndex].InsertToOutputs(instructions);
                                instructions.Clear();
                            }
                            break;
                        case OperationCode.ParallelBranchBegin:
                        case OperationCode.ParallelBranchEnd:
                        case OperationCode.ParallelBranchNext:
                            countOfEnds = 0;
                            program.Lines[lineIndex].Instructions.Add(new Instruction((OperationCode)opCode));
                            break;
                        case OperationCode.Counter:
                            countOfEnds = 0;
                            {
                                InstructionList instructions = new InstructionList();
                                instructions.Add(new Instruction((OperationCode)opCode));
                                instructions[instructions.Count - 1].SetOperand(0, program.addressing.Find(AddressTypeEnum.DigitalMemoryCounter, (Int32)Convert.ToChar(content.Substring(position + 1, 1))));
                                ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Type = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                                ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Preset = (Int32)Convert.ToChar(content.Substring(position + 3, 1));

                                instructions[instructions.Count - 1].SetOperand(1, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Type);
                                instructions[instructions.Count - 1].SetOperand(2, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Counter.Preset);
                                position += 3;
                                program.Lines[lineIndex].InsertToOutputs(instructions);
                                instructions.Clear();
                            }
                            break;
                        case OperationCode.Timer:
                            countOfEnds = 0;
                            {
                                InstructionList instructions = new InstructionList();
                                instructions.Add(new Instruction((OperationCode)opCode));
                                instructions[instructions.Count - 1].SetOperand(0, program.addressing.Find(AddressTypeEnum.DigitalMemoryTimer, (Int32)Convert.ToChar(content.Substring(position + 1, 1))));
                                ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Type = (Int32)Convert.ToChar(content.Substring(position + 2, 1));
                                ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.TimeBase = (Int32)Convert.ToChar(content.Substring(position + 3, 1));
                                ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Preset = (Int32)Convert.ToChar(content.Substring(position + 4, 1));

                                instructions[instructions.Count - 1].SetOperand(1, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Type);
                                instructions[instructions.Count - 1].SetOperand(2, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.Preset);
                                instructions[instructions.Count - 1].SetOperand(4, ((Address)instructions[instructions.Count - 1].GetOperand(0)).Timer.TimeBase);

                                position += 4;
                                program.Lines[lineIndex].InsertToOutputs(instructions);
                                instructions.Clear();
                            }
                            break;
                    }

                    /// fim dos códigos
                    if (countOfEnds >= 2)
                    {
                        MSP430IntegrationServices p = new MSP430IntegrationServices();
                        //p.CreateFile("codigosinterpretaveis.txt", content.Substring(content.IndexOf("@laddermic.com"), i - content.IndexOf("@laddermic.com") + 1));
                        //position = content.Length;
                        break;
                    }
                }
                projectForm = new ProjectForm(program);
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
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(LadderProgram));
                StreamWriter fsWriter = new StreamWriter(FileName);
                xmlSerializer.Serialize(fsWriter, projectForm.Program);
                fsWriter.Close();
                projectForm.PathFile = FileName;
                projectForm.Program.Name = Path.GetFileNameWithoutExtension(FileName);
                projectForm.Status = ProjectForm.ProgramStatus.Saved;
                projectForm.SetText();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The file cannot be saved!" + GetMessage(ex), "Salve As ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string GetMessage(Exception ex)
        {
            return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
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
                projectForm.Program.GenerateExecutable(mnuMicrocontrollerLadderSaveInsideMic.Checked, mnuMicrocontrollerLadderAskPasswordToRead.Checked, true);

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

        private bool Simulation = false;
        private void btnSimulateLadder_Click(object sender, EventArgs e)
        {
            /// inverte condição da simulação - habilitada / desabilitada
            Simulation = (Simulation == true ? false : true);

            if (Simulation)
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
                projectForm.Program.GenerateExecutable(mnuMicrocontrollerLadderSaveInsideMic.Checked, mnuMicrocontrollerLadderAskPasswordToRead.Checked, false);
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
                string content = p.ReadsViaUSB();
                if (VerifyPassword(content))
                    ReadExecutable(content, "No Name");
            }
            catch (CouldNotInitializeTIUSBException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool VerifyPassword(String content)
        {
            Text2OpCodeServices textToOpCode = new Text2OpCodeServices(content);
            if (textToOpCode.ExistsHeader())
            {
                textToOpCode.GetHeaderInformation();
                if (textToOpCode.askPassword)
                {
                    bool passwordOK = false;
                    PasswordForm passwordForm = new PasswordForm();
                    passwordForm.Text = "Type the password (1/2):";
                    passwordForm.lblPassword.Text = "Password:";

                    for (int i = 0; i < 2; i++)
                    {
                        DialogResult result = passwordForm.ShowDialog();
                        if (result == DialogResult.Cancel)
                        {
                            MessageBox.Show("Operation canceled!", "LadderApp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else
                        {
                            if (textToOpCode.password != passwordForm.txtSenha.Text)
                            {
                                passwordForm.txtSenha.Text = "";
                                passwordForm.Text = "Type the password (1/2):";
                            }
                            else
                            {
                                passwordOK = true;
                                i = 5; //sai
                            }
                        }
                    }
                    if (!passwordOK)
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

        private void mnuMicrocontrollerLadderSaveInsideMic_Click(object sender, EventArgs e)
        {
            mnuMicrocontrollerLadderSaveInsideMic.Checked = ((mnuMicrocontrollerLadderSaveInsideMic.Checked == true) ? false : true);
        }

        private void printPreviewToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }

}
