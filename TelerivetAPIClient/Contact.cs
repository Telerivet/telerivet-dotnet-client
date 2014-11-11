
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Fields:
    
      - id (string, max 34 characters)
          * ID of the contact
          * Read-only
      
      - name
          * Name of the contact
          * Updatable via API
      
      - phone_number (string)
          * Phone number of the contact
          * Updatable via API
      
      - time_created (UNIX timestamp)
          * Time the contact was added in Telerivet
          * Read-only
      
      - last_message_time (UNIX timestamp)
          * Last time the contact sent or received a message (null if no messages have been sent
              or received)
          * Read-only
      
      - last_message_id
          * ID of the last message sent or received by this contact (null if no messages have
              been sent or received)
          * Read-only
      
      - default_route_id
          * ID of the phone or route that Telerivet will use by default to send messages to this
              contact (null if using project default route)
          * Updatable via API
      
      - group_ids (array of strings)
          * List of IDs of groups that this contact belongs to
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this contact
          * Updatable via API
      
      - project_id
          * ID of the project this contact belongs to
          * Read-only
 */
public class Contact : Entity 
{    
    /**
        Returns true if this contact is in a particular group, false otherwise.
     */
    public bool IsInGroup(Group group)
    {
        AssertLoaded();
        return groupIdsSet.Contains(group.Id);
    }
      
    /**
        Adds this contact to a group.
     */
    public async Task AddToGroupAsync(Group group)
    {
        await api.DoRequestAsync("PUT", group.GetBaseApiPath() + "/contacts/" + Id);
        groupIdsSet.Add(group.Id);
    }
    
    /**
        Removes this contact from a group.
     */    
    public async Task RemoveFromGroupAsync(Group group)
    {    
        await api.DoRequestAsync("DELETE", group.GetBaseApiPath() + "/contacts/" + Id);
        groupIdsSet.Remove(group.Id);
    }
    
    private HashSet<String> groupIdsSet;
    
    public override void SetData(JObject data)
    {
        base.SetData(data);
        
        groupIdsSet = new HashSet<String>();
        
        JArray groupIds = (JArray) data["group_ids"];
        if (groupIds != null)
        {            
            int numGroupIds = groupIds.Count;
            for (int i = 0; i < numGroupIds; i++)
            {
                groupIdsSet.Add((String)groupIds[i]);
            }
        }
    }

    /**
        Queries messages sent or received by this contact.
    */
    public APICursor<Message> QueryMessages(JObject options = null)
    {
        return api.NewCursor<Message>(GetBaseApiPath() + "/messages", options);
    }

    /**
        Queries groups for which this contact is a member.
    */
    public APICursor<Group> QueryGroups(JObject options = null)
    {
        return api.NewCursor<Group>(GetBaseApiPath() + "/groups", options);
    }

    /**
        Queries messages scheduled to this contact (not including messages scheduled to groups that
        this contact is a member of)
    */
    public APICursor<ScheduledMessage> QueryScheduledMessages(JObject options = null)
    {
        return api.NewCursor<ScheduledMessage>(GetBaseApiPath() + "/scheduled", options);
    }

    /**
        Queries data rows associated with this contact (in any data table).
    */
    public APICursor<DataRow> QueryDataRows(JObject options = null)
    {
        return api.NewCursor<DataRow>(GetBaseApiPath() + "/rows", options);
    }

    /**
        Queries this contact's current states for any service
    */
    public APICursor<ContactServiceState> QueryServiceStates(JObject options = null)
    {
        return api.NewCursor<ContactServiceState>(GetBaseApiPath() + "/states", options);
    }

    /**
        Saves any fields or custom variables that have changed for this contact.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Deletes this contact.
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

    public string PhoneNumber
    {
      get {
          return (string) Get("phone_number");
      }
      set {
          Set("phone_number", value);
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long LastMessageTime
    {
      get {
          return (long) Get("last_message_time");
      }
    }

    public String LastMessageId
    {
      get {
          return (String) Get("last_message_id");
      }
    }

    public String DefaultRouteId
    {
      get {
          return (String) Get("default_route_id");
      }
      set {
          Set("default_route_id", value);
      }
    }

    public JArray GroupIds
    {
      get {
          return (JArray) Get("group_ids");
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
        return "/projects/" + ProjectId + "/contacts/" + Id + "";
    }

    public Contact(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}