using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO.Compression;
using System.IO;

namespace Telerivet.Client
{

public class TelerivetAPI : IDisposable
{
    public static String ClientVersion = "1.8.0";

    private int numRequests = 0;

    private String apiKey;
    private String apiUrl;
    private HttpClient httpClient;
    private bool disposed;

    /**
        Initializes a client handle to the Telerivet REST API.
        
        Each API key is associated with a Telerivet user account, and all
        API actions are performed with that user's permissions. If you want to restrict the
        permissions of an API client, simply add another user account at
        <https://telerivet.com/dashboard/users> with the desired permissions.
    */
    public TelerivetAPI(String apiKey, String apiUrl = "https://api.telerivet.com/v1")
    {
        this.apiKey = apiKey;
        this.apiUrl = apiUrl;
    }

    /**
        Retrieves the Telerivet project with the given ID.
    */
    public async Task<Project> GetProjectByIdAsync(string id)
    {
        return new Project(this, (JObject) await this.DoRequestAsync("GET", GetBaseApiPath() + "/projects/" + id));
    }

    /**
        Initializes the Telerivet project with the given ID without making an API request.
    */
    public Project InitProjectById(string id)
    {
        return new Project(this, Util.Options("id", id), false);
    }

    /**
        Queries projects accessible to the current user account.
    */
    public APICursor<Project> QueryProjects(JObject options = null)
    {
        return this.NewCursor<Project>(GetBaseApiPath() + "/projects", options);
    }

    /**
        Retrieves the Telerivet organization with the given ID.
    */
    public async Task<Organization> GetOrganizationByIdAsync(string id)
    {
        return new Organization(this, (JObject) await this.DoRequestAsync("GET", GetBaseApiPath() + "/organizations/" + id));
    }

    /**
        Initializes the Telerivet organization with the given ID without making an API request.
    */
    public Organization InitOrganizationById(string id)
    {
        return new Organization(this, Util.Options("id", id), false);
    }

    /**
        Queries organizations accessible to the current user account.
    */
    public APICursor<Organization> QueryOrganizations(JObject options = null)
    {
        return this.NewCursor<Organization>(GetBaseApiPath() + "/organizations", options);
    }

    public string GetBaseApiPath()
    {
        return "";
    }
    public int NumRequests
    {
        get
        {
            return numRequests;
        }
    }

    public String getBaseApiPath()
    {
        return "";
    }

    private void encodeParamsRec(String paramName, JToken token, List<String> paramArr)
    {
        if (token == null)
        {
            return;
        }
        if (token is JArray)
        {
            JArray arr = (JArray)token;
            int len = arr.Count;

            for (int i = 0; i < len; i++)
            {
                encodeParamsRec(paramName + "[" +i + "]", arr[i], paramArr);
            }
        }
        else if (token is JObject)
        {
            JObject obj = (JObject)token;

            foreach (KeyValuePair<string, JToken> kvp in obj)
            {
                encodeParamsRec(paramName + "[" + kvp.Key+ "]", kvp.Value, paramArr);
            }
        }
        else if (token is JValue)
        {
            JValue value = (JValue)token;

            object val = value.Value;

            paramArr.Add(paramName + "=" + HttpUtility.UrlEncode("" + val));
        }
        else
        {
            throw new ArgumentException("Unknown type: " + token.GetType());
        }
    }

    private String encodeParams(JObject jsonOptions)
    {
        List<String> paramsList = new List<String>();

        if (jsonOptions != null)
        {
            foreach (KeyValuePair<string, JToken> kvp in jsonOptions)
            {
                encodeParamsRec(kvp.Key, kvp.Value, paramsList);
            }
        }
        return String.Join("&", paramsList.ToArray());
    }

    public APICursor<T> NewCursor<T>(String path, JObject options = null) where T : class
    {
        return new APICursor<T>(this, path, options);
    }

    public async Task<object> DoRequestAsync(String method, String path, JObject parameters = null)
    {
        if (httpClient == null)
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip;

            httpClient = new HttpClient(handler);

            byte[] apiKeyBytes = Encoding.UTF8.GetBytes(apiKey + ":");

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Telerivet .NET Client/" + ClientVersion + " .NET/" + Environment.Version);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic " + Convert.ToBase64String(apiKeyBytes));
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }

        String requestUri = this.apiUrl + path;

        HttpResponseMessage httpResponse;

        HttpContent content = null;
        if (method == "POST" || method == "PUT")
        {
            String json = JsonConvert.SerializeObject(parameters);

            if (json.Length >= 400)
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                using (var compressedStream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        gZipStream.Write(jsonBytes, 0, jsonBytes.Length);
                    }
                    var gzipBytes = compressedStream.ToArray();
                    content = new ByteArrayContent(gzipBytes);
                    content.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    content.Headers.Add("Content-Encoding", "gzip");
                }
            }
            else
            {
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }
        }
        else
        {
            String uriParams = encodeParams(parameters);
            if (!String.IsNullOrEmpty(uriParams))
            {
                requestUri = requestUri + "?" + uriParams;
            }
        }

        if (method == "GET")
        {
            httpResponse = await httpClient.GetAsync(requestUri);
        }
        else if (method == "DELETE")
        {
            httpResponse = await httpClient.DeleteAsync(requestUri);
        }
        else if (method == "POST")
        {
            httpResponse = await httpClient.PostAsync(requestUri, content);
        }
        else if (method == "PUT")
        {
            httpResponse = await httpClient.PutAsync(requestUri, content);
        }
        else
        {
            throw new ArgumentException("Invalid HTTP method: " + method);
        }

        numRequests += 1;

        string responseString = await httpResponse.Content.ReadAsStringAsync();

        object responseObj;
        try
        {
            responseObj = JsonConvert.DeserializeObject(responseString);
        }
        catch (JsonException ex)
        {
            throw new TelerivetAPIException("Unexpected response from Telerivet API (HTTP " + ((int)httpResponse.StatusCode) + "): " + responseString, null);
        }

        if (responseObj is JObject && ((JObject)responseObj)["error"] != null)
        {
            JObject error = (JObject)((JObject)responseObj)["error"];

            String code = (String)error["code"];
            String message = (String)error["message"];

            if (code == "invalid_param")
            {
                throw new TelerivetInvalidParameterException(message, code, (String)error["param"]);
            }
            else if (code == "not_found")
            {
                throw new TelerivetNotFoundException(message, code);
            }
            else
            {
                throw new TelerivetAPIException(message, code);
            }
        }
        else
        {
            return responseObj;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }

            disposed = true;
        }
    }

    ~TelerivetAPI()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
    }
}

public class TelerivetAPIException : Exception
{
    protected String code;

    public TelerivetAPIException(String message, String code = null)
           : base(message)
    {
        this.code = code;
    }

    public String Code
    {
        get
        {
            return code;
        }
    }
}

public class TelerivetInvalidParameterException : TelerivetAPIException
{
    protected String param;

    public TelerivetInvalidParameterException(String message, String code = null, String param = null) : base(message, code)
    {
        this.param = param;
    }

    public String Param
    {
        get
        {
            return param;
        }
    }
}

public class TelerivetNotFoundException : TelerivetAPIException
{
    public TelerivetNotFoundException(String message, String code = null) : base(message, code) { }
}

}
