using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class Counter
    {
        //====================================================================================
        // Tipo definido para Contadores
        //====================================================================================
        public Int32 Tipo = 0; // 0 = Contador Crescente, 1 = Contador Decrescente,...
        public Int32 Preset; // valor de preset
        public Int32 Acumulado = 0; //Valor acumulado
        public Boolean EN = false; // Habilitado
        public Boolean Pulso = true; // Pulso (Auxiliar)
        public Boolean DN = false; // Contagem realizada (done)
        public Boolean Reset = false; // reset do contador
    }
}
