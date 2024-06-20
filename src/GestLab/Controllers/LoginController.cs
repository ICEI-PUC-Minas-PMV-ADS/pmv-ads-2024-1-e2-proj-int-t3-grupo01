using GestLab.Data;
using GestLab.Models;
using GestLab.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GestLab.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger, GestLabContext context):base(context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return PartialView(new LoginModel());
        }

        [HttpPost]
        public IActionResult Index(LoginModel login)
        {
            if (login.Email == "admin@admin.com" && login.Senha == "123!@#")
            {
                HttpContext.Session.SetInt32(Constantes.SessionKeyID, 0);
                HttpContext.Session.SetString(Constantes.SessionKeyNome, "Administrador");
                HttpContext.Session.SetString(Constantes.SessionKeyEmail, login.Email);
                HttpContext.Session.SetString(Constantes.SessionKeyTipo, "ADM");
                HttpContext.Session.SetString(Constantes.SessionKeyClienteId, "-1");
                HttpContext.Session.SetString(Constantes.SessionKeyClienteNome, "");

                return RedirectToAction("Index", "Home");
            }
            else if (_context.Usuarios.Any(x => x.Email == login.Email && x.Senha == login.Senha))
            {
                UsuarioModel usuario = _context.Usuarios.Where(x => x.Email == login.Email && x.Senha == login.Senha).FirstOrDefault();
                HttpContext.Session.SetInt32(Constantes.SessionKeyID, usuario.Id);
                HttpContext.Session.SetString(Constantes.SessionKeyNome, usuario.Nome);
                HttpContext.Session.SetString(Constantes.SessionKeyEmail, usuario.Email);
                HttpContext.Session.SetString(Constantes.SessionKeyTipo, usuario.Tipo);
                HttpContext.Session.SetString(Constantes.SessionKeyClienteId, usuario.Cliente?.Id.ToString() ?? "-1");
                HttpContext.Session.SetString(Constantes.SessionKeyClienteNome, usuario.Cliente?.Nome.ToString() ?? "");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Tipo"] = "Erro";
                TempData["Mensagem"] = "E-mail ou senha inválidos.";

                return PartialView(login);
            }
        }
    }
}
