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

        private ProjectForm frmProj = null;


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

            myDelegateInvalidateDiagrama = new InvalidateDiagrama(InvalidateDiagramaMethod);

            myDelegateUncheckBtnSimular = new UncheckBtnSimularType(UncheckBtnSimularMethod);

            InitializePrintPreviewDialog();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
            {
                frmProj = new ProjectForm();
                frmProj.MdiParent = this;
                frmProj.Show();
                frmProj.Text = "No Name";
            }
            else
            {
                DialogResult _result = MessageBox.Show(VisualResources.STR_QUESTIONA_SALVAR_PROJETO.Replace("%%", frmProj.Text.Trim()).Trim(), "LadderApp",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if (_result == DialogResult.Yes)
                {
                    frmProj.Close();
                }
            }
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            catch (SecurityException ex)
            {
                MessageBox.Show(String.Format("Security error. Impossible to continue. {0} {1}", ex.Message, ex.PermissionState));
                return;
            }
            openFileDialog.Filter = "LadderApp files (*.xml;*.a43)|*.xml;*.a43|XML files (*.xml)|*.xml|MSP430 Executable files (*.a43)|*.a43";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = "";
                FileName = openFileDialog.FileName;

                switch (FileName.Substring(FileName.Length - 4, 4).ToLower())
                {
                    case ".xml":

                        try
                        {
                           // Using a FileStream, create an XmlTextReader.
                            Stream fs = new FileStream(FileName, FileMode.Open);
                            XmlReader reader = new XmlTextReader(fs);
                            XmlSerializer serializer = new XmlSerializer(typeof(LadderProgram));
                            if (serializer.CanDeserialize(reader))
                            {
                                Object o = serializer.Deserialize(reader);

                                ((LadderProgram)o).StsPrograma = LadderProgram.StatusPrograma.ABERTO;

                                frmProj = new ProjectForm((LadderProgram)o);
                                frmProj.programa.PathFile = FileName;
                                frmProj.MdiParent = this;
                                frmProj.Show();
                                frmProj.SetText();
                            }
                            fs.Close();
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

                            String strLido = p.ConvertHex2String(FileName);

                            if (VerificaSenha(strLido))
                                LerExecutavel(strLido, FileName.Substring(FileName.LastIndexOf(@"\") + 1, FileName.Length - FileName.LastIndexOf(@"\") - 1));
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
            if (!IsProjetoAberto())
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Arquivos XML (*.xml)|*.xml";
            saveFileDialog.FileName = frmProj.programa.Nome + ".xml";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                Salvar(saveFileDialog.FileName);
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
            List<Symbol> _lstSB = null;
            if (IsDiagramaAberto())
            {
                if (frmProj.frmDiagLadder.ControleSelecionado != null)
                    if (!frmProj.frmDiagLadder.ControleSelecionado.IsDisposed)
                    {
                        OperationCode _cI = frmProj.frmDiagLadder.ControleSelecionado.OpCode;
                        switch (_cI)
                        {
                            case OperationCode.PARALELO_INICIAL:
                            case OperationCode.PARALELO_PROXIMO:
                            case OperationCode.PARALELO_FINAL:
                                _lstSB = frmProj.frmDiagLadder.VariosSelecionados(frmProj.frmDiagLadder.ControleSelecionado, frmProj.frmDiagLadder.LinhaSelecionada);
                                break;
                            default:
                                _lstSB = frmProj.frmDiagLadder.VariosSelecionados(frmProj.frmDiagLadder.ControleSelecionado, frmProj.frmDiagLadder.LinhaSelecionada);
                                break;
                        }

                        DataFormats.Format myFormat = DataFormats.GetFormat("List<SimboloBasico>");

                        try
                        {
                            // Insert code to set properties and fields of the object.
                            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Symbol>));
                            XmlSerializer mySerializer2 = new XmlSerializer(typeof(Device));
                            // To write to a file, create a StreamWriter object.
                            StreamWriter myWriter = new StreamWriter("myFileName.xml");
                            StreamWriter myWriter2 = new StreamWriter("myDevice.xml");
                            mySerializer.Serialize(myWriter, _lstSB);
                            mySerializer2.Serialize(myWriter2, frmProj.programa.dispositivo);
                            myWriter.Close();
                            myWriter2.Close();

                            Clipboard.SetData(myFormat.Name, _lstSB);
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
            if (IsDiagramaAberto())
            {
                if (frmProj.frmDiagLadder.ControleSelecionado != null)
                    if (!frmProj.frmDiagLadder.ControleSelecionado.IsDisposed)
                    {
                        DataFormats.Format myFormat = DataFormats.GetFormat("List<SimboloBasico>");
                        Object returnObject = null;
                        List<Symbol> _lstSB = new List<Symbol>();
                        SymbolList _lstSB2 = new SymbolList();

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

                        _lstSB = (List<Symbol>)returnObject;

                        _lstSB2.InsertAllWithClearBefore(_lstSB);

                        FreeUserControl _controle = frmProj.frmDiagLadder.LinhaSelecionada.InsereSimboloIndefinido(true, frmProj.frmDiagLadder.ControleSelecionado, _lstSB2);
                        frmProj.frmDiagLadder.ReorganizandoLinhas();
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

        public void ArrangeProjeto()
        {
            if(this.MdiChildren.Length > 0)
            {
                if (IsProjetoAberto())
                {
                    if(!frmProj.ValidaDiagrama())
                        frmProj.AbreDiagramaLadder();
                }

                this.LayoutMdi(MdiLayout.TileVertical);
                int _quartoHorizontal = frmProj.Width / 2;

                frmProj.Width = _quartoHorizontal - 1;
                frmProj.Location = new Point(0, 0);
                frmProj.frmDiagLadder.Width = 3 * (_quartoHorizontal - 1);
                frmProj.frmDiagLadder.Location = new Point(frmProj.Width, frmProj.frmDiagLadder.Location.Y);
                frmProj.frmDiagLadder.Activate();

                frmProj.frmDiagLadder.ReorganizandoLinhas();
            }
        }

        private void ArrangeProjetoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArrangeProjeto();
        }

        private void InsereSimbolo(VisualLine.LocalInsereSimbolo _lIS, params OperationCode[] _cI)
        {
            if (!IsDiagramaAberto())
                return;

            if (frmProj.frmDiagLadder.ControleSelecionado.IsDisposed)
                return;

            /// aborta a simulação quando for executar uma alteração
            if (btnSimular.Checked)
            {
                btnSimular.Checked = false;
                Thread.Sleep(100);
            }

            FreeUserControl _controle = frmProj.frmDiagLadder.ControleSelecionado;
            VisualLine _linha = frmProj.frmDiagLadder.LinhaSelecionada;

            _controle = _linha.InsereSimbolo(_lIS, _controle, _cI);

            /// Redesenha linhas e fundo
            frmProj.frmDiagLadder.ReorganizandoLinhas();
            //_linha.simboloDesenhoFundo.Invalidate();

            _controle.Select();
        }

        private void btnContatoNA_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SIMBOLOS, OperationCode.CONTATO_NA);
        }

        private void btnContatoNF_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SIMBOLOS, OperationCode.CONTATO_NF);
        }

        private void btnBobinaSaida_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SAIDA, OperationCode.BOBINA_SAIDA);
        }

        private void btnParalelo_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SIMBOLOS, OperationCode.PARALELO_INICIAL, OperationCode.PARALELO_PROXIMO, OperationCode.PARALELO_FINAL);
        }

        private void btnLinha_Click(object sender, EventArgs e)
        {
            if (IsDiagramaAberto())
            {
                frmProj.frmDiagLadder.InsereLinha();
            }
        }

        private void btnVerificarLadder_Click(object sender, EventArgs e)
        {
            if (IsDiagramaAberto())
            {
                Boolean _bResult = frmProj.programa.VerificaPrograma();

                if (_bResult)
                    MessageBox.Show("OK");
                else
                    MessageBox.Show("Error");
            }
        }

        private Boolean IsProjetoAberto()
        {
            if (this.MdiChildren.Length > 0)
            {
                switch (this.MdiChildren.Length)
                {
                    case 0:
                        return false;
                    default:
                        if (this.MdiChildren.Length > 1)
                            return true;
                        return false;
                }
            }
            return false;
        }

        private Boolean IsDiagramaAberto()
        {
            if (this.MdiChildren.Length > 0)
            {
                switch (this.MdiChildren.Length)
                {
                    case 2:
                        return true;
                }
            }
            return false;
        }

        private void btnTemporizador_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SAIDA, OperationCode.TEMPORIZADOR);
        }

        private void btnContador_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SAIDA, OperationCode.CONTADOR);
        }

        private void EditorLadder_FormClosed(object sender, FormClosedEventArgs e)
        {
            /// para garantir que a thread não estará executando qnd a aplicação fechar
            if (btnSimular.Checked)
            {
                btnSimular.Checked = false;
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

            if (IsDiagramaAberto())

            // Set the PrintDocument object's name to the selectedNode
            // object's  tag, which in this case contains the 
            // fully-qualified name of the document. This value will 
            // show when the dialog reports progress.
            {
                document.DocumentName = frmProj.frmDiagLadder.Name;
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
            if (btnSimular.Checked == true || simularToolStripMenuItem.Checked == true)
            {
                btnSimular.Checked = true;
                simularToolStripMenuItem.Checked = true;
                newThread = new Thread(new ThreadStart(this.ExecutaSimuladorContinuo));
                newThread.Start();
            }
        }

        /// <summary>
        /// Thread - executa continuamente enquanto a opção de simulação estiver ativa
        /// </summary>
        public void ExecutaSimuladorContinuo()
        {
            /// mantém loop enquanto opção de simulação estiver ativa
            while (btnSimular.Checked)
            {

                /// verifica se a janela do diagrama ladder está aberta
                if (!IsDiagramaAberto())
                {
                    UncheckBtnSimular(false);
                    return;
                }

                /// verifica se o programa ladder não está inconsistente
                if (!frmProj.programa.VerificaPrograma())
                {
                    UncheckBtnSimular(false);
                    return;
                }

                /// executa a função dos temporizadores
                frmProj.programa.ExecutaSimuladoTemporizadores();

                /// executa a lógica ladder
                if (!frmProj.programa.ExecutaLadderSimulado())
                {
                    UncheckBtnSimular(false);
                    return;
                }

                /// atualiza o janela do diagrama ladder
                this.InvalidaFormulario(true);

                /// aguarda 100 ms
                Thread.Sleep(100);
            }
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            InsereSimbolo(VisualLine.LocalInsereSimbolo.SAIDA, OperationCode.RESET);
        }

        private void btnSimular_CheckStateChanged(object sender, EventArgs e)
        {
        }

        public void InvalidaFormulario(bool bstate)
        {
            if (frmProj.frmDiagLadder.InvokeRequired)
            {
                this.Invoke(this.myDelegateInvalidateDiagrama);
            }
            else
            {
                this.frmProj.frmDiagLadder.Invalidate(bstate);
            }

        }

        public void UncheckBtnSimular(bool bstate)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(this.myDelegateUncheckBtnSimular);
            }
            else
            {
                btnSimular.Checked = false;
                simularToolStripMenuItem.Checked = false;
            }

        }
        
        public void InvalidateDiagramaMethod()
        {
            frmProj.frmDiagLadder.Invalidate(true);
        }

        public void UncheckBtnSimularMethod()
        {
            btnSimular.Checked = false;
            simularToolStripMenuItem.Checked = false;
        }


        private void LerExecutavel(String DadosConvertidosChar, String strNomeProjeto)
        {
            List<int> lstCodigosLidos = new List<int>();
            OperationCode guarda = OperationCode.None;

            if (DadosConvertidosChar.IndexOf("@laddermic.com") != -1)
            {
                Int32 intContaFim = 0;
                Int32 intIndiceLinha = 0;
                Int32 iNumOperandos = 0;
                Address _endLido;
                AddressTypeEnum _tpEndLido;
                Int32 _iIndiceEndLido = 0;


                /// Cria um programa novo vazio
                LadderProgram programa = new LadderProgram();
                programa.StsPrograma = LadderProgram.StatusPrograma.NOVO;
                programa.Nome = strNomeProjeto;
                programa.dispositivo = new Device(1);
                programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstMemoria, AddressTypeEnum.DIGITAL_MEMORIA, 10);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstTemporizador, AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR, 10);
                programa.endereco.AlocaEnderecamentoMemoria(programa.dispositivo, programa.endereco.lstContador, AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR, 10);
                intIndiceLinha = programa.InsereLinhaNoFinal(new Line());

                for (int i = DadosConvertidosChar.IndexOf("@laddermic.com") + 15; i < DadosConvertidosChar.Length; i++)
                {
                    guarda = (OperationCode)Convert.ToChar(DadosConvertidosChar.Substring(i, 1));

                    switch (guarda)
                    {
                        case OperationCode.None:
                            intContaFim++;
                            iNumOperandos = 0;
                            break;
                        case OperationCode.FIM_DA_LINHA:
                            intContaFim++;
                            iNumOperandos = 0;
                            if ((OperationCode)Convert.ToChar(DadosConvertidosChar.Substring(i+1, 1)) != OperationCode.None)
                            intIndiceLinha = programa.InsereLinhaNoFinal(new Line());
                            break;
                        //case CodigosInterpretaveis.INICIO_DA_LINHA:
                        case OperationCode.CONTATO_NA:
                        case OperationCode.CONTATO_NF:
                            intContaFim = 0;
                            iNumOperandos = 2;
                            {
                                Symbol _sb = new Symbol((OperationCode)guarda);

                                _tpEndLido =(AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1));
                                _iIndiceEndLido = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                if (_endLido == null)
                                {
                                    programa.dispositivo.lstBitPorta[_iIndiceEndLido - 1].TipoDefinido = _tpEndLido;
                                    programa.dispositivo.RealocaEnderecoDispositivo();
                                    programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                                    _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                }
                                _sb.setOperando(0, _endLido);

                                i += 2;
                                programa.linhas[intIndiceLinha].simbolos.Add(_sb);
                            }
                            break;
                        case OperationCode.BOBINA_SAIDA:
                        case OperationCode.RESET:
                            intContaFim = 0;
                            iNumOperandos = 2;
                            {
                                SymbolList _lstSB = new SymbolList();
                                _lstSB.Add(new Symbol((OperationCode)guarda));
                                _tpEndLido = (AddressTypeEnum)Convert.ToChar(DadosConvertidosChar.Substring(i+1, 1));
                                _iIndiceEndLido = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i+2, 1));
                                _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                if (_endLido == null)
                                {
                                    programa.dispositivo.lstBitPorta[_iIndiceEndLido - 1].TipoDefinido = _tpEndLido;
                                    programa.dispositivo.RealocaEnderecoDispositivo();
                                    programa.endereco.AlocaEnderecamentoIO(programa.dispositivo);
                                    _endLido = programa.endereco.Find(_tpEndLido, _iIndiceEndLido);
                                }
                                _lstSB[_lstSB.Count - 1].setOperando(0, _endLido);
                                i+=2;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case OperationCode.PARALELO_INICIAL:
                        case OperationCode.PARALELO_FINAL:
                        case OperationCode.PARALELO_PROXIMO:
                            intContaFim = 0;
                            iNumOperandos = 0;
                            programa.linhas[intIndiceLinha].simbolos.Add(new Symbol((OperationCode)guarda));
                            break;
                        case OperationCode.CONTADOR:
                            intContaFim = 0;
                            iNumOperandos = 3;
                            {
                                SymbolList _lstSB = new SymbolList();
                                _lstSB.Add(new Symbol((OperationCode)guarda));
                                _lstSB[_lstSB.Count - 1].setOperando(0, programa.endereco.Find(AddressTypeEnum.DIGITAL_MEMORIA_CONTADOR, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));

                                _lstSB[_lstSB.Count - 1].setOperando(1, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Tipo);
                                _lstSB[_lstSB.Count - 1].setOperando(2, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Contador.Preset);
                                i += 3;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                        case OperationCode.TEMPORIZADOR:
                            intContaFim = 0;
                            iNumOperandos = 4;
                            {
                                SymbolList _lstSB = new SymbolList();
                                _lstSB.Add(new Symbol((OperationCode)guarda));
                                _lstSB[_lstSB.Count - 1].setOperando(0, programa.endereco.Find(AddressTypeEnum.DIGITAL_MEMORIA_TEMPORIZADOR, (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 1, 1))));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Tipo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 2, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.BaseTempo = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 3, 1));
                                ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Preset = (Int32)Convert.ToChar(DadosConvertidosChar.Substring(i + 4, 1));

                                _lstSB[_lstSB.Count - 1].setOperando(1, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Tipo);
                                _lstSB[_lstSB.Count - 1].setOperando(2, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.Preset);
                                _lstSB[_lstSB.Count - 1].setOperando(4, ((Address)_lstSB[_lstSB.Count - 1].getOperandos(0)).Temporizador.BaseTempo);
                                
                                i += 4;
                                programa.linhas[intIndiceLinha].Insere2Saida(_lstSB);
                                _lstSB.Clear();
                            }
                            break;
                    }

                    /// fim dos códigos
                    if (intContaFim >= 2)
                    {
                        /// grava os dados lidos do codigo intepretavel
                        MSP430IntegrationServices p = new MSP430IntegrationServices();
                        p.CriaArquivo("codigosinterpretaveis.txt", DadosConvertidosChar.Substring(DadosConvertidosChar.IndexOf("@laddermic.com"), i - DadosConvertidosChar.IndexOf("@laddermic.com") + 1));

                        /// força saída do loop
                        i = DadosConvertidosChar.Length;
                    }
                }
                frmProj = new ProjectForm(programa);
                frmProj.MdiParent = this;
                frmProj.Show();
                frmProj.SetText();

            }
            else
                MessageBox.Show("Unknown file format!", "Open files ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog();
        }

        private void mnuEditarComentário_Click(object sender, EventArgs e)
        {
            if (IsDiagramaAberto())
                if (frmProj.frmDiagLadder.ControleSelecionado != null)
                    if (!frmProj.frmDiagLadder.ControleSelecionado.IsDisposed)
                    {
                        Symbol _sb = frmProj.frmDiagLadder.ControleSelecionado.SimboloBasico;
                        if (_sb.getOperandos(0) != null)
                            if ((_sb.getOperandos(0).GetType().Name == Address.ClassName()))
                            {
                                ChangeCommentForm frmAltComent = new ChangeCommentForm();

                                frmAltComent.txtComentario.Text = ((Address)_sb.getOperandos(0)).Apelido.Trim();
                                frmAltComent.Text = frmAltComent.Text.Replace("#ENDERECO#",((Address)_sb.getOperandos(0)).Nome);

                                DialogResult _result = frmAltComent.ShowDialog();
                                if (_result == DialogResult.OK)
                                {
                                    ((Address)_sb.getOperandos(0)).Apelido = frmAltComent.txtComentario.Text;
                                    frmProj.frmDiagLadder.Invalidate(true);
                                }
                            }
                    }
        }

        private void SalvarArquivo(object sender, EventArgs e)
        {
            if (!IsProjetoAberto())
                return;

            switch (frmProj.programa.StsPrograma)
            {
                case LadderProgram.StatusPrograma.ABERTO:
                case LadderProgram.StatusPrograma.SALVO:
                    Salvar(frmProj.programa.PathFile);
                    break;
                default:
                    SaveAsToolStripMenuItem_Click(sender, e);
                    break;
            }
        }

        private void Salvar(String FileName)
        {
            try
            {
                // TODO: Add code here to save the current contents of the form to a file.
                XmlSerializer mySerializer = new XmlSerializer(typeof(LadderProgram));
                //teste XmlSerializer mySerializer = new XmlSerializer(typeof(DispositivoLadder));
                //XmlSerializer mySerializer = new XmlSerializer(typeof(DispositivoLadder));
                // To write to a file, create a StreamWriter object.
                StreamWriter myWriter = new StreamWriter(FileName);
                mySerializer.Serialize(myWriter, frmProj.programa);
                //teste mySerializer.Serialize(myWriter, frmProj.programa.dispositivo);
                //mySerializer.Serialize(myWriter, frmProj.programa.dispositivo);
                myWriter.Close();
                frmProj.programa.PathFile = FileName;
                frmProj.programa.StsPrograma = LadderProgram.StatusPrograma.SALVO;
                frmProj.SetText();
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

        private void opçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mnuGravarLadderNoExecutavel_Click_1(object sender, EventArgs e)
        {
            mnuGravarLadderNoExecutavel.Checked = ((mnuGravarLadderNoExecutavel.Checked == true) ? false : true);
        }

        private void jTAGUSBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jTAGUSBToolStripMenuItem.Checked = true;
            jTAGParaleloToolStripMenuItem.Checked = false;
        }

        private void jTAGParaleloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jTAGUSBToolStripMenuItem.Checked = false;
            jTAGParaleloToolStripMenuItem.Checked = true;
        }

        private void baixarProgramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsProjetoAberto())
                return;

            try
            {
                frmProj.programa.GeraExecutavel(mnuGravarLadderNoExecutavel.Checked, mnuSolicitarSenhaParaLerLadder.Checked, true);

                File.Delete(Application.StartupPath + @"\" + frmProj.programa.Nome.Replace(' ', '_') + ".a43");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Write Executable");
            }
        }

        private void verificarLadderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsDiagramaAberto())
            {
                Boolean _bResult = frmProj.programa.VerificaPrograma();

                if (_bResult)
                    MessageBox.Show("OK");
                else
                    MessageBox.Show("Error");
            }
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void mnuGravarLadderNoExecutavel_CheckedChanged(object sender, EventArgs e)
        {
            if (!mnuGravarLadderNoExecutavel.Checked)
            {
                mnuSolicitarSenhaParaLerLadder.Checked = false;
                mnuSolicitarSenhaParaLerLadder.Enabled = false;
            }
            else
                mnuSolicitarSenhaParaLerLadder.Enabled = true;
        }

        private bool bSimulacao = false;
        private void Simulacao(object sender, EventArgs e)
        {
            /// inverte condição da simulação - habilitada / desabilitada
            bSimulacao = (bSimulacao == true ? false : true);

            if (bSimulacao)
            {
                btnSimular.Checked = true;
                simularToolStripMenuItem.Checked = true;
                newThread = new Thread(new ThreadStart(this.ExecutaSimuladorContinuo));
                newThread.Start();
            }
            else
            {
                btnSimular.Checked = false;
                simularToolStripMenuItem.Checked = false;
            }
        }

        private void mnuGerarExecutável_Click(object sender, EventArgs e)
        {
            if (!IsProjetoAberto())
                return;

            try
            {
                frmProj.programa.GeraExecutavel(mnuGravarLadderNoExecutavel.Checked, mnuSolicitarSenhaParaLerLadder.Checked, false);
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + frmProj.programa.Nome.Replace(' ', '_') + ".a43"))
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + frmProj.programa.Nome.Replace(' ', '_') + ".a43");

                File.Move(Application.StartupPath + @"\" + frmProj.programa.Nome.Replace(' ', '_') + ".a43", Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + frmProj.programa.Nome.Replace(' ', '_') + ".a43");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Write Executable");
            }
        }

        private void lerProgramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MSP430IntegrationServices p = new MSP430IntegrationServices();
            try
            {
                String strLido = p.LeViaUSB();

                if (VerificaSenha(strLido))
                    LerExecutavel(strLido, "No Name");
            }
            catch (CouldNotInitializeTIUSBException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool VerificaSenha(String strLido)
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
                    _frmSenha.lblSenhaAtual.Text = "Senha:";

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
