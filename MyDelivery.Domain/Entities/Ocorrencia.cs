using MyDelivery.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Domain.Entities
{
    public class Ocorrencia
    {
        public int IdOcorrencia { get; private set; }
        public ETipoOcorrencia TipoOcorrencia { get; private set; }
        public DateTime HoraOcorrencia { get; private set; }
        public bool IndFinalizadora { get; private set; }

        // ctor para ORM
        protected Ocorrencia() { }

        private Ocorrencia(ETipoOcorrencia tipo, DateTime hora, bool indFinalizadora = false)
        {
            TipoOcorrencia = tipo;
            HoraOcorrencia = hora;
            IndFinalizadora = indFinalizadora;
        }

        public static Ocorrencia Criar(ETipoOcorrencia tipo, DateTime hora) =>
            new Ocorrencia(tipo, hora);

        public void MarcarComoFinalizadora() => IndFinalizadora = true;
    }

}
