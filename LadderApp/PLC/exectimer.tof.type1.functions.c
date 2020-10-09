	case 1: // TOF
		if (Timer->Enable)
		{
			Timer->Accumulated = Timer->Preset;
			Timer->Done = 1;
		}
		else
		{
			if (System.Event10ms)
			{
				(Timer->Parcial)++;
				switch (Timer->TimeBase)
				{
				case 0:
					EventPresent = 1;
					break;
				case 1:
					if ((Timer->Parcial) >= 10)
						EventPresent = 1;
					break;
				case 2:
					if ((Timer->Parcial) >= 100)
						EventPresent = 1;
					break;
				case 3:
					if ((Timer->Parcial) >= 6000)
						EventPresent = 1;
					break;
				default:
					(Timer->Parcial) = 0;
					break;
				}

				if (EventPresent == 1)
				{
					(Timer->Parcial) = 0;
					if (Timer->Accumulated > 0)
						(Timer->Accumulated)--;
				}
			}
		}

		if ((Timer->Accumulated == 0) || (Timer->Reset))
		{
			Timer->Done = 0;
		}
		break;
