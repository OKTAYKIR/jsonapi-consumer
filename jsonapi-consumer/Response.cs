using System.Net;
using JsonApiSerializer.JsonApi;

namespace JsonApiConsumer
{
	public class Response<T>
	{
		public DocumentRoot<T> DocumentRoot { get; internal set; }
		public HttpStatusCode HttpStatusCode { get; internal set; }
		public Error Error { get; set; }
		public bool IsSuccess { get; internal set; }
	}
}