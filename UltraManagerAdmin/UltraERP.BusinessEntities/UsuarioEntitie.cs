using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class UsuarioEntitie
    {
        public string strCedula { get; set; }
        public string strLogin { get; set; }
        public string strNombre { get; set; }
        public string strClave { get; set; }
        public string strTelefono { get; set; }
        public short intNivel_Seguridad { get; set; }
        public int intPrivilegios { get; set; }
        public string strCorreo { get; set; }
        public short intEstado { get; set; }
        public int intTipo_Gestion { get; set; }
    }
}
