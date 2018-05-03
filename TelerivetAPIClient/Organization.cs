
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents a Telerivet organization.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the organization
          * Read-only
      
      - name
          * Name of the organization
          * Updatable via API
      
      - timezone_id
          * Billing quota time zone ID; see
              <http://en.wikipedia.org/wiki/List_of_tz_database_time_zones>
          * Updatable via API
*/

public class Organization : Entity
{
    /**
        Saves any fields that have changed for this organization.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Retrieves information about the organization's service plan and account balance.
    */
    public async Task<JObject> GetBillingDetailsAsync()
    {
        return (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/billing");
    }

    /**
        Retrieves the current usage count associated with a particular service plan limit. Available
        usage types are `phones`, `projects`, `users`, `contacts`, `messages_day`,
        `stored_messages`, `data_rows`, and `api_requests_day`.
    */
    public async Task<int> GetUsageAsync(string usage_type)
    {
        return Convert.ToInt32(await api.DoRequestAsync("GET", GetBaseApiPath() + "/usage/" + usage_type));
    }

    /**
        Queries projects in this organization.
    */
    public APICursor<Project> QueryProjects(JObject options = null)
    {
        return api.NewCursor<Project>(GetBaseApiPath() + "/projects", options);
    }

    public string Id
    {
      get {
          return (string) Get("id");
      }
    }

    public String Name
    {
      get {
          return (String) Get("name");
      }
      set {
          Set("name", value);
      }
    }

    public String TimezoneId
    {
      get {
          return (String) Get("timezone_id");
      }
      set {
          Set("timezone_id", value);
      }
    }

    public override string GetBaseApiPath()
    {
        return "/organizations/" + Id + "";
    }

    public Organization(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}
