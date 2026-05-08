namespace UltraERP.Models
{
    public class JsonResponse
    {
        public JsonResponse()
        {
        }

        public JsonResponse(string internalMessage, string message, object result, bool status)
        {
            InternalMessage = internalMessage;
            Message = message;
            Result = result;
            Status = status;
        }

        public string InternalMessage { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public bool Status { get; set; }
    }
}
