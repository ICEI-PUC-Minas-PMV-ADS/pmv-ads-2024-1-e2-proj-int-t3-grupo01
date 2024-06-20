using GestLab.Data;
using GestLab.Models;
using GestLab.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GestLab.Controllers
{
    public class PedidoController : Controller
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly GestLabContext _context;

        public PedidoController(ILogger<PedidoController> logger, GestLabContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var pedido = _context.Pedido
                .Include(x => x.Cliente)
                .Include(x => x.MontadorResponsavel)
                .ToList();

            if (Constantes.UsuarioModel.Tipo == Constantes.PerfilCliente)
            {
                pedido = pedido.Where(x => x.Cliente.Id == Constantes.UsuarioModel.Cliente.Id).ToList();
            }
            else if (Constantes.UsuarioModel.Tipo == Constantes.PerfilMontador)
            {
                pedido = pedido.Where(x =>
                (x.MontadorResponsavel?.Id == Constantes.UsuarioModel.Id
                && x.Status == Constantes.StatusEmMontagem)
                || x.Status == Constantes.StatusNovo
                || x.Status == Constantes.StatusPendenteArmacao
                || x.Status == Constantes.StatusPendenteLentes
                || x.Status == Constantes.StatusPendenteLenteArmacao
                ).ToList();
            }

            return View("Index", pedido);
        }

        [HttpPost]
        public IActionResult Detail(PedidoViewModel pedidoView)
        {
            var pedido = RecuperaPedido(pedidoView.Pedido.Id);
            pedido.Aplicar(pedidoView.Pedido);

            //gambi provisoria para cliente, campo deve virar um combo com os clientes disponiveis
            var cliente = _context.Cliente.FirstOrDefault(x => x.Id == pedidoView.ClienteId);
            if (cliente == null)
            {
                cliente = _context.Cliente.FirstOrDefault(x => x.Nome == "Cliente manual");
                if (cliente == null)
                    cliente = _context.Cliente.Add(new ClienteModel() { Nome = "Cliente manual" }).Entity;
            }

            pedido.Cliente = cliente;

            var possuiArmacao = pedido.ArmacaoEntreguePeloCliente;
            if (pedido.ArmacaoEntreguePeloCliente && (pedido.Armacao == null || pedido.Armacao.Id == 0))
            {
                possuiArmacao = true;
                var armacao = new ProdutoModel();
                armacao.DataSaida = armacao.DataEntrada = DateTime.Now;
                armacao.Descricao = pedido.IdentificacaoArmacao;
                armacao.Tipo = "Armação";
                armacao.Cor = "-";
                armacao.Utilizado = true;
                pedido.Armacao = armacao;
            }

            if (!pedido.PossuiLentesEmEstoque)
            {
                var lentes = _context.Produto.Where(x => x.Tipo == "Lentes" && !x.Utilizado && x.Cor == pedido.CorLentes)?.Take(2)?.ToList();
                if (lentes?.Count() == 2)
                {
                    pedido.PossuiLentesEmEstoque = true;
                    pedido.LenteEsquerda = lentes.First();
                    pedido.LenteDireita = lentes.Last();
                    pedido.LenteDireita.Utilizado = pedido.LenteEsquerda.Utilizado = true;
                    pedido.LenteDireita.DataSaida = pedido.LenteEsquerda.DataSaida = DateTime.Now;
                }
            }

            if (pedido.ValorPedido == 0 && pedido.PossuiLentesEmEstoque)
            {
                pedido.ValorPedido += pedido.LenteDireita?.Custo ?? 50;
                pedido.ValorPedido += pedido.LenteEsquerda?.Custo ?? 50;
                pedido.ValorPedido += 100;//Custo fixo
                pedido.ValorPedido *= 1.45m;//Margem lucro
            }

            if (!pedido.PedidoSemPendencia())
            {
                if (!possuiArmacao && !pedido.PossuiLentesEmEstoque)
                    pedido.Status = Constantes.StatusPendenteLenteArmacao;
                else if (!possuiArmacao)
                    pedido.Status = Constantes.StatusPendenteArmacao;
                else if (!pedido.PossuiLentesEmEstoque)
                    pedido.Status = Constantes.StatusPendenteLentes;
                else
                    pedido.Status = Constantes.StatusNovo;
            }

            if (pedido.Id == 0)
            {
                pedido.DataPedido = DateTime.Now;
                _context.Pedido.Add(pedido);
            }
            else
                _context.Pedido.Update(pedido);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult AcaoPedido(int id, string acao)
        {
            var pedido = RecuperaPedido(id);

            if (pedido == null)
            {
                return NotFound();
            }

            switch (acao)
            {
                case "Inciar Montagem":
                    pedido.Status = Constantes.StatusEmMontagem;
                    pedido.MontadorResponsavel = _context.Usuarios.Find(Constantes.UsuarioModel.Id);
                    break;
                case "Concluir Montagem":
                    pedido.Status = Constantes.StatusAguardandoPagamento;
                    break;
                case "Realizar Pagamento":
                    pedido.Status = Constantes.StatusFinalizado;
                    break;
            }

            _context.Pedido.Update(pedido);
            _context.SaveChanges();

            return Index();
        }

        public ActionResult Detail(int id)
        {
            var pedido = RecuperaPedido(id);

            if (pedido == null)
            {
                return NotFound();
            }

            PedidoViewModel pedidoModel = new(pedido);
            pedidoModel.Cores = StaticLists.ObterCores().Select(x => new SelectListItem() { Text = x, Value = x });
            pedidoModel.Clientes = _context.Cliente.Select(x => new SelectListItem() { Text = x.Nome, Value = x.Id.ToString() });
            pedidoModel.ClienteId = pedido.Cliente.Id;

            if (Constantes.UsuarioModel.Tipo == Constantes.PerfilCliente)
            {
                pedidoModel.Clientes = _context.Cliente
                    .Where(x => x.Id == Constantes.UsuarioModel.Cliente.Id)
                    .Select(x => new SelectListItem() { Text = x.Nome, Value = x.Id.ToString() });

            }

            return View("Detail", pedidoModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private PedidoModel RecuperaPedido(int id)
        {
            PedidoModel pedido = null;

            if (id > 0)
            {
                pedido = _context.Pedido
                    .Where(x => x.Id == id)
                    .Include(x => x.Receita)
                    .Include(x => x.LenteDireita)
                    .Include(x => x.LenteEsquerda)
                    .Include(x => x.Armacao)
                    .Include(x => x.Cliente)
                    .Include(x => x.MontadorResponsavel)
                    .FirstOrDefault();
            }

            if (id == 0) pedido = new();

            return pedido;
        }
        public async Task<IActionResult> Relatorio(int? mes, int? ano)
        {
            var Pedido = await _context.Pedido
                .Include(x => x.Receita)
                .OrderByDescending(c => c.DataPedido)
                .ToListAsync();


            var mesAno = $"{mes}/{ano}";

            ViewBag.MesAno = mesAno;

            return View("Relatorio", Pedido);
        }
    }
}
