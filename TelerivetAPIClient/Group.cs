
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a group used to organize contacts within Telerivet.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the group
          * Read-only
      
      - name
          * Name of the group
          * Updatable via API
      
      - num_members (int)
          * Number of contacts in the group
          * Read-only
      
      - time_created (UNIX timestamp)
          * Time the group was created in Telerivet
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this group
          * Updatable via API
      
      - project_id
          * ID of the project this group belongs to
          * Read-only
*/

public class Group : Entity
{
    /**
        Queries contacts that are members of the given group.
    */
    public APICursor<Contact> QueryContacts(JObject options = null)
    {
        return api.NewCursor<Contact>(GetBaseApiPath() + "/contacts", options);
    }

    /**
        Queries scheduled messages to the given group.
    */
    public APICursor<ScheduledMessage> QueryScheduledMessages(JObject options = null)
    {
        return api.NewCursor<ScheduledMessage>(GetBaseApiPath() + "/scheduled", options);
    }

    /**
        Saves any fields that have changed for this group.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Deletes this group (Note: no contacts are deleted.)
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

    public int NumMembers
    {
      get {
          return (int) Get("num_members");
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
        return "/projects/" + ProjectId + "/groups/" + Id + "";
    }

    public Group(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
