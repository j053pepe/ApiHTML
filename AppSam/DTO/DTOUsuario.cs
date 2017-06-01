namespace AppSam.DTO
{
    public class DTOUsuario
    {
        public int usuariosId { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string llave { get; set; }
        public string pass { get; set; }
        public int EstatusId { get; set; }
    }
}