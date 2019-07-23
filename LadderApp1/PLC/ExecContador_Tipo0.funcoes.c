	case 0: // Contador Crescente
		if (Contador->Reset == 1)
		{
			Contador->DN = 0;
			Contador->Acumulado = 0;
			Contador->Reset = 0;
		}
		if (Contador->EN == 1 && Contador->Pulso == 1)
		{
			Contador->Pulso = 0;
			if (Contador->Acumulado <= 255)
			{
				Contador->Acumulado++;
				if (Contador->Acumulado >= Contador->Preset)
					Contador->DN = 1;
				else
					Contador->DN = 0;
			}
		}
		break;
