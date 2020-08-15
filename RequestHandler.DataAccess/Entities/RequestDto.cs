namespace RequestHandler.DataAccess.Entities
{
    public class RequestDto
    {
        public string _Status { get; set; }
        public string _Currency { get; set; }
        public decimal _Amount { get; set; }
    }
    public class RequestDtoo
    {
        public string Status { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    public class RequestByClient
    {
        public int Id { get; set; }
        public string DepartmentAddress { get; set; }
    }
}