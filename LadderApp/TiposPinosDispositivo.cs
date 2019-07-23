using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp
{
    public enum TiposPinosDispositivo
    {
        NENHUM = 0,
        IO_DIGITAL_ENTRADA = 1,
        IO_DIGITAL_SAIDA = 2,
        IO_DIGITAL_ENTRADA_OU_SAIDA = 3,
        IO_ANALOGICO_ENTRADA = 4,
        IO_ANALOGICO_SAIDA = 5,
        IO_ANALOGICO_ENTRADA_OU_SAIDA = 6,
        IO_ANALOGICO_PT100 = 7,
        COMUNICACAO_SERIAL = 8,
        COMUNICACAO_USB = 9
    }
}
