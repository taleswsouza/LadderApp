//====================================================================================
// Arquivo de interrupções do sistema
// Autor: Silvano Fonseca Paganoto
// Data: 15/09/2010
//====================================================================================/*

//====================================================================================
// Includes
//====================================================================================
#include "ladderprogram.h"

unsigned char AccMs = 0, Acc100Ms = 0, AccSeg = 0;

//====================================================================================
// Rotina de tratamento do TIMER A0
//====================================================================================
interrupt(TIMERA0_VECTOR) isr()
{
	CCR0 += RECARGA_CCR0; // Add Offset to CCR0

	AccMs++;
	Sys.Evento10ms = 1;
	if (AccMs >= 10)
	{
		AccMs = 0;
		Acc100Ms++;
		Sys.Evento100ms = 1;
		if (Acc100Ms >= 10)
		{
			Acc100Ms = 0;
			AccSeg++;
			Sys.Evento1s = 1;
			if (AccSeg >= 60)
			{
				AccSeg = 0;
				Sys.Evento1m = 1;
			}
		}
	}
}
