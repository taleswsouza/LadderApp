//====================================================================================
// Arquivo principal
// Autor: Silvano Fonseca Paganoto
// Data: 15/09/2010
//====================================================================================

//====================================================================================
// Includes
//====================================================================================
#include "definitions.h"
#include "hardwaresetup.h"
#include "ladderprogram.h"

#LADDER_INSTRUCTIONS#

//====================================================================================
// Rotina Principal
//====================================================================================
int main(void)
{
	StartupParameterization();
	SystemSetup();
	IOSetup();
	while (1)
	{
		ReadInputs();
		#EXEC_TIMERS_CALL#
		ExecuteLadderProgram();
		WriteOutputs();
	}
}

