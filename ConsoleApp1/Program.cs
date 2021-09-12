using SereneApi.Core.Options;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Soap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SereneApi.Core.Handler.Factories;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly ApiFactory _apiFactory = new ApiFactory();

        static void Main(string[] args)
        {
            _apiFactory.RegisterApi<IMemberApi, MemberApiHandler>(o =>
            {
                o.SetSource("http://mfssydacr24:8080", "Member", "AcurityWebServices");
            });

            IMemberApi memberApi = _apiFactory.Build<IMemberApi>();

            memberApi.GetMemberAndClientDetail("001234");

            Console.WriteLine("Hello World!");
        }
    }

    public interface IMemberApi
    {
        Task<IApiResponse<object>> GetMemberAndClientDetail(string memberNo);
    }

    public class MemberApiHandler : SoapApiHandler, IMemberApi
    {
        public MemberApiHandler(IApiOptions<MemberApiHandler> options) : base(options)
        {

        }

        public Task<IApiResponse<object>> GetMemberAndClientDetail(string memberNo)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {"Fund", "MASP"}, {"MemberNo", memberNo}
            };

            return MakeRequest
                .AgainstService("getMemberAndClientDetails00002")
                .WithParameters(parameters)
                .RespondsWith<object>()
                .ExecuteAsync();
        }
    }
}
