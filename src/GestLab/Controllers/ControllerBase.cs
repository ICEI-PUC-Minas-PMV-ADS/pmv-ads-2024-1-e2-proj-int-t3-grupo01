using GestLab.Data;
using GestLab.Models;
using GestLab.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestLab.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly GestLabContext _context;

        public ControllerBase(GestLabContext context)
        {
            _context = context;
        }

        protected UsuarioModel GetDadosUsuario()
        {
            var id = Convert.ToInt32(HttpContext.Session.GetInt32(Constantes.SessionKeyID));
            var usuario = _context.Usuarios
                    .Include(x => x.Cliente)
                    .FirstOrDefault(x => x.Id == id);

            Constantes.UsuarioModel = usuario;
            return usuario;
        }
    }
}
