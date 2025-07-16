
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents an automated service on Telerivet, for example a poll, auto-reply, webhook
    service, etc.
    
    A service, generally, defines some automated behavior that can be
    invoked/triggered in a particular context, and may be invoked either manually or when a
    particular event occurs.
    
    Most commonly, services work in the context of a particular message, when
    the message is originally received by Telerivet.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the service
          * Read-only
      
      - name
          * Name of the service
          * Updatable via API
      
      - service_type
          * Type of the service.
          * Read-only
      
      - active (bool)
          * Whether the service is active or inactive. Inactive services are not automatically
              triggered and cannot be invoked via the API.
          * Updatable via API
      
      - priority (int)
          * A number that determines the order that services are triggered when a particular
              event occurs (smaller numbers are triggered first). Any service can determine whether
              or not execution "falls-through" to subsequent services (with larger priority values)
              by setting the return_value variable within Telerivet's Rules Engine.
          * Updatable via API
      
      - contexts (JObject)
          * A key/value map where the keys are the names of contexts supported by this service
              (e.g. message, contact), and the values are themselves key/value maps where the keys
              are event names and the values are all true. (This structure makes it easy to test
              whether a service can be invoked for a particular context and event.)
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this service. Variable names may be up to 32 characters
              in length and can contain the characters a-z, A-Z, 0-9, and _.
              Values may be strings, numbers, or boolean (true/false).
              String values may be up to 4096 bytes in length when encoded as UTF-8.
              Up to 100 variables are supported per object.
              Setting a variable to null will delete the variable.
          * Updatable via API
      
      - project_id
          * ID of the project this service belongs to
          * Read-only
      
      - response_table_id
          * ID of the data table where responses to this service will be stored
          * Updatable via API
      
      - phone_ids
          * IDs of phones (basic routes) associated with this service, or null if the service is
              associated with all routes. Only applies for service types that handle incoming
              messages, voice calls, or USSD sessions.
          * Updatable via API
      
      - apply_mode
          * If apply_mode is `unhandled`, the service will not be triggered if another service
              has already handled the incoming message. If apply_mode is `always`, the service will
              always be triggered regardless of other services. Only applies to services that handle
              incoming messages.
          * Allowed values: always, unhandled
          * Updatable via API
      
      - contact_number_filter
          * If contact_number_filter is `long_number`, this service will only be triggered if
              the contact phone number has at least 7 digits (ignoring messages from shortcodes and
              alphanumeric senders). If contact_number_filter is `all`, the service will be
              triggered for all contact phone numbers.  Only applies to services that handle
              incoming messages.
          * Allowed values: long_number, all
          * Updatable via API
      
      - show_action (bool)
          * Whether this service is shown in the 'Actions' menu within the Telerivet web app
              when the service is active. Only provided for service types that are manually
              triggered.
          * Updatable via API
      
      - direction
          * Determines whether the service handles incoming voice calls, outgoing voice calls,
              or both. Only applies to services that handle voice calls.
          * Allowed values: incoming, outgoing, both
          * Updatable via API
      
      - webhook_url
          * URL that a third-party can invoke to trigger this service. Only provided for
              services that are triggered by a webhook request.
          * Read-only
 */
public class Service : Entity 
{    
    /**
        Gets the current state for a particular contact for this service.
        
        If the contact doesn't already have a state, this method will return
        a valid state object with id=null. However this object would not be returned by
        queryContactStates() unless it is saved with a non-null state id.
     */
    public async Task<ContactServiceState> GetContactStateAsync(Contact contact)
    {
        return new ContactServiceState(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/states/" + contact.Id));
    }
    
    /**
        Initializes or updates the current state for a particular contact for the given service. If
        the state id is null, the contact's state will be reset.
     */
    public async Task<ContactServiceState> SetContactStateAsync(Contact contact, JObject options)
    {
        return new ContactServiceState(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/states/" + contact.Id, options));
    }    
        
    /**
        Resets the current state for a particular contact for the given service.
     */
    public async Task<ContactServiceState> ResetContactStateAsync(Contact contact)
    {
        return new ContactServiceState(api, (JObject) await api.DoRequestAsync("DELETE", GetBaseApiPath() + "/states/" + contact.Id));
    }            
    
    /**
        Manually invoke this service in a particular context.
        
        For example, to send a poll to a particular contact (or resend the
        current question), you can invoke the poll service with context=contact, and `contact_id` as
        the ID of the contact to send the poll to. (To trigger a service to multiple contacts, use
        [project.sendBroadcast](#Project.sendBroadcast). To schedule a service in the future, use
        [project.scheduleMessage](#Project.scheduleMessage).)
        
        Or, to manually apply a service for an incoming message, you can
        invoke the service with `context`=`message`, `event`=`incoming_message`, and `message_id` as
        the ID of the incoming message. (This is normally not necessary, but could be used if you
        want to override Telerivet's standard priority-ordering of services.)
    */
    public async Task<JObject> InvokeAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/invoke", options);
    }

    /**
        Query the current states of contacts for this service.
    */
    public APICursor<ContactServiceState> QueryContactStates(JObject options = null)
    {
        return api.NewCursor<ContactServiceState>(GetBaseApiPath() + "/states", options);
    }

    /**
        Gets configuration specific to the type of automated service.
        
        Only certain types of services provide their configuration via the
        API.
    */
    public async Task<JObject> GetConfigAsync()
    {
        return (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/config");
    }

    /**
        Updates configuration specific to the type of automated service.
        
        Only certain types of services support updating their configuration
        via the API.
        
        Note: when updating a service of type custom_template_instance,
        the validation script will be invoked when calling this method.
    */
    public async Task<JObject> SetConfigAsync(JObject options)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/config", options);
    }

    /**
        Saves any fields or custom variables that have changed for this service.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Deletes this service.
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

    public string ServiceType
    {
      get {
          return (string) Get("service_type");
      }
    }

    public bool Active
    {
      get {
          return (bool) Get("active");
      }
      set {
          Set("active", value);
      }
    }

    public int Priority
    {
      get {
          return (int) Get("priority");
      }
      set {
          Set("priority", value);
      }
    }

    public JObject Contexts
    {
      get {
          return (JObject) Get("contexts");
      }
    }

    public string ProjectId
    {
      get {
          return (string) Get("project_id");
      }
    }

    public string ResponseTableId
    {
      get {
          return (string) Get("response_table_id");
      }
      set {
          Set("response_table_id", value);
      }
    }

    public string PhoneIds
    {
      get {
          return (string) Get("phone_ids");
      }
      set {
          Set("phone_ids", value);
      }
    }

    public string ApplyMode
    {
      get {
          return (string) Get("apply_mode");
      }
      set {
          Set("apply_mode", value);
      }
    }

    public string ContactNumberFilter
    {
      get {
          return (string) Get("contact_number_filter");
      }
      set {
          Set("contact_number_filter", value);
      }
    }

    public bool ShowAction
    {
      get {
          return (bool) Get("show_action");
      }
      set {
          Set("show_action", value);
      }
    }

    public string Direction
    {
      get {
          return (string) Get("direction");
      }
      set {
          Set("direction", value);
      }
    }

    public string WebhookUrl
    {
      get {
          return (string) Get("webhook_url");
      }
    }

    public override string GetBaseApiPath()
    {
        return "/projects/" + ProjectId + "/services/" + Id + "";
    }

    public Service(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}