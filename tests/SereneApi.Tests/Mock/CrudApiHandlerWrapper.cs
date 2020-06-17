namespace SereneApi.Tests.Mock
{
    public class CrudApiHandlerWrapper: CrudApiHandler<MockPersonDto, long>
    {
        public CrudApiHandlerWrapper(IApiHandlerOptions options) : base(options)
        {
        }
    }
}
