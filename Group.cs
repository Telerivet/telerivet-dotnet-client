
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
      
      - dynamic (bool)
          * Whether this is a dynamic or normal group
          * Read-only
      
      - num_members (int)
          * Number of contacts in the group (null if the group is dynamic)
          * Read-only
      
      - time_created (UNIX timestamp)
          * Time the group was created in Telerivet
          * Read-only
      
      - allow_sending (bool)
          * True if messages can be sent to this group, false otherwise.
          * Updatable via API
      
      - add_time_variable (string)
          * Variable name of a custom contact field that will automatically be set to the
              current date/time on any contact that is added to the group. This variable will only
              be set if the contact does not already have a value for this variable.
          * Updatable via API
      
      - vars (JObject)
          * Custom variables stored for this group. Variable names may be up to 32 characters in
              length and can contain the characters a-z, A-Z, 0-9, and _.
              Values may be strings, numbers, or boolean (true/false).
              String values may be up to 4096 bytes in length when encoded as UTF-8.
              Up to 100 variables are supported per object.
              Setting a variable to null will delete the variable.
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

    public string Name
    {
      get {
          return (string) Get("name");
      }
      set {
          Set("name", value);
      }
    }

    public bool Dynamic
    {
      get {
          return (bool) Get("dynamic");
      }
    }

    public int? NumMembers
    {
      get {
          return (int?) Get("num_members");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public bool AllowSending
    {
      get {
          return (bool) Get("allow_sending");
      }
      set {
          Set("allow_sending", value);
      }
    }

    public string AddTimeVariable
    {
      get {
          return (string) Get("add_time_variable");
      }
      set {
          Set("add_time_variable", value);
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
        return "/projects/" + ProjectId + "/groups/" + Id + "";
    }

    public Group(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}
