namespace SereneApi.Handlers.Soap.Models
{
    public interface ISoapEnvelope
    {
        public object Body { get; set; }
        public object Header { get; set; }
    }
}