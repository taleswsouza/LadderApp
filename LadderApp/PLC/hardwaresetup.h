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
void SetupSistema(void);
void SetupIO(void);
void SetupTemporizador(void);
void SetupContador(void);
void ExecContador(TContador *Contador);
void LeEntradas(void);
void ExecPrograma(void);
void EscreveSaidas(void);

#endif
