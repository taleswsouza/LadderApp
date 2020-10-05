	case 1: // Contador Decrescente
		if (Contador->Reset == 1)
		{
			Contador->Acumulado = Contador->Preset;
		}
		if (Contador->EN == 1 && Contador->Pulso == 1)
		{
			Contador->Pulso = 0;
			if (Contador->Acumulado > 0)
			{
				Contador->Acumulado--;
				if (Contador->Acumulado == 0)
					Contador->DN = 1;

				else
					Contador->DN = 0;
			}
		}
		break;
