
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a single message.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the message
          * Read-only
      
      - direction
          * Direction of the message: incoming messages are sent from one of your contacts to
              your phone; outgoing messages are sent from your phone to one of your contacts
          * Allowed values: incoming, outgoing
          * Read-only
      
      - status
          * Current status of the message
          * Allowed values: ignored, processing, received, sent, queued, failed, failed_queued,
              cancelled, delivered, not_delivered
          * Read-only
      
      - message_type
          * Type of the message
          * Allowed values: sms, mms, ussd, call
          * Read-only
      
      - source
          * How the message originated within Telerivet
          * Allowed values: phone, provider, web, api, service, webhook, scheduled
          * Read-only
      
      - time_created (UNIX timestamp)
          * The time that the message was created on Telerivet's servers
          * Read-only
      
      - time_sent (UNIX timestamp)
          * The time that the message was reported to have been sent (null for incoming messages
              and messages that have not yet been sent)
          * Read-only
      
      - from_number (string)
          * The phone number that the message originated from (your number for outgoing
              messages, the contact's number for incoming messages)
          * Read-only
      
      - to_number (string)
          * The phone number that the message was sent to (your number for incoming messages,
              the contact's number for outgoing messages)
          * Read-only
      
      - content (string)
          * The text content of the message (null for USSD messages and calls)
          * Read-only
      
      - starred (bool)
          * Whether this message is starred in Telerivet
          * Updatable via API
      
      - simulated (bool)
          * Whether this message is was simulated within Telerivet for testing (and not actually
              sent to or received by a real phone)
          * Read-only
      
      - label_ids (array)
          * List of IDs of labels applied to this message
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this message
          * Updatable via API
      
      - error_message
          * A description of the error encountered while sending a message. (This field is
              omitted from the API response if there is no error message.)
          * Updatable via API
      
      - external_id
          * The ID of this message from an external SMS gateway provider (e.g. Twilio or Nexmo),
              if available.
          * Read-only
      
      - price (number)
          * The price of this message, if known. By convention, message prices are negative.
          * Read-only
      
      - price_currency
          * The currency of the message price, if applicable.
          * Read-only
      
      - mms_parts (array)
          * A list of parts in the MMS message, the same as returned by the
              [getMMSParts](#Message.getMMSParts) method.
              
              Note: This property is only present when retrieving an individual
              MMS message by ID, not when querying a list of messages. In other cases, use
              [getMMSParts](#Message.getMMSParts).
          * Read-only
      
      - phone_id (string, max 34 characters)
          * ID of the phone that sent or received the message
          * Read-only
      
      - contact_id (string, max 34 characters)
          * ID of the contact that sent or received the message
          * Read-only
      
      - project_id
          * ID of the project this contact belongs to
          * Read-only
 */
public class Message : Entity 
{    
    /**
        Returns true if this message has a particular label, false otherwise.
     */
    public bool HasLabel(Label label)
    {
        AssertLoaded();
        return labelIdsSet.Contains(label.Id);
    }
      
    /**
        Adds a label to the given message.
     */
    public async Task AddLabelAsync(Label label)
    {
        await api.DoRequestAsync("PUT", label.GetBaseApiPath() + "/messages/" + Id);
        labelIdsSet.Add(label.Id);
    }
    
    /**
        Removes a label from the given message.
     */    
    public async Task RemoveLabelAsync(Label label)
    {    
        await api.DoRequestAsync("DELETE", label.GetBaseApiPath() + "/messages/" + Id);
        labelIdsSet.Remove(label.Id);
    }
    
    private HashSet<String> labelIdsSet;
    
    public override void SetData(JObject data)
    {
        base.SetData(data);
        
        labelIdsSet = new HashSet<String>();
        
        JArray labelIds = (JArray) data["label_ids"];
        if (labelIds != null)
        {            
            int numLabelIds = labelIds.Count;
            for (int i = 0; i < numLabelIds; i++)
            {
                labelIdsSet.Add((String)labelIds[i]);
            }
        }
    }

    /**
        Retrieves a list of MMS parts for this message (empty for non-MMS messages).
        
        Each MMS part in the list is an object with the following
        properties:
        
        - cid: MMS content-id
        - type: MIME type
        - filename: original filename
        - size (int): number of bytes
        - url: URL where the content for this part is stored (secret but
        publicly accessible, so you could link/embed it in a web page without having to re-host it
        yourself)
    */
    public async Task<JArray> GetMMSPartsAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/mms_parts");
    }

    /**
        Saves any fields that have changed for this message.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Resends a message, for example if the message failed to send or if it was not delivered. If
        the message was originally in the queued, retrying, failed, or cancelled states, then
        Telerivet will return the same message object. Otherwise, Telerivet will create and return a
        new message object.
    */
    public async Task<Message> ResendAsync()
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/resend"));
    }

    /**
        Deletes this message.
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

    public String Direction
    {
      get {
          return (String) Get("direction");
      }
    }

    public String Status
    {
      get {
          return (String) Get("status");
      }
    }

    public String MessageType
    {
      get {
          return (String) Get("message_type");
      }
    }

    public String Source
    {
      get {
          return (String) Get("source");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long TimeSent
    {
      get {
          return (long) Get("time_sent");
      }
    }

    public string FromNumber
    {
      get {
          return (string) Get("from_number");
      }
    }

    public string ToNumber
    {
      get {
          return (string) Get("to_number");
      }
    }

    public string Content
    {
      get {
          return (string) Get("content");
      }
    }

    public bool Starred
    {
      get {
          return (bool) Get("starred");
      }
      set {
          Set("starred", value);
      }
    }

    public bool Simulated
    {
      get {
          return (bool) Get("simulated");
      }
    }

    public JArray LabelIds
    {
      get {
          return (JArray) Get("label_ids");
      }
    }

    public String ErrorMessage
    {
      get {
          return (String) Get("error_message");
      }
      set {
          Set("error_message", value);
      }
    }

    public String ExternalId
    {
      get {
          return (String) Get("external_id");
      }
    }

    public double Price
    {
      get {
          return (double) Get("price");
      }
    }

    public String PriceCurrency
    {
      get {
          return (String) Get("price_currency");
      }
    }

    public JArray MmsParts
    {
      get {
          return (JArray) Get("mms_parts");
      }
    }

    public string PhoneId
    {
      get {
          return (string) Get("phone_id");
      }
    }

    public string ContactId
    {
      get {
          return (string) Get("contact_id");
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
        return "/projects/" + ProjectId + "/messages/" + Id + "";
    }

    public Message(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}