namespace UltraERP.Models
{
    public class DocumentoDetalleLineaViewModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal DescuentoPorcentaje { get; set; }
        public decimal DescuentoMonto { get; set; }
        public decimal ImpuestoPorcentaje { get; set; }
        public decimal TotalLinea { get; set; }
        public bool Regalia { get; set; }
        public string Observacion { get; set; }
    }
}
