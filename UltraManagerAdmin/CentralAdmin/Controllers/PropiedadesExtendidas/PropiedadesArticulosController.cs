using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.UltraERPServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.PropiedadesExtendidas
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "prop-articulos")]
    public class PropiedadesArticulosController : Controller
    {
         private readonly UltraERPServiceClient.UltraERPServiceClient _objService = new UltraERPServiceClient.UltraERPServiceClient();

        public ActionResult Inicio()
        {
            return View("~/Views/Manager/PropiedadesExtendidas/PropiedadesArticulos.cshtml");
        }

        public JsonResult GetInitData()
        {
            List<EN_ItemCustomProperty> props = _objService.GetCustomProperty(PropertyAuthorizationAttribute.PropertiesAvailable(Session)).ToList();
            List<EN_Store> stores = _objService.GetAllStore(StoreAuthorizationAttribute.StoresAvailable(Session), 0, 0).ToList();

            return Json(new { customProperties = props, stores = stores }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAll_ItemProperties(string searchValue)
        {
            string draw = Request.Form.GetValues("draw").FirstOrDefault(); // Draw del DataTable
            int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault()); // Columna por la que se está ordenando
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault(); // La dirección en la que se está ordenando
            string start = Request.Form.GetValues("start").FirstOrDefault(); // Valor de posición de donde se empieza a mostrar registros
            string length = Request.Form.GetValues("length").FirstOrDefault(); // Valor de la cantidad de registros a mostrar
            int take = Convert.ToInt32(length); // Valor de registros a omitir a int
            int skip = Convert.ToInt32(start); // Valor de registros a mostrar a int

            List<EN_ItemProperty> list = _objService.GetAllItemsProperties(
                StoreAuthorizationAttribute.StoresAvailable(Session),
                PropertyAuthorizationAttribute.PropertiesAvailable(Session),
                searchValue,
                orderColumn,
                sortColumnDir,
                skip,
                take
            ).ToList();

            Dictionary<string, int> recordsNumber = _objService.Get_ItemProperties_CountRecord(StoreAuthorizationAttribute.StoresAvailable(Session), searchValue);

            return Json(
                new
                {
                    draw = draw,
                    recordsFiltered = recordsNumber["TOTAL_FILTERED"],
                    recordsTotal = recordsNumber["TOTAL_RECORDS"],
                    data = list
                }
            );
        }

        public JsonResult Get_Item_Properties(int itemID)
        {
            List<EN_ItemExt> list = _objService.GetItemPropertiesByItem(
                itemID,
                StoreAuthorizationAttribute.StoresAvailable(Session),
                PropertyAuthorizationAttribute.PropertiesAvailable(Session)
            ).ToList();

            return Json(new { itemProperties = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save_Item_Properties(EN_ItemExt[] properties, string stores, int itemID)
        {
            string xml = $"";
            string propsAvailable = PropertyAuthorizationAttribute.PropertiesAvailable(Session);
            string[] propsNames = propsAvailable.Split(',');

            foreach (EN_ItemExt i in properties)
            {
                if (propsAvailable == "%" || propsNames.Contains(i.PropertyName))
                    xml += $"<Property Name=\"{i.PropertyName}\" Type=\"{GetDataTypeProperty(i.PropertyType)}\">{i.Value}</Property>";
            }

            Respuesta r = _objService.Save_ItemExtProperty(xml, itemID, propsAvailable, stores, Convert.ToInt32(Session["USER_AUTOID"]));

            r.Message = Resources.messages.ResourceManager.GetString(r.Message);

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        private string GetDataTypeProperty(int type)
        {
            switch (type)
            {
                case 1: return "System.DateTime";
                case 2: return "System.Decimal";
                case 3: return "System.Decimal";
                case 4: return "System.Boolean";
                case 5: return "System.Collections.ArrayList";
                default: return "System.String";
            }
        }
    }
}