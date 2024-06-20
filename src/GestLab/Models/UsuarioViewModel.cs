using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GestLab.Models
{
    public class UsuarioViewModel
    {
        public UsuarioViewModel()
        {

        }
        public UsuarioViewModel(UsuarioModel usuario)
        {
            Usuario = usuario;
        }
        public UsuarioModel Usuario { get; set; }
        public int? Cliente { get; set; }
        public IEnumerable<SelectListItem> Tipos { get; set; }
        public IEnumerable<SelectListItem> Clientes { get; set; }
    }
}
