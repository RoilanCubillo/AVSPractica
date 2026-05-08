using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraERP.BusinessEntities
{
    public class EN_Worksheet
    {
        public int ID { get; set; }
        public int Style { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string EffectiveDateString {  get { return EffectiveDate == null ? "" : EffectiveDate.Value.ToString("dd/MM/yyyy HH:mm"); } }
        public DateTime? DateApplied { get; set; }
        public string DateAppliedString {  get { return DateApplied == null ? "" : DateApplied.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string TaskDescription { get; set; }
        public string TaskCode { get; set; }
        public string StroresID { get; set; }
        public string StoresName { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
        public int WorksheetContentID { get; set; }

        public abstract class WorksheetStatus
        {
            public int ID { get; set; }
            public int WorksheetID { get; set; }
            public int StoreID { get; set; }
            public string StoreName { get; set; }
        }

        public abstract class WorksheetDetail : WorksheetStatus
        {
            public int ItemID { get; set; }
            public string ItemLookupcode { get; set; }
            public string Description { get; set; }
            public string ExtendedDescription { get; set; }
            public bool StoreAvailibity { get; set; }
        }

        /**
         * Para almacenar el histórico de movimientos de la hoja de trabajo.
         */
        public class WorksheetHistory : WorksheetStatus
        {
            public int Status { get; set; }
            public DateTime? HistoryDate { get; set; }
            public string HistoryDateString { get { return HistoryDate == null ? "" : HistoryDate.Value.ToString("dd/MM/yyyy HH:mm:ss"); } }
            public string Comment { get; set; }
        }

        /**
         * Para almacenar el listado de tiendas relacionados a la hoja de trabajo.
         */
        public class WorksheetStore : WorksheetStatus
        {
            public int Status { get; set; }
            public DateTime? DateProcessed { get; set; }
        }

        /**
         * Para almacenar el contenido de la hoja de trabajo cuando es estilo=251.
         */
        public class WorksheetUpdateItemPrice : WorksheetDetail
        {
            public string ItemTaxDescription { get; set; }
            public decimal SalePrice { get; set; }
            public DateTime? SaleStartDate { get; set; }
            public DateTime? SaleEndDate { get; set; }
            public string SaleStartDateString { get { return SaleStartDate == null ? "" : SaleStartDate.Value.ToString("dd/MM/yyyy"); } }
            public string SaleEndDateString { get { return SaleEndDate == null ? "" : SaleEndDate.Value.ToString("dd/MM/yyyy"); } }
            public decimal LowerBound { get; set; }
            public decimal UpperBound { get; set; }
            public decimal BuydownPrice { get; set; }
            public double BuydownQuantity { get; set; }
        }

        /**
         * Para almacenar el contenido de la hoja de trabajo cuando es estilo=261.
         */
        public class Worksheet_ItemUpdate : WorksheetDetail { }

        /**
         * Para almacenar el contenido de la hoja de trabajo cuando es estilo=320.
         */
        public class Worksheet_ItemTax : WorksheetDetail
        {
            public string ItemTaxDescription { get; set; }
        }

        public class WorksheetContent
        {
            public int ID { get; set; }
            public int HQUserID { get; set; }
            public DateTime DateApplied { get; set; }
            public string DateAppliedStringShort { get { return DateApplied.ToString("dd/MM/yyyy"); } }
            public string DateAppliedStringLarge { get { return DateApplied.ToString("dd/MM/yyyy HH:mm:ss"); } }
            public string DateAppliedStringFile { get { return DateApplied.ToString("ddMMyyyyHHmmss"); } }
            public string TaskCode { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public long ContentLenght { get; set; }
            public byte[] ContentData { get; set; }
        }
    }
}
