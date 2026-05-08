using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.Inventario
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "inv-articulos")]
    public class ArticulosController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();

        private string UserName(int length)
        {
            if (Session["USER_NAME"] == null || length <= 0)
                return "";
            else
            {
                string name = Convert.ToString(Session["USER_NAME"]);

                return name.Length > length ? name.Substring(0, length) : name;
            }
        }

        private string UserID()
        {
            if (Session["USER_AUTOID"] == null)
                return "";
            else
                return Convert.ToString(Session["USER_AUTOID"]);
        }

        // GET: Articulos
        public ActionResult Inicio()
        {
            return View("~/Views/Manager/Inventario/Articulos.cshtml");
        }

        private String GetAllDataList(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            try
            {
                DataSet list = _objService.GetAllItems(valorBusqueda, estado, resultCount);
                DataRowCollection rc = list.Tables["Item"].Rows;

                object[] data = rc.Cast<object>().ToArray();

                for (int i = 0; i < rc.Count; i++) data[i] = ((DataRow)data[i]).ItemArray;

                return serializer.Serialize(Json(new Respuesta("", "", data, true)));
            }
            catch (Exception e)
            {
                return serializer.Serialize(Json(new Respuesta(e.Message, "", null, true)));
            }
        }

        private String GetAllList(string valorBusqueda = "", int estado = 0, int resultCount = 0)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            try
            {
                List<EN_Item> data = _objService.GetAllItemsList(valorBusqueda, estado, resultCount).ToList();

                return serializer.Serialize(Json(new Respuesta("", "", data, true)));
            }
            catch (Exception e)
            {
                return serializer.Serialize(Json(new Respuesta(e.Message, "", null, true)));
            }
        }

        [HttpPost]
        public ActionResult GetDynamicJson(int[] families, int[] departments, int[] categories, int[] subCategories, int[] segments)
        {
            // Request de DataTable con los datos del filtrado y paginación
            string draw = Request.Form.GetValues("draw").FirstOrDefault(); // Draw del DataTable
            int orderColumn = Convert.ToInt32(Request.Form.GetValues("order[0][column]").FirstOrDefault()); // Columna por la que se está ordenando
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault(); // La dirección en la que se está ordenando
            var searchValue = Request.Form.GetValues("filtered").FirstOrDefault(); // Valor de filtrado
            string start = Request.Form.GetValues("start").FirstOrDefault(); // Valor de posición de donde se empieza a mostrar registros
            string length = Request.Form.GetValues("length").FirstOrDefault(); // Valor de la cantidad de registros a mostrar
            int take = Convert.ToInt32(length); // Valor de registros a omitir a int
            int skip = Convert.ToInt32(start); // Valor de registros a mostrar a int

            // Obtención de registros
            List<EN_Item> list = _objService.GetAllItemsDynamicList(searchValue, orderColumn, sortColumnDir, skip, take, families, departments, categories, subCategories, segments, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList();

            // Obtención de cantidades totales de base de datos y cantidad de registros filtrados
            Dictionary<string, int> data = _objService.GetItemsCountRecords(searchValue, families, departments, categories, subCategories, segments, StoreAuthorizationAttribute.StoresAvailable(Session));

            return Json(new { draw = draw, recordsFiltered = data["FILTERED"], recordsTotal = data["TOTAL"], data = list });
        }

        public JsonResult GetInitialData()
        {
            List<EN_ExtCentral_Family> families = _objService.GetAllFamilies("", 0, 0).ToList();
            List<EN_Department> departs = _objService.GetAllDepartments("", 0, 0).ToList();
            List<EN_ExtCentral_Brand> brands = _objService.GetAllBrands().ToList();
            List<EN_ExtCentral_Manufacturer> manus = _objService.GetAllManufacturers().ToList();
            List<EN_Purchaser> purchs = _objService.GetAllPurchasers().ToList();
            List<EN_Supplier> supp = _objService.GetAllSupplier(StoreAuthorizationAttribute.StoresAvailable(Session), "", 0, 0).ToList();
            List<EN_UOM> uoms = _objService.GetAllUOMByInactive(false).ToList();
            List<EN_Tax> impuesto = _objService.GetAllTax("", 0, 0).ToList();

         return Json(new
            {
                families = families,
                departments = departs,
                brands = brands,
                manufacturers = manus,
                purchasers = purchs,
                suppliers = supp,
                uoms = uoms,
                list_tax = impuesto
         }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemsDetail(int itemID)
        {
            try
            {
                string storesAvailable = StoreAuthorizationAttribute.StoresAvailable(Session);

                List<EN_ItemStore> list = _objService.GetAllItem_ItemStore(itemID, storesAvailable).ToList();
                List<EN_SupplierList> slist = _objService.GetAllSupplierListByItemID(itemID, storesAvailable).ToList();
                List<EN_POC_ItemPrice> plist = _objService.GetAllPOC_ItemPriceByItemID(itemID, storesAvailable).ToList();
                List<EN_Store> stores = _objService.GetStores_ItemStatus(itemID, storesAvailable).ToList();
                return Json(new Respuesta("", "", new
                {
                    details = list,
                    suppliers = slist,
                    supplierPrices = plist,
                    stores = stores
                }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new Respuesta("ERROR OBTENER DETALLES DE ARTICULO",
                    "Error interno. No se pudieron obtener los datos de los artículos de las tiendas",
                    null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveItem(string item, int[] stores)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                EN_Item i = JsonConvert.DeserializeObject<EN_Item>(item, settings);

                string val = validate(i);
                if (String.IsNullOrEmpty(val))
                {
                    Dictionary<string, object> result = _objService.SaveItem(i, $"AVS WEB:{i.ItemLookupCode}-{UserName(42-i.ItemLookupCode.Length)}", UserID());

                    return Json(new Respuesta("", Resources.messages.ResourceManager.GetString((string)result["RESPUESTA"]), result["SCOPE"], !((string)result["RESPUESTA"]).Contains("error")), JsonRequestBehavior.AllowGet);
                }
                return Json(new Respuesta(item, item , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno + Resources.messages.error_intento_guardar, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        private string validate(EN_Item item)
        {
            if (item != null)
            {
                string resp = "";
                if (String.IsNullOrEmpty(item.ItemLookupCode)) resp += "[ItemLookupCode],";
                if (String.IsNullOrEmpty(item.Description)) resp += "[Description],";
                if (String.IsNullOrEmpty(item.ExtendedDescription)) resp += "[ExtendedDescription],";
                if (String.IsNullOrEmpty(item.SubDescription3)) resp += "[Cabys],";
                if (item.DepartmentID <= 0) resp += "[Departamento]";
                if (item.FamilyID <= 0) resp += "[Familia]";
                if (item.CategoryID <= 0) resp += "[Categoría]";
                if (item.TaxID <= 0) resp += "[Impuesto]";
                if (item.SupplierID <= 0) resp += "[Proveedor]";
                if (item.ReplacementCost <= 0) resp += "[Costo Neto]";
                if (item.Utility <= 0 || item.Utility > 800) resp += "[Utilidad]";
                if (item.InvoiceDiscount < 0 || item.InvoiceDiscount >= 100) resp += "[Descuento en Factura]";
                if (item.CustomerDiscount < 0 || item.CustomerDiscount >= 100) resp += "[Descuento para cliente]";
                if (item.MSRP < 0) resp += "[Impuesto Específico]";
                return resp;
            }
            else return "[ITEM]";
        }

        public JsonResult GetItemByItemLookupCode(string itemLookupCode)
        {
            EN_Item item = _objService.GetItemByItemLookupCode(itemLookupCode, StoreAuthorizationAttribute.StoresAvailable(Session));
            return Json(new Respuesta("", "", item, item != null), JsonRequestBehavior.AllowGet);
        }

        #region Departamentos
        public JsonResult GetDepartments()
        {
            try
            {
                using (_objService)
                {
                    List<EN_Department> departs = _objService.GetAllDepartments("", 0, 0).ToList();
                    List<EN_ExtCentral_Family> fams = _objService.GetAllFamilies("", 0, 0).ToList();

                    return Json(new Respuesta("", "", new { departments = departs, families = fams }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno.", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Categorias
        public JsonResult GetCategoriesByDepartmentID(int departmentID)
        {
            try
            {
                List<EN_Category> list = _objService.GetAllCategories_ByDepartmentID(departmentID).ToList();

                return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetListCategoriesByDepartments(int[] departments)
        {
            List<EN_Category> list_categories = _objService.GetAllCategories("", 0, 0).ToList();

            var new_list = list_categories.Where(x => departments.Contains(x.DepartmentID));

            return Json(new { categories = new_list }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Subcategorías
        public JsonResult GetSubcategoriesByCategoryID(int categoryID)
        {
            try
            {
                List<EN_ExtCentral_SubCategory> list = _objService.GetAllSubCategory_By_CategoryID(categoryID).ToList();

                return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetListSubcategoriesByCategories(int[] categories)
        {
            List<EN_ExtCentral_SubCategory> list_subcategories = _objService.GetAllSubCategories("", 0, 0).ToList();

            var new_list = list_subcategories.Where(x => categories.Contains(x.CategoryID));

            return Json(new { subCategories = new_list }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Segmentos
        public JsonResult GetSegmentsBySubCategoryID(int subCategoryID)
        {
            try
            {
                List<EN_ExtCentral_Segment> list = _objService.GetAllSegment_By_SubCategoryID(subCategoryID).ToList();

                return Json(new Respuesta("", "", list, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetSegmentsBySubCategories(int[] subCategoriesID)
        {
            try
            {
                using (_objService)
                {
                    List<EN_ExtCentral_Segment> list_segs = _objService.GetAll_ExtCentral_Segment().ToList(), new_list = new List<EN_ExtCentral_Segment>();

                    new_list = list_segs.Where(x => subCategoriesID.Contains(x.SubCategoryID)).ToList();

                    return Json(new Respuesta("", "", new { list_segments = new_list }, true), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, Resources.messages.error_interno + Resources.messages.error_obt_regs, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Item-Store
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
        #endregion
    }
}