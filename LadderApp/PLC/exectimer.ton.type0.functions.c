	case 0:// TON ====================================================================

		if ((Temporizador->EN) && (!Temporizador->Reset))
		{
			//-------------------------------------------------------
			if (Sistema.Evento10ms == 1)
			{
				(Temporizador->Parcial)++;
				switch (Temporizador->Base)
				{
				case 0:
					EventoPresente = 1;
					break;
				case 1:
					if ((Temporizador->Parcial) >= 10)
						EventoPresente = 1;
					break;
				case 2:
					if ((Temporizador->Parcial) >= 100)
						EventoPresente = 1;
					break;
				case 3:
					if ((Temporizador->Parcial) >= 6000)
						EventoPresente = 1;
					break;
				}
				if (EventoPresente == 1)
				{
					(Temporizador->Acumulado)++;
					(Temporizador->Parcial) = 0;
				}
			}
			//-------------------------------------------------------
			if (Temporizador->Acumulado >= Temporizador->Preset)
			{
				Temporizador->Acumulado = Temporizador->Preset;
				Temporizador->DN = 1;
			}
		}
		else
		{
			Temporizador->DN = 0;
			Temporizador->Acumulado = 0;
			Temporizador->Parcial = 0;
		}
		break;