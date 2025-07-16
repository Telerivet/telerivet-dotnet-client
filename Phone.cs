
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents a basic route (i.e. a phone or gateway) that you use to send/receive messages via
    Telerivet.
    
    Basic Routes were formerly referred to as "Phones" within Telerivet. API
    methods, parameters, and properties related to Basic Routes continue to use the term "Phone"
    to maintain backwards compatibility.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the phone
          * Read-only
      
      - name
          * Name of the phone
          * Updatable via API
      
      - phone_number (string)
          * Phone number or sender ID
          * Updatable via API
      
      - phone_type
          * Type of this phone/route (e.g. android, twilio, nexmo, etc)
          * Read-only
      
      - country
          * 2-letter country code (ISO 3166-1 alpha-2) where phone is from
          * Read-only
      
      - send_paused (bool)
          * True if sending messages is currently paused, false if the phone can currently send
              messages
          * Updatable via API
      
      - time_created (UNIX timestamp)
          * Time the phone was created in Telerivet
          * Read-only
      
      - last_active_time (UNIX timestamp)
          * Approximate time this phone last connected to Telerivet
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this phone. Variable names may be up to 32 characters in
              length and can contain the characters a-z, A-Z, 0-9, and _.
              Values may be strings, numbers, or boolean (true/false).
              String values may be up to 4096 bytes in length when encoded as UTF-8.
              Up to 100 variables are supported per object.
              Setting a variable to null will delete the variable.
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
      
      - internet_type
          * String describing the current type of internet connectivity for an Android phone,
              for example WIFI or MOBILE (only present for Android phones)
          * Read-only
      
      - app_version
          * Currently installed version of Telerivet Android app (only present for Android
              phones)
          * Read-only
      
      - android_sdk (int)
          * Android SDK level, indicating the approximate version of the Android OS installed on
              this phone; see [list of Android SDK
              levels](http://developer.android.com/guide/topics/manifest/uses-sdk-element.html#ApiLevels)
              (only present for Android phones)
          * Read-only
      
      - mccmnc
          * Code indicating the Android phone's current country (MCC) and mobile network
              operator (MNC); see [Mobile country code Wikipedia
              article](https://en.wikipedia.org/wiki/Mobile_country_code) (only present for Android
              phones). Note this is a string containing numeric digits, not an integer.
          * Read-only
      
      - manufacturer
          * Android phone manufacturer (only present for Android phones)
          * Read-only
      
      - model
          * Android phone model (only present for Android phones)
          * Read-only
      
      - send_limit (int)
          * Maximum number of SMS messages per hour that can be sent by this Android phone. To
              increase this limit, install additional SMS expansion packs in the Telerivet Gateway
              app. (only present for Android phones)
          * Read-only
*/

public class Phone : Entity
{
    /**
        Queries messages sent or received by this basic route.
    */
    public APICursor<Message> QueryMessages(JObject options = null)
    {
        return api.NewCursor<Message>(GetBaseApiPath() + "/messages", options);
    }

    /**
        Saves any fields or custom variables that have changed for this basic route.
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

    public string PhoneNumber
    {
      get {
          return (string) Get("phone_number");
      }
      set {
          Set("phone_number", value);
      }
    }

    public string PhoneType
    {
      get {
          return (string) Get("phone_type");
      }
    }

    public string Country
    {
      get {
          return (string) Get("country");
      }
    }

    public bool SendPaused
    {
      get {
          return (bool) Get("send_paused");
      }
      set {
          Set("send_paused", value);
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long? LastActiveTime
    {
      get {
          return (long?) Get("last_active_time");
      }
    }

    public string ProjectId
    {
      get {
          return (string) Get("project_id");
      }
    }

    public int? Battery
    {
      get {
          return (int?) Get("battery");
      }
    }

    public bool? Charging
    {
      get {
          return (bool?) Get("charging");
      }
    }

    public string InternetType
    {
      get {
          return (string) Get("internet_type");
      }
    }

    public string AppVersion
    {
      get {
          return (string) Get("app_version");
      }
    }

    public int? AndroidSdk
    {
      get {
          return (int?) Get("android_sdk");
      }
    }

    public string Mccmnc
    {
      get {
          return (string) Get("mccmnc");
      }
    }

    public string Manufacturer
    {
      get {
          return (string) Get("manufacturer");
      }
    }

    public string Model
    {
      get {
          return (string) Get("model");
      }
    }

    public int? SendLimit
    {
      get {
          return (int?) Get("send_limit");
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
