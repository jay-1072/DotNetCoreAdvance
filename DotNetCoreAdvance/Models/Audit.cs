namespace DotNetCoreAdvance.Models
{
    public class Audit : BaseEntity
    {
        public string Module { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public string RequestObject { get; set; } = string.Empty;
        public string ResponseObject { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Message {  get; set; } = string.Empty;
        public DateTime LoggedDate { get; set; } = DateTime.UtcNow;
        public LogLevel? LogLevel { get; set; } = null;
    }
}