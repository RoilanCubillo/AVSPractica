using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraERP.BusinessDataAccess.DataAccessIntegration;
using UltraERP.BusinessEntities;

namespace UltraERP.BusinessLogic
{
    public class CT_Wizard
    {
        public CT_Wizard() { }

        /** Cambio de costos */
        public Respuesta Task102(int storeID, string title, string notes, List<EN_WizardStructs.TablaCost> items, string userID)
        {
            return new DT_Wizard().Task102(title, notes, storeID, items, userID);
        }

        /** Cambio de Impuestos */
        public Respuesta Task104(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemTax> items, string userID)
        {
            return new DT_Wizard().Task104(title, notes, storeID, items, userID);
        }

        /** Cambio de Estados */
        public Respuesta Task105(int storeID, string title, string notes, List<EN_WizardStructs.TablaEstado> items, string userID)
        {
            return new DT_Wizard().Task105(title, notes, storeID, items, userID);
        }

        /** Cambio de Descripción */
        public Respuesta Task107(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemDes> items, string userID)
        {
            return new DT_Wizard().Task107(title, notes, storeID, items, userID);
        }

        /** Cambio de Descripción Larga */
        public List<Respuesta> Task108(int[] stores, string title, string notes, List<EN_WizardStructs.TablaItemSub1> items, string userID)
        {
            return new DT_Wizard().Task108(stores, title, notes, items, userID);
        }

        /** Cambio de Subdescripción2 */
        public Respuesta Task109(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub2> items, string userID)
        {
            return new DT_Wizard().Task109(storeID, title, notes, items, userID);
        }

        /** Cambio de Subdescripción3 (Cabys) */
        public Respuesta Task110(int storeID, string title, string notes, List<EN_WizardStructs.TablaItemSub3> items, string userID)
        {
            return new DT_Wizard().Task110(storeID, title, notes, items, userID);
        }

        /** Cambio de Proveedor - Costos */
        public Respuesta Task120(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProveedor> items, string userID, int workSheetContentID)
        {
            return new DT_Wizard().Task120(storesID, tittle, notes, effectiveDate, items, userID, workSheetContentID);
        }

        /** Cambio precios: Dinámicos */
        public Respuesta Task121(int storeID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceDynamic> items, string userID)
        {
            return new DT_Wizard().Task121(storeID, title, notes, effectiveDate, items, userID);
        }

        /** Cambio precios: Descuentos Compre X lleve Y por Z porcentaje de descuento */
        public Respuesta Task122(int storeID, string title, string notes, int quantityDiscountID, DateTime startDate, DateTime endDate, List<EN_WizardStructs.TablaItemQuantityDiscount> items, string userID)
        {
            return new DT_Wizard().Task122(storeID, title, notes, quantityDiscountID, startDate, endDate, items, userID);
        }
        
        /** Cambio precios: Regular */
        public Respuesta Task123(int[] storesID, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemPriceRegular> items, string userID)
        {
            return new DT_Wizard().Task123(storesID, title, notes, effectiveDate, items, userID);
        }

        /** Cambio precios: Margen de Utilidad */
        public Respuesta Task124(int[] stores, string title, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaMargenUtility> items, string userID)
        {
            return new DT_Wizard().Task124(stores, title, notes, effectiveDate, items, userID);
        }

        /** Imprtar productos */
        public Respuesta Task130(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, List<EN_WizardStructs.TablaItem> items, string userID)
        {
            return new DT_Wizard().Task130(storesID, title, notes, effectiveDate, fromDate, items, userID);
        }

        /** Activación de productos */
        public Respuesta Task131(int[] storesID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemActivate> items, string userID, int workSheetContentID)
        {
            return new DT_Wizard().Task131(storesID, tittle, notes, effectiveDate, items, userID, workSheetContentID);
        }

        /** Sincroniza el listado de productos a las respectivas tiendas */
        public Respuesta Wizard_Download_Items_Stores(int[] storesID, string title, string notes, DateTime effectiveDate, DateTime fromDate, string[] items, string userID, int workSheetContentID)
        {
            return new DT_Wizard().Wizard_Download_Items_Stores(storesID, title, notes, effectiveDate, fromDate, items, userID, workSheetContentID);
        }

        /** Cambio de propiedades */
        public Respuesta Task201(string stores, string properties, int dataContentID, string userID, string tittle, string notes, DateTime effectiveDate, List<EN_WizardStructs.TablaItemProperties> items)
        {
            return new DT_Wizard().Task201(stores, properties, dataContentID, userID, tittle, notes, effectiveDate, items);
        }
    }
}
