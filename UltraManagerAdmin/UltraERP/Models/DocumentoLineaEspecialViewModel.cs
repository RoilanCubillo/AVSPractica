namespace UltraERP.Models
{
    public class DocumentoLineaEspecialViewModel
    {
        public string CodigoProducto { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }
        public string TipoLinea { get; set; }
        public bool RequiereAuditoria { get; set; }
    }
}
