using System;

namespace UltraERP.Models
{
    public class LogEntryViewModel
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserAccount { get; set; }
        public string Screen { get; set; }
        public string ActionName { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetail { get; set; }
        public string RequestData { get; set; }
        public string Url { get; set; }
    }
}
