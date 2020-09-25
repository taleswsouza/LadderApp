using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class VisualProgram
    {
        private LadderProgram program;
        private LadderForm ladderForm;

        public VisualProgram(LadderProgram program, LadderForm ladderForm)
        {
            this.program = program;
            this.ladderForm = ladderForm;

            if (this.program.Lines.Count > 0)
            {
                foreach (Line line in this.program.Lines)
                {
                    VisualLine visualLine = CreateVisualLine(line);
                    InsereLinhaNoFinal(visualLine);
                }
            }
        }
        
        private List<VisualLine> linhasPrograma = new List<VisualLine>();
        public IList<VisualLine> linhas
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
        public int InsereLinhaNoFinal(VisualLine _lc)
        {
            linhasPrograma.Add(_lc);
            return (linhasPrograma.Count - 1);
        }

        /// <summary>
        /// Insere uma linha no programa na primeira linha (antes de todas)
        /// </summary>
        /// <param name="_lc">nova linha a ser inserida</param>
        /// <returns></returns>
        public int InsereLinhaNoInicio(VisualLine _lc)
        {

            return InsereLinhaNoIndice(0, _lc);
        }

        public int InsereLinhaNoIndice(int linha, VisualLine _lc)
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
            linha = program.InsereLinhaNoIndice(linha, new Line());

            VisualLine _lc = CreateVisualLine(program.Lines[linha]);

            return InsereLinhaNoIndice(linha, _lc);
        }


        public int InsereLinhaNoFinal()
        {
            int linha = program.InsereLinhaNoFinal(new Line());

            VisualLine _lc = CreateVisualLine(program.Lines[linha]);

            return InsereLinhaNoFinal(_lc);
        }


        public void ApagaLinha(int linha)
        {
            linhasPrograma[linha].ApagaLinha();
            linhasPrograma.RemoveAt(linha);

            program.ApagaLinha(linha);
        }

        /// <summary>
        /// Insere linha abaixo ou acima da linha selecionada
        /// </summary>
        /// <param name="_acima">true - acima / false - abaixo</param>
        public VisualLine CreateVisualLine(Line line)
        {
            VisualLine visualLine = new VisualLine(ladderForm, line);
            visualLine.LineBegin.MudaLinha += new MudaLinhaEventHandler(ladderForm.simboloInicioLinha_MudaLinha);

            return visualLine;
        }
    }
}
