
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a custom route that can be used to send messages via one or more Phones.
    
    Note: Routing rules can currently only be configured via Telerivet's web UI.
    
    Fields:
    
      - id (string, max 34 characters)
          * Telerivet's internal ID for the route
          * Read-only
      
      - name
          * The name of the route
          * Updatable via API
      
      - vars (JObject)
          * Custom variables stored for this route
          * Updatable via API
      
      - project_id
          * ID of the project this route belongs to
          * Read-only
*/

public class Route : Entity
{
    /**
        Saves any fields or custom variables that have changed for this route.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
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

    public String ProjectId
    {
      get {
          return (String) Get("project_id");
      }
    }

    public override string GetBaseApiPath()
    {
        return "/projects/" + ProjectId + "/routes/" + Id + "";
    }

    public Route(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
