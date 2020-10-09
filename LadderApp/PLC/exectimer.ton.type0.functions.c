	case 0:// TON ====================================================================

		if ((Timer->Enable) && (!Timer->Reset))
		{
			//-------------------------------------------------------
			if (System.Event10ms == 1)
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
				}
				if (EventPresent == 1)
				{
					(Timer->Accumulated)++;
					(Timer->Parcial) = 0;
				}
			}
			//-------------------------------------------------------
			if (Timer->Accumulated >= Timer->Preset)
			{
				Timer->Accumulated = Timer->Preset;
				Timer->Done = 1;
			}
		}
		else
		{
			Timer->Done = 0;
			Timer->Accumulated = 0;
			Timer->Parcial = 0;
		}
		break;