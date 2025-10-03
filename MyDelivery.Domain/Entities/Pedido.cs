using MyDelivery.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Domain.Entities
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; private set; }
        public int NumeroPedido { get; private set; }
        public DateTime HoraPedido { get; private set; }
        public bool IndEntregue { get; private set; }
        private readonly List<Ocorrencia> _ocorrencias = new();
        public IReadOnlyCollection<Ocorrencia> Ocorrencias => _ocorrencias.AsReadOnly();

        protected Pedido() { }

        public Pedido(int numeroPedido, DateTime horaPedido)
        {
            NumeroPedido = numeroPedido;
            HoraPedido = horaPedido;
            IndEntregue = false;
        }

        public void AdicionarOcorrencia(Ocorrencia oc)
        {
            if (EstaConcluido())
                throw new InvalidOperationException("Pedido já concluído; não é possível adicionar ocorrências.");

            // regra: não permite 2 ocorrências do mesmo tipo em intervalo de 10 minutos
            var ultimaMesmoTipo = _ocorrencias
                .Where(o => o.TipoOcorrencia == oc.TipoOcorrencia)
                .OrderByDescending(o => o.HoraOcorrencia)
                .FirstOrDefault();

            if (ultimaMesmoTipo != null && (oc.HoraOcorrencia - ultimaMesmoTipo.HoraOcorrencia).TotalMinutes < 10)
                throw new InvalidOperationException("Não é possível cadastrar duas ocorrências do mesmo tipo em menos de 10 minutos.");


            if (_ocorrencias.Count == 1)
                oc.MarcarComoFinalizadora();

            _ocorrencias.Add(oc);

            // se a oc foi finalizadora, atualizar status do pedido
            if (oc.IndFinalizadora)
            {
                IndEntregue = oc.TipoOcorrencia == ETipoOcorrencia.EntregueComSucesso;
            }
        }

        public void RemoverOcorrencia(int idOcorrencia)
        {
            if (EstaConcluido())
                throw new InvalidOperationException("Pedido já concluído; exclusão de ocorrência não permitida.");

            var oc = _ocorrencias.FirstOrDefault(o => o.IdOcorrencia == idOcorrencia);
            if (oc == null) throw new KeyNotFoundException("Ocorrência não encontrada.");

            _ocorrencias.Remove(oc);

            // após remover, reavaliar se existe algum finalizador e atualizar IndEntregue
            var finalizadora = _ocorrencias.LastOrDefault(o => o.IndFinalizadora);
            if (finalizadora != null)
                IndEntregue = finalizadora.TipoOcorrencia == ETipoOcorrencia.EntregueComSucesso;
            else
                IndEntregue = false;
        }

        //public bool EstaConcluido() => _ocorrencias.Any(o => o.IndFinalizadora);
        public bool EstaConcluido() => _ocorrencias.Count > 1;

    }

}
