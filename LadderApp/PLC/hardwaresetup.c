//====================================================================================
// hardware configuration
// Author: Silvano Fonseca Paganoto
// Date: 15/09/2010
//====================================================================================

//====================================================================================
// includes
//====================================================================================
#include "hardwaresetup.h"

void SystemSetup(void)
{
	WDTCTL = WDTPW + WDTHOLD; // Stop WDT
	//------------------------------------------------------------------
	// TIMER A0 configuration
	CCTL0 = CCIE; // CCR0 interrupt enabled
	CCR0 = RECHARGE_CCR0;
	TACTL = TASSEL_2 + MC_2; // SMCLK, contmode
	//------------------------------------------------------------------
	// enable interruptions
	_BIS_SR(GIE);
	//------------------------------------------------------------------
}

//====================================================================================
// IO configuration
//====================================================================================
void IOSetup(void)
{
#IO_HARDWARE_SETUP_C#
}

void ReadInputs(void)
{
#READ_INPUTS#
	System.Byte = Sys.Byte;
	Sys.Byte = 0;
}

void WriteOutputs(void)
{
#WRITE_OUTPUTS#
}

