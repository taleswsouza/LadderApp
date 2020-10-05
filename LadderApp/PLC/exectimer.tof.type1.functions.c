	case 1: // TOFF
		if (Temporizador->EN)
		{
			Temporizador->Acumulado = Temporizador->Preset;
			Temporizador->DN = 1;
		}
		else
		{
			if (Sistema.Evento10ms)
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
				default:
					(Temporizador->Parcial) = 0;
					break;
				}

				if (EventoPresente == 1)
				{
					(Temporizador->Parcial) = 0;
					if (Temporizador->Acumulado > 0)
						(Temporizador->Acumulado)--;
				}
			}
		}

		if ((Temporizador->Acumulado == 0) || (Temporizador->Reset))
		{
			Temporizador->DN = 0;
		}
