	case 0: // Counter Crescente
		if (Counter->Reset == 1)
		{
			Counter->Done = 0;
			Counter->Accumulated = 0;
			Counter->Reset = 0;
		}
		if (Counter->Enable == 1 && Counter->Pulse == 1)
		{
			Counter->Pulse = 0;
			if (Counter->Accumulated <= 255)
			{
				Counter->Accumulated++;
				if (Counter->Accumulated >= Counter->Preset)
					Counter->Done = 1;
				else
					Counter->Done = 0;
			}
		}
		break;
