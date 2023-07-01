
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents a custom route that can be used to send messages via one or more basic routes
    (phones).
    
    Custom Routes were formerly referred to simply as "Routes" within Telerivet. API methods,
    parameters, and properties related to Custom Routes continue to use the term "Route" to
    maintain backwards compatibility.
    
    Custom routing rules can currently only be configured via Telerivet's web
    UI.
    
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
        Saves any fields or custom variables that have changed for this custom route.
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

    public string Name
    {
      get {
          return (string) Get("name");
      }
      set {
          Set("name", value);
      }
    }

    public string ProjectId
    {
      get {
          return (string) Get("project_id");
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
