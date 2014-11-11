
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a scheduled message within Telerivet.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the scheduled message
          * Read-only
      
      - content
          * Text content of the scheduled message
          * Read-only
      
      - rrule
          * Recurrence rule for recurring scheduled messages, e.g. 'FREQ=MONTHLY' or
              'FREQ=WEEKLY;INTERVAL=2'; see <https://tools.ietf.org/html/rfc2445#section-4.3.10>
          * Read-only
      
      - timezone_id
          * Timezone ID used to compute times for recurring messages; see
              <http://en.wikipedia.org/wiki/List_of_tz_database_time_zones>
          * Read-only
      
      - group_id
          * ID of the group to send the message to (null if scheduled to an individual contact)
          * Read-only
      
      - contact_id
          * ID of the contact to send the message to (null if scheduled to a group)
          * Read-only
      
      - to_number
          * Phone number to send the message to (null if scheduled to a group)
          * Read-only
      
      - route_id
          * ID of the phone or route to the message will be sent from
          * Read-only
      
      - message_type
          * Type of scheduled message
          * Allowed values: sms, ussd
          * Read-only
      
      - time_created (UNIX timestamp)
          * Time the scheduled message was created in Telerivet
          * Read-only
      
      - start_time (UNIX timestamp)
          * The time that the message will be sent (or first sent for recurring messages)
          * Read-only
      
      - end_time (UNIX timestamp)
          * Time after which a recurring message will stop (not applicable to non-recurring
              scheduled messages)
          * Read-only
      
      - prev_time (UNIX timestamp)
          * The most recent time that Telerivet has sent this scheduled message (null if it has
              never been sent)
          * Read-only
      
      - next_time (UNIX timestamp)
          * The next upcoming time that Telerivet will sent this scheduled message (null if it
              will not be sent again)
          * Read-only
      
      - occurrences (int)
          * Number of times this scheduled message has already been sent
          * Read-only
      
      - is_template (bool)
          * Set to true if Telerivet will render variables like [[contact.name]] in the message
              content, false otherwise
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this scheduled message (copied to Message when sent)
          * Updatable via API
      
      - label_ids (array)
          * IDs of labels to add to the Message
          * Read-only
      
      - project_id
          * ID of the project this scheduled message belongs to
          * Read-only
*/

public class ScheduledMessage : Entity
{
    /**
        Saves any fields or custom variables that have changed for this scheduled message.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Cancels this scheduled message.
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

    public String Content
    {
      get {
          return (String) Get("content");
      }
    }

    public String Rrule
    {
      get {
          return (String) Get("rrule");
      }
    }

    public String TimezoneId
    {
      get {
          return (String) Get("timezone_id");
      }
    }

    public String GroupId
    {
      get {
          return (String) Get("group_id");
      }
    }

    public String ContactId
    {
      get {
          return (String) Get("contact_id");
      }
    }

    public String ToNumber
    {
      get {
          return (String) Get("to_number");
      }
    }

    public String RouteId
    {
      get {
          return (String) Get("route_id");
      }
    }

    public String MessageType
    {
      get {
          return (String) Get("message_type");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long StartTime
    {
      get {
          return (long) Get("start_time");
      }
    }

    public long EndTime
    {
      get {
          return (long) Get("end_time");
      }
    }

    public long PrevTime
    {
      get {
          return (long) Get("prev_time");
      }
    }

    public long NextTime
    {
      get {
          return (long) Get("next_time");
      }
    }

    public int Occurrences
    {
      get {
          return (int) Get("occurrences");
      }
    }

    public bool IsTemplate
    {
      get {
          return (bool) Get("is_template");
      }
    }

    public JArray LabelIds
    {
      get {
          return (JArray) Get("label_ids");
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
        return "/projects/" + ProjectId + "/scheduled/" + Id + "";
    }

    public ScheduledMessage(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
