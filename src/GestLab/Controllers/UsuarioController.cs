using GestLab.Data;
using GestLab.Models;
using GestLab.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GestLab.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly GestLabContext _context;

        public UsuarioController(ILogger<UsuarioController> logger, GestLabContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var usuario = _context.Usuarios
               .Include(x => x.Cliente)
               .ToList();
               
            return View("Index", usuario);
        }

        [HttpPost]
        public IActionResult Detail(UsuarioViewModel usuarioView)
        {
            var usuario = usuarioView.Usuario;

            if (usuario.Tipo!= Constantes.PerfilCliente)
                usuario.Cliente = null;
            else
            {
                if (!usuarioView.Cliente.HasValue || usuarioView.Cliente < 1)
                    throw new Exception("Para usuarios do tipo cliente, o cliente deve ser informado");
                usuario.Cliente = _context.Cliente.Find(usuarioView.Cliente);
            }

            if (usuario.Id == 0)
                _context.Usuarios.Add(usuario);
            else
                _context.Usuarios.Update(usuario);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int id)
        {
            var usuario = RecuperaUsuario(id);

            if (usuario == null)
            {
                return NotFound();
            }

            UsuarioViewModel usuarioModel = new(usuario);

            usuarioModel.Tipos = StaticLists.ObterTiposUsuario().Select(x => new SelectListItem() { Text = x, Value = x });
            var clientes = new List<SelectListItem>() { new() { Text = "Usuario interno", Value = null } };
            clientes.AddRange(_context.Cliente.Select(x => new SelectListItem() { Text = x.Nome, Value = x.Id.ToString() }));
            usuarioModel.Clientes = clientes;
            usuarioModel.Cliente = usuario.Cliente?.Id;

            return View("Detail", usuarioModel);
        }

        private UsuarioModel RecuperaUsuario(int id)
        {
            UsuarioModel usuario = null;

            if (id > 0)
            {
                usuario = _context.Usuarios
                   .Include(x => x.Cliente)
                   .FirstOrDefault(x => x.Id == id);
            }

            if (id == 0) usuario = new();

            return usuario;
        }

        public IActionResult Search(string searchTerm)
        {
            var usuarios = _context.Usuarios.ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                usuarios = usuarios.Where(u => u.Nome.ToLower().Contains(searchTerm) || u.Email.ToLower().Contains(searchTerm)).ToList();
            }

            return View("Index", usuarios);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id?? HttpContext.TraceIdentifier });
        }
    }
}