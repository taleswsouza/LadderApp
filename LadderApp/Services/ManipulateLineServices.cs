using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LadderApp
{
    public static class ManipulateLineServices
    {
        public static void AjustaPosicionamento(VisualLine lc)
        {
            FreeUserControl _ctrlLivreAux = null;

            int _maiorX = 0;

            VisualParallelBranch _par = null;
            List<VisualParallelBranch> _lst_par = new List<VisualParallelBranch>();

            int _ultTamY2ParaleloFinal = 0; // acumula o tamanho Y (pos+tam) do ultimo
            // VPI tratado dentro de um mesmo paralelo
            // seu valor e usado para informar
            // ao PF qual o seu tamanho Y

            int _tamY2DesenhoFundo = 0;

            // Variaveis auxiliares para posicionamento
            //  dos controles
            int _posX = lc.posX;  // auxiliar para posX
            int _posY = lc.posY;  // auxiliar para posY
            int _tamX = lc.tamX;  // auxiliar para tamX
            int _tamY = lc.tamY;  // auxiliar para tamY
            int _acumTamX = 0;  // usado para acumular o valor de X na sequencia
            // em que os simbolos sao montados
            int _acumTamY = lc.tamY;  // auxiliar para tamY

            List<FreeUserControl> _simbProxLinha = new List<FreeUserControl>();

            _acumTamX += lc.tamX;

            foreach (FreeUserControl simbAux in lc.simbolos)
            {
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
                        _tamY = lc.tamY; // restaura tamanho Y base
                        _posX = _acumTamX;
                        _tamX = lc.tamX / 3;

                        _par = new VisualParallelBranch();
                        _lst_par.Add(_par);

                        _par._yAcum = _posY;
                        _par.maiorY = _posY;
                        _par.maiorX = _maiorX;
                        _par.par = simbAux;

                        _acumTamX = _posX + _tamX;
                        break;
                    case OperationCode.ParallelBranchEnd:
                        _tamX = lc.tamX / 3;
                        //                        _tamY = _par.ultimoVPI.Location.Y - _par.par.Location.Y + _par.ultimoVPI.Size.Height; // _ultTamY2ParaleloFinal;
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
                        _tamY = lc.tamY; // restaura tamanho Y base
                        _tamX = lc.tamX / 3; // tamanho X reduzido

                        if (_maiorX > _par.maiorX)
                            _par.maiorX = _maiorX;
                        _maiorX = 0;

                        _posX = _par.par.posicaoXY.X;

                        _par._yAcum = _par.maiorY;
                        _posY = _par.maiorY + (lc.tamY * (_par.numVPITratados + 1));

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

                        _par.numVPITratados += 1;
                        _par.ultimoVPI = simbAux;
                        _ultTamY2ParaleloFinal = _posY;

                        _par.lstVPI.Add(simbAux);

                        _acumTamX = _posX + _tamX;
                        break;
                    default:
                        _tamY = lc.tamY;
                        _tamX = lc.tamX;
                        _posX = _acumTamX;
                        _acumTamX += _tamX;
                        break;
                }

                // posiciona e dimensiona simbolo
                simbAux.posicaoXY = new Point(_posX, _posY);
                simbAux.tamanhoXY = new Size(_tamX, _tamY);

                simbAux.Location = new Point(_posX, _posY);
                simbAux.Size = new Size(_tamX, _tamY);
            }

            _tamY2DesenhoFundo += lc.tamY;

            //--Inicio da linha
            lc.simboloInicioLinha.posicaoXY = new Point(lc.posX, lc.posY);
            lc.simboloInicioLinha.tamanhoXY = new Size(lc.tamX, _tamY2DesenhoFundo);

            //--Fim da linha
            LadderForm dlFormAux = null;
            dlFormAux = (LadderForm)lc.frmDiag;
            if (_acumTamX < (dlFormAux.Width - lc.tamX))
                _acumTamX = (dlFormAux.Width - lc.tamX);

            lc.simboloFimLinha.posicaoXY = new Point(_acumTamX, lc.posY);
            lc.simboloFimLinha.tamanhoXY = new Size(lc.tamX, _tamY2DesenhoFundo);

            //--Desenho de fundo
            lc.simboloFimLinha.posicaoXY = new Point(lc.posX, lc.posY);
            lc.simboloFimLinha.tamanhoXY = new Size(_acumTamX, _tamY2DesenhoFundo);

            RedimensionaSimbolos(lc);

        }
        private static void RedimensionaSimbolos(VisualLine lc)
        {

            //--Inicio da linha
            int iTabStop = 0;
            lc.simboloInicioLinha.TabIndex = iTabStop;
            lc.simboloInicioLinha.TabStop = false;

            foreach (FreeUserControl _simbAux in lc.simbolos)
            {
                iTabStop++;
                _simbAux.TabIndex = iTabStop;
                _simbAux.TabStop = true;

                _simbAux.Size = new Size(Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * lc.percReducaoSimbolo), _simbAux.tamanhoXY.Height);

                _simbAux.Location = new Point(_simbAux.posicaoXY.X + (_simbAux.tamanhoXY.Width - (Convert.ToInt32(Convert.ToDouble(_simbAux.tamanhoXY.Width) * lc.percReducaoSimbolo))) / 2, _simbAux.posicaoXY.Y);
            }

            //--Fim da linha
            iTabStop = 0;
            lc.simboloFimLinha.TabIndex = iTabStop;
            lc.simboloFimLinha.TabStop = false;

            //--Desenho de fundo
            iTabStop = 0;
            lc.simboloDesenhoFundo.TabIndex = iTabStop;
            lc.simboloDesenhoFundo.TabStop = false;
        }

        //public static void CopiaSimbolo2Livre(Line linhaFonte, VisualLine linhaDestinho)
        //{
        //    /// transfere os simbolos basicos da linha
        //    linhaDestinho.simbolos.Clear();
        //    foreach(Instruction auxSB in linhaFonte.simbolos)
        //    {
        //        linhaDestinho.simbolos.Add(new FreeUserControl());
        //        linhaDestinho.simbolos[linhaDestinho.simbolos.Count - 1].OpCode = auxSB.OpCode;
        //        linhaDestinho.simbolos[linhaDestinho.simbolos.Count - 1].Operands = auxSB.Operands;

        //    }

        //    /// transfere os simbolos basicos de saida
        //    linhaDestinho.saida.Clear();
        //    foreach (Instruction auxSB in linhaFonte.saida)
        //    {
        //        linhaDestinho.saida.Add(new FreeUserControl());
        //        linhaDestinho.saida[linhaDestinho.saida.Count - 1].OpCode = auxSB.OpCode;
        //        for(int i = 0; i < auxSB.Operands.Length; i++)
        //            linhaDestinho.saida[linhaDestinho.saida.Count - 1].SetOperand(i, auxSB.GetOperand(i));

        //    }
        //}

        //public static void CopiaLivre2Simbolo(VisualLine linhaFonte, Line linhaDestinho)
        //{
        //    /// transfere os simbolos basicos da linha
        //    linhaDestinho.simbolos.Clear();
        //    foreach (FreeUserControl auxSB in linhaFonte.simbolos)
        //    {
        //        linhaDestinho.simbolos.Add(new Instruction());
        //        linhaDestinho.simbolos[linhaDestinho.simbolos.Count - 1].OpCode = auxSB.OpCode;
        //        linhaDestinho.simbolos[linhaDestinho.simbolos.Count - 1].Operands = auxSB.Operands;

        //    }

        //    /// transfere os simbolos basicos de saida
        //    linhaDestinho.saida.Clear();
        //    foreach (FreeUserControl auxSB in linhaFonte.saida)
        //    {
        //        linhaDestinho.saida.Add(new Instruction());
        //        linhaDestinho.saida[linhaDestinho.saida.Count - 1].OpCode = auxSB.OpCode;
        //        for (int i = 0; i < auxSB.Operands.Length; i++)
        //            linhaDestinho.saida[linhaDestinho.saida.Count - 1].SetOperand(i, auxSB.GetOperand(i));

        //    }
        //}
    
    }
}
