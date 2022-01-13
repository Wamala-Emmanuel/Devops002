using System;

namespace GatewayService.Extensions
{
    public static class Documentation
    {
        public static string Version { get; set; } = "1.0";

        public static string Title { get; set; } = "API Description";

        public static string Description { get; set; } =
            $"The API provides an interface that developers of systems that need to verify Ugandan National IDs can use in a very simple and straightforward way. Under the hood, the API interfaces with the National Identification and Registration Authority (NIRA) API. This means that the endpoints provided by this API as well as request and response times largely depend on the abilities of the NIRA API." +
            $"{Environment.NewLine} {Environment.NewLine}" +
            $"To ensure that as many ID verification requests as possible are processed, all incoming calls are queued and a response is sent back immediately indicating that the request has been received for processing. This response also includes an <strong>unique identifier</strong> of the request that can be used later to check for the status of the request. This approach has also been primarily influenced by the capabilities of the NIRA API whose response times are not very predictable." +
            $"{Environment.NewLine} {Environment.NewLine}" +
            $"This means that the ID verification request is a two (2) step process from your end." +
            $"{Environment.NewLine} {Environment.NewLine}" +
            $"<ol><li>Send a request to the <em>Id Verification</em> endpoint the response of which will include the unique identifier of the request.</li>" +
            $"<li>Use this unique identifier to get the status of the request by calling the <em>Get request status</em> endpoint.</li></ol>" +
            $"{Environment.NewLine} {Environment.NewLine}" +
            $"Later, you can either search or export a selection of previous requests to CSV format.";
    }
}