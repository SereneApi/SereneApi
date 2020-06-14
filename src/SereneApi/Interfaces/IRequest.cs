namespace SereneApi.Interfaces
{
    public interface IRequest : IRequestContent
    {
        IRequestContent WithEndPoint(object parameter = null);
        IRequestContent WithEndPoint(string endPoint);
        IRequestContent WithEndPoint(string endPointTemplate, params object[] templateParameters);
    }
}
