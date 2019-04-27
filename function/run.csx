#r "Microsoft.Azure.EventGrid"
#r "Newtonsoft.Json"
using Microsoft.Azure.EventGrid.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

const string SubscriptionKey = "";

const string CognitiveServicesUrl = "https://<AZURE REGION>.api.cognitive.microsoft.com/vision/v2.0/analyze";

const string LogicAppsUrl = "";

public static async Task Run(EventGridEvent eventGridEvent, ILogger log)
{
    log.LogInformation(eventGridEvent.Data.ToString());
    var createdEvent = ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();
    string result = await MakeAnalysisRequest(createdEvent.Url, log);

    if(!string.IsNullOrEmpty(result)) {
        HttpClient client = new HttpClient();
        await client.PostAsync(LogicAppsUrl, new StringContent(result, Encoding.UTF8, "application/json"));
        log.LogInformation("Response sent to Logic Apps");
    }

}

static async Task<string> MakeAnalysisRequest(string imageUrl, ILogger log)
{

    string result = "";

    try
    {
        HttpClient client = new HttpClient();

        // Request headers.
        client.DefaultRequestHeaders.Add(
            "Ocp-Apim-Subscription-Key", SubscriptionKey);

        // Request parameters. A third optional parameter is "details".
        // The Analyze Image method returns information about the following
        // visual features:
        // Categories:  categorizes image content according to a
        //              taxonomy defined in documentation.
        // Description: describes the image content with a complete
        //              sentence in supported languages.
        // Color:       determines the accent color, dominant color, 
        //              and whether an image is black & white.
        string requestParameters =
            "visualFeatures=Categories,Description,Color";

        // Assemble the URI for the REST API method.
        string uri = CognitiveServicesUrl + "?" + requestParameters;

        HttpResponseMessage response;

        // Read the contents of the specified local image
        // into a byte array.
        byte[] byteData = GetImageAsByteArray(imageUrl);

        // Add the byte array as an octet stream to the request body.
        using (ByteArrayContent content = new ByteArrayContent(byteData))
        {
            // This example uses the "application/octet-stream" content type.
            // The other content types you can use are "application/json"
            // and "multipart/form-data".
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            // Asynchronously call the REST API method.
            response = await client.PostAsync(uri, content);
        }

        // Asynchronously get the JSON response.
        string contentString = await response.Content.ReadAsStringAsync();
        result = JToken.Parse(contentString).ToString();

        // Log the JSON response.
        log.LogInformation("\nResponse:\n\n{0}\n", result);
    }
    catch (Exception e)
    {
        log.LogInformation("\n" + e.Message);
    }
    return result;
}

static byte[] GetImageAsByteArray(string imageUrl)
{
    using (var webClient = new WebClient()) { 
        byte[] imageBytes = webClient.DownloadData(imageUrl);
        return imageBytes;
    }
}
