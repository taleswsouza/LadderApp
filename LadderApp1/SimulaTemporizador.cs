using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp1
{
    public class SimulaTemporizador
    {
        //====================================================================================
        // Tipo definido para Temporizador
        //====================================================================================
        public Int32 Tipo = 0; // 0 = Contador Crescente, 1 = Contador Decrescente,...
        public Int32 BaseTempo; // 0 = "10 ms", 1 = "100 ms", 2 = "1 s", 3 = "1 m"

        public Int32 PresetParcial
        {
            get
            {
                Int32 _intPresetParcial = 0;

                switch (this.BaseTempo)
                {
                    case 1: /// 100 ms
                        _intPresetParcial = 1;
                        break;
                    case 2: /// 1 segundo
                        _intPresetParcial = 1 * 10;
                        break;
                    case 3: /// 1 minuto
                        _intPresetParcial = 1 * 10 * 60;
                        break;
                    default:
                        _intPresetParcial = 0;
                        break;
                }
                return _intPresetParcial;
            }
        }

        public Int32 Preset; // valor de preset
        public Int32 Acumulado = 0; //Valor acumulado
        public Int32 AcumuladoParcial = 0; /// valor acumulado parcial para suporte ao simulado no PC
        public Boolean EN = false; // Habilitado
        public Boolean Pulso = true; // Pulso (Auxiliar)
        public Boolean DN = false; // Contagem realizada (done)
        public Boolean Reset = false; // reset do contador
    }
}
