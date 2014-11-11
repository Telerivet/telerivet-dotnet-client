
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a row in a custom data table.
    
    For example, each response to a poll is stored as one row in a data table.
    If a poll has a question with ID 'q1', the verbatim response to that question would be
    stored in row.vars.q1, and the response code would be stored in row.vars.q1_code.
    
    Each custom variable name within a data row corresponds to a different
    column/field of the data table.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the data row
          * Read-only
      
      - contact_id
          * ID of the contact this row is associated with (or null if not associated with any
              contact)
          * Updatable via API
      
      - from_number (string)
          * Phone number that this row is associated with (or null if not associated with any
              phone number)
          * Updatable via API
      
      - vars (JObject)
          * Custom variables stored for this data row
          * Updatable via API
      
      - table_id
          * ID of the table this data row belongs to
          * Read-only
      
      - project_id
          * ID of the project this data row belongs to
          * Read-only
*/

public class DataRow : Entity
{
    /**
        Saves any fields or custom variables that have changed for this data row.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Deletes this data row.
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

    public String ContactId
    {
      get {
          return (String) Get("contact_id");
      }
      set {
          Set("contact_id", value);
      }
    }

    public string FromNumber
    {
      get {
          return (string) Get("from_number");
      }
      set {
          Set("from_number", value);
      }
    }

    public String TableId
    {
      get {
          return (String) Get("table_id");
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
        return "/projects/" + ProjectId + "/tables/" + TableId + "/rows/" + Id + "";
    }

    public DataRow(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
