using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Configuracion
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize]
    public class TiendaController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        public JsonResult GetAll_By_StoreGroupID(int storeGroupID)
        {
            try
            {
                List<EN_Store> st = _objService.GetAllStore_By_StoreGroupID(storeGroupID, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList();

                return Json(new Respuesta("", "", new { stores = st }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new Respuesta("GetAllStore_By_StoreGroupID",
                    Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAll_By_StoreGroupsID(int[] storeGroupsID)
        {
            try
            {
                List<EN_Store> st = new List<EN_Store>();
                
                if (storeGroupsID != null)
                {
                    foreach (int id in storeGroupsID)
                    {
                        st.AddRange(_objService.GetAllStore_By_StoreGroupID(id, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList());
                    }
                }

                return Json(new Respuesta("", "", new { stores = st }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new Respuesta("GetAllStore_By_StoreGroupID",
                    Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStores_ItemStatus(int itemID, string storesAvailable = "%")
        {
            try
            {
                List<EN_Store> st = _objService.GetStores_ItemStatus(itemID, storesAvailable).ToList();

                List<object> lst = new List<object>();

                st.ForEach(x => { lst.Add(new { id = x.IDS, text = x.NameS, itemStatus = x.ItemStatus }); });

                return Json(new { lst }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
    }
}