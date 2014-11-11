
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
        
namespace Telerivet.Client
{
/**
    Represents a custom data table that can store arbitrary rows.
    
    For example, poll services use data tables to store a row for each
    response.
    
    DataTables are schemaless -- each row simply stores custom variables. Each
    variable name is equivalent to a different "column" of the data table.
    Telerivet refers to these variables/columns as "fields", and automatically
    creates a new field for each variable name used in a row of the table.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the data table
          * Read-only
      
      - name
          * Name of the data table
          * Updatable via API
      
      - num_rows (int)
          * Number of rows in the table
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this data table
          * Updatable via API
      
      - project_id
          * ID of the project this data table belongs to
          * Read-only
*/

public class DataTable : Entity
{
    /**
        Queries rows in this data table.
    */
    public APICursor<DataRow> QueryRows(JObject options = null)
    {
        return api.NewCursor<DataRow>(GetBaseApiPath() + "/rows", options);
    }

    /**
        Adds a new row to this data table.
    */
    public async Task<DataRow> CreateRowAsync(JObject options = null)
    {
        return new DataRow(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/rows", options));
    }

    /**
        Retrieves the row in the given table with the given ID.
    */
    public async Task<DataRow> GetRowByIdAsync(string id)
    {
        return new DataRow(api, (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/rows/" + id));
    }

    /**
        Initializes the row in the given table with the given ID, without making an API request.
    */
    public DataRow InitRowById(string id)
    {
        return new DataRow(api, Util.Options("project_id", Get("project_id"), "table_id", Get("id"), "id", id), false);
    }

    /**
        Gets a list of all fields (columns) defined for this data table. The return value is an
        array of objects with the properties 'name' and 'variable'. (Fields are automatically
        created any time a DataRow's 'vars' property is updated.)
    */
    public async Task<JArray> GetFieldsAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/fields");
    }

    /**
        Returns the number of rows for each value of a given variable. This can be used to get the
        total number of responses for each choice in a poll, without making a separate query for
        each response choice. The return value is an object mapping values to row counts, e.g.
        `{"yes":7,"no":3}`
    */
    public async Task<JObject> CountRowsByValueAsync(string variable)
    {
        return (JObject) await api.DoRequestAsync("GET", GetBaseApiPath() + "/count_rows_by_value", Util.Options("variable", variable));
    }

    /**
        Saves any fields that have changed for this data table.
    */
    public override async Task SaveAsync()
    {
        await base.SaveAsync();
    }

    /**
        Permanently deletes the given data table, including all its rows
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

    public String Name
    {
      get {
          return (String) Get("name");
      }
      set {
          Set("name", value);
      }
    }

    public int NumRows
    {
      get {
          return (int) Get("num_rows");
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
        return "/projects/" + ProjectId + "/tables/" + Id + "";
    }

    public DataTable(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }   
}

}
