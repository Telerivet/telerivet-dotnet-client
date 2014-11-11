
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
          * Custom variables stored for this service
          * Updatable via API
      
      - project_id
          * ID of the project this service belongs to
          * Read-only
      
      - label_id
          * ID of the label containing messages sent or received by this service (currently only
              used for polls)
          * Read-only
      
      - response_table_id
          * ID of the data table where responses to this service will be stored (currently only
              used for polls)
          * Read-only
      
      - sample_group_id
          * ID of the group containing contacts that have been invited to interact with this
              service (currently only used for polls)
          * Read-only
      
      - respondent_group_id
          * ID of the group containing contacts that have completed an interaction with this
              service (currently only used for polls)
          * Read-only
      
      - questions (array)
          * Array of objects describing each question in a poll (only used for polls). Each
              object has the properties "id" (the question ID), "content" (the text of the
              question), and "question_type" (either "multiple_choice", "missed_call", or "open").
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
        current question), you can invoke the poll service with context=contact, and contact_id as
        the ID of the contact to send the poll to.
        
        Or, to manually apply a service for an incoming message, you can
        invoke the service with context=message, event=incoming\_message, and message_id as the ID
        of the incoming message. (This is normally not necessary, but could be used if you want to
        override Telerivet's standard priority-ordering of services.)
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
        Saves any fields or custom variables that have changed for this service.
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

    public String ProjectId
    {
      get {
          return (String) Get("project_id");
      }
    }

    public String LabelId
    {
      get {
          return (String) Get("label_id");
      }
    }

    public String ResponseTableId
    {
      get {
          return (String) Get("response_table_id");
      }
    }

    public String SampleGroupId
    {
      get {
          return (String) Get("sample_group_id");
      }
    }

    public String RespondentGroupId
    {
      get {
          return (String) Get("respondent_group_id");
      }
    }

    public JArray Questions
    {
      get {
          return (JArray) Get("questions");
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