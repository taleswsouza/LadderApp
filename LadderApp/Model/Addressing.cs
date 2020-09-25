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
        public List<Address> ListMemoryAddress = new List<Address>();
        public List<Address> ListTimerAddress = new List<Address>();
        public List<Address> ListCounterAddress = new List<Address>();
        public List<Address> ListInputAddress = new List<Address>();
        public List<Address> ListOutputAddress = new List<Address>();

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
            return Find(_end.AddressType, _end.Id);
        }

        public Address Find(AddressTypeEnum tpEnderecamento, Int32 indice)
        {
            List<Address> _lstGenerica;
            switch (tpEnderecamento)
            {
                case AddressTypeEnum.DigitalMemory:
                    _lstGenerica = ListMemoryAddress;
                    break;
                case AddressTypeEnum.DigitalMemoryCounter:
                    _lstGenerica = ListCounterAddress;
                    break;
                case AddressTypeEnum.DigitalMemoryTimer:
                    _lstGenerica = ListTimerAddress;
                    break;
                case AddressTypeEnum.DigitalInput:
                    _lstGenerica = ListInputAddress;
                    break;
                case AddressTypeEnum.DigitalOutput:
                    _lstGenerica = ListOutputAddress;
                    break;
                default:
                    return null;
            }

            foreach (Address _endCada in _lstGenerica)
            {
                if (_endCada.AddressType == tpEnderecamento &&
                    _endCada.Id == indice)
                {
                    return _endCada;
                }
            }

            return null;
        }


        public List<Address> ListaNomes(OperationCode _ci)
        {
            List<Address> _listaEnderecos = new List<Address>();
            List<AddressTypeEnum> tiposListados = new List<AddressTypeEnum>();
            List<Address> _lstGenerica;

            switch (_ci)
            {
                case OperationCode.NormallyOpenContact:
                case OperationCode.NormallyClosedContact:
                    tiposListados.Add(AddressTypeEnum.DigitalInput);
                    tiposListados.Add(AddressTypeEnum.DigitalMemory);
                    tiposListados.Add(AddressTypeEnum.DigitalMemoryCounter);
                    tiposListados.Add(AddressTypeEnum.DigitalMemoryTimer);
                    break;
                case OperationCode.Timer:
                    tiposListados.Add(AddressTypeEnum.DigitalMemoryTimer);
                    break;
                case OperationCode.Counter:
                    tiposListados.Add(AddressTypeEnum.DigitalMemoryCounter);
                    break;
                case OperationCode.OutputCoil:
                    tiposListados.Add(AddressTypeEnum.DigitalMemory);
                    tiposListados.Add(AddressTypeEnum.DigitalOutput);
                    break;
            }

            foreach (AddressTypeEnum _cadaTipo in tiposListados)
            {
                switch (_cadaTipo)
                {
                    case AddressTypeEnum.DigitalMemory:
                        _lstGenerica = ListMemoryAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryCounter:
                        _lstGenerica = ListCounterAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryTimer:
                        _lstGenerica = ListTimerAddress;
                        break;
                    case AddressTypeEnum.DigitalInput:
                        _lstGenerica = ListInputAddress;
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        _lstGenerica = ListOutputAddress;
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
            List<AddressTypeEnum> _lstTpEnd = new List<AddressTypeEnum>();

            _lstTpEnd.Add(AddressTypeEnum.DigitalMemory);
            _lstTpEnd.Add(AddressTypeEnum.DigitalMemoryCounter);
            _lstTpEnd.Add(AddressTypeEnum.DigitalMemoryTimer);
            _lstTpEnd.Add(AddressTypeEnum.DigitalInput);
            _lstTpEnd.Add(AddressTypeEnum.DigitalOutput);

            foreach (AddressTypeEnum _tp in _lstTpEnd)
            {

                switch (_tp)
                {
                    case AddressTypeEnum.DigitalMemory:
                        _lstGenerica = ListMemoryAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryCounter:
                        _lstGenerica = ListCounterAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryTimer:
                        _lstGenerica = ListTimerAddress;
                        break;
                    case AddressTypeEnum.DigitalInput:
                        _lstGenerica = ListInputAddress;
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        _lstGenerica = ListOutputAddress;
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
            List<AddressTypeEnum> _lstTpEnd = new List<AddressTypeEnum>();
            List<Address> _lstResult = new List<Address>();

            _lstTpEnd.Add(AddressTypeEnum.DigitalMemory);
            _lstTpEnd.Add(AddressTypeEnum.DigitalMemoryCounter);
            _lstTpEnd.Add(AddressTypeEnum.DigitalMemoryTimer);
            _lstTpEnd.Add(AddressTypeEnum.DigitalInput);
            _lstTpEnd.Add(AddressTypeEnum.DigitalOutput);

            foreach (AddressTypeEnum _tp in _lstTpEnd)
            {

                switch (_tp)
                {
                    case AddressTypeEnum.DigitalMemory:
                        _lstGenerica = ListMemoryAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryCounter:
                        _lstGenerica = ListCounterAddress;
                        break;
                    case AddressTypeEnum.DigitalMemoryTimer:
                        _lstGenerica = ListTimerAddress;
                        break;
                    case AddressTypeEnum.DigitalInput:
                        _lstGenerica = ListInputAddress;
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        _lstGenerica = ListOutputAddress;
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
            this.ListInputAddress.Clear();
            this.ListOutputAddress.Clear();
            foreach (Address el in dispositivo.lstEndBitPorta)
            {
                el.ApontaDispositivo(dispositivo);
                switch (el.AddressType)
                {
                    case AddressTypeEnum.DigitalInput:
                        this.ListInputAddress.Add(el);
                        break;
                    case AddressTypeEnum.DigitalOutput:
                        this.ListOutputAddress.Add(el);
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
        public int AlocaEnderecamentoMemoria(Device dispositivo, List<Address> _lstE, AddressTypeEnum tp, int qtdEnd)
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
