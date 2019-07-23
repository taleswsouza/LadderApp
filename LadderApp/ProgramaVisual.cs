using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class ProgramaVisual
    {
        ProgramaBasico prgBasico = null;
        DiagramaLadder frmDiag = null;

        /// <summary>
        /// Construtor da classe do programa de linhas da visao (controlelivre)
        /// </summary>
        /// <param name="_prgBasico">link para o programa basico (simbolobasico)</param>
        public ProgramaVisual(ProgramaBasico _prgBasico, DiagramaLadder _frmDiag)
        {
            prgBasico = _prgBasico;
            frmDiag = _frmDiag;

            if (prgBasico.linhas.Count > 0)
            {
                foreach (LinhaCompleta _lc in prgBasico.linhas)
                {
                    LinhaCompletaVisual _lcL = PreparaLinhaQueSeraCriada(_lc);
                    InsereLinhaNoFinal(_lcL);
                }
            }
        }
        
        private List<LinhaCompletaVisual> linhasPrograma = new List<LinhaCompletaVisual>();
        public IList<LinhaCompletaVisual> linhas
        {
            get 
            {
                //IList<LinhaCompletaLivre> linhasprg = linhasPrograma.AsReadOnly();
                return linhasPrograma.AsReadOnly(); 
            }
        }

        /// <summary>
        /// Insere uma linha no programa no final das linhas
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns>indice da linha inserida</returns>
        public int InsereLinhaNoFinal(LinhaCompletaVisual _lc)
        {
            linhasPrograma.Add(_lc);
            return (linhasPrograma.Count - 1);
        }

        /// <summary>
        /// Insere uma linha no programa na primeira linha (antes de todas)
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns></returns>
        public int InsereLinhaNoInicio(LinhaCompletaVisual _lc)
        {

            return InsereLinhaNoIndice(0, _lc);
        }

        public int InsereLinhaNoIndice(int linha, LinhaCompletaVisual _lc)
        {
            if (linha > linhasPrograma.Count)
                linha = linhasPrograma.Count;

            if (linha < 0)
                linha = 0;

            linhasPrograma.Insert(linha, _lc);
            return linha;
        }

        public int InsereLinhaNoIndice(int linha)
        {
            linha = prgBasico.InsereLinhaNoIndice(linha, new LinhaCompleta());

            LinhaCompletaVisual _lc = PreparaLinhaQueSeraCriada(prgBasico.linhas[linha]);

            return InsereLinhaNoIndice(linha, _lc);
        }


        public int InsereLinhaNoFinal()
        {
            int linha = prgBasico.InsereLinhaNoFinal(new LinhaCompleta());

            LinhaCompletaVisual _lc = PreparaLinhaQueSeraCriada(prgBasico.linhas[linha]);

            return InsereLinhaNoFinal(_lc);
        }


        public void ApagaLinha(int linha)
        {
            linhasPrograma[linha].ApagaLinha();
            linhasPrograma.RemoveAt(linha);

            prgBasico.ApagaLinha(linha);
        }

        /// <summary>
        /// Insere linha abaixo ou acima da linha selecionada
        /// </summary>
        /// <param name="_acima">true - acima / false - abaixo</param>
        public LinhaCompletaVisual PreparaLinhaQueSeraCriada(LinhaCompleta _linhaBasica)
        {
            LinhaCompletaVisual _novaLinhaTela = new LinhaCompletaVisual(frmDiag, _linhaBasica);
            _novaLinhaTela.simboloInicioLinha.MudaLinha += new MudaLinhaEventHandler(frmDiag.simboloInicioLinha_MudaLinha);

            return _novaLinhaTela;
        }
    }
}
