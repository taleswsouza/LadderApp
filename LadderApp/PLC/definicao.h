//====================================================================================
// Arquivo de definição geral
// Autor: Silvano Fonseca Paganoto
// Data: 15/09/2010
//====================================================================================


#ifndef DEFINICAO_H_
#define DEFINICAO_H_
#define RECARGA_CCR0 10000
//====================================================================================
// Includes
//====================================================================================

#include <msp430.h> // #include <io.h>
#include <iomacros.h>
#include <legacymsp430.h> // #include <signal.h>

//====================================================================================
// Tipo definido para as portas
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
// Tipo definido para as portas
//====================================================================================
typedef union
{
	unsigned char Byte;
	struct
	{
		unsigned char Evento10ms :1; // tempo de 1 milisegundo
		unsigned char Evento100ms :1; // tempo de 100 milisegundo
		unsigned char Evento1s :1; // tempo de 1 segundo
		unsigned char Evento1m :1; // tempo de 1 minuto
		unsigned char Bit4 :1;
		unsigned char Bit5 :1;
		unsigned char Bit6 :1;
		unsigned char Bit7 :1;
	};
} TSistema;

//====================================================================================
// Tipo definido para Temporizadores
//====================================================================================
typedef struct
{
	unsigned int Preset; // valor de preset
	unsigned int Acumulado; //Valor acumulado
	unsigned int Parcial; //Valor parcial
	union
	{
		unsigned char Config;
		struct
		{
			unsigned char Tipo :2; // 0 = TON, 1 = TOFF,...
			unsigned char Base :2; // Base de tempo: 0 = milisegundos, 1 = 100milisegundos, 2 = segundos, 3 = minutos
			unsigned char EN :1; // Habilitado
			unsigned char DN :1; // Temporização realizada (done)
			unsigned char Reset :1; // reset do contador
			unsigned char Bit7 :1; // reserva
		};
	};
} TTemporizador;

//====================================================================================
// Tipo definido para Contadores
//====================================================================================
typedef struct
{
	unsigned char Preset; // valor de preset
	unsigned char Acumulado; //Valor acumulado
	union
	{
		unsigned char Config;
		struct
		{
			unsigned char Tipo :2; // 0 = Contador Crescente, 1 = Contador Decrescente,...
			unsigned char EN :1; // Habilitado
			unsigned char Pulso :1; // Pulso (Auxiliar)
			unsigned char DN :1; // Contagem realizada (done)
			unsigned char Reset :1; // reset do contador
			unsigned char Bit6 :1; // reserva
			unsigned char Bit7 :1; // reserva
		};
	};
} TContador;

#endif
