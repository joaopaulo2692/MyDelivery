using Microsoft.EntityFrameworkCore;
using MyDelivery.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MyDelivery.Infrastructure.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

       
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Ocorrencia> Ocorrencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

       
        }
    }
}
