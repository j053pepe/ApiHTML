using System.Collections.Generic;

namespace AppSam.DTO
{
    public class DTOMenu
    {
        public int MenuTituloId { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public List<DTOPantalla> Pantallas { get; set; }
    }
}