//====================================================================================
// Arquivo de protótipo das funções para configuração de hardware
// Autor: Silvano Fonseca Paganoto
// Data: 15/09/2010
//====================================================================================

#ifndef SETUPHARDWARE_H_
#define SETUPHARDWARE_H_

//====================================================================================
// Includes
//====================================================================================
#include "definitions.h"
#include "addresses.h"

//====================================================================================
// Prototipos
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
