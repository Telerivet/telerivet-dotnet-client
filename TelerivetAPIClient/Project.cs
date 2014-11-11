
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a Telerivet project.
    
    Provides methods for sending and scheduling messages, as well as
    accessing, creating and updating a variety of entities, including contacts, messages,
    scheduled messages, groups, labels, phones, services, and data tables.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the project
          * Read-only
      
      - name
          * Name of the project
          * Updatable via API
      
      - timezone_id
          * Default TZ database timezone ID; see
              <http://en.wikipedia.org/wiki/List_of_tz_database_time_zones>
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this project
          * Updatable via API
*/

public class Project : Entity
{
    /**
        Sends one message (SMS or USSD request).
    */
    public async Task<Message> SendMessageAsync(JObject options)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/send", options));
    }

    /**
        Sends an SMS message (optionally with mail-merge templates) to a group or a list of up to
        500 phone numbers
    */
    public async Task<JObject> SendMessagesAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/send_batch", options);
    }

    /**
        Schedules an SMS message to a group or single contact. Note that Telerivet only sends
        scheduled messages approximately once per minute, so it is not possible to control the exact
        second at which a scheduled message is sent.
    */
    public async Task<ScheduledMessage> ScheduleMessageAsync(JObject options)
    {
        return new ScheduledMessage(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/scheduled", options));
    }

    /**
        Retrieves OR creates and possibly updates a contact by name or phone number.
        
        If a phone number is provided, Telerivet will search for an existing
        contact with that phone number (including suffix matches to allow finding contacts with
        phone numbers in a different format).
        
        If a phone number is not provided but a name is provided, Telerivet
        will search for a contact with that exact name (case insensitive).
        
        If no existing contact is found, a new contact will be created.
        
        Then that contact will be updated with any parameters provided
        (name, phone_number, and vars).
    */
    public async Task<Contact> GetOrCreateContactAsync(JObject options)
    {
        return new Contact(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/contacts", options));
    }

    /**
        Queries contacts within the given project.
    */
    public APICursor<Contact> QueryContacts(JObject options = null)
    {
        return api.NewCursor<Contact>(GetBaseApiPath() + "/contacts", options);
    }

    /**
        Retrieves the contact with the given ID.
    */
    public async Task<Contact> GetContactByIdAsync(string id)
    {
        return new Contact(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/contacts/" + id));
    }

    /**
        Initializes the Telerivet contact with the given ID without making an API request.
    */
    public Contact InitContactById(string id)
    {
        return new Contact(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries phones within the given project.
    */
    public APICursor<Phone> QueryPhones(JObject options = null)
    {
        return api.NewCursor<Phone>(GetBaseApiPath() + "/phones", options);
    }

    /**
        Retrieves the phone with the given ID.
    */
    public async Task<Phone> GetPhoneByIdAsync(string id)
    {
        return new Phone(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/phones/" + id));
    }

    /**
        Initializes the phone with the given ID without making an API request.
    */
    public Phone InitPhoneById(string id)
    {
        return new Phone(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries messages within the given project.
    */
    public APICursor<Message> QueryMessages(JObject options = null)
    {
        return api.NewCursor<Message>(GetBaseApiPath() + "/messages", options);
    }

    /**
        Retrieves the message with the given ID.
    */
    public async Task<Message> GetMessageByIdAsync(string id)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/messages/" + id));
    }

    /**
        Initializes the Telerivet message with the given ID without making an API request.
    */
    public Message InitMessageById(string id)
    {
        return new Message(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries groups within the given project.
    */
    public APICursor<Group> QueryGroups(JObject options = null)
    {
        return api.NewCursor<Group>(GetBaseApiPath() + "/groups", options);
    }

    /**
        Retrieves or creates a group by name.
    */
    public async Task<Group> GetOrCreateGroupAsync(string name)
    {
        return new Group(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/groups", Util.Options("name", name)));
    }

    /**
        Retrieves the group with the given ID.
    */
    public async Task<Group> GetGroupByIdAsync(string id)
    {
        return new Group(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/groups/" + id));
    }

    /**
        Initializes the group with the given ID without making an API request.
    */
    public Group InitGroupById(string id)
    {
        return new Group(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries labels within the given project.
    */
    public APICursor<Label> QueryLabels(JObject options = null)
    {
        return api.NewCursor<Label>(GetBaseApiPath() + "/labels", options);
    }

    /**
        Gets or creates a label by name.
    */
    public async Task<Label> GetOrCreateLabelAsync(string name)
    {
        return new Label(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/labels", Util.Options("name", name)));
    }

    /**
        Retrieves the label with the given ID.
    */
    public async Task<Label> GetLabelByIdAsync(string id)
    {
        return new Label(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/labels/" + id));
    }

    /**
        Initializes the label with the given ID without making an API request.
    */
    public Label InitLabelById(string id)
    {
        return new Label(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries data tables within the given project.
    */
    public APICursor<DataTable> QueryDataTables(JObject options = null)
    {
        return api.NewCursor<DataTable>(GetBaseApiPath() + "/tables", options);
    }

    /**
        Gets or creates a data table by name.
    */
    public async Task<DataTable> GetOrCreateDataTableAsync(string name)
    {
        return new DataTable(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/tables", Util.Options("name", name)));
    }

    /**
        Retrieves the data table with the given ID.
    */
    public async Task<DataTable> GetDataTableByIdAsync(string id)
    {
        return new DataTable(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/tables/" + id));
    }

    /**
        Initializes the data table with the given ID without making an API request.
    */
    public DataTable InitDataTableById(string id)
    {
        return new DataTable(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries scheduled messages within the given project.
    */
    public APICursor<ScheduledMessage> QueryScheduledMessages(JObject options = null)
    {
        return api.NewCursor<ScheduledMessage>(GetBaseApiPath() + "/scheduled", options);
    }

    /**
        Retrieves the scheduled message with the given ID.
    */
    public async Task<ScheduledMessage> GetScheduledMessageByIdAsync(string id)
    {
        return new ScheduledMessage(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/scheduled/" + id));
    }

    /**
        Initializes the scheduled message with the given ID without making an API request.
    */
    public ScheduledMessage InitScheduledMessageById(string id)
    {
        return new ScheduledMessage(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries services within the given project.
    */
    public APICursor<Service> QueryServices(JObject options = null)
    {
        return api.NewCursor<Service>(GetBaseApiPath() + "/services", options);
    }

    /**
        Retrieves the service with the given ID.
    */
    public async Task<Service> GetServiceByIdAsync(string id)
    {
        return new Service(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/services/" + id));
    }

    /**
        Initializes the service with the given ID without making an API request.
    */
    public Service InitServiceById(string id)
    {
        return new Service(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries mobile money receipts within the given project.
    */
    public APICursor<MobileMoneyReceipt> QueryReceipts(JObject options = null)
    {
        return api.NewCursor<MobileMoneyReceipt>(GetBaseApiPath() + "/receipts", options);
    }

    /**
        Retrieves the mobile money receipt with the given ID.
    */
    public async Task<MobileMoneyReceipt> GetReceiptByIdAsync(string id)
    {
        return new MobileMoneyReceipt(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/receipts/" + id));
    }

    /**
        Initializes the mobile money receipt with the given ID without making an API request.
    */
    public MobileMoneyReceipt InitReceiptById(string id)
    {
        return new MobileMoneyReceipt(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Queries custom routes that can be used to send messages (not including Phones).
    */
    public APICursor<Route> QueryRoutes(JObject options = null)
    {
        return api.NewCursor<Route>(GetBaseApiPath() + "/routes", options);
    }

    /**
        Gets a custom route by ID
    */
    public async Task<Route> GetRouteByIdAsync(string id)
    {
        return new Route(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/routes/" + id));
    }

    /**
        Initializes a custom route by ID without making an API request.
    */
    public Route InitRouteById(string id)
    {
        return new Route(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Saves any fields or custom variables that have changed for the project.
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

    public String TimezoneId
    {
      get {
          return (String) Get("timezone_id");
      }
    }

    public override string GetBaseApiPath()
    {
        return "/projects/" + Id + "";
    }

    public Project(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
