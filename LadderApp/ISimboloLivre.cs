using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LadderApp
{
    interface ISimboloLivre
    {
        Size tamanhoXY
        {
            get;
            set;
        }

        Point posicaoXY
        {
            get;
            set;
        }
    }
}
