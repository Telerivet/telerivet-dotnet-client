
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a phone or gateway that you use to send/receive messages via Telerivet.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the phone
          * Read-only
      
      - name
          * Name of the phone
          * Updatable via API
      
      - phone_number (string)
          * Phone number of the phone
          * Updatable via API
      
      - phone_type
          * Type of this phone/gateway (e.g. android, twilio, nexmo, etc)
          * Read-only
      
      - country
          * 2-letter country code (ISO 3166-1 alpha-2) where phone is from
          * Read-only
      
      - time_created (UNIX timestamp)
          * Time the phone was created in Telerivet
          * Read-only
      
      - last_active_time (UNIX timestamp)
          * Approximate time this phone last connected to Telerivet
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this phone
          * Updatable via API
      
      - project_id
          * ID of the project this phone belongs to
          * Read-only
      
      - battery (int)
          * Current battery level, on a scale from 0 to 100, as of the last time the phone
              connected to Telerivet (only present for Android phones)
          * Read-only
      
      - charging (bool)
          * True if the phone is currently charging, false if it is running on battery, as of
              the last time it connected to Telerivet (only present for Android phones)
          * Read-only
      
      - app_version
          * Currently installed version of Telerivet Android app (only present for Android
              phones)
          * Read-only
      
      - android_sdk (int)
          * Android SDK level, indicating the approximate version of the Android OS installed on
              this phone; see
              <http://developer.android.com/guide/topics/manifest/uses-sdk-element.html#ApiLevels>
              (only present for Android phones)
          * Read-only
      
      - mccmnc
          * Code indicating the Android phone's current country (MCC) and mobile network
              operator (MNC); see <http://en.wikipedia.org/wiki/Mobile_country_code> (only present
              for Android phones). Note this is a string containing numeric digits, not an integer.
          * Read-only
      
      - manufacturer
          * Android phone manufacturer (only present for Android phones)
          * Read-only
      
      - model
          * Android phone model (only present for Android phones)
          * Read-only
      
      - send_limit (int)
          * Maximum number of SMS messages per hour that can be sent by this Android phone. To
              increase this limit, install additional SMS expansion packs in the Telerivet app.
              (only present for Android phones)
          * Read-only
*/

public class Phone : Entity
{
    /**
        Queries messages sent or received by this phone.
    */
    public APICursor<Message> QueryMessages(JObject options = null)
    {
        return api.NewCursor<Message>(GetBaseApiPath() + "/messages", options);
    }

    /**
        Saves any fields or custom variables that have changed for this phone.
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

    public string PhoneNumber
    {
      get {
          return (string) Get("phone_number");
      }
      set {
          Set("phone_number", value);
      }
    }

    public String PhoneType
    {
      get {
          return (String) Get("phone_type");
      }
    }

    public String Country
    {
      get {
          return (String) Get("country");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long LastActiveTime
    {
      get {
          return (long) Get("last_active_time");
      }
    }

    public String ProjectId
    {
      get {
          return (String) Get("project_id");
      }
    }

    public int Battery
    {
      get {
          return (int) Get("battery");
      }
    }

    public bool Charging
    {
      get {
          return (bool) Get("charging");
      }
    }

    public String AppVersion
    {
      get {
          return (String) Get("app_version");
      }
    }

    public int AndroidSdk
    {
      get {
          return (int) Get("android_sdk");
      }
    }

    public String Mccmnc
    {
      get {
          return (String) Get("mccmnc");
      }
    }

    public String Manufacturer
    {
      get {
          return (String) Get("manufacturer");
      }
    }

    public String Model
    {
      get {
          return (String) Get("model");
      }
    }

    public int SendLimit
    {
      get {
          return (int) Get("send_limit");
      }
    }

    public override string GetBaseApiPath()
    {
        return "/projects/" + ProjectId + "/phones/" + Id + "";
    }

    public Phone(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
