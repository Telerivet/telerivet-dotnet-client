
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
          * Allowed values: sms, mms, ussd, call, service
          * Read-only
      
      - source
          * How the message originated within Telerivet
          * Allowed values: phone, provider, web, api, service, webhook, scheduled, integration
          * Read-only
      
      - time_created (UNIX timestamp)
          * The time that the message was created on Telerivet's servers
          * Read-only
      
      - time_sent (UNIX timestamp)
          * The time that the message was reported to have been sent (null for incoming messages
              and messages that have not yet been sent)
          * Read-only
      
      - time_updated (UNIX timestamp)
          * The time that the message was last updated in Telerivet.
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
          * Whether this message was simulated within Telerivet for testing (and not actually
              sent to or received by a real phone)
          * Read-only
      
      - label_ids (array)
          * List of IDs of labels applied to this message
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this message
          * Updatable via API
      
      - priority (int)
          * Priority of this message. Telerivet will attempt to send messages with higher
              priority numbers first. Only defined for outgoing messages.
          * Read-only
      
      - error_message
          * A description of the error encountered while sending a message. (This field is
              omitted from the API response if there is no error message.)
          * Updatable via API
      
      - external_id
          * The ID of this message from an external SMS gateway provider (e.g. Twilio or Nexmo),
              if available.
          * Read-only
      
      - price (number)
          * The price of this message, if known.
          * Read-only
      
      - price_currency
          * The currency of the message price, if applicable.
          * Read-only
      
      - duration (number)
          * The duration of the call in seconds, if known, or -1 if the call was not answered.
          * Read-only
      
      - ring_time (number)
          * The length of time the call rang in seconds before being answered or hung up, if
              known.
          * Read-only
      
      - audio_url
          * For voice calls, the URL of an MP3 file to play when the contact answers the call
          * Read-only
      
      - tts_lang
          * For voice calls, the language of the text-to-speech voice
          * Allowed values: en-US, en-GB, en-GB-WLS, en-AU, en-IN, da-DK, nl-NL, fr-FR, fr-CA,
              de-DE, is-IS, it-IT, pl-PL, pt-BR, pt-PT, ru-RU, es-ES, es-US, sv-SE
          * Read-only
      
      - tts_voice
          * For voice calls, the text-to-speech voice
          * Allowed values: female, male
          * Read-only
      
      - mms_parts (array)
          * A list of parts in the MMS message, the same as returned by the
              [getMMSParts](#Message.getMMSParts) method.
              
              Note: This property is only present when retrieving an individual
              MMS message by ID, not when querying a list of messages. In other cases, use
              [getMMSParts](#Message.getMMSParts).
          * Read-only
      
      - track_clicks (boolean)
          * If true, URLs in the message content are short URLs that redirect to a destination
              URL.
          * Read-only
      
      - short_urls (array)
          * For text messages containing short URLs, this is an array of objects with the
              properties `short_url`, `link_type`, and `time_clicked` (the first time that URL was
              clicked). If `link_type` is "redirect", the object also contains a `destination_url`
              property. If `link_type` is "media", the object also contains an `media_index`
              property (the index in the media array). If `link_type` is "service", the object also
              contains a `service_id` property. This property is undefined for messages that do not
              contain short URLs.
          * Read-only
      
      - media (array)
          * For text messages containing media files, this is an array of objects with the
              properties `url`, `type` (MIME type), `filename`, and `size` (file size in bytes).
              Unknown properties are null. This property is undefined for messages that do not
              contain media files. Note: For files uploaded via the Telerivet web app, the URL is
              temporary and may not be valid for more than 1 day.
          * Read-only
      
      - time_clicked (UNIX timestamp)
          * If the message contains any short URLs, this is the first time that a short URL in
              the message was clicked.  This property is undefined for messages that do not contain
              short URLs.
          * Read-only
      
      - service_id (string, max 34 characters)
          * ID of the service that handled the message (for voice calls, the service defines the
              call flow)
          * Read-only
      
      - phone_id (string, max 34 characters)
          * ID of the phone (basic route) that sent or received the message
          * Read-only
      
      - contact_id (string, max 34 characters)
          * ID of the contact that sent or received the message
          * Read-only
      
      - route_id (string, max 34 characters)
          * ID of the custom route that sent the message (if applicable)
          * Read-only
      
      - broadcast_id (string, max 34 characters)
          * ID of the broadcast that this message is part of (if applicable)
          * Read-only
      
      - scheduled_id (string, max 34 characters)
          * ID of the scheduled message that created this message is part of (if applicable)
          * Read-only
      
      - user_id (string, max 34 characters)
          * ID of the Telerivet user who sent the message (if applicable)
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
    public async Task<Message> ResendAsync(JObject options = null)
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/resend", options));
    }

    /**
        Cancels sending a message that has not yet been sent. Returns the updated message object.
        Only valid for outgoing messages that are currently in the queued, retrying, or cancelled
        states. For other messages, the API will return an error with the code 'not_cancellable'.
    */
    public async Task<Message> CancelAsync()
    {
        return new Message(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/cancel"));
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

    public long? TimeSent
    {
      get {
          return (long?) Get("time_sent");
      }
    }

    public long TimeUpdated
    {
      get {
          return (long) Get("time_updated");
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

    public int Priority
    {
      get {
          return (int) Get("priority");
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

    public double? Price
    {
      get {
          return (double?) Get("price");
      }
    }

    public String PriceCurrency
    {
      get {
          return (String) Get("price_currency");
      }
    }

    public double? Duration
    {
      get {
          return (double?) Get("duration");
      }
    }

    public double? RingTime
    {
      get {
          return (double?) Get("ring_time");
      }
    }

    public String AudioUrl
    {
      get {
          return (String) Get("audio_url");
      }
    }

    public String TtsLang
    {
      get {
          return (String) Get("tts_lang");
      }
    }

    public String TtsVoice
    {
      get {
          return (String) Get("tts_voice");
      }
    }

    public JArray MmsParts
    {
      get {
          return (JArray) Get("mms_parts");
      }
    }

    public String TrackClicks
    {
      get {
          return (String) Get("track_clicks");
      }
    }

    public JArray ShortUrls
    {
      get {
          return (JArray) Get("short_urls");
      }
    }

    public JArray Media
    {
      get {
          return (JArray) Get("media");
      }
    }

    public long TimeClicked
    {
      get {
          return (long) Get("time_clicked");
      }
    }

    public string ServiceId
    {
      get {
          return (string) Get("service_id");
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

    public string RouteId
    {
      get {
          return (string) Get("route_id");
      }
    }

    public string BroadcastId
    {
      get {
          return (string) Get("broadcast_id");
      }
    }

    public string ScheduledId
    {
      get {
          return (string) Get("scheduled_id");
      }
    }

    public string UserId
    {
      get {
          return (string) Get("user_id");
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