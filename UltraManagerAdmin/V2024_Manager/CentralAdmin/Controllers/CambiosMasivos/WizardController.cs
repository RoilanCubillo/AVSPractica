using CentralAdmin.AuthorizationManager.AuthorizationAttributes;
using CentralAdmin.wcfClient;
using Newtonsoft.Json;
using Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using UltraERP.BusinessEntities;

namespace CentralAdmin.Controllers.CambiosMasivos
{
    [Authorize]
    [CustomAuthorize]
    [SystemAuthorize]
    [ViewAuthorize(Permissions = "camb-masivos-wizard")]
    public class WizardController : Controller
    {
        private readonly UltraERPServiceClient _objService = new UltraERPServiceClient();
        private readonly string DataWizardCode = WebConfigurationManager.AppSettings["DataWizardCode"];
        private readonly string DataStoreCode = WebConfigurationManager.AppSettings["DataStoreCode"];

        public ActionResult Inicio()
        {
            Session["WORKSHEET_CONTENT_ID"] = null;
            return View("~/Views/Manager/CambiosMasivos/Wizard.cshtml");
        }

        private string UserName(int length)
        {
            if (Session["USER_NAME"] == null)
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

        #region Info inicial
        private List<EN_ExtCentral_WizardList> getWizardList()
        {
            List<EN_ExtCentral_WizardList> wData = _objService.GetAll_WizardList().ToList()
                    , wList = new List<EN_ExtCentral_WizardList>();
            EN_SC_DataAccess[] accessW = Array.FindAll((EN_SC_DataAccess[])Session["USER_DATAACCESS"], a => a.Code == DataWizardCode);

            if (accessW.Length > 0)
            {
                if (Array.Exists(accessW, x => x.EnableAll)) wList.AddRange(wData);
                else
                {
                    foreach (EN_ExtCentral_WizardList w in wData)
                        if (Array.Exists(accessW, a => a.DataIDs != null && a.DataIDs.Split(',').Contains(w.Codigo)))
                            wList.Add(w);
                }
            }

            return wList;
        }

        public JsonResult GetInitData()
        {
            try
            {
                List<EN_ExtCentral_WizardList> wList = getWizardList();
                List<EN_StoreGroup> sList = _objService.GetAll_StoreGroup(StoreAuthorizationAttribute.StoresAvailable(Session)).ToList();

                return Json(new Respuesta("", "", new { tasks = wList, groups = sList }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new Respuesta("GET INIT DATA WIZARD",
                    Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Info adicional

        #region Tienda
        public JsonResult GetAllStores_By_StoreGroupID(int storeGroupID)
        {
            try
            {
                List<EN_Store> st = _objService.GetAllStore_By_StoreGroupID(storeGroupID, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList(); // FALTA AQUÍ

                return Json(new Respuesta("", "", new { stores = st }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new Respuesta("GetAllStore_By_StoreGroupID",
                    Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetAllStores_By_StoreGroupsID(int[] storeGroupsID)
        {
            try
            {
                List<EN_Store> st = new List<EN_Store>();

                if (storeGroupsID != null)
                {
                    foreach (int id in storeGroupsID)
                    {
                        st.AddRange(_objService.GetAllStore_By_StoreGroupID(id, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList()); // FALTA AQUÍ
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
        #endregion

        #region Artículos
        public JsonResult GetItemByItemLookupCode(string itemLookupCode)
        {
            Session["WORKSHEET_CONTENT_ID"] = null;
            EN_Item item = _objService.GetItemByItemLookupCode(itemLookupCode, StoreAuthorizationAttribute.StoresAvailable(Session));
            return Json(new Respuesta("", "", item, item != null), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Proveedores
        public JsonResult GetSuppliers()
        {
            try
            {
                List<EN_Supplier> supps = _objService.GetAllSupplier(StoreAuthorizationAttribute.StoresAvailable(Session), "", 0, 0).ToList();

                return Json(new Respuesta("", "", new { suppliers = supps }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #endregion

        #region Wizard Files
        [HttpPost]
        public JsonResult UploadFileWizard(string taskCode)
        {
            HttpPostedFileWrapper file = (HttpPostedFileWrapper)HttpContext.Request.Files[0];

            byte[] buffer = new byte[file.ContentLength];

            file.InputStream.Read(buffer, 0, file.ContentLength);

            Respuesta respuesta = _objService.EN_Worksheet_Save_WorksheetContent(new EN_Worksheet.WorksheetContent()
            {
                ContentData = buffer,
                ContentLenght = file.ContentLength,
                ContentType = file.ContentType,
                FileName = file.FileName,
                HQUserID = Convert.ToInt32(Session["USER_AUTOID"]),
                TaskCode = taskCode.Replace("T", "")
            });

            if (respuesta.Status) Session["WORKSHEET_CONTENT_ID"] = respuesta.Result;
            else Session["WORKSHEET_CONTENT_ID"] = null;

            return Json(respuesta);
        }
        #endregion

        #region Task102 - Cambio de Costos [--OBSOLETO--]
        [HttpPost]
        [WizardAuthorization(Permissions = "102")]
        private JsonResult Task102(int[] stores, EN_WizardStructs.TablaCost[] items)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaCost i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task102(i, "", "", items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task104 - Cambio de Impuestos
        [WizardAuthorization(Permissions = "104")]
        public JsonResult GetTaxs()
        {
            try
            {
                List<EN_ItemTax> iTaxs = _objService.GetAll_ItemTax().ToList();
                List<EN_Tax> taxs = _objService.GetAllTax("", 0, 0).ToList();

                return Json(new Respuesta("", "", new { taxs = taxs, itemTaxs = iTaxs }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [WizardAuthorization(Permissions = "104")]
        public JsonResult Task104(int[] stores, EN_WizardStructs.TablaItemTax[] items, string notes)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemTax i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task104(i, $"104-Cambio Impuesto Venta-{UserName(24)}", notes, items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task105 - Cambio de Estados
        [HttpPost]
        [WizardAuthorization(Permissions = "105")]
        public JsonResult Task105(int[] stores, EN_WizardStructs.TablaEstado[] items)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaEstado i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task105(i, $"105-Cambio de Estados-{UserName(28)}", "", items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task107 - Cambio de Descripcion (Corta)
        [HttpPost]
        [WizardAuthorization(Permissions = "107")]
        public JsonResult Task107(int[] stores, EN_WizardStructs.TablaItemDes[] items, string notes)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemDes i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task107(i, $"107-Cambio Desc Corta-{UserName(28)}", notes, items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task108 - Cambio de Descripción Larga
        [HttpPost]
        [WizardAuthorization(Permissions = "108")]
        public JsonResult Task108(int[] stores, EN_WizardStructs.TablaItemSub1[] items, string notes)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemSub1 i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode) || String.IsNullOrEmpty(i.SubDescription1)) status = false;

                if (status)
                {

                    List<Respuesta> respuestas = _objService.Wizard_Task108(stores, $"108-Cambio Desc Ext-{UserName(30)}", notes, items, UserID()).ToList();

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task109 - Cambio de Subdescripción 2 [--OBSOLETO--]
        [HttpPost]
        [WizardAuthorization(Permissions = "109")]
        private JsonResult Task109(int[] stores, EN_WizardStructs.TablaItemSub2[] items)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemSub2 i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task109(i, "", "", items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task110 - Cambio de Subdescripción 3 (Cabys)
        [HttpPost]
        [WizardAuthorization(Permissions = "110")]
        public JsonResult Task110(int[] stores, EN_WizardStructs.TablaItemSub3[] items, string notes)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemSub3 i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task110(i, $"110-Cambios de Cabys-{UserName(29)}", notes, items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Task120 - Cambio Proveedor - Costo
        [WizardAuthorization(Permissions = "120")]
        public JsonResult Task120Verificar(string[] itemLookupCodes)
        {
            try
            {
                Session["WORKSHEET_CONTENT_ID"] = null;

                List<Respuesta> respuestas = new List<Respuesta>();
                var str = string.Join(",", itemLookupCodes);
                Respuesta respuesta;

                List<EN_Item> item = _objService.GetItemByItemLookupCodebyLista(str, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList();

                respuesta = ((item == null) ? new Respuesta("PRODUCTO NO EXISTE", $"No se encontró el producto en el Sistema.", "", false) : new Respuesta("", "", item, true));
                respuestas.Add(respuesta);

                var jsonResult = Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [WizardAuthorization(Permissions = "120")]
        public JsonResult Task120(int[] stores, string notes, EN_WizardStructs.TablaItemProveedor[] items)
        {
            try
            {
                bool status = true;

                int cID = Convert.ToInt32(Session["WORKSHEET_CONTENT_ID"]);

                foreach (EN_WizardStructs.TablaItemProveedor i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();

                    Respuesta r = _objService.Wizard_Task120(stores, $"120-Cambio Prov. Costos-{UserName(26)}", notes, DateTime.Now, items, UserID(), cID);
                    respuestas.Add(r);


                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cambio precios: Dinámicos
        [HttpPost]
        [WizardAuthorization(Permissions = "121")]
        public JsonResult Task121(int[] stores, string notes, EN_WizardStructs.TablaItemPriceDynamic[] items)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemPriceDynamic i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task121(i, $"121-Prec: Descuentos-{UserName(39)}", notes, DateTime.Now, items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  " La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, " Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cambio precios: Promoción Compre X lleve Y por Z Descuento
        [WizardAuthorization(Permissions = "122")]
        public JsonResult GetInitDataTask122()
        {
            try
            {
                Session["WORKSHEET_CONTENT_ID"] = null;
                List<EN_Supplier> supps = _objService.GetAllSupplier(StoreAuthorizationAttribute.StoresAvailable(Session), "", 0, 0).ToList();
                List<EN_QuantityDiscount> qds = _objService.GetAllSimpleQuantityDiscountsByType(4).ToList();

                return Json(new Respuesta("", "", new { suppliers = supps, quantityDiscounts = qds }, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [WizardAuthorization(Permissions = "122")]
        public JsonResult Task122(int[] stores, string notes, int quantityDiscountID, string startDate, string endDate, EN_WizardStructs.TablaItemQuantityDiscount[] items)
        {
            try
            {
                bool status = true;

                DateTime sDate = DateTime.Parse(startDate);
                DateTime eDate = DateTime.Parse(endDate);

                foreach (EN_WizardStructs.TablaItemQuantityDiscount i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();
                    foreach (int i in stores)
                    {
                        Respuesta r = _objService.Wizard_Task122(i, $"122-Prec: Desc. Codicion-{UserName(25)}", notes, quantityDiscountID, sDate, eDate, items, UserID());
                        respuestas.Add(r);
                    }

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cambio de precios: Regular
        [HttpPost]
        [WizardAuthorization(Permissions = "123")]
        public JsonResult Task123(int[] stores, string notes, EN_WizardStructs.TablaItemPriceRegular[] items)
        {
            try
            {
                bool status = true;

                foreach (EN_WizardStructs.TablaItemPriceRegular i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();

                    Respuesta r = _objService.Wizard_Task123(stores, $"123-Cambio Prec: Regular-{UserName(25)}", notes, DateTime.Now, items, UserID());
                    respuestas.Add(r);

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }

        [WizardAuthorization(Permissions = "123")]
        public JsonResult Task123Verificar(string[] itemLookupCodes)
        {
            try
            {
                Session["WORKSHEET_CONTENT_ID"] = null;
                List<Respuesta> respuestas = new List<Respuesta>();
                var str = string.Join(",", itemLookupCodes);
                Respuesta respuesta;

                List<EN_Item> item = _objService.GetItemByItemLookupCodebyLista(str, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList();

                respuesta = ((item == null) ? new Respuesta("PRODUCTO NO EXISTE", $"No se encontró el producto en el Sistema.", "", false) : new Respuesta("", "", item, true));
                respuestas.Add(respuesta);

                var jsonResult = Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cambio de precios: Margen de Utilidad
        [WizardAuthorization(Permissions = "124")]
        public JsonResult Task124Verificar(string[] itemLookupCodes)
        {
            try
            {
                Session["WORKSHEET_CONTENT_ID"] = null;
                List<Respuesta> respuestas = new List<Respuesta>();
                var str = string.Format("{0}", string.Join(",", itemLookupCodes));
                Respuesta respuesta;

                List<EN_Item> item = _objService.GetItemByItemLookupCodebyLista(str, StoreAuthorizationAttribute.StoresAvailable(Session)).ToList();

                respuesta = ((item == null) ? new Respuesta("PRODUCTO NO EXISTE", $"No se encontró el producto en el Sistema.", "", false) : new Respuesta("", "", item, true));
                respuestas.Add(respuesta);

                //foreach (string i in itemLookupCodes)
                //{
                //    EN_Item item = _objService.GetItemByItemLookupCode(i);

                //    respuesta = ((item == null) ? new Respuesta("PRODUCTO NO EXISTE", $"No se econtró el producto ({i}) en el Sistema.", i, false) : new Respuesta("", "", item, true));

                //    respuestas.Add(respuesta);
                //}

                var jsonResult = Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [WizardAuthorization(Permissions = "124")]
        public JsonResult Task124(int[] stores, EN_WizardStructs.TablaMargenUtility[] items, string notes)
        {
            try
            {
                bool status = true;
                EN_WizardStructs.TablaMargenUtility item = new EN_WizardStructs.TablaMargenUtility();
                foreach (EN_WizardStructs.TablaMargenUtility i in items)
                {
                    if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode))
                    {
                        item = i;
                        status = false;
                        break;
                    }
                }

                if (status)
                {
                    Respuesta respuesta = _objService.Wizard_Task124(stores, $"124-Cambio Prec: Utilidad-{UserName(24)}", notes, DateTime.Now, items, UserID());

                    return Json(respuesta, JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Ingresado de productos
        [WizardAuthorization(Permissions = "130")]
        public JsonResult GetInitDataTask130()
        {
            try
            {
                List<EN_Supplier> supps = _objService.GetAllSupplier(StoreAuthorizationAttribute.StoresAvailable(Session), "", 0, 0).ToList();
                List<EN_ExtCentral_Family> fams = _objService.GetAllFamilies("", 0, 0).ToList();
                List<EN_Department> deps = _objService.GetAllDepartments("", 0, 0).ToList();
                List<EN_Category> cats = _objService.GetAllCategories("", 0, 0).ToList();
                List<EN_ExtCentral_SubCategory> subcats = _objService.GetAllSubCategories("", 0, 0).ToList();
                List<EN_ExtCentral_Segment> segs = _objService.GetAll_ExtCentral_Segment().ToList();
                List<EN_Tax> ts = _objService.GetAllTax("", 0, 0).ToList();
                List<EN_UOM> uoms = _objService.GetAllUOMByInactive(false).ToList();
                List<EN_ExtCentral_Manufacturer> mans = _objService.GetAllManufacturers().ToList();
                List<EN_ExtCentral_Brand> brands = _objService.GetAllBrands().ToList();
                List<EN_Purchaser> purchasers = _objService.GetAllPurchasers().ToList();

                return Json(
                    new Respuesta("", "",
                        new
                        {
                            suppliers = supps,
                            families = fams,
                            departments = deps,
                            categories = cats,
                            subCategories = subcats,
                            segments = segs,
                            taxs = ts,
                            uoms = uoms,
                            manufacturers = mans,
                            brands = brands,
                            purchasers = purchasers
                        }, true
                ), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [WizardAuthorization(Permissions = "130")]
        public JsonResult Task130ValidateItemCodes(string[] itemLookupCodes)
        {
            try
            {
                Session["WORKSHEET_CONTENT_ID"] = null;

                List<Respuesta> respuestas = _objService.ValidateItemsExistsList(itemLookupCodes).ToList();

                return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [WizardAuthorization(Permissions = "130")]
        public JsonResult Task130(int[] storesID, string notes)
        {
            try
            {
                var data = Request.Form.GetValues("items")[0];

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                List<EN_WizardStructs.TablaItem> list = JsonConvert.DeserializeObject<List<EN_WizardStructs.TablaItem>>(data, settings);

                bool status = true;

                foreach (EN_WizardStructs.TablaItem i in list) if (String.IsNullOrEmpty(i.ItemLookupCode) || String.IsNullOrEmpty(i.Description) || String.IsNullOrEmpty(i.DescriptionExtended) || String.IsNullOrEmpty(i.Subdescription3)) status = false;

                if (status)
                {
                    Respuesta r = _objService.Wizard_Task130(storesID, $"130-Importar Productos-{UserName(27)}", notes, DateTime.Now, DateTime.Now, list.ToArray(), UserID());

                    return Json(r, JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                  "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                  , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [WizardAuthorization(Permissions = "130")]
        public JsonResult WizardDownloadItems(int[] storesID, string[] items, string notes)
        {
            try
            {
                int cID = Convert.ToInt32(Session["WORKSHEET_CONTENT_ID"]);

                Respuesta respuesta = _objService.Wizard_Download_Items_Stores(storesID, $"130-Importar Productos-{UserName(27)}", notes, DateTime.Now, DateTime.Now, items, UserID(), cID);

                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new Respuesta("", "Error al intentar sincronizar los productos a las tiendas respectivas.", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Activación de Productos
        [WizardAuthorization(Permissions = "131")]
        public JsonResult Task131Verificar(string[] itemLookupCodes)
        {
            try
            {
                Session["WORKSHEET_CONTENT_ID"] = null;
                List<Respuesta> respuestas = new List<Respuesta>();
                var str = string.Join(",", itemLookupCodes);
                Respuesta respuesta;

                List<EN_Item> item = _objService.GetItemByItemLookupCodebyLista(str, "%").ToList();

                respuesta = ((item == null) ? new Respuesta("PRODUCTO NO EXISTE", $"No se encontró el producto en el Sistema.", "", false) : new Respuesta("", "", item, true));
                respuestas.Add(respuesta);

                var jsonResult = Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;

                return jsonResult;
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.ToString(), Resources.messages.error_interno, null, false), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [WizardAuthorization(Permissions = "131")]
        public JsonResult Task131(int[] stores, string notes, EN_WizardStructs.TablaItemActivate[] items)
        {
            try
            {
                bool status = true;

                int cID = Convert.ToInt32(Session["WORKSHEET_CONTENT_ID"]);

                foreach (EN_WizardStructs.TablaItemActivate i in items) if (i.ID <= 0 || String.IsNullOrEmpty(i.ItemLookupCode)) status = false;

                if (status)
                {
                    List<Respuesta> respuestas = new List<Respuesta>();

                    Respuesta r = _objService.Wizard_Task131(stores, $"131-Cambio Costo y Activar-{UserName(23)}", notes, DateTime.Now, items, UserID(), cID);
                    respuestas.Add(r);

                    return Json(new Respuesta("", "", respuestas, true), JsonRequestBehavior.AllowGet);
                }
                else return Json(new Respuesta("",
                 "La información recibida está dañada o algunos productos con contienen la información requerida para aplicar el proceso."
                 , null, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new Respuesta(e.Message, "Error interno", null, false), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cambio de Propiedades
        [HttpGet]
        [WizardAuthorization(Permissions = "201")]
        public JsonResult Task201_GetProperties()
        {
            List<EN_ItemCustomProperty> props = _objService.GetCustomProperty(
                PropertyAuthorizationAttribute.PropertiesAvailable(Session)
            ).ToList();

            return Json(props, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [WizardAuthorization(Permissions = "201")]
        public JsonResult Task201Verificar(string[] items, string propsSelected)
        {
            Respuesta respuesta;
            Session["WORKSHEET_CONTENT_ID"] = null;

            try
            {
                List<EN_ItemProperty> list = _objService.GetAllItemsProperties_By_List(String.Join(",", items), StoreAuthorizationAttribute.StoresAvailable(Session), propsSelected).ToList();

                respuesta = new Respuesta("", list.Count == items.Length ? "Productos Verificados" : "Uno o varios productos no lograron ser verificados, valide que los códigos sean correctos y que los productos sean accesibles a su rol.", list, list.Count == items.Length);
            } catch(Exception e)
            {
                respuesta = new Respuesta(e.Message, "Error Interno. Error al verificar los productos", null, false);
            }

            return Json(respuesta, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [WizardAuthorization(Permissions = "201")]
        public JsonResult Task201(EN_WizardStructs.TablaItemProperties[] items, string propsSelected, string stores, string notes)
        {
            int cID = Convert.ToInt32(Session["WORKSHEET_CONTENT_ID"]);

            for (int i = 0; i < items.Length; i++)
            {
                items[i].Properties = items[i].Properties.Replace("[{", "<");
                items[i].Properties = items[i].Properties.Replace("}]", ">");
            }

            Respuesta respuesta = _objService.EN_Worksheet_Change_Item_Properties(stores, propsSelected, cID, UserID(), $"201-Cambio de Propiedades-{UserName(24)}", notes, DateTime.Now, items);

            _objService.Close();

            return Json(respuesta);
        }
        #endregion
    }
}