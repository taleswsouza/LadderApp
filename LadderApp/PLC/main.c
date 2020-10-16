//====================================================================================
// Main file
// Author: Silvano Fonseca Paganoto
// Date: 15/09/2010
//====================================================================================

//====================================================================================
// Includes
//====================================================================================
#include "definitions.h"
#include "hardwaresetup.h"
#include "ladderprogram.h"

#LADDER_INSTRUCTIONS#

//====================================================================================
// Main routine
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

