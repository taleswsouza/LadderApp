using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public class Timer
    {
        public int Type = 0; // 0 = Contador Crescente, 1 = Contador Decrescente,...
        public int TimeBase; // 0 = "10 ms", 1 = "100 ms", 2 = "1 s", 3 = "1 m"

        public int ParcialPreset
        {
            get
            {
                int parcialPreset = 0;

                switch (this.TimeBase)
                {
                    case 1: /// 100 ms
                        parcialPreset = 1;
                        break;
                    case 2: /// 1 segundo
                        parcialPreset = 1 * 10;
                        break;
                    case 3: /// 1 minuto
                        parcialPreset = 1 * 10 * 60;
                        break;
                    default:
                        parcialPreset = 0;
                        break;
                }
                return parcialPreset;
            }
        }

        public int Preset; // valor de preset
        public int Accumulated = 0; //Valor acumulado
        public int ParcialAccumulated = 0; /// valor acumulado parcial para suporte ao simulado no PC
        public bool Enable = false; // Habilitado
        public bool Pulse = true; // Pulso (Auxiliar)
        public bool Done = false; // Contagem realizada (done)
        public bool Reset = false; // reset do contador
    }
}
