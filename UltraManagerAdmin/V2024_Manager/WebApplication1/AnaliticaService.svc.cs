using Analitic.Entities;
using Analitic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebApplication1
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "AnaliticaService" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione AnaliticaService.svc o AnaliticaService.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class AnaliticaService : IAnaliticaService
    {
        #region Graphics
        public List<EN_Graphics> GetAllGraphics(string storeID = "", string codSucursal = "", string usersID = "", string busqueda = "", string fromDate = "", string toDate = "", string tipo = "")
        {
            return new CT_Graphics().GetAll(storeID, codSucursal, usersID, busqueda, fromDate, toDate, tipo);
        }

        #endregion

        #region Asientos ERP
        public List<EN_Asientos> GetAllAsientos(string storesID, string hqUsersID, string searchValue, string estadoAsiento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new CT_AsientosERP().GetAllAsientos(storesID, hqUsersID, searchValue, estadoAsiento, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }


        public Dictionary<string, int> GetCountRecordAsientosERP(string storesID, string hqUsersID, string searchValue, string estadoAsiento, string fromDate, string toDate)
        {
            return new CT_AsientosERP().GetCountRecordAsientosERP(storesID, hqUsersID, searchValue, estadoAsiento, fromDate, toDate);
        }

        #endregion

        #region DocumentosERP
        public List<EN_DocumentosERP> GetAllDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate)
        {
            return new CT_DocumentosERP().GetAllDocsERP(storesID, hqUsersID, searchValue, tipoDocumento, orderColumn, orderDirection, skip, take, fromDate, toDate);
        }


        public Dictionary<string, int> GetCountRecordDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, string fromDate, string toDate)
        {
            return new CT_DocumentosERP().GetCountRecordDocsMH(storesID, hqUsersID, searchValue, tipoDocumento, fromDate, toDate);
        }

        #endregion
    }
}
