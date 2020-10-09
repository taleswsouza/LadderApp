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
	CCR0 += RECHARGE_CCR0; // Add Offset to CCR0

	AccMs++;
	Sys.Event10ms = 1;
	if (AccMs >= 10)
	{
		AccMs = 0;
		Acc100Ms++;
		Sys.Event100ms = 1;
		if (Acc100Ms >= 10)
		{
			Acc100Ms = 0;
			AccSeg++;
			Sys.Event1s = 1;
			if (AccSeg >= 60)
			{
				AccSeg = 0;
				Sys.Event1m = 1;
			}
		}
	}
}
