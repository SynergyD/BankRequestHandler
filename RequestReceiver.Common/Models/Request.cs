namespace RequestReceiver.Common.Models
{
    public class Request
    {
        public int ClientId { get; set; }
        public string Currency { get; set; }
        public string DepartmentAddress { get; set; }
        public int Amount { get; set; }
        public string IpAddress { get; set; }
        
    }
}