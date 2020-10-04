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
                    InsertLineAtEnd(visualLine);
                }
            }
        }
        
        private List<VisualLine> lines = new List<VisualLine>();
        public IList<VisualLine> Lines => lines.AsReadOnly();

        public int InsertLineAtEnd(VisualLine visualLine)
        {
            lines.Add(visualLine);
            return (lines.Count - 1);
        }

        public int InsertLineAtBegin(VisualLine visualLine)
        {
            return InsetLineAt(0, visualLine);
        }

        public int InsetLineAt(int index, VisualLine visualLine)
        {
            if (index > lines.Count)
                index = lines.Count;

            if (index < 0)
                index = 0;

            lines.Insert(index, visualLine);
            return index;
        }

        public int InsertLineAt(int index)
        {
            index = program.InsertLineAt(index, new Line());

            VisualLine _lc = CreateVisualLine(program.Lines[index]);

            return InsetLineAt(index, _lc);
        }


        public int InsereLinhaNoFinal()
        {
            int linha = program.InsertLineAtEnd(new Line());

            VisualLine _lc = CreateVisualLine(program.Lines[linha]);

            return InsertLineAtEnd(_lc);
        }


        public void DeleteLine(int linha)
        {
            lines[linha].DeleteLine();
            lines.RemoveAt(linha);

            program.RemoveLineAt(linha);
        }

        /// <summary>
        /// Insere linha abaixo ou acima da linha selecionada
        /// </summary>
        /// <param name="_acima">true - acima / false - abaixo</param>
        public VisualLine CreateVisualLine(Line line)
        {
            VisualLine visualLine = new VisualLine(ladderForm, line);
            visualLine.LineBegin.ChangeLineEvent += new ChangeLineEventHandler(ladderForm.LineBeginVisualInstruction_ChangeLine);

            return visualLine;
        }
    }
}
