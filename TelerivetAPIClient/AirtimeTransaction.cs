
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents a transaction where airtime is sent to a mobile phone number.
    
    To send airtime, first [create a Custom Actions service to send a particular amount of
    airtime](/dashboard/add_service?subtype_id=main.service.rules.contact&action_id=main.rule.sendairtime),
    then trigger the service using [service.invoke](#Service.invoke),
    [project.sendBroadcast](#Project.sendBroadcast), or
    [project.scheduleMessage](#Project.scheduleMessage).
    
    Fields:
    
      - id
          * ID of the airtime transaction
          * Read-only
      
      - to_number
          * Destination phone number in international format (no leading +)
          * Read-only
      
      - operator_name
          * Operator name
          * Read-only
      
      - country
          * Country code
          * Read-only
      
      - status
          * Current status of airtime transaction (`successful`, `failed`, `cancelled`,
              `queued`, `pending_approval`, or `pending_payment`)
          * Read-only
      
      - status_text
          * Error or success message returned by airtime provider, if available
          * Read-only
      
      - value
          * Value of airtime sent to destination phone number, in units of value_currency
          * Read-only
      
      - value_currency
          * Currency code of price
          * Read-only
      
      - price
          * Price charged for airtime transaction, in units of price_currency
          * Read-only
      
      - price_currency
          * Currency code of price
          * Read-only
      
      - contact_id
          * ID of the contact the airtime was sent to
          * Read-only
      
      - service_id
          * ID of the service that sent the airtime
          * Read-only
      
      - project_id
          * ID of the project that the airtime transaction belongs to
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this transaction
          * Updatable via API
*/

public class AirtimeTransaction : Entity
{
    public String Id
    {
      get {
          return (String) Get("id");
      }
    }

    public String ToNumber
    {
      get {
          return (String) Get("to_number");
      }
    }

    public String OperatorName
    {
      get {
          return (String) Get("operator_name");
      }
    }

    public String Country
    {
      get {
          return (String) Get("country");
      }
    }

    public String Status
    {
      get {
          return (String) Get("status");
      }
    }

    public String StatusText
    {
      get {
          return (String) Get("status_text");
      }
    }

    public String Value
    {
      get {
          return (String) Get("value");
      }
    }

    public String ValueCurrency
    {
      get {
          return (String) Get("value_currency");
      }
    }

    public String Price
    {
      get {
          return (String) Get("price");
      }
    }

    public String PriceCurrency
    {
      get {
          return (String) Get("price_currency");
      }
    }

    public String ContactId
    {
      get {
          return (String) Get("contact_id");
      }
    }

    public String ServiceId
    {
      get {
          return (String) Get("service_id");
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
        return "/projects/" + ProjectId + "/airtime_transactions/" + Id + "";
    }

    public AirtimeTransaction(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}
