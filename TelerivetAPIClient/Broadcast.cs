
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents a collection of related outgoing messages.
    Typically, messages in a broadcast have the same content template and were
    sent at the same time; however, a broadcast can also contain messages with unrelated content
    and messages that were sent at different times.
    A broadcast is automatically created when sending a message to a group of
    contacts.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the broadcast
          * Read-only
      
      - recipients (array of objects)
          * List of recipients. Each recipient is an object with a string `type` property, which
              may be `"phone_number"`, `"group"`, or `"filter"`.
              
              If the type is `"phone_number"`, the `phone_number` property will
              be set to the recipient's phone number.
              
              If the type is `"group"`, the `group_id` property will be set to
              the ID of the group, and the `group_name` property will be set to the name of the
              group.
              
              If the type is `"filter"`, the `filter_type` property (string) and
              `filter_params` property (object) describe the filter used to send the broadcast. (API
              clients should not rely on a particular value or format of the `filter_type` or
              `filter_params` properties, as they may change without notice.)
          * Read-only
      
      - title
          * Title of the broadcast. If a title was not provided when the broadcast was sent, it
              is automatically set to a human readable description of the first few recipients
              (possibly truncated)
          * Read-only
      
      - time_created (UNIX timestamp)
          * Time the broadcast was sent in Telerivet
          * Read-only
      
      - last_message_time (UNIX timestamp)
          * Time the most recent message was queued to send in this broadcast
          * Read-only
      
      - last_send_time (UNIX timestamp)
          * Time the most recent message was sent (or failed to send) in this broadcast, or null
              if no messages have been sent yet
          * Read-only
      
      - status_counts (JObject)
          * An object whose keys are the possible status codes (`"queued"`, `"sent"`,
              `"failed"`, `"failed_queued"`, `"delivered"`, `"not_delivered"`, and `"cancelled"`),
              and whose values are the number of messages in the broadcast currently in that status.
          * Read-only
      
      - message_count (int)
          * The total number of messages created for this broadcast. For large broadcasts, the
              messages may not be created immediately after the broadcast is sent. The
              `message_count` includes messages in any status, including messages that are still
              queued.
          * Read-only
      
      - estimated_count (int)
          * The estimated number of messages in this broadcast when it is complete. The
              `estimated_count` is calculated at the time the broadcast is sent. When the broadcast
              is completed, the `estimated_count` may differ from `message_count` if there are any
              blocked contacts among the recipients (blocked contacts are included in
              `estimated_count` but not in `message_count`), if any contacts don't have phone
              numbers, or if the group membership changed while the broadcast was being sent.
          * Read-only
      
      - message_type
          * Type of message sent from this broadcast
          * Allowed values: sms, mms, ussd, call, service
          * Read-only
      
      - content (string)
          * The text content of the message (null for USSD messages and calls)
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
      
      - is_template (bool)
          * Set to true if Telerivet will render variables like [[contact.name]] in the message
              content, false otherwise
          * Read-only
      
      - status
          * The current status of the broadcast.
          * Allowed values: queuing, sending, complete, cancelled
          * Read-only
      
      - source
          * How the message originated within Telerivet
          * Allowed values: phone, provider, web, api, service, webhook, scheduled, integration
          * Read-only
      
      - simulated (bool)
          * Whether this message was simulated within Telerivet for testing (and not actually
              sent to or received by a real phone)
          * Read-only
      
      - track_clicks (boolean)
          * If true, URLs in the message content will automatically be replaced with unique
              short URLs.
          * Read-only
      
      - clicked_count (int)
          * The number of messages in this broadcast containing short links that were clicked.
              At most one click per message is counted. If track_clicks is false, this property will
              be null.
          * Read-only
      
      - label_ids (array)
          * List of IDs of labels applied to all messages in the broadcast
          * Read-only
      
      - media (array)
          * For text messages containing media files, this is an array of objects with the
              properties `url`, `type` (MIME type), `filename`, and `size` (file size in bytes).
              Unknown properties are null. This property is undefined for messages that do not
              contain media files. Note: For files uploaded via the Telerivet web app, the URL is
              temporary and may not be valid for more than 1 day.
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this broadcast
          * Read-only
      
      - price (number)
          * The total price of all messages in this broadcast, if known.
          * Read-only
      
      - price_currency
          * The currency of the message price, if applicable.
          * Read-only
      
      - reply_count (int)
          * The number of replies received in response to a message sent in this broadcast.
              (Replies are not included in `message_count` ,`status_counts`, or `price`.)
          * Read-only
      
      - last_reply_time (UNIX timestamp)
          * Time the most recent reply was received in response to a message sent in this
              broadcast, or null if no replies have been sent yet
          * Read-only
      
      - route_id (string, max 34 characters)
          * ID of the phone or route used to send the broadcast (if applicable)
          * Read-only
      
      - service_id (string, max 34 characters)
          * The service associated with this broadcast (for voice calls, the service defines the
              call flow)
          * Read-only
      
      - user_id (string, max 34 characters)
          * ID of the Telerivet user who sent the broadcast (if applicable)
          * Read-only
      
      - project_id
          * ID of the project this broadcast belongs to
          * Read-only
*/

public class Broadcast : Entity
{
    /**
        Cancels sending a broadcast that has not yet been completely sent. No additional messages
        will be queued, and any existing queued messages will be cancelled when they would otherwise
        have been sent (except for messages already queued on the Telerivet Android app, which will
        not be automatically cancelled).
    */
    public async Task<Broadcast> CancelAsync()
    {
        return new Broadcast(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/cancel"));
    }

    public string Id
    {
      get {
          return (string) Get("id");
      }
    }

    public JArray Recipients
    {
      get {
          return (JArray) Get("recipients");
      }
    }

    public String Title
    {
      get {
          return (String) Get("title");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long? LastMessageTime
    {
      get {
          return (long?) Get("last_message_time");
      }
    }

    public long? LastSendTime
    {
      get {
          return (long?) Get("last_send_time");
      }
    }

    public JObject StatusCounts
    {
      get {
          return (JObject) Get("status_counts");
      }
    }

    public int MessageCount
    {
      get {
          return (int) Get("message_count");
      }
    }

    public int EstimatedCount
    {
      get {
          return (int) Get("estimated_count");
      }
    }

    public String MessageType
    {
      get {
          return (String) Get("message_type");
      }
    }

    public string Content
    {
      get {
          return (string) Get("content");
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

    public bool IsTemplate
    {
      get {
          return (bool) Get("is_template");
      }
    }

    public String Status
    {
      get {
          return (String) Get("status");
      }
    }

    public String Source
    {
      get {
          return (String) Get("source");
      }
    }

    public bool Simulated
    {
      get {
          return (bool) Get("simulated");
      }
    }

    public String TrackClicks
    {
      get {
          return (String) Get("track_clicks");
      }
    }

    public int? ClickedCount
    {
      get {
          return (int?) Get("clicked_count");
      }
    }

    public JArray LabelIds
    {
      get {
          return (JArray) Get("label_ids");
      }
    }

    public JArray Media
    {
      get {
          return (JArray) Get("media");
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

    public int ReplyCount
    {
      get {
          return (int) Get("reply_count");
      }
    }

    public long? LastReplyTime
    {
      get {
          return (long?) Get("last_reply_time");
      }
    }

    public string RouteId
    {
      get {
          return (string) Get("route_id");
      }
    }

    public string ServiceId
    {
      get {
          return (string) Get("service_id");
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
        return "/projects/" + ProjectId + "/broadcasts/" + Id + "";
    }

    public Broadcast(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}
