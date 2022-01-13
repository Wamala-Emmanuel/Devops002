using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GatewayService.Helpers
{
    public class ApiClient
    {
        public (string, string)[] Headers;
        public string Token;
        private readonly ILogger _logger;

        public ApiClient(string token, List<(string, string)> headers, ILogger logger)
        {
            Token = token;
            _logger = logger;
            Headers = headers.ToArray();
        }

        public async Task<ApiResponse> GetAsync(string url)
        {
            _logger.LogInformation($"get.request url:{url}");
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var client = new HttpClient();
            var request = PrepareRequest(url);
            request.Method = HttpMethod.Get;

            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            var response = await client.SendAsync(request);
            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            
            _logger.LogInformation($"processing response statusCode:{response.StatusCode} url:{url}");
            return await ProcessResponse(response);
        }

        public async Task<ApiResponse> DeleteAsync(string url)
        {
            _logger.LogInformation($"delete.request url:{url}");
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var client = new HttpClient();
            var request = PrepareRequest(url);
            request.Method = HttpMethod.Delete;

            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            var response = await client.SendAsync(request);
            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");

            _logger.LogInformation($"processing response statusCode:{response.StatusCode} url:{url}");
            return await ProcessResponse(response);
        }

        public async Task<ApiResponse> PostAsync(string url, object obj)
        {
            _logger.LogInformation($"post.request Url:{url} Data:{obj}");
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var buffer = Encoding.UTF8.GetBytes(json);
            var content = new ByteArrayContent(buffer);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = PrepareRequest(url);
            request.Method = HttpMethod.Post;
            request.Content = content;

            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            var response = await client.SendAsync(request);
            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");

            _logger.LogInformation($"processing response statusCode:{response.StatusCode} url:{url}");
            return await ProcessResponse(response);
        }

        public async Task<ApiResponse> PostFormAsync(string url, MultipartFormDataContent formDataContent)
        {

            _logger.LogInformation($"post.request Url:{url}");
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient();

            using var form = formDataContent;
            form.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            var response = await client.PostAsync(url, form);
            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");

            _logger.LogInformation($"processing response statusCode:{response.StatusCode} url:{url}");
            return await ProcessResponse(response);
        }

        public async Task<ApiResponse> PostUrlEncodedFormAsync(string url, FormUrlEncodedContent urlEncodedContent)
        {

            _logger.LogInformation($"post.request Url:{url}");
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient();

            var request = PrepareRequest(url);
            request.Method = HttpMethod.Post;
            using var content = urlEncodedContent;
            request.Content = content;

            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            var response = await client.SendAsync(request);
            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");

            _logger.LogInformation($"processing response statusCode:{response.StatusCode} url:{url}");
            return await ProcessResponse(response);
        }

        public async Task<ApiResponse> UpdateAsync(string url, object obj)
        {
            _logger.LogInformation($"update.request Url:{url} Data:{obj}");
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(obj);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var content = new ByteArrayContent(buffer);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = PrepareRequest(url);
            request.Method = HttpMethod.Put;
            request.Content = content;

            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");
            var response = await client.SendAsync(request);
            _logger.LogInformation($"Response recieved at: {DateTime.UtcNow}.");

            _logger.LogInformation($"processing response statusCode:{response.StatusCode} url:{url}");
            return await ProcessResponse(response);
        }


        private async Task<ApiResponse> ProcessResponse(HttpResponseMessage response)
        {
            var apiResponse = new ApiResponse
            {
                StatusCode = response.StatusCode,
                Errors = new List<string>()
            };
            var data = await response.Content.ReadAsStringAsync();
            apiResponse.Data = data; // Keep this data anyway
            if (response.IsSuccessStatusCode)
            {
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.Message = "success";
            }
            // We did not get any data from the service
            else if (string.IsNullOrEmpty(data))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        apiResponse.Message = "Access denied: refresh session";
                        apiResponse.Errors.Add("Access denied: refresh session");
                        break;
                    case HttpStatusCode.NotFound:
                        apiResponse.Message = "Invalid service path";
                        apiResponse.Errors.Add("Access denied: Login again");
                        break;
                    default:
                        apiResponse.Message = "Unknown Server Error";
                        apiResponse.Errors.Add("Unknown Server Error");
                        break;
                }
            }
            // service sent us errors
            else
            {
                try
                {
                    //try Parsing a data validation error
                    var errorStruct = new
                    {
                        Message = "",
                        Errors = new Dictionary<string, string[]>(), // List of errors
                        ModelState = new Dictionary<string, string[]>()
                    };

                    var errorObject = JsonConvert.DeserializeAnonymousType(data, errorStruct);

                    apiResponse.Message = errorObject?.Message ?? "Unknown server error";
                    // It could be a validation error
                    if (errorObject?.ModelState != null)
                    {
                        var errors = errorObject.ModelState.Select(kvp => string.Join("\n", kvp.Value))
                            .ToList();
                        for (var i = 0; i < errors.Count; i++)
                        {
                            apiResponse.Errors.Add(errors.ElementAt(i));
                        }
                    }
                    // Or a list of errors
                    else if (errorObject?.Errors != null)
                    {
                        var errorList = errorObject.Errors.Select(it => $"{it.Key}: {it.Value[0]}");
                        apiResponse.Errors.AddRange(errorList);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "failed to parse server errors");
                }
            }

            _logger.LogDebug(
                $"complete response statusCode:{apiResponse.StatusCode} message:{apiResponse.Message} data:{data}");
            _logger.LogInformation($"complete response statusCode:{apiResponse.StatusCode} message:{apiResponse.Message}");
            return apiResponse;
        }

        private HttpRequestMessage PrepareRequest(string url)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url)
            };

            if (!string.IsNullOrWhiteSpace(Token))
            {
               request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }

            foreach (var header in Headers)
            {
                request.Headers.Add(header.Item1, header.Item2);
            }

            return request;
        }
    }
}