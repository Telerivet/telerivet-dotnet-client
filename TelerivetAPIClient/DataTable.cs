
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
    
    For example, poll services use data tables to store a row for each response.
    
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
          * Number of rows in the table. For performance reasons, this number may sometimes be
              out-of-date.
          * Read-only
      
      - show_add_row (bool)
          * Whether to allow adding or importing rows via the web app
          * Updatable via API
      
      - show_stats (bool)
          * Whether to show row statistics in the web app
          * Updatable via API
      
      - show_contact_columns (bool)
          * Whether to show 'Contact Name' and 'Phone Number' columns in the web app
          * Updatable via API
      
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
        array of objects with the properties 'name', 'variable', 'type', 'order', 'readonly', and
        'lookup_key'. (Fields are automatically created any time a DataRow's 'vars' property is
        updated.)
    */
    public async Task<JArray> GetFieldsAsync()
    {
        return (JArray) await api.DoRequestAsync("GET", GetBaseApiPath() + "/fields");
    }

    /**
        Allows customizing how a field (column) is displayed in the Telerivet web app.
    */
    public async Task<JObject> SetFieldMetadataAsync(string variable, JObject options = null)
    {
        return (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/fields/" + variable, options);
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

    public bool ShowAddRow
    {
      get {
          return (bool) Get("show_add_row");
      }
      set {
          Set("show_add_row", value);
      }
    }

    public bool ShowStats
    {
      get {
          return (bool) Get("show_stats");
      }
      set {
          Set("show_stats", value);
      }
    }

    public bool ShowContactColumns
    {
      get {
          return (bool) Get("show_contact_columns");
      }
      set {
          Set("show_contact_columns", value);
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
