using SereneApi.Interfaces;
using System;
using System.Text;

namespace SereneApi.Types.Headers.Authentication
{
    public class BasicAuthentication: IAuthentication
    {
        public string Scheme => "Basic";

        public string Parameter { get; }

        public BasicAuthentication(string username, string password)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");

            Parameter = Convert.ToBase64String(byteArray);
        }
    }
}
