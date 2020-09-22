using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class VisualParallelBranch
    {
        public ControleLivre par = null;
        public ControleLivre ultimoVPI = null;
        public List<ControleLivre> lstVPI = new List<ControleLivre>();

        public int maiorY = 0;
        public int maiorX = 0;
        public int numVPITratados = 0;
        public int _yAcum = 0;
        public int _paralelosTratados = 0;
        public int _VPI_Tratados = 0;
    }
}
