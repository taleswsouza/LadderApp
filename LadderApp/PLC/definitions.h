//====================================================================================
// Arquivo de definição geral
// Autor: Silvano Fonseca Paganoto
// Data: 15/09/2010
//====================================================================================


#ifndef DEFINITIONS_H
#define DEFINITIONS_H

#define RECHARGE_CCR0 10000
//====================================================================================
// Includes
//====================================================================================

#include <msp430.h> // #include <io.h>
#include <iomacros.h>
#include <legacymsp430.h> // #include <signal.h>

//====================================================================================
// type for ports
//====================================================================================
typedef union
{
	unsigned char Byte;
	struct
	{
		unsigned char Bit0 :1;
		unsigned char Bit1 :1;
		unsigned char Bit2 :1;
		unsigned char Bit3 :1;
		unsigned char Bit4 :1;
		unsigned char Bit5 :1;
		unsigned char Bit6 :1;
		unsigned char Bit7 :1;
	};
} TPort;

//====================================================================================
// type for system
//====================================================================================
typedef union
{
	unsigned char Byte;
	struct
	{
		unsigned char Event10ms :1;
		unsigned char Event100ms :1;
		unsigned char Event1s :1;
		unsigned char Event1m :1;
		unsigned char Bit4 :1;
		unsigned char Bit5 :1;
		unsigned char Bit6 :1;
		unsigned char Bit7 :1;
	};
} TSystem;

//====================================================================================
// type for timers
//====================================================================================
typedef struct
{
	unsigned int Preset;
	unsigned int Accumulated;
	unsigned int Parcial;
	union
	{
		unsigned char Config;
		struct
		{
			unsigned char Type :2; // 0 = TON, 1 = TOFF,...
			unsigned char TimeBase :2; // TimeBase de tempo: 0 = milisegundos, 1 = 100milisegundos, 2 = segundos, 3 = minutos
			unsigned char Enable :1; // Habilitado
			unsigned char Done :1; // Temporização realizada (done)
			unsigned char Reset :1; // reset do contador
			unsigned char Bit7 :1; // reserva
		};
	};
} TTimer;

//====================================================================================
// type for counters
//====================================================================================
typedef struct
{
	unsigned char Preset; // valor de preset
	unsigned char Accumulated; //Valor acumulado
	union
	{
		unsigned char Config;
		struct
		{
			unsigned char Type :2; // 0 = Counter Crescente, 1 = Counter Decrescente,...
			unsigned char Enable :1; // Habilitado
			unsigned char Pulse :1; // Pulse (Auxiliar)
			unsigned char Done :1; // Contagem realizada (done)
			unsigned char Reset :1; // reset do contador
			unsigned char Bit6 :1; // reserva
			unsigned char Bit7 :1; // reserva
		};
	};
} TCounter;

#endif // DEFINITIONS_H
