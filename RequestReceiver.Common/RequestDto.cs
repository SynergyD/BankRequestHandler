using System;

namespace RequestReceiver.Common
{
    public class RequestDto
    {
        public decimal _Amount { get; set; }
        public string _Currency { get; set; }
        public string _Status { get; set; }
    }

    public class RequestByClient
    {
        public int Id { get; set; }
        public string DepartmentAddress { get; set; }
    }
}