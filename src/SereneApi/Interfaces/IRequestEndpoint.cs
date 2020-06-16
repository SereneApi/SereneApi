namespace SereneApi.Interfaces
{
    public interface IRequestEndPoint: IRequestContent
    {
        IRequestContent WithEndPoint(object parameter = null);
        IRequestContent WithEndPoint(string endPoint);

        IRequestContent WithEndPointTemplate(string template, params object[] parameters);
    }
}
