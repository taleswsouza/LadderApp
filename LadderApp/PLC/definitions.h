//====================================================================================
// General definitions
// Author: Silvano Fonseca Paganoto
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
			unsigned char Type :2;		// 0 = TON, 1 = TOF,...
			unsigned char TimeBase :2;	// 0 = 10ms, 1 = 100ms, 2 = 1s, 3 = 1m
			unsigned char Enable :1;
			unsigned char Done :1;
			unsigned char Reset :1;
			unsigned char Bit7 :1;		// spare
		};
	};
} TTimer;

//====================================================================================
// type for counters
//====================================================================================
typedef struct
{
	unsigned char Preset;
	unsigned char Accumulated;
	union
	{
		unsigned char Config;
		struct
		{
			unsigned char Type :2;	// 0 = ascending counter, 1 = descending counter
			unsigned char Enable :1;
			unsigned char Pulse :1;
			unsigned char Done :1;
			unsigned char Reset :1;
			unsigned char Bit6 :1;	// spare
			unsigned char Bit7 :1;	// spare
		};
	};
} TCounter;

#endif // DEFINITIONS_H
