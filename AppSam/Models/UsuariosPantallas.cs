//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppSam.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsuariosPantallas
    {
        public int UsuarioPantallaId { get; set; }
        public int UsuarioId { get; set; }
        public int PantallaId { get; set; }
    
        public virtual Pantallas Pantallas { get; set; }
        public virtual usuarios usuarios { get; set; }
    }
}