using System;

namespace RequestHandler.DataAccess.Entities
{
    public class Request
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public string IpAddress { get; set; }
        public string DepartmentAddress { get; set; }
        public string Status { get; set; }
    }
}