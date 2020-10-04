using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class VisualParallelBranch
    {
        public VisualInstructionUserControl parallelBranchBegin = null;
        public VisualInstructionUserControl lastParallelBranchNext = null;
        public List<VisualInstructionUserControl> parallelBranchList = new List<VisualInstructionUserControl>();

        public int biggerY = 0;
        public int biggerX = 0;
        public int numVPITratados = 0;
        public int accumulatedY = 0;
        //public int _paralelosTratados = 0;
        //public int _VPI_Tratados = 0;
    }
}
