using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LadderApp1
{
    [XmlInclude(typeof(EnderecamentoLadder))]
    [Serializable]
    public class EnderecamentoPrograma
    {
        public List<EnderecamentoLadder> lstMemoria = new List<EnderecamentoLadder>();
        public List<EnderecamentoLadder> lstTemporizador = new List<EnderecamentoLadder>();
        public List<EnderecamentoLadder> lstContador = new List<EnderecamentoLadder>();
        public List<EnderecamentoLadder> lstIOEntrada = new List<EnderecamentoLadder>();
        public List<EnderecamentoLadder> lstIOSaida = new List<EnderecamentoLadder>();

        public EnderecamentoPrograma()
        {
        }

        /// <summary>
        /// Procura o endereco de parametro nas listas do objeto
        /// da classe EnderecamentoPrograma 
        /// </summary>
        /// <param name="_end">Endereco a ser buscado na lista de enderecos da classe</param>
        /// <returns>Caso encontre o endereco em uma das listas retorna o
        /// endereco das listas encontrado. caso contrario retorna null</returns>
        public EnderecamentoLadder Find(EnderecamentoLadder _end)
        {
            return Find(_end.TpEnderecamento, _end.Indice);
        }

        public EnderecamentoLadder Find(TipoEnderecamentoDispositivo tpEnderecamento, Int32 indice)
        {
            List<EnderecamentoLadder> _lstGenerica;
            switch (tpEnderecamento)
            {
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                    _lstGenerica = lstMemoria;
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                    _lstGenerica = lstContador;
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                    _lstGenerica = lstTemporizador;
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                    _lstGenerica = lstIOEntrada;
                    break;
                case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                    _lstGenerica = lstIOSaida;
                    break;
                default:
                    return null;
            }

            foreach (EnderecamentoLadder _endCada in _lstGenerica)
            {
                if (_endCada.TpEnderecamento == tpEnderecamento &&
                    _endCada.Indice == indice)// &&
                    //_endCada.Nome == _end.Nome)
                {
                    return _endCada;
                }
            }

            return null;
        }


        public List<EnderecamentoLadder> ListaNomes(CodigosInterpretaveis _ci)
        {
            List<EnderecamentoLadder> _listaEnderecos = new List<EnderecamentoLadder>();
            List<TipoEnderecamentoDispositivo> tiposListados = new List<TipoEnderecamentoDispositivo>();
            List<EnderecamentoLadder> _lstGenerica;

            switch(_ci)
            {
                case CodigosInterpretaveis.CONTATO_NA:
                case CodigosInterpretaveis.CONTATO_NF:
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_ENTRADA);
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA);
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR);
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR);
                    //tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_SAIDA);
                    break;
                case CodigosInterpretaveis.TEMPORIZADOR:
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR);
                    break;
                case CodigosInterpretaveis.CONTADOR:
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR);
                    break;
                case CodigosInterpretaveis.BOBINA_SAIDA:
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA);
                    tiposListados.Add(TipoEnderecamentoDispositivo.DIGITAL_SAIDA);
                    break;
            }

            foreach (TipoEnderecamentoDispositivo _cadaTipo in tiposListados)
            {
                switch (_cadaTipo)
                {
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                        _lstGenerica = lstMemoria;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                        _lstGenerica = lstContador;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                        _lstGenerica = lstTemporizador;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        _lstGenerica = lstIOEntrada;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        _lstGenerica = lstIOSaida;
                        break;
                    default:
                        return null;
                }

                foreach (EnderecamentoLadder _endCada in _lstGenerica)
                    _listaEnderecos.Add(_endCada);
            }

            return _listaEnderecos;
        }

        public void LimpaIndicacaoEmUso()
        {
            List<EnderecamentoLadder> _lstGenerica;
            List<TipoEnderecamentoDispositivo> _lstTpEnd = new List<TipoEnderecamentoDispositivo>();

            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_ENTRADA);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_SAIDA);

            foreach (TipoEnderecamentoDispositivo _tp in _lstTpEnd)
            {

                switch (_tp)
                {
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                        _lstGenerica = lstMemoria;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                        _lstGenerica = lstContador;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                        _lstGenerica = lstTemporizador;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        _lstGenerica = lstIOEntrada;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        _lstGenerica = lstIOSaida;
                        break;
                    default:
                        return;
                }

                foreach (EnderecamentoLadder _endCada in _lstGenerica)
                    _endCada.EmUso = false;
            }
        }

        public List<EnderecamentoLadder> ListaEnderecamentosEmUso()
        {
            List<EnderecamentoLadder> _lstGenerica;
            List<TipoEnderecamentoDispositivo> _lstTpEnd = new List<TipoEnderecamentoDispositivo>();
            List<EnderecamentoLadder> _lstResult = new List<EnderecamentoLadder>();

            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_ENTRADA);
            _lstTpEnd.Add(TipoEnderecamentoDispositivo.DIGITAL_SAIDA);

            foreach (TipoEnderecamentoDispositivo _tp in _lstTpEnd)
            {

                switch (_tp)
                {
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
                        _lstGenerica = lstMemoria;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
                        _lstGenerica = lstContador;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
                        _lstGenerica = lstTemporizador;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        _lstGenerica = lstIOEntrada;
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
                        _lstGenerica = lstIOSaida;
                        break;
                    default:
                        return _lstResult;
                }

                foreach (EnderecamentoLadder _endCada in _lstGenerica)
                    if (_endCada.EmUso == true)
                        _lstResult.Add(_endCada);
            }

            return _lstResult;
        }

        public void AlocaEnderecamentoIO(DispositivoLadder dispositivo)
        {
            /// Atalho para o No de enderecamento
            this.lstIOEntrada.Clear();
            this.lstIOSaida.Clear();
            foreach (EnderecamentoLadder el in dispositivo.lstEndBitPorta)
            {
                el.ApontaDispositivo(dispositivo);
                switch (el.TpEnderecamento)
                {
                    case TipoEnderecamentoDispositivo.DIGITAL_ENTRADA:
                        this.lstIOEntrada.Add(el);
                        break;
                    case TipoEnderecamentoDispositivo.DIGITAL_SAIDA:
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
        public int AlocaEnderecamentoMemoria(DispositivoLadder dispositivo, List<EnderecamentoLadder> _lstE, TipoEnderecamentoDispositivo tp, int qtdEnd)
        {
            /// Atalho para o No de enderecamento
            //TreeNode _NoEnderecamento = ArvoreProjeto.Nodes["NoProjeto"].Nodes["NoEnderecamento"];
            //String _txtNoEndereco = "";

            int _qtdAtual = 1;
            //switch (tp)
            //{
            //    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA:
            //        _txtNoEndereco = "NoMemoria";
            //        break;
            //    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_CONTADOR:
            //        _txtNoEndereco = "NoContadores";
            //        break;
            //    case TipoEnderecamentoDispositivo.DIGITAL_MEMORIA_TEMPORIZADOR:
            //        _txtNoEndereco = "NoTemporizadores";
            //        break;
            //}

            //IndicaEnderecoEmUso(this.programa, tp);

            //_NoEnderecamento.Nodes[_txtNoEndereco].Nodes.Clear();
            _qtdAtual = _lstE.Count;
            if ((_qtdAtual == 0) || (_qtdAtual < qtdEnd))
            {
                for (int i = _qtdAtual + 1; i <= qtdEnd; i++)
                    _lstE.Add(new EnderecamentoLadder(tp, i, dispositivo));
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

            //foreach (EnderecamentoLadder el in _lstE)
            //{
            //    _NoEnderecamento.Nodes[_txtNoEndereco].Nodes.Add(el.Nome, el.Nome);
            //    _NoEnderecamento.Nodes[_txtNoEndereco].Nodes[el.Nome].Tag = el;
            //}

            return 0;
        }
    }
}
