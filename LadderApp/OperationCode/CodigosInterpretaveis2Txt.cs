using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    /// <summary>
    /// CodigosInterpretaveis2Txt - permite converter c�digos interpret�veis em texto que ser�o gravados no
    ///     programa ladder
    /// </summary>
    public class CodigosInterpretaveis2Txt
    {
        private String txtInternal = "";
        private String txtInternalWithTypeCast = "";
        public CodigosInterpretaveis2Txt txtCabecalho = null;
        private Int32 posCabecalho2Internal = 0;
        private Int32 posCabecalho2InternalWithTypeCast = 0;

        /// <summary>
        /// Identificador do c�digo interpret�vel
        /// </summary>
        const String idCodigo = "@laddermic.com";

        /// <summary>
        /// CodigosInterpretaveis2Txt() - Construtor padr�o
        /// </summary>
        public CodigosInterpretaveis2Txt()
        {
            this.Add(idCodigo);
            posCabecalho2Internal = txtInternal.Length;
            posCabecalho2InternalWithTypeCast = txtInternalWithTypeCast.Length;
        }

        /// <summary>
        /// CodigosInterpretaveis2Txt(bool) - Construtor interno somente para o atributo txtCabecalho
        /// </summary>
        /// <param name="bCabecalho">Somente para diferenciar do contrutor padr�o</param>
        internal CodigosInterpretaveis2Txt(bool bCabecalho)
        {
        }

        private bool bTxtWithTypeCast = true;
        public bool TxtWithTypeCast
        {
            get { return bTxtWithTypeCast; }
            set { bTxtWithTypeCast = value; }
        }

        /// <summary>
        /// Length - Retorna o tamanha do c�digo interpret�vel
        /// </summary>
        public Int32 Length
        {
            get { return txtInternal.Length; }
        }

        /// <summary>
        /// AddCabecalho() - Adiciona ao c�digo interpret�vel que ser� convertido para texto um cabe�alho
        /// </summary>
        public void AddCabecalho()
        {
            if (txtCabecalho == null)
                txtCabecalho = new CodigosInterpretaveis2Txt(true);
        }

        /// <summary>
        /// FinalizaCabecalho() - Finaliza o cabe�alho e o adiciona ao codigo interpret�vel principal
        /// </summary>
        public void FinalizaCabecalho()
        {
            if (txtCabecalho != null)
                if (txtCabecalho.Length > 0)
                {
                    this.txtCabecalho.Insert(txtCabecalho.Length);
                    this.txtCabecalho.Insert(CodigosInterpretaveis.CABECALHO_TAMANHO);

                    txtInternalWithTypeCast = txtInternalWithTypeCast.Insert(this.posCabecalho2InternalWithTypeCast, txtCabecalho.ToStringInternalWithTypeCast() + ", ");
                    txtInternal = txtInternal.Insert(this.posCabecalho2Internal, txtCabecalho.ToStringInternal());

                    txtCabecalho = null;
                }
        }

        /// <summary>
        /// Add(String) - Adiciona uma String ao final do texto do c�digo interpret�vel
        /// </summary>
        /// <param name="_str">String a ser adiocionado no texto do c�digo interpret�vel</param>
        public void Add(String _str)
        {
            txtInternal += _str;
            for(int i = 0; i < _str.Length; i++)
                txtInternalWithTypeCast += "'" + _str.Substring(i, 1) + "', ";
        }

        /// <summary>
        /// Add(Int32) - Adiciona um Int32 ao final do texto do c�digo interpret�vel
        /// </summary>
        /// <param name="_num">Int32 a ser adiocionado no texto do c�digo interpret�vel</param>
        public void Add(Int32 _num)
        {
            txtInternal += Convert.ToChar(_num);
            txtInternalWithTypeCast += "(char)" + _num.ToString() + ", ";
        }

        internal void Insert(Int32 _num)
        {
            txtInternal = Convert.ToChar(_num) + txtInternal;
            txtInternalWithTypeCast = "(char)" + _num.ToString() + ", " + txtInternalWithTypeCast;
        }


        public void Add(CodigosInterpretaveis _ci)
        {
            Add((Int32)_ci);
        }

        internal void Insert(CodigosInterpretaveis _ci)
        {
            Insert((Int32)_ci);
        }

        private void Add(EnderecamentoLadder _end)
        {
            Add((int)_end.TpEnderecamento);
            Add((int)_end.Indice);
        }

        public void Add(SimboloBasico _sb)
        {
            Add(_sb.getCI());

            switch (_sb.getCI())
            {
                case CodigosInterpretaveis.CONTADOR:
                    if (_sb.iNumOperandos > 0)
                    {
                        if (_sb.getOperandos(0) != null)
                            if (_sb.getOperandos(0).GetType().Name == EnderecamentoLadder.ClassName())
                            {
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Indice);
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Contador.Tipo);
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Contador.Preset);
                            }
                    }
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    if (_sb.iNumOperandos > 0)
                    {
                        if (_sb.getOperandos(0) != null)
                            if (_sb.getOperandos(0).GetType().Name == EnderecamentoLadder.ClassName())
                            {
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Indice);
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.Tipo);
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.BaseTempo);
                                Add(((EnderecamentoLadder)_sb.getOperandos(0)).Temporizador.Preset);
                            }
                    }
                    break;
                default:
                    if (_sb.iNumOperandos > 0)
                    {
                        for (int i = 0; i < _sb.iNumOperandos; i++)
                        {
                            if (_sb.getOperandos(i) != null)
                                if (_sb.getOperandos(i).GetType().Name == EnderecamentoLadder.ClassName())
                                    Add((EnderecamentoLadder)_sb.getOperandos(i));
                        }
                    }
                    break;
            }
        }

        public override string ToString()
        {
            if (bTxtWithTypeCast)
                return ToStringInternalWithTypeCast();
            else
                return "\"" + txtInternal + "\"";
        }

        internal string ToStringInternalWithTypeCast()
        {
            return txtInternalWithTypeCast.Substring(0, txtInternalWithTypeCast.Length - 2);
        }

        internal string ToStringInternal()
        {
            return txtInternal;
        }

    }
}
