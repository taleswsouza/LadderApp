//====================================================================================
// functions prototypes for hardware configuration
// Author: Silvano Fonseca Paganoto
// Date: 15/09/2010
//====================================================================================

#ifndef SETUPHARDWARE_H_
#define SETUPHARDWARE_H_

//====================================================================================
// Includes
//====================================================================================
#include "definitions.h"
#include "addresses.h"

//====================================================================================
// prototypes
//====================================================================================
void SystemSetup(void);
void IOSetup(void);
//void TimerSetup(void);
//void SetupCounter(void);
void ExecuteCounter(TCounter *Counter);
void ReadInputs(void);
void ExecuteLadderProgram(void);
void WriteOutputs(void);

#endif
