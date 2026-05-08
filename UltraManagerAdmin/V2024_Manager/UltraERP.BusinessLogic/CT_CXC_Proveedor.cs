using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_CXC_Proveedor
    {
        #region Variables
        private DT_CXC_Proveedor dT_CXC_Proveedor = new DT_CXC_Proveedor();
        #endregion

        #region Constructors
        public CT_CXC_Proveedor() { }
        #endregion

        #region Methods
        public List<EN_CXC_Proveedor> GetAll()
        {
            return dT_CXC_Proveedor.GetAll();
        }
        public List<EN_CXC_FormaRebajo> GetAll_FormaRebajos()
        {
            return dT_CXC_Proveedor.GetAll_FormaRebajos();
        }
        public List<EN_Tipo> GetAll_IDCTipo()
        {
            return dT_CXC_Proveedor.GetAll_IDCTipo();
        }
        public List<EN_Tipo> GetAll_IICTipo()
        {
            return dT_CXC_Proveedor.GetAll_IICTipo();
        }
        public List<EN_Tipo> GetAll_FrecuenciaPagos()
        {
            return dT_CXC_Proveedor.GetAll_FrecuenciaPagos();
        }
        public List<EN_Tipo> GetAll_Negociaciones()
        {
            return dT_CXC_Proveedor.GetAll_Negociaciones();
        }
        public List<EN_CXC_IIC> GetAll_IIC(int fichaID)
        {
            return dT_CXC_Proveedor.GetAll_IIC(fichaID);
        }
        public List<EN_CXC_IDC> GetAll_IDC(int fichaID)
        {
            return dT_CXC_Proveedor.GetAll_IDC(fichaID);
        }
        public Dictionary<string, object> Save_CXC_Proveedor(EN_CXC_Proveedor ficha)
        {
            return dT_CXC_Proveedor.Save_CXC_Proveedor(ficha);
        }
        public Respuesta Save_IDC(EN_CXC_IDC idc, int CXC_ProveedorID)
        {
            return dT_CXC_Proveedor.Save_IDC(idc, CXC_ProveedorID);
        }
        public Respuesta Save_IIC(EN_CXC_IIC iic, int CXC_ProveedorID)
        {
            return dT_CXC_Proveedor.Save_IIC(iic, CXC_ProveedorID);
        }
        public Respuesta DeleteIDC(int ID)
        {
            return dT_CXC_Proveedor.DeleteIDC(ID);
        }
        public Respuesta DeleteIIC(int ID)
        {
            return dT_CXC_Proveedor.DeleteIIC(ID);
        }
        public Respuesta ChangeStatus_CXC_Proveedor(int proveedorID, string status)
        {
            return dT_CXC_Proveedor.ChangeStatus_CXC_Proveedor(proveedorID, status);
        }
        #endregion
    }
}
