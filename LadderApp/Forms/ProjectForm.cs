using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LadderApp
{
    public partial class ProjetoLadder : Form
    {
        public DiagramaLadder frmDiagLadder = null;

       // composicao do projeto ladder
        public ProgramaBasico programa = new ProgramaBasico();

        public ProjetoLadder()
        {
            InitializeComponent();
        }

        public ProjetoLadder(ProgramaBasico _prgB)
        {
            InitializeComponent();
            //_prgB.StsPrograma = ProgramaBasico.StatusPrograma.ABERTO;
            programa = _prgB;
        }

        public void SetText()
        {
            this.Text = programa.Nome;
        }

        private void ProjetoLadder_Resize(object sender, EventArgs e)
        {
            ArvoreProjeto.Location = new Point(0, 0);
            ArvoreProjeto.Size = new Size(this.Size.Width, this.Size.Height);
        }

        private void ProjetoLadder_Load(object sender, EventArgs e)
        {
            // Expande apenas a arvore projeto
            ArvoreProjeto.Nodes[0].Expand();

            if (programa.StsPrograma == ProgramaBasico.StatusPrograma.NAOINICIADO)
            {
                programa.dispositivo = new Device(1);

                AlocaEnderecamentoIO();

                AlocaEnderecamentoMemoria(programa.endereco.lstMemoria, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA, 10);
                AlocaEnderecamentoMemoria(programa.endereco.lstTemporizador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR, 10);
                AlocaEnderecamentoMemoria(programa.endereco.lstContador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR, 10);
            }
            else
            {
                if (programa.dispositivo == null)
                    programa.dispositivo = new Device(1);
                AlocaEnderecamentoIO();

                AlocaEnderecamentoMemoria(programa.endereco.lstMemoria, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA, programa.endereco.lstMemoria.Count);
                AlocaEnderecamentoMemoria(programa.endereco.lstTemporizador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR, programa.endereco.lstTemporizador.Count);
                AlocaEnderecamentoMemoria(programa.endereco.lstContador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR, programa.endereco.lstContador.Count);

                programa.ReindexaEnderecos();
            }

            if (!ValidaDiagrama())
                AbreDiagramaLadder();
        }

        private void AbrirArquivo()
        {
        }

        private void ProjetoLadder_Shown(object sender, EventArgs e)
        {
            EditorLadder _frmEditorLadder;
            _frmEditorLadder = (EditorLadder)this.MdiParent;
            _frmEditorLadder.ArrangeProjeto();
        }

        private void ProjetoLadder_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult _result = MessageBox.Show(String.Format("Do you want to save the project {0}?", Text), "LadderApp", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (_result)
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    frmDiagLadder.Close();
                    break;
                case DialogResult.No:
                    frmDiagLadder.Close();
                    break;
            }
        }

        private void ArvoreProjeto_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Name)
            {
                case "NoProgramaLadder":
                    if (!ValidaDiagrama())
                    {
                        AbreDiagramaLadder();
                    } 
                    break;
                case "NoDispositivo":
                    frmDispositivo frmDisp = new frmDispositivo(programa.dispositivo);

                    if (frmDisp.ShowDialog() == DialogResult.OK)
                    {
                        int i = 0;
                        foreach (BitPortasDispositivo pd in programa.dispositivo.lstBitPorta)
                        {
                            pd.TipoDefinido = frmDisp.lstEndModificado[i];
                            i++;
                        }
                        programa.dispositivo.RealocaEnderecoDispositivo();

                        AlocaEnderecamentoIO();
                    }
                    break;
                case "NoConfMemoria":
                    Memoria frmMem = new Memoria(programa.endereco);
                    frmMem.Owner = this;

                    if (frmMem.ShowDialog() == DialogResult.OK)
                    {
                        AlocaEnderecamentoMemoria(programa.endereco.lstMemoria, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA, frmMem.QtdMemoria);
                        AlocaEnderecamentoMemoria(programa.endereco.lstTemporizador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR, frmMem.QtdTemporizador);
                        AlocaEnderecamentoMemoria(programa.endereco.lstContador, TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR, frmMem.QtdContador);
                    }
                    break;
                default:
                    if (e.Node.Tag != null)
                        if (this.frmDiagLadder.ControleSelecionado != null)
                        {
                            InsereEnderecoNoSimbolo(this.frmDiagLadder.ControleSelecionado, (Address)e.Node.Tag);
                            this.frmDiagLadder.ActiveControl = this.frmDiagLadder.ControleSelecionado;
                        }
                    break;
            }
        }

        public void AbreDiagramaLadder()
        {
            DiagramaLadder childDiagramaForm = new DiagramaLadder(this.programa);
            childDiagramaForm.linkProjeto = this;
            childDiagramaForm.MdiParent = this.MdiParent;

            frmDiagLadder = childDiagramaForm;

            childDiagramaForm.ReorganizandoLinhas();

            childDiagramaForm.Show();
        }

        public void AlocaEnderecamentoIO()
        {
            /// Atalho para o No de enderecamento
            TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            _NoEnderecamento.Nodes["NoEntradas"].Nodes.Clear();
            _NoEnderecamento.Nodes["NoSaidas"].Nodes.Clear();
            programa.endereco.lstIOEntrada.Clear();
            programa.endereco.lstIOSaida.Clear();
            foreach (Address el in programa.dispositivo.lstEndBitPorta)
            {
                el.ApontaDispositivo(programa.dispositivo);
                switch (el.TpEnderecamento)
                {
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        programa.endereco.lstIOEntrada.Add(el);
                        //if (!_NoEnderecamento.Nodes["NoEntradas"].Nodes.ContainsKey(el.Nome))
                        //{
                        _NoEnderecamento.Nodes["NoEntradas"].Nodes.Add(el.Nome, el.Nome + (el.Apelido == "" ? "" : " - " + el.Apelido));
                        _NoEnderecamento.Nodes["NoEntradas"].Nodes[el.Nome].Tag = el;
                        //el.MudouComentario += new MudouComentarioEventHandler(Endereco_MudouComentario);
                        //}
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        programa.endereco.lstIOSaida.Add(el);
                        //if (!_NoEnderecamento.Nodes["NoSaidas"].Nodes.ContainsKey(el.Nome))
                        //{
                        _NoEnderecamento.Nodes["NoSaidas"].Nodes.Add(el.Nome, el.Nome + (el.Apelido == "" ? "" : " - " + el.Apelido));
                        _NoEnderecamento.Nodes["NoSaidas"].Nodes[el.Nome].Tag = el;
                        //el.MudouComentario += new MudouComentarioEventHandler(Endereco_MudouComentario);
                        //}
                        break;
                }
                el.MudouComentario += new MudouComentarioEventHandler(Endereco_MudouComentario);
            }

        }

        void Endereco_MudouComentario(Address sender)
        {
            TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            int _pos = 0;
            switch (sender.TpEnderecamento)
            {
                case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                    _pos = _NoEnderecamento.Nodes["NoEntradas"].Nodes.IndexOfKey(sender.Nome);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoEntradas"].Nodes[_pos].Text = sender.Nome + (sender.Apelido == "" ? "" : " - " + sender.Apelido);
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                    _pos = _NoEnderecamento.Nodes["NoSaidas"].Nodes.IndexOfKey(sender.Nome);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoSaidas"].Nodes[_pos].Text = sender.Nome + (sender.Apelido == "" ? "" : " - " + sender.Apelido);
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                    _pos = _NoEnderecamento.Nodes["NoMemoria"].Nodes.IndexOfKey(sender.Nome);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoMemoria"].Nodes[_pos].Text = sender.Nome + (sender.Apelido == "" ? "" : " - " + sender.Apelido);
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                    _pos = _NoEnderecamento.Nodes["NoContadores"].Nodes.IndexOfKey(sender.Nome);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoContadores"].Nodes[_pos].Text = sender.Nome + (sender.Apelido == "" ? "" : " - " + sender.Apelido);
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                    _pos = _NoEnderecamento.Nodes["NoTemporizadores"].Nodes.IndexOfKey(sender.Nome);

                    if (_pos >= 0)
                        _NoEnderecamento.Nodes["NoTemporizadores"].Nodes[_pos].Text = sender.Nome + (sender.Apelido == "" ? "" : " - " + sender.Apelido);
                    break;
            }

        }

        /// <summary>
        /// Aloca e realoca na No objeto de enderecamento do programa atual
        /// uma quantidade especificada do tipo de memoria solicitado
        /// </summary>
        /// <param name="e">Enderecamento do programa</param>
        /// <param name="tp">tipo de memoria a ser realocada</param>
        /// <param name="qtdEnd">Quantidade do tipo desejada</param>
        public int AlocaEnderecamentoMemoria(List<Address> _lstE, TipoEnderecamentoDispositivo tp, int qtdEnd)
        {
            /// Atalho para o No de enderecamento
            TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            String _txtNoEndereco = "";

            int _qtdAtual = 1;
            switch (tp)
            {
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                    _txtNoEndereco = "NoMemoria";
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                    _txtNoEndereco = "NoContadores";
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                    _txtNoEndereco = "NoTemporizadores";
                    break;
            }

            IndicaEnderecoEmUso(this.programa, tp);

            _NoEnderecamento.Nodes[_txtNoEndereco].Nodes.Clear();
            _qtdAtual = _lstE.Count;
            if ((_qtdAtual == 0) || (_qtdAtual < qtdEnd))
            {
                for (int i = _qtdAtual + 1; i <= qtdEnd; i++)
                    _lstE.Add(new Address(tp, i, programa.dispositivo));
            }
            else if (_qtdAtual > qtdEnd)
            {
                for (int i = (_qtdAtual - 1); i >= qtdEnd; i--)
                {
                    if (!_lstE[i].EmUso)
                    {
                        _lstE[i] = null;
                        GC.Collect();
                        _lstE.RemoveAt(i);
                    }
                    else
                        break;
                }
            }

            foreach (Address el in _lstE)
            {
                _NoEnderecamento.Nodes[_txtNoEndereco].Nodes.Add(el.Nome, el.Nome + ( el.Apelido == "" ? "" : " - " + el.Apelido ));
                _NoEnderecamento.Nodes[_txtNoEndereco].Nodes[el.Nome].Tag = el;
                el.MudouComentario += new MudouComentarioEventHandler(Endereco_MudouComentario);
            }

            return 0;
        }

        private void IndicaEnderecoEmUso(ProgramaBasico _pl, TipoEnderecamentoDispositivo _te)
        {
            _pl.endereco.LimpaIndicacaoEmUso();
            foreach(Line _lc in _pl.linhas)
            {
                _lc.simbolos.AddRange(_lc.saida);
                foreach (Symbol _sb in _lc.simbolos)
                {
                    switch (_sb.getCI())
                    {
                            /// pporque disso aqui
                        case CodigosInterpretaveis.CONTATO_NA:
                        case CodigosInterpretaveis.CONTATO_NF:
                        case CodigosInterpretaveis.BOBINA_SAIDA:
                        case CodigosInterpretaveis.TEMPORIZADOR:
                        case CodigosInterpretaveis.CONTADOR:
                        case CodigosInterpretaveis.RESET:
                            if (_sb.getOperandos(0) != null)
                            {
                                Address _el = (Address)_sb.getOperandos(0);
                                if (_el.TpEnderecamento == _te)
                                    _el.EmUso = true;
                            }
                            break;
                    }
                }
                _lc.simbolos.RemoveRange(_lc.simbolos.Count - _lc.saida.Count, _lc.saida.Count);
            }
        }

        public void InsereEnderecoNoSimbolo(ControleLivre _cL, Address _end)
        {
            if (!_cL.IsDisposed)
            {
                _cL.setOperando(0, _end);
                _cL.Refresh();
            }
        }

        public bool ValidaDiagrama()
        {
            if (frmDiagLadder != null)
            {
                if (!frmDiagLadder.IsDisposed)
                    return true;
            }

            return false;
        }

        //private void RealocaEnderecoIOePortas()
        //{
        //    List<BitPortasDispositivo> _lstBitsEnderecamentoOrdenado = new List<BitPortasDispositivo>();
        //    _lstBitsEnderecamentoOrdenado.AddRange(programa.dispositivo.lstBitPorta);

        //    foreach(EnderecamentoLadder _endCada in programa.endereco.lstIOEntrada)
        //        _lstBitsEnderecamentoOrdenado


        //    foreach 

        //    foreach (BitPortasDispositivo pd in programa.dispositivo.lstBitPorta)
        //    {
        //        pd.TipoDefinido = frmDisp.lstEndModificado[i];
        //        i++;
        //    }
        //    programa.dispositivo.RealocaEnderecoDispositivo();
        //}
    }
}