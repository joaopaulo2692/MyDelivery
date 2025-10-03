using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MyDelivery.Infrastructure.Data
{
    public class MyDbContext(DbContextOptions<MyDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {




        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Ocorrencia> Ocorrencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.IdPedido);

                entity.HasMany(p => p.Ocorrencias)
                      .WithOne(o => o.Pedido)
                      .HasForeignKey(o => o.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
                // Cascade = se deletar Pedido, deleta as Ocorrências
            });

            modelBuilder.Entity<Ocorrencia>(entity =>
            {
                entity.HasKey(o => o.IdOcorrencia);
                entity.Property(o => o.TipoOcorrencia)
                      .IsRequired();

                entity.Property(o => o.HoraOcorrencia)
                      .IsRequired();
            });


        }
    }
}

//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using MyDelivery.Domain.Entities;

//namespace MyDelivery.Infrastructure.Data
//{
//    public class MyDbContext : IdentityDbContext<ApplicationUser>
//    {
//        public MyDbContext(DbContextOptions<MyDbContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<Pedido> Pedidos { get; set; }
//        public DbSet<Ocorrencia> Ocorrencias { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);
//            // Configurações adicionais aqui, se precisar
//        }
//    }
//}

