using GestLab.Models;
using Microsoft.EntityFrameworkCore;

namespace GestLab.Data
{
    public class GestLabContext : DbContext
    {
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<ClienteModel> Cliente { get; set; }
        public DbSet<PedidoModel> Pedido { get; set; }
        public DbSet<ProdutoModel> Produto { get; set; }
        public DbSet<ReceitaModel> Receita { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=GestLab;Trusted_Connection=True;");
#else
            optionsBuilder.UseSqlServer("Data Source=SQL8010.site4now.net;Initial Catalog=db_aa9921_gestlab;User Id=db_aa9921_gestlab_admin;Password=Senha@2022");
#endif
            base.OnConfiguring(optionsBuilder);
        }
    }
}
