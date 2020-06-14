namespace SereneApi.Tests.Mock
{
    public class TestCrudApiHandler : CrudApiHandler<MockPersonDto, long>
    {
        public TestCrudApiHandler(IApiHandlerOptions options) : base(options)
        {
        }
    }
}
