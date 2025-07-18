
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Telerivet.Client
{
/**
    Represents an asynchronous task that is applied to all entities matching a filter.
    
    Tasks include services applied to contacts, messages, or data rows; adding
    or removing contacts from a group; blocking or unblocking sending messages to a contact;
    updating a custom variable; deleting contacts, messages, or data rows; or
    exporting data to CSV.
    
    Fields:
    
      - id (string, max 34 characters)
          * ID of the task
          * Read-only
      
      - task_type (string)
          * The task type
          * Read-only
      
      - task_params (JObject)
          * Parameters applied to all matching rows (specific to `task_type`). See
              [project.createTask](#Project.createTask).
          * Read-only
      
      - filter_type
          * Type of filter defining the rows that the task is applied to
          * Read-only
      
      - filter_params (JObject)
          * Parameters defining the rows that the task is applied to (specific to
              `filter_type`). See [project.createTask](#Project.createTask).
          * Read-only
      
      - time_created (UNIX timestamp)
          * Time the task was created in Telerivet
          * Read-only
      
      - time_active (UNIX timestamp)
          * Time Telerivet started executing the task
          * Read-only
      
      - time_complete (UNIX timestamp)
          * Time Telerivet finished executing the task
          * Read-only
      
      - total_rows (int)
          * The total number of rows matching the filter (null if not known)
          * Read-only
      
      - current_row (int)
          * The number of rows that have been processed so far
          * Read-only
      
      - status (string)
          * The current status of the task
          * Allowed values: created, queued, active, complete, failed, cancelled
          * Read-only
      
      - vars (JObject)
          * Custom variables stored for this task. Variable names may be up to 32 characters in
              length and can contain the characters a-z, A-Z, 0-9, and _.
              Values may be strings, numbers, or boolean (true/false).
              String values may be up to 4096 bytes in length when encoded as UTF-8.
              Up to 100 variables are supported per object.
              Setting a variable to null will delete the variable.
          * Read-only
      
      - table_id (string, max 34 characters)
          * ID of the data table this task is applied to (if applicable)
          * Read-only
      
      - user_id (string, max 34 characters)
          * ID of the Telerivet user who created the task (if applicable)
          * Read-only
      
      - project_id
          * ID of the project this task belongs to
          * Read-only
*/

public class BatchTask : Entity
{
    /**
        Cancels a task that is not yet complete.
    */
    public async Task<BatchTask> CancelAsync()
    {
        return new BatchTask(api, (JObject) await api.DoRequestAsync("POST", GetBaseApiPath() + "/cancel"));
    }

    public string Id
    {
      get {
          return (string) Get("id");
      }
    }

    public string TaskType
    {
      get {
          return (string) Get("task_type");
      }
    }

    public JObject TaskParams
    {
      get {
          return (JObject) Get("task_params");
      }
    }

    public string FilterType
    {
      get {
          return (string) Get("filter_type");
      }
    }

    public JObject FilterParams
    {
      get {
          return (JObject) Get("filter_params");
      }
    }

    public long TimeCreated
    {
      get {
          return (long) Get("time_created");
      }
    }

    public long? TimeActive
    {
      get {
          return (long?) Get("time_active");
      }
    }

    public long? TimeComplete
    {
      get {
          return (long?) Get("time_complete");
      }
    }

    public int? TotalRows
    {
      get {
          return (int?) Get("total_rows");
      }
    }

    public int? CurrentRow
    {
      get {
          return (int?) Get("current_row");
      }
    }

    public string Status
    {
      get {
          return (string) Get("status");
      }
    }

    public string TableId
    {
      get {
          return (string) Get("table_id");
      }
    }

    public string UserId
    {
      get {
          return (string) Get("user_id");
      }
    }

    public string ProjectId
    {
      get {
          return (string) Get("project_id");
      }
    }

    public override string GetBaseApiPath()
    {
        return "/projects/" + ProjectId + "/tasks/" + Id + "";
    }

    public BatchTask(TelerivetAPI api, JObject data, bool isLoaded = true)
        : base(api, data, isLoaded)
    {
    }
}

}
