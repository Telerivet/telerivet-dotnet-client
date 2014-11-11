using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
    public abstract class Entity
    {
        protected TelerivetAPI api;
        protected JObject data;
        protected bool isLoaded;
        protected CustomVars vars;
        protected JObject dirty = new JObject();

        public Entity(TelerivetAPI api, JObject data, bool isLoaded = true)
        {
            this.api = api;
            SetData(data);
            this.isLoaded = isLoaded;
        }

        public override string ToString()
        {
            String res = this.GetType().Name;
            if (!isLoaded)
            {
                res += " (not loaded)";
            }
            res += " JSON: " + data.ToString();
            return res;
        }

        public virtual void SetData(JObject data)
        {
            this.data = data;

            JObject varsData = (JObject)data["vars"];

            if (varsData == null)
            {
                varsData = new JObject();
            }
            
            this.vars = new CustomVars(varsData);
        }

        public CustomVars Vars
        {
            get
            {
                return vars;
            }
        }

        public JToken Get(string name)
        {
            JToken res = data[name];
            
            if (res == null)
            {
                AssertLoaded();
            }
            
            return res;
        }

        public void Set(string name, object value)
        {
            dirty[name] = data[name] = JToken.FromObject(value);
        }

        public virtual async Task SaveAsync()
        {
            JObject dirtyProps = dirty;

            if (vars != null)
            {
                JObject dirtyVars = vars.getDirtyVariables();
                if (dirtyVars.Count  > 0)
                {
                    dirtyProps["vars"] = dirtyVars;
                }
            } 
            await api.DoRequestAsync("POST", GetBaseApiPath(), dirtyProps);
            dirty = new JObject();
            
            if (vars != null)
            {
                vars.ClearDirtyVariables();
            }
        }

        protected void AssertLoaded()
        {
            if (!isLoaded)
            {
                throw new Exception("Entity data is not loaded yet; await LoadAsync() first");
            }
        }
        
        public abstract string GetBaseApiPath();

        public async Task LoadAsync() 
        {
            if (!isLoaded)
            {
                isLoaded = true;
                SetData((JObject) await api.DoRequestAsync("GET", GetBaseApiPath()));
            
                foreach (KeyValuePair<string, JToken> kvp in dirty)
                {
                    data[kvp.Key] = kvp.Value;
                }
            }        
        }
    }

    public class CustomVars
    {
        private JObject dirty = new JObject();
        private JObject vars;

        public CustomVars(JObject vars)
        {
            this.vars = vars;
        }

        public JObject all()
        {
            return vars;
        }

        public JObject getDirtyVariables()
        {
            return dirty;
        }

        public void ClearDirtyVariables()
        {
            dirty = new JObject();
        }

        public JToken Get(String name)
        {
            return vars[name];
        }

        public long GetLong(String name)
        {
            return (long)Get(name);
        }

        public int GetInt(String name)
        {
            return (int)Get(name);
        }

        public bool GetBoolean(String name)
        {
            return (bool)Get(name);
        }

        public String GetString(String name)
        {
            return (String)Get(name);
        }

        public void Set(String name, Object value)
        {
            dirty[name] = vars[name] = JToken.FromObject(value);
        }
    }
}
