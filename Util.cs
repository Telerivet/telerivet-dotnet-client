using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
    public class Util
    {
        public static JObject Options(params Object[] parameters)
        {
            JObject options = new JObject();
            int numParams = parameters.Length;

            for (int i = 0; i < numParams - 1; i += 2)
            {
                String paramName = parameters[i].ToString();
                Object paramValue = parameters[i + 1];

                options.Add(paramName, JToken.FromObject(paramValue));
            }
            return options;
        }
    }
}
