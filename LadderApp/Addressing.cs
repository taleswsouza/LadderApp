using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LadderApp
{
    [XmlInclude(typeof(Address))]
    [Serializable]
    public class Addressing
    {
        public List<Address> lstMemoria = new List<Address>();
        public List<Address> lstTemporizador = new List<Address>();
        public List<Address> lstContador = new List<Address>();
        public List<Address> lstIOEntrada = new List<Address>();
        public List<Address> lstIOSaida = new List<Address>();

        public Addressing()
        {
        }

        /// <summary>
        /// Procura o endereco de parametro nas listas do objeto
        /// da classe EnderecamentoPrograma 
        /// </summary>
        /// <param name="_end">Endereco a ser buscado na lista de enderecos da classe</param>
        /// <returns>Caso encontre o endereco em uma das listas retorna o
        /// endereco das listas encontrado. caso contrario retorna null</returns>
        public Address Find(Address _end)
        {
            return Find(_end.TpEnderecamento, _end.Indice);
        }

        public Address Find(AddressType tpEnderecamento, Int32 indice)
        {
            List<Address> _lstGenerica;
            switch (tpEnderecamento)
            {
                case AddressType.DIGITAL_MEMORIA:
                    _lstGenerica = lstMemoria;
                    break;
                case AddressType.DIGITAL_MEMORIA_CONTADOR:
                    _lstGenerica = lstContador;
                    break;
                case AddressType.DIGITAL_MEMORIA_TEMPORIZADOR:
                    _lstGenerica = lstTemporizador;
                    break;
                case AddressType.DIGITAL_ENTRADA:
                    _lstGenerica = lstIOEntrada;
                    break;
                case AddressType.DIGITAL_SAIDA:
                    _lstGenerica = lstIOSaida;
                    break;
                default:
                    return null;
            }

            foreach (Address _endCada in _lstGenerica)
            {
                if (_endCada.TpEnderecamento == tpEnderecamento &&
                    _endCada.Indice == indice)
                {
                    return _endCada;
                }
            }

            return null;
        }


        public List<Address> ListaNomes(CodigosInterpretaveis _ci)
        {
            List<Address> _listaEnderecos = new List<Address>();
            List<AddressType> tiposListados = new List<AddressType>();
            List<Address> _lstGenerica;

            switch(_ci)
            {
                case CodigosInterpretaveis.CONTATO_NA:
                case CodigosInterpretaveis.CONTATO_NF:
                    tiposListados.Add(AddressType.DIGITAL_ENTRADA);
                    tiposListados.Add(AddressType.DIGITAL_MEMORIA);
                    tiposListados.Add(AddressType.DIGITAL_MEMORIA_CONTADOR);
                    tiposListados.Add(AddressType.DIGITAL_MEMORIA_TEMPORIZADOR);
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    tiposListados.Add(AddressType.DIGITAL_MEMORIA_TEMPORIZADOR);
                    break;
                case CodigosInterpretaveis.CONTADOR:
                    tiposListados.Add(AddressType.DIGITAL_MEMORIA_CONTADOR);
                    break;
                case CodigosInterpretaveis.BOBINA_SAIDA:
                    tiposListados.Add(AddressType.DIGITAL_MEMORIA);
                    tiposListados.Add(AddressType.DIGITAL_SAIDA);
                    break;
            }

            foreach (AddressType _cadaTipo in tiposListados)
            {
                switch (_cadaTipo)
                {
                    case AddressType.DIGITAL_MEMORIA:
                        _lstGenerica = lstMemoria;
                        break;
                    case AddressType.DIGITAL_MEMORIA_CONTADOR:
                        _lstGenerica = lstContador;
                        break;
                    case AddressType.DIGITAL_MEMORIA_TEMPORIZADOR:
                        _lstGenerica = lstTemporizador;
                        break;
                    case AddressType.DIGITAL_ENTRADA:
                        _lstGenerica = lstIOEntrada;
                        break;
                    case AddressType.DIGITAL_SAIDA:
                        _lstGenerica = lstIOSaida;
                        break;
                    default:
                        return null;
                }

                foreach (Address _endCada in _lstGenerica)
                    _listaEnderecos.Add(_endCada);
            }

            return _listaEnderecos;
        }

        public void LimpaIndicacaoEmUso()
        {
            List<Address> _lstGenerica;
            List<AddressType> _lstTpEnd = new List<AddressType>();

            _lstTpEnd.Add(AddressType.DIGITAL_MEMORIA);
            _lstTpEnd.Add(AddressType.DIGITAL_MEMORIA_CONTADOR);
            _lstTpEnd.Add(AddressType.DIGITAL_MEMORIA_TEMPORIZADOR);
            _lstTpEnd.Add(AddressType.DIGITAL_ENTRADA);
            _lstTpEnd.Add(AddressType.DIGITAL_SAIDA);

            foreach (AddressType _tp in _lstTpEnd)
            {

                switch (_tp)
                {
                    case AddressType.DIGITAL_MEMORIA:
                        _lstGenerica = lstMemoria;
                        break;
                    case AddressType.DIGITAL_MEMORIA_CONTADOR:
                        _lstGenerica = lstContador;
                        break;
                    case AddressType.DIGITAL_MEMORIA_TEMPORIZADOR:
                        _lstGenerica = lstTemporizador;
                        break;
                    case AddressType.DIGITAL_ENTRADA:
                        _lstGenerica = lstIOEntrada;
                        break;
                    case AddressType.DIGITAL_SAIDA:
                        _lstGenerica = lstIOSaida;
                        break;
                    default:
                        return;
                }

                foreach (Address _endCada in _lstGenerica)
                    _endCada.EmUso = false;
            }
        }

        public List<Address> ListaEnderecamentosEmUso()
        {
            List<Address> _lstGenerica;
            List<AddressType> _lstTpEnd = new List<AddressType>();
            List<Address> _lstResult = new List<Address>();

            _lstTpEnd.Add(AddressType.DIGITAL_MEMORIA);
            _lstTpEnd.Add(AddressType.DIGITAL_MEMORIA_CONTADOR);
            _lstTpEnd.Add(AddressType.DIGITAL_MEMORIA_TEMPORIZADOR);
            _lstTpEnd.Add(AddressType.DIGITAL_ENTRADA);
            _lstTpEnd.Add(AddressType.DIGITAL_SAIDA);

            foreach (AddressType _tp in _lstTpEnd)
            {

                switch (_tp)
                {
                    case AddressType.DIGITAL_MEMORIA:
                        _lstGenerica = lstMemoria;
                        break;
                    case AddressType.DIGITAL_MEMORIA_CONTADOR:
                        _lstGenerica = lstContador;
                        break;
                    case AddressType.DIGITAL_MEMORIA_TEMPORIZADOR:
                        _lstGenerica = lstTemporizador;
                        break;
                    case AddressType.DIGITAL_ENTRADA:
                        _lstGenerica = lstIOEntrada;
                        break;
                    case AddressType.DIGITAL_SAIDA:
                        _lstGenerica = lstIOSaida;
                        break;
                    default:
                        return _lstResult;
                }

                foreach (Address _endCada in _lstGenerica)
                    if (_endCada.EmUso == true)
                        _lstResult.Add(_endCada);
            }

            return _lstResult;
        }

        public void AlocaEnderecamentoIO(Device dispositivo)
        {
            /// Atalho para o No de enderecamento
            this.lstIOEntrada.Clear();
            this.lstIOSaida.Clear();
            foreach (Address el in dispositivo.lstEndBitPorta)
            {
                el.ApontaDispositivo(dispositivo);
                switch (el.TpEnderecamento)
                {
                    case AddressType.DIGITAL_ENTRADA:
                        this.lstIOEntrada.Add(el);
                        break;
                    case AddressType.DIGITAL_SAIDA:
                        this.lstIOSaida.Add(el);
                        break;
                }
            }

        }

        /// <summary>
        /// Aloca e realoca na No objeto de enderecamento do programa atual
        /// uma quantidade especificada do tipo de memoria solicitado
        /// </summary>
        /// <param name="e">Enderecamento do programa</param>
        /// <param name="tp">tipo de memoria a ser realocada</param>
        /// <param name="qtdEnd">Quantidade do tipo desejada</param>
        public int AlocaEnderecamentoMemoria(Device dispositivo, List<Address> _lstE, AddressType tp, int qtdEnd)
        {
            int _qtdAtual = 1;

            _qtdAtual = _lstE.Count;
            if ((_qtdAtual == 0) || (_qtdAtual < qtdEnd))
            {
                for (int i = _qtdAtual + 1; i <= qtdEnd; i++)
                    _lstE.Add(new Address(tp, i, dispositivo));
            }
            else if (_qtdAtual > qtdEnd)
            {
                for (int i = (_qtdAtual - 1); i >= qtdEnd; i--)
                {
                    if (!_lstE[i].EmUso)
                    {
                        _lstE[i] = null;
                        GC.Collect();
                        _lstE.RemoveAt(i);
                    }
                    else
                        break;
                }
            }

            return 0;
        }
    }
}
