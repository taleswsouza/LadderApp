void ExecuteCounter(TCounter *Counter)
{
	switch (Counter->Type)
	{
#EXEC_COUNTER_TYPE_0_FUNCTION_C#
#EXEC_COUNTER_TYPE_1_FUNCTION_C#
	default:
		break;
	}
	if (Counter->Enable == 0) 
	{
		Counter->Pulse = 1;
	}
}
