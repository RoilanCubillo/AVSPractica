using System;

namespace UltraERP.Models
{
    public class DocumentoAuditoriaEventoViewModel
    {
        public string Evento { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Comentario { get; set; }
    }
}
