using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LadderApp
{
    public class ControleBasico : UserControl, ISimbolo
    {
        //private SimboloBasico codigoInterpretavelAnterior = null;

        protected SimboloBasico codigoInterpretavel = null;
        //private bool bSimboloSalvo = false;

        public bool selecionado = false;
        //public bool editSimbolo = false;

        protected Point xyConexao;
        public Point XYConexao
        {
            get { return xyConexao; }
        }

        protected Size TamanhoXY;
        public Size tamanhoXY
        {
            get { return TamanhoXY; }
            set { TamanhoXY = value; }
        }

        protected Point PosicaoXY;
        public Point posicaoXY
        {
            get { return PosicaoXY; }
            set { PosicaoXY = value; }
        }

        public ControleBasico()
        {
        }

        public virtual void setCI(CodigosInterpretaveis codInterpretavelNovo)
        {
            //// salva o simbolo anterior para suas informacoes
            //// (ex. codigo inter., operandos) sejam utilizados quando necessario
            //SimboloBasico salvaCodigoAnterior =new SimboloBasico();

            //// para aqueles codigos ou simbos que precisam salvam o simbolo anterior
            //if (((codInterpretavelNovo == CodigosInterpretaveis.PARALELO_INICIAL ||
            //    codInterpretavelNovo == CodigosInterpretaveis.PARALELO_FINAL ||
            //    codInterpretavelNovo == CodigosInterpretaveis.PARALELO_PROXIMO ||
            //    codInterpretavelNovo == CodigosInterpretaveis.JUNCAO_VEM_PARALELO_FINAL) &&
            //    (getCI() == CodigosInterpretaveis.JUNCAO_OU)) ||
            //    (codInterpretavelNovo == CodigosInterpretaveis.CONTATO_NA ||
            //     codInterpretavelNovo == CodigosInterpretaveis.CONTATO_NF))
            //{

            //    salvaCodigoAnterior.codigoInterpretavel = getCI();
            //    salvaCodigoAnterior.Operandos = getOperandos();
            //}

            //if (editSimbolo == true || (codInterpretavelNovo != CodigosInterpretaveis.INICIO_DA_LINHA ||
            //                            codInterpretavelNovo != CodigosInterpretaveis.INICIO_DA_LINHA_EXECUTAVEL ||
            //                            codInterpretavelNovo != CodigosInterpretaveis.INICIO_DA_LINHA_COMPLEMENTO ||
            //                            codInterpretavelNovo != CodigosInterpretaveis.FIM_DA_LINHA))
                codigoInterpretavel.setCI(codInterpretavelNovo);


            //switch (getCI())
            //{
            //    case CodigosInterpretaveis.NENHUM:
            //        break;
            //    case CodigosInterpretaveis.INICIO_DA_LINHA:
            //    case CodigosInterpretaveis.INICIO_DA_LINHA_EXECUTAVEL:
            //    case CodigosInterpretaveis.INICIO_DA_LINHA_COMPLEMENTO:
            //        break;
            //    case CodigosInterpretaveis.FIM_DA_LINHA:
            //        break;
            //    case CodigosInterpretaveis.LINHA_HORIZONTAL:
            //        break;
            //    case CodigosInterpretaveis.CONTATO_NA:
            //        break;
            //    case CodigosInterpretaveis.CONTATO_NF:
            //        break;
            //    case CodigosInterpretaveis.BOBINA_SAIDA:
            //        break;
            //    case CodigosInterpretaveis.TEMPORIZADOR:
            //        break;
            //    case CodigosInterpretaveis.CONTADOR:
            //        break;
            //    case CodigosInterpretaveis.PARALELO_INICIAL:
            //    case CodigosInterpretaveis.PARALELO_FINAL:
            //    case CodigosInterpretaveis.PARALELO_PROXIMO:
            //    case CodigosInterpretaveis.JUNCAO_VEM_PARALELO_FINAL:
            //        break;
            //    default:
            //        break;
            //}
            ////DesenhaSimbolo();
        }

        virtual public void DesenhaSimbolo()
        {
        }

        public CodigosInterpretaveis getCI()
        {
            if (codigoInterpretavel != null)
                return codigoInterpretavel.getCI();
            else
                return CodigosInterpretaveis.NENHUM;

        }

        public Object[] getOperandos()
        {
            return codigoInterpretavel.getOperandos();
        }

        public Object getOperandos(int posicao)
        {
            return codigoInterpretavel.getOperandos(posicao);
        }

        public void setOperando(int iNumOperando, Object valor)
        {
            codigoInterpretavel.setOperando(iNumOperando, valor);
        }

        public void setOperando(Object[] operandos)
        {
            codigoInterpretavel.setOperando(operandos);
        }

        /// <summary>
        /// Salva o simbolo atual
        /// </summary>
        //protected void Salva()
        //{
        //    // cria um simbolo para armazenar os dados do simbolo atual
        //    codigoInterpretavelAnterior = new SimboloBasico();

        //    // salva o codigo interpretavel
        //    codigoInterpretavelAnterior.setCI(this.getCI());

        //    // salva os operandos
        //    //for (int i = 0; i < this.codigoInterpretavel.iNumOperandos; i++)
        //    //    codigoInterpretavelAnterior.setOperando(i, this.getOperandos(i));
        //    codigoInterpretavelAnterior.Operandos = this.getOperandos();

        //    bSimboloSalvo = true;
        //}

        /// <summary>
        /// Restaura o simbolo anterior
        /// </summary>
        //protected bool Restaura()
        //{
        //    // caso tenha sido salvo algum simbolo
        //    if (bSimboloSalvo)
        //    {
        //        // recupera o codigo interpretavel do simbolo anterior
        //        setCI(codigoInterpretavelAnterior.getCI());

        //        // recupera os operandos do simbolo anterior
        //        //for (int i = 0; i < codigoInterpretavelAnterior.iNumOperandos; i++)
        //        //    this.setOperando(i, codigoInterpretavelAnterior.getOperandos(i));
        //        this.codigoInterpretavel.Operandos = codigoInterpretavelAnterior.getOperandos();

        //        // indica que nao ha mais simbolo salvo
        //        bSimboloSalvo = false;

        //        return true;
        //    }
        //    else
        //        return false;

        //}

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ControleBasico
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "ControleBasico";
            this.ResumeLayout(false);

        }
    }
}
