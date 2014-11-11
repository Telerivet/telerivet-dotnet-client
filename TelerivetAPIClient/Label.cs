
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a label used to organize messages within Telerivet.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the label
          * Read-only
      
      - name
          * Name of the label
          * Updatable via API
      
      - time_created (UNIX timestamp)
          * Time the label was created in Telerivet
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this label
          * Updatable via API
      
      - project_id
          * ID of the project this label belongs to
          * Read-only
*/

public class Label : Entity
{
    /**
        Queries messages with the given label.
    */
    public APICursor<Message> QueryMessages(JObject options = null)
    {
        return api.NewCursor<Message>(GetBaseApiPath() + "/messages", options);
    }

    /**
        Saves any fields that have changed for the label.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Deletes the given label (Note: no messages are deleted.)
    */
    public async Task DeleteAsync()
    {
        await api.DoRequestAsync("DELETE", GetBaseApiPath());
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

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
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
        return "/projects/" + ProjectId + "/labels/" + Id + "";
    }

    public Label(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
