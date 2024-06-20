using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestLab.Models
{
    public class PedidoViewModel
    {
        public PedidoViewModel()
        {

        }
        public PedidoViewModel(PedidoModel pedido)
        {
            Pedido = pedido;
        }
        public PedidoModel Pedido { get; set; }
        public int ClienteId { get; set; }
        public IEnumerable<SelectListItem> Cores { get; set; }
        public IEnumerable<SelectListItem> Clientes { get; set; }
    }
}
