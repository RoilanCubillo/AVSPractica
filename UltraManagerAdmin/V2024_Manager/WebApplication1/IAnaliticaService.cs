using Analitic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebApplication1
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IAnaliticaService" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IAnaliticaService
    {
        #region Graphics

        [OperationContract]
        List<EN_Graphics> GetAllGraphics(string storeID = "", string codSucursal = "", string usersID = "", string busqueda = "", string fromDate = "", string toDate = "", string tipo = "");

        #endregion

        #region Asientos ERP
        [OperationContract]
        List<EN_Asientos> GetAllAsientos(string storesID, string hqUsersID, string searchValue, string estadoAsiento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate);
        [OperationContract]
        Dictionary<string, int> GetCountRecordAsientosERP(string storesID, string hqUsersID, string searchValue , string estadoAsiento, string fromDate, string toDate);


        #endregion

        #region DocumentosERP
        [OperationContract]
        List<EN_DocumentosERP> GetAllDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, int orderColumn, string orderDirection, int skip, int take, string fromDate, string toDate);
        [OperationContract]
        Dictionary<string, int> GetCountRecordDocsERP(string storesID, string hqUsersID, string searchValue, string tipoDocumento, string fromDate, string toDate);


        #endregion
    }
}
