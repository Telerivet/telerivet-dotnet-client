using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Telerivet.Client
{
    public class APICursor<T> where T : Entity
    {
        private int? limit = null;
        private String nextMarker;
        private JArray data;
        private bool truncated;
        int pos = -1;
        int offset = 0;

        private int? count = null;

        private TelerivetAPI api;
        private string path;
        private JObject parameters;
        private ConstructorInfo ctor;

        public APICursor(TelerivetAPI api, string path, JObject parameters)
        {
            if (parameters == null)
            {
                parameters = new JObject();
            }

            if (parameters["count"] != null)
            {
                throw new ArgumentException("Cannot construct APICursor with 'count' parameter. Call the count() method instead.");
            }

            this.api = api;
            this.path = path;
            this.parameters = parameters;

            this.ctor = typeof(T).GetConstructor(new Type[] {
                typeof(TelerivetAPI),
                typeof(JObject),
                typeof(bool)
            });
        }
   
        public APICursor<T> Limit(int limit)
        {
            this.limit = limit;
            return this;
        }

        public async Task<int> CountAsync()
        {
            if (count == null)
            {
                JObject requestParams = new JObject(parameters);
                requestParams["count"] = 1;
                JObject res = (JObject) await api.DoRequestAsync("GET", path, requestParams);
                count = (int) res["count"];
            }
            return count.Value;
        }

        public async Task<List<T>> AllAsync()
        {
            List<T> res = new List<T>();

            while (true)
            {
                T item = await NextAsync();
                if (item == null)
                {
                    break;
                }
                res.Add(item);
            }

            return res;
        }

        public async Task<T> NextAsync()
        {    
            if (limit != null && offset >= limit)
            {
                return null;
            }
        
            if (data == null || (pos >= data.Count && truncated))
            {                
                await loadNextPage();
            }
        
            if (pos < data.Count)
            {
                JObject itemData = (JObject) data[pos];
                pos += 1;
                offset += 1;
            
                return (T)ctor.Invoke(new object[] { api, itemData, true });    
            }
            else
            {
                return null;
            }
        }

        private async Task loadNextPage()
        {
            JObject requestParams = new JObject(parameters);

            if (nextMarker != null)
            {
                requestParams["marker"] = nextMarker;
            }


            if (limit != null && requestParams["page_size"] == null)
            {
                requestParams["page_size"] = Math.Min(limit.Value, 200);
            }

            JObject response = (JObject) await api.DoRequestAsync("GET", path, requestParams);

            data = (JArray) response["data"];
            truncated = (bool) response["truncated"];
            nextMarker = (String) response["next_marker"];

            pos = 0;
        }
    
    }
}
