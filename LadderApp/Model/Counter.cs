using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class Counter
    {
        public int Type = 0; // 0 = Contador Crescente, 1 = Contador Decrescente,...
        public int Preset; // valor de preset
        public int Accumulated = 0; //Valor acumulado
        public bool Enable = false; // Habilitado
        public bool Pulse = true; // Pulso (Auxiliar)
        public bool Done = false; // Contagem realizada (done)
        public bool Reset = false; // reset do contador
    }
}
