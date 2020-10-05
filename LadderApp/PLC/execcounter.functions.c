//====================================================================================
// Rotina de execução do contador
//====================================================================================
void ExecContador(TContador *Contador)
{
	switch (Contador->Tipo)
	{
#EXEC_COUNTER_TYPE_0_FUNCTION_C#
#EXEC_COUNTER_TYPE_1_FUNCTION_C#
	default:
		break;
	}
	if (Contador->EN == 0)
		Contador->Pulso = 1;
}
