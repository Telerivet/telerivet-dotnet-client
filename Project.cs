
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
          * Default TZ database timezone ID; see [List of tz database time zones Wikipedia
              article](http://en.wikipedia.org/wiki/List_of_tz_database_time_zones).
          * Updatable via API
      
      - url_slug
          * Unique string used as a component of the project's URL in the Telerivet web app
          * Updatable via API
      
      - default_route_id
          * The ID of a basic route or custom route that will be used to send messages by
              default (via both the API and web app), unless a particular route ID is specified when
              sending the message.
          * Updatable via API
      
      - auto_create_contacts (bool)
          * If true, a contact will be automatically created for each unique phone number that a
              message is sent to or received from. If false, contacts will not automatically be
              created (unless contact information is modified by an automated service). The
              Conversations tab in the web app will only show messages that are associated with a
              contact.
          * Updatable via API
      
      - vars (JObject)
          * Custom variables stored for this project. Variable names may be up to 32 characters
              in length and can contain the characters a-z, A-Z, 0-9, and _.
              Values may be strings, numbers, or boolean (true/false).
              String values may be up to 4096 bytes in length when encoded as UTF-8.
              Up to 100 variables are supported per object.
              Setting a variable to null will delete the variable.
          * Updatable via API
      
      - organization_id (string, max 34 characters)
          * ID of the organization this project belongs to
          * Read-only
*/

public class Project : Entity
{
    /**
        Sends one message (SMS, MMS, chat app message, voice call, or USSD request).
    */
    public async Task<Message> SendMessageAsync(JObject options)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/send", options));
    }

    /**
        Sends a text message (optionally with mail-merge templates) or voice call to a group or a
        list of up to 500 phone numbers.
        
        With `message_type`=`service`, invokes an automated service (such as
        a poll) for a group or list of phone numbers. Any service that can be triggered for a
        contact can be invoked via this method, whether or not the service actually sends a message.
    */
    public async Task<Broadcast> SendBroadcastAsync(JObject options)
    {
        return new Broadcast(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/send_broadcast", options));
    }

    /**
        Sends up to 100 different messages in a single API request. This method is significantly
        faster than sending a separate API request for each message.
    */
    public async Task<JObject> SendMultiAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/send_multi", options);
    }

    /**
        (Deprecated) Send a message a to group or a list of phone numbers.
        This method is only needed to maintain backward compatibility with
        code developed using previous versions of the client library.
        Use `sendBroadcast` or `sendMulti` instead.
    */
    public async Task<JObject> SendMessagesAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/send_batch", options);
    }

    /**
        Schedules a message to a group or single contact. Note that Telerivet only sends scheduled
        messages approximately once every 15 seconds, so it is not possible to control the exact
        second at which a scheduled message is sent.
        
        Only one of the parameters group_id, to_number, and contact_id
        should be provided.
        
        With `message_type`=`service`, schedules an automated service (such
        as a poll) to be invoked for a group or list of phone numbers. Any service that can be
        triggered for a contact can be scheduled via this method, whether or not the service
        actually sends a message.
    */
    public async Task<ScheduledMessage> ScheduleMessageAsync(JObject options)
    {
        return new ScheduledMessage(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/scheduled", options));
    }

    /**
        Creates a relative scheduled message. This allows scheduling messages on a different date
        for each contact, for example on their birthday, a certain number of days before an
        appointment, or a certain number of days after enrolling in a campaign.
        
        Telerivet will automatically create a
        [ScheduledMessage](#ScheduledMessage) for each contact matching a RelativeScheduledMessage.
        
        Relative scheduled messages can be created for a group or an
        individual contact, although dynamic groups are not supported. Only one of the parameters
        group_id, to_number, and contact_id should be provided.
        
        With message_type=service, schedules an automated service (such as a
        poll). Any service that can be triggered for a contact can be scheduled via this method,
        whether or not the service actually sends a message.
    */
    public async Task<RelativeScheduledMessage> CreateRelativeScheduledMessageAsync(JObject options)
    {
        return new RelativeScheduledMessage(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/relative_scheduled", options));
    }

    /**
        Add an incoming message to Telerivet. Acts the same as if the message was received by a
        phone. Also triggers any automated services that apply to the message.
    */
    public async Task<Message> ReceiveMessageAsync(JObject options)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/messages/receive", options));
    }

    /**
        Retrieves OR creates and possibly updates a contact by name or phone number.
        
        If a phone number is provided, by default, Telerivet will search for
        an existing contact with that phone number (including suffix matches to allow finding
        contacts with phone numbers in a different format). If a phone number is not provided but a
        name is provided, Telerivet will search for a contact with that exact name (case
        insensitive). This behavior can be modified by setting the `lookup_key` parameter to look up
        a contact by another field, including a custom variable.
        
        If no existing contact is found, a new contact will be created.
        
        Then that contact will be updated with any parameters provided
        (`name`, `phone_number`, `vars`, `default_route_id`, `send_blocked`, `add_group_ids`,
        `remove_group_ids`).
    */
    public async Task<Contact> GetOrCreateContactAsync(JObject options = null)
    {
        return new Contact(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/contacts", options));
    }

    /**
        Creates and/or updates up to 200 contacts in a single API call. When creating or updating a
        large number of contacts, this method is significantly faster than sending a separate API
        request for each contact.
        
        By default, if the phone number for any contact matches an existing
        contact, the existing contact will be updated with any information provided. This behavior
        can be modified by setting the `lookup_key` parameter to look up contacts by another field,
        including a custom variable.
        
        If any contact was not found matching the provided `lookup_key`, a
        new contact will be created.
    */
    public async Task<JObject> ImportContactsAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/import_contacts", options);
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
        Queries basic routes within the given project.
    */
    public APICursor<Phone> QueryPhones(JObject options = null)
    {
        return api.NewCursor<Phone>(GetBaseApiPath() + "/phones", options);
    }

    /**
        Retrieves the basic route with the given ID.
    */
    public async Task<Phone> GetPhoneByIdAsync(string id)
    {
        return new Phone(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/phones/" + id));
    }

    /**
        Initializes the basic route with the given ID without making an API request.
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
        Queries broadcasts within the given project.
    */
    public APICursor<Broadcast> QueryBroadcasts(JObject options = null)
    {
        return api.NewCursor<Broadcast>(GetBaseApiPath() + "/broadcasts", options);
    }

    /**
        Retrieves the broadcast with the given ID.
    */
    public async Task<Broadcast> GetBroadcastByIdAsync(string id)
    {
        return new Broadcast(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/broadcasts/" + id));
    }

    /**
        Initializes the Telerivet broadcast with the given ID without making an API request.
    */
    public Broadcast InitBroadcastById(string id)
    {
        return new Broadcast(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Creates and starts an asynchronous task that is applied to all entities matching a filter
        (e.g. contacts, messages, or data rows).
        Tasks are designed to efficiently process a large number of
        entities. When processing a large number of entities,
        tasks are much faster than using the API to query and loop over
        all objects matching a filter.
        
        Several different types of tasks are supported, including
        applying services to contacts, messages, or data rows;
        adding or removing contacts from a group; blocking or unblocking
        sending messages to a contact; updating a custom variable;
        deleting contacts, messages, or data rows; or exporting data to
        CSV.
        
        When using a task to apply a Custom Actions or Cloud Script API
        service (`apply_service_to_contacts`, `apply_service_to_rows`, or
        `apply_service_to_messages`),
        the `task` variable will be available within the service. The
        service can use custom variables on the task object (e.g. `task.vars.example`), such as
        to store aggregate statistics for the rows matching the filter.
    */
    public async Task<BatchTask> CreateTaskAsync(JObject options)
    {
        return new BatchTask(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/tasks", options));
    }

    /**
        Queries batch tasks within the given project.
    */
    public APICursor<BatchTask> QueryTasks(JObject options = null)
    {
        return api.NewCursor<BatchTask>(GetBaseApiPath() + "/tasks", options);
    }

    /**
        Retrieves the task with the given ID.
    */
    public async Task<BatchTask> GetTaskByIdAsync(string id)
    {
        return new BatchTask(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/tasks/" + id));
    }

    /**
        Initializes the task with the given ID without making an API request.
    */
    public BatchTask InitTaskById(string id)
    {
        return new BatchTask(api, Util.Options("project_id", Get("id"), "id", id), false);
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
        Queries relative scheduled messages within the given project.
    */
    public APICursor<RelativeScheduledMessage> QueryRelativeScheduledMessages(JObject options = null)
    {
        return api.NewCursor<RelativeScheduledMessage>(GetBaseApiPath() + "/relative_scheduled", options);
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
        Retrieves the scheduled message with the given ID.
    */
    public async Task<RelativeScheduledMessage> GetRelativeScheduledMessageByIdAsync(string id)
    {
        return new RelativeScheduledMessage(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/relative_scheduled/" + id));
    }

    /**
        Initializes the relative scheduled message with the given ID without making an API request.
    */
    public RelativeScheduledMessage InitRelativeScheduledMessageById(string id)
    {
        return new RelativeScheduledMessage(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Creates a new automated service.
        
        Only certain types of automated services can be created via the API.
        Other types of services can only be created via the web app.
        
        Although Custom Actions services cannot be created directly via the
        API, they may be converted to a template,
        and then instances of the template can be created via this method
        with `service_type`=`custom_template_instance`. Converting a service
        to a template requires the Service Templates feature to be enabled
        for the organization.
    */
    public async Task<Service> CreateServiceAsync(JObject options)
    {
        return new Service(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/services", options));
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
        Queries service log entries associated with this project.
        
        Note: Service logs are automatically deleted and no longer available
        via the API after approximately one month.
    */
    public APICursor<JObject> QueryServiceLogs(JObject options = null)
    {
        return api.NewCursor<JObject>(GetBaseApiPath() + "/service_logs", options);
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
        Returns an array of user accounts that have access to this project. Each item in the array
        is an object containing `id`, `email`, and `name` properties. (The id corresponds to the
        `user_id` property of the Message object.)
    */
    public async Task<JArray> GetUsersAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/users");
    }

    /**
        Returns information about each airtime transaction.
    */
    public APICursor<AirtimeTransaction> QueryAirtimeTransactions(JObject options = null)
    {
        return api.NewCursor<AirtimeTransaction>(GetBaseApiPath() + "/airtime_transactions", options);
    }

    /**
        Gets an airtime transaction by ID
    */
    public async Task<AirtimeTransaction> GetAirtimeTransactionByIdAsync(string id)
    {
        return new AirtimeTransaction(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/airtime_transactions/" + id));
    }

    /**
        Initializes an airtime transaction by ID without making an API request.
    */
    public AirtimeTransaction InitAirtimeTransactionById(string id)
    {
        return new AirtimeTransaction(api, Util.Options("project_id", Get("id"), "id", id), false);
    }

    /**
        Gets a list of all custom fields defined for contacts in this project. The return value is
        an array of objects with the properties 'name', 'variable', 'type', 'order', 'readonly', and
        'lookup_key'. (Fields are automatically created any time a Contact's 'vars' property is
        updated.)
    */
    public async Task<JArray> GetContactFieldsAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/contact_fields");
    }

    /**
        Allows customizing how a custom contact field is displayed in the Telerivet web app.
        
        The variable path parameter can contain the characters a-z, A-Z,
        0-9, and _, and may be up to 32 characters in length.
    */
    public async Task<JObject> SetContactFieldMetadataAsync(string variable, JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/contact_fields/" + variable, options);
    }

    /**
        Gets a list of all custom fields defined for messages in this project. The return value is
        an array of objects with the properties 'name', 'variable', 'type', 'order', 'readonly', and
        'lookup_key'. (Fields are automatically created any time a Contact's 'vars' property is
        updated.)
    */
    public async Task<JArray> GetMessageFieldsAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/message_fields");
    }

    /**
        Allows customizing how a custom message field is displayed in the Telerivet web app.
        
        The variable path parameter can contain the characters a-z, A-Z,
        0-9, and _, and may be up to 32 characters in length.
    */
    public async Task<JObject> SetMessageFieldMetadataAsync(string variable, JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/message_fields/" + variable, options);
    }

    /**
        Retrieves statistics about messages sent or received via Telerivet. This endpoint returns
        historical data that is computed shortly after midnight each day in the project's time zone,
        and does not contain message statistics for the current day.
    */
    public async Task<JObject> GetMessageStatsAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/message_stats", options);
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

    public string Name
    {
      get {
          return (string) Get("name");
      }
      set {
          Set("name", value);
      }
    }

    public string TimezoneId
    {
      get {
          return (string) Get("timezone_id");
      }
      set {
          Set("timezone_id", value);
      }
    }

    public string UrlSlug
    {
      get {
          return (string) Get("url_slug");
      }
      set {
          Set("url_slug", value);
      }
    }

    public string DefaultRouteId
    {
      get {
          return (string) Get("default_route_id");
      }
      set {
          Set("default_route_id", value);
      }
    }

    public bool AutoCreateContacts
    {
      get {
          return (bool) Get("auto_create_contacts");
      }
      set {
          Set("auto_create_contacts", value);
      }
    }

    public string OrganizationId
    {
      get {
          return (string) Get("organization_id");
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
