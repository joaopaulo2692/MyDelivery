using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Ocorrencia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdOcorrencia { get; private set; }

    public ETipoOcorrencia TipoOcorrencia { get; private set; }
    public DateTime HoraOcorrencia { get; private set; }
    public bool IndFinalizadora { get; private set; }

    public int PedidoId { get; private set; }
    [JsonIgnore]
    public Pedido Pedido { get; private set; } = null!;


    protected Ocorrencia() { }

    private Ocorrencia(ETipoOcorrencia tipo, DateTime hora, int pedidoId, bool indFinalizadora = false)
    {
        TipoOcorrencia = tipo;
        HoraOcorrencia = hora;
        PedidoId = pedidoId;
        IndFinalizadora = indFinalizadora;
    }

    public static Ocorrencia Criar(ETipoOcorrencia tipo, DateTime hora, int pedidoId) =>
        new Ocorrencia(tipo, hora, pedidoId);

    public void MarcarComoFinalizadora() => IndFinalizadora = true;
}
