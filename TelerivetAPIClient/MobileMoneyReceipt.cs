
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a receipt received from a mobile money system such as Safaricom M-Pesa (Kenya),
    Vodacom M-Pesa (Tanzania), or Tigo Pesa (Tanzania).
    
    When your Android phone receives a SMS receipt from a supported mobile money
    service that Telerivet can understand, Telerivet will automatically parse it and create a
    MobileMoneyReceipt object.
    
    Fields:
    
      - id (string, max 34 characters)
          * Telerivet's internal ID for the receipt
          * Read-only
      
      - tx_id
          * Transaction ID from the receipt
          * Read-only
      
      - tx_type
          * Type of mobile money transaction
          * Allowed values: receive_money, send_money, pay_bill, deposit, withdrawal,
              airtime_purchase, balance_inquiry, reversal
          * Read-only
      
      - currency
          * [ISO 4217 Currency code](http://en.wikipedia.org/wiki/ISO_4217) for the transaction,
              e.g. KES or TZS. Amount, balance, and fee are expressed in units of this currency.
          * Read-only
      
      - amount (number)
          * Amount of this transaction; positive numbers indicate money added to your account,
              negative numbers indicate money removed from your account
          * Read-only
      
      - balance (number)
          * The current balance of your mobile money account (null if not available)
          * Read-only
      
      - fee (number)
          * The transaction fee charged by the mobile money system (null if not available)
          * Read-only
      
      - name
          * The name of the other person in the transaction (null if not available)
          * Read-only
      
      - phone_number
          * The phone number of the other person in the transaction (null if not available)
          * Read-only
      
      - time_created (UNIX timestamp)
          * The time this receipt was created in Telerivet
          * Read-only
      
      - other_tx_id
          * The other transaction ID listed in the receipt (e.g. the transaction ID for a
              reversed transaction)
          * Read-only
      
      - content
          * The raw content of the mobile money receipt
          * Read-only
      
      - provider_id
          * Telerivet's internal ID for the mobile money provider
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this mobile money receipt
          * Updatable via API
      
      - contact_id
          * ID of the contact associated with the name/phone number on the receipt. Note that
              some mobile money systems do not provide the other person's phone number, so it's
              possible Telerivet may not automatically assign a contact_id, or may assign it to a
              different contact with the same name.
          * Updatable via API
      
      - phone_id
          * ID of the phone that received the receipt
          * Read-only
      
      - message_id
          * ID of the message corresponding to the receipt
          * Read-only
      
      - project_id
          * ID of the project this receipt belongs to
          * Read-only
*/

public class MobileMoneyReceipt : Entity
{
    /**
        Saves any fields or custom variables that have changed for this mobile money receipt.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Deletes this receipt.
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

    public String TxId
    {
      get {
          return (String) Get("tx_id");
      }
    }

    public String TxType
    {
      get {
          return (String) Get("tx_type");
      }
    }

    public String Currency
    {
      get {
          return (String) Get("currency");
      }
    }

    public double Amount
    {
      get {
          return (double) Get("amount");
      }
    }

    public double Balance
    {
      get {
          return (double) Get("balance");
      }
    }

    public double Fee
    {
      get {
          return (double) Get("fee");
      }
    }

    public String Name
    {
      get {
          return (String) Get("name");
      }
    }

    public String PhoneNumber
    {
      get {
          return (String) Get("phone_number");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public String OtherTxId
    {
      get {
          return (String) Get("other_tx_id");
      }
    }

    public String Content
    {
      get {
          return (String) Get("content");
      }
    }

    public String ProviderId
    {
      get {
          return (String) Get("provider_id");
      }
    }

    public String ContactId
    {
      get {
          return (String) Get("contact_id");
      }
      set {
          Set("contact_id", value);
      }
    }

    public String PhoneId
    {
      get {
          return (String) Get("phone_id");
      }
    }

    public String MessageId
    {
      get {
          return (String) Get("message_id");
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
        return "/projects/" + ProjectId + "/receipts/" + Id + "";
    }

    public MobileMoneyReceipt(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
