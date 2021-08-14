using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Soap.Parameters
{
    internal class ParameterService: IParameterService
    {
        public Dictionary<string, string> GetParameters<TParam>(TParam parameters) where TParam : class
        {
            Type parameterType = typeof(TParam);

            PropertyInfo[] properties = parameterType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(parameters);
            }
        }
    }
}
