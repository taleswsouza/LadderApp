	case 1: // descending counter
		if (Counter->Reset == 1)
		{
			Counter->Accumulated = Counter->Preset;
		}
		if (Counter->Enable == 1 && Counter->Pulse == 1)
		{
			Counter->Pulse = 0;
			if (Counter->Accumulated > 0)
			{
				Counter->Accumulated--;
				if (Counter->Accumulated == 0)
					Counter->Done = 1;

				else
					Counter->Done = 0;
			}
		}
		break;
