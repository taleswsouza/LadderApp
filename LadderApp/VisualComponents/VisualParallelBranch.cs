using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class VisualParallelBranch
    {
        public VisualInstructionUserControl par = null;
        public VisualInstructionUserControl ultimoVPI = null;
        public List<VisualInstructionUserControl> lstVPI = new List<VisualInstructionUserControl>();

        public int maiorY = 0;
        public int maiorX = 0;
        public int numVPITratados = 0;
        public int _yAcum = 0;
        public int _paralelosTratados = 0;
        public int _VPI_Tratados = 0;
    }
}
