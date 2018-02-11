using System;
using Newtonsoft.Json;
using System.Net.Http;
using JsonApiSerializer;
using System.Net.Http.Headers;
using JsonApiSerializer.JsonApi;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace JsonApiConsumer
{
	public static class JsonApiConsumer
	{
        #region Static variables
        /// <summary>
        /// ContentType parameter.
        /// </summary>
        static string strContentType = "application/vnd.api+json";

		static MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue(strContentType);

		public static void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
		{
			errorArgs.ErrorContext.Handled = true;
		}

        /// <summary>
        /// Jsonapi serializer settings.
        /// </summary>
		static JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
		{
			Error = HandleDeserializationError
		};
        #endregion

        #region Static methods
        public static Response<TReqeust[]> Get<TReqeust>(string baseURI,
										                 string path,
                                                         Dictionary<string, string> query = null,
                                                         Dictionary<string, string> headers = null,
                                                         IList<string> relations = null) where TReqeust : class, new()
		{
			var response = new Response<TReqeust[]>();

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseURI);

				client.DefaultRequestHeaders.Accept.Add(contentType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }

                string uri = $"/{path}";

                if (relations != null)
                {
                    uri = relations.Aggregate(uri, (current, relation) => $"{current}/{relation}");
                }

                if (query != null)
                {
                    uri = QueryHelpers.AddQueryString(uri, query);
                }

                HttpResponseMessage responseMessage = client.GetAsync(uri).Result;

                response.HttpStatusCode = responseMessage.StatusCode;

				response.IsSuccess = responseMessage.IsSuccessStatusCode;

				if (responseMessage.IsSuccessStatusCode)
				{
					response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TReqeust[]>>(responseMessage.Content.ReadAsStringAsync().Result, settings);
				}
				else
				{
					response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
				}
			}

			return response;
		}

		public static Response<TRequest> GetByID<TRequest>(string id,
			                                               string baseURI,
			                                               string path,
                                                           Dictionary<string, string> query = null,
                                                           Dictionary<string, string> headers = null,
			                                               IList<string> relations = null) where TRequest : class, new()
		{
			var response = new Response<TRequest>();

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseURI);

				client.DefaultRequestHeaders.Accept.Add(contentType);

				if (headers != null)
				{
					foreach (var header in headers)
					{
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
					}
				}

				string uri = $"/{path}/{id}";

                if (relations != null)
                {
                    uri = relations.Aggregate(uri, (current, relation) => $"{current}/{relation}");
                }

                if (query != null)
                {
                    uri = QueryHelpers.AddQueryString(uri, query);
                }

				HttpResponseMessage responseMessage = client.GetAsync(uri).Result;

				response.HttpStatusCode = responseMessage.StatusCode;

				response.IsSuccess = responseMessage.IsSuccessStatusCode;

                if (responseMessage.IsSuccessStatusCode)
                {
                    response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TRequest>>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
                else
                {
                    response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
			}

			return response;
		}

        public static Response<TResponse> Create<TRequest, TResponse>(TRequest model,
                                                                      string baseURI,
                                                                      string path,
                                                                      Dictionary<string, string> headers = null) where TRequest : class, new()
                                                                                                                 where TResponse : class, new()
        {
            var response = new Response<TResponse>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURI);

                client.DefaultRequestHeaders.Accept.Add(contentType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }

                string stringData = JsonConvert.SerializeObject(model, settings);

                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, strContentType);

                contentData.Headers.ContentType = contentType;

                HttpResponseMessage responseMessage = client.PostAsync("/" + path, contentData).Result;

                response.HttpStatusCode = responseMessage.StatusCode;

                response.IsSuccess = responseMessage.IsSuccessStatusCode;

                if (responseMessage.IsSuccessStatusCode)
                {
                    response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TResponse>>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
                else
                {
                    response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
            }

            return response;
        }

        public static Response<TResponse> PostFile<TResponse>(string key,
                                                              byte[] data,
                                                              string fileName,
                                                              string baseURI,
                                                              string path,
                                                              Dictionary<string, string> headers = null) where TResponse : class, new()
        {
            var response = new Response<TResponse>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURI);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }

                var bytes = new ByteArrayContent(data);

                var contentData = new MultipartFormDataContent() {
                    { bytes, key, fileName ?? key }
                };

                HttpResponseMessage responseMessage = client.PostAsync("/" + path, contentData).Result;

                response.HttpStatusCode = responseMessage.StatusCode;

                response.IsSuccess = responseMessage.IsSuccessStatusCode;

                if (responseMessage.IsSuccessStatusCode)
                {
                    response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TResponse>>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
                else
                {
                    response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
            }

            return response;
        }

        public static Response<TResponse> Update<TRequest, TResponse>(string id, 
                                                                      TRequest model, 
                                                                      string baseURI, 
                                                                      string path, 
                                                                      Dictionary<string, string> headers = null) where TRequest : class, new()
                                                                                                                 where TResponse : class, new()
        {
			var response = new Response<TResponse>();

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseURI);

				client.DefaultRequestHeaders.Accept.Add(contentType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), string.Format("/{0}/{1}", path, id));

				var contentData = new StringContent(JsonConvert.SerializeObject(model, settings),
													System.Text.Encoding.UTF8,
													strContentType);

				contentData.Headers.ContentType = contentType;

				request.Content = contentData;

				HttpResponseMessage responseMessage = client.SendAsync(request).Result;

				response.HttpStatusCode = responseMessage.StatusCode;

				response.IsSuccess = responseMessage.IsSuccessStatusCode;

				if (responseMessage.IsSuccessStatusCode)
				{
					response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TResponse>>(responseMessage.Content.ReadAsStringAsync().Result, settings);
				}
				else
				{
					response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
				}
			}

			return response;
		}

		public static Response<TResponse> Patch<TRequest, TResponse>(TRequest model, 
                                                                     string baseURI, 
                                                                     string path, 
                                                                     Dictionary<string, string> headers = null) where TRequest : class, new()
                                                                                                                where TResponse : class, new()
        {
			var response = new Response<TResponse>();

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseURI);

				client.DefaultRequestHeaders.Accept.Add(contentType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), string.Format("/{0}", path))
				{
					Content = new StringContent(JsonConvert.SerializeObject(model, settings),
					                        	System.Text.Encoding.UTF8,
					                        	strContentType)
				};

				HttpResponseMessage responseMessage = client.SendAsync(request).Result;

				response.HttpStatusCode = responseMessage.StatusCode;

				response.IsSuccess = responseMessage.IsSuccessStatusCode;

                if (!response.IsSuccess)
                {
                    response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }

				if (responseMessage.IsSuccessStatusCode)
				{
					response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TResponse>>(responseMessage.Content.ReadAsStringAsync().Result, settings);
				}
				else
				{
					response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
				}
			}

			return response;
		}

		public static Response<TResponse> Delete<TResponse>(string id, 
                                                            string baseURI, 
                                                            string path, 
                                                            Dictionary<string, string> headers = null) where TResponse : class, new()
		{
			var response = new Response<TResponse>();

			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseURI);

				client.DefaultRequestHeaders.Accept.Add(contentType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Value))
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                }

                HttpResponseMessage responseMessage = client.DeleteAsync(string.Format("/{0}/{1}", path, id)).Result;

				response.HttpStatusCode = responseMessage.StatusCode;

				response.IsSuccess = responseMessage.IsSuccessStatusCode;

				if (!responseMessage.IsSuccessStatusCode)
				{
					response.DocumentRoot = JsonConvert.DeserializeObject<DocumentRoot<TResponse>>(responseMessage.ReasonPhrase, settings);
				}
                else
                {
                    response.Error = JsonConvert.DeserializeObject<Error>(responseMessage.Content.ReadAsStringAsync().Result, settings);
                }
            }

			return response;
		}
		#endregion
	}
}
