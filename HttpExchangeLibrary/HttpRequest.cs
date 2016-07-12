using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace HttpExchangeLibrary
{
    public interface IHttpRequest
    {
        IHttpResponse getResponse();
        void addHeader(string key, string value);
        WebHeaderCollection removeHeader(string key);
        void clearHeaders();
        void setBody(string jsonBody);
        void setUserAgent(string agent);
        void setTimeout(int timeout);
        string getUserAgent();
    }

    public sealed class HttpRequest : IHttpRequest
    {
        public string Url { get; private set; }
        public string Method { get; private set; }
        public bool AllowRedirects { get; private set; }
        public int MaxRedirects {get; private set; }
        public string Body { get; private set; }
        public int Timeout { get; private set; }
        public WebHeaderCollection Headers { get; private set; }

        public HttpRequest(string url)
        {
            Url = url;
            Method = WebRequestMethods.Http.Get;
            Headers = new WebHeaderCollection();
            Body = String.Empty;
            AllowRedirects = false;
            MaxRedirects = 0;
        }

        public HttpRequest(string url, string method)
            : this(url)
        {
            Method = method;
            Headers = new WebHeaderCollection();
            Body = String.Empty;
        }

        public HttpRequest(string url, string method, WebHeaderCollection headers)
            : this(url, method)
        {
            Headers = headers;
            Body = String.Empty;
        }

        public HttpRequest(string url, string method, WebHeaderCollection headers, string body)
            : this(url, method, headers)
        {
            Body = body;
        }

        /// <summary>
        /// Adds the specified Key Value Pair as a header to the Request Object.
        /// <para>NOTE: If there is already a header with the specified key that new header value will be appeneded to the previous value.</para>
        /// </summary>
        /// <param name="key">Identifying Key for the header. i.e. Authorization.</param>
        /// <param name="value">Value the Header Key should contain.</param>
        /// <returns>Current Headers to the Request.</returns>
        public void addHeader(string key, string value)
        {
            Headers.Add(key, value);
        }

        /// <summary>
        /// Removes the Header and value for the specified header key.
        /// </summary>
        /// <param name="key">Name of the Header to be removed.</param>
        /// <returns>A WebHeaderCollection containing only the header that was removed.</returns>
        public WebHeaderCollection removeHeader(string key)
        {
            WebHeaderCollection removedHeader = new WebHeaderCollection();
            removedHeader.Add(key, Headers[key]);

            Headers.Remove(key);

            return removedHeader;
        }

        /// <summary>
        /// Removes all of the Request Headers.
        /// </summary>
        public void clearHeaders()
        {
            Headers.Clear();
        }

        /// <summary>
        /// Sets the time (in seconds) this request will wait for a response before timine out.
        /// </summary>
        /// <param name="timeout">time in seconds to wait for a response.</param>
        public void setTimeout(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Sets the User Agent Header with the specified value
        /// </summary>
        /// <param name="agent">UserAgent Identifier</param>
        public void setUserAgent(string agent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)")
        {
            addHeader("user-agent", agent);
        }

        /// <summary>
        /// Retrieves the current user agent set for this request.
        /// </summary>
        /// <returns>A user agent identifier</returns>
        public string getUserAgent()
        {
            return Headers["user-agent"];
        }

        /// <summary>
        /// Sets the Body of the request
        /// </summary>
        /// <param name="jsonBody">JSON Formatted Body for the Request.</param>
        public void setBody(string jsonBody)
        {
            Body = jsonBody;
        }

        /// <summary>
        /// Enables and sets maximum number of Redirects allowed.
        /// </summary>
        public void enableRedirects(int max)
        {
            AllowRedirects = true;
            MaxRedirects = max;
        }

        /// <summary>
        /// Disbales redirects from being allowed and sets the max number of allowed redirects to zero.
        /// </summary>
        public void disableRedirects()
        {
            AllowRedirects = false;
            MaxRedirects = 0;
        }

        /// <summary>
        /// Returns the Response for the configured HTTP Request.
        /// </summary>
        /// <returns>The Response received from this request.</returns>
        public IHttpResponse getResponse()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Method = Method;
            request.Timeout = Timeout;
            request.AllowAutoRedirect = AllowRedirects;
            request.MaximumAutomaticRedirections = MaxRedirects;
            request.Headers = Headers;
            if (!String.IsNullOrEmpty(Body))
            {
                request.Headers.Add("content-type", "application/json");
                using (var stream = request.GetRequestStream())
                using (var writer = new StreamWriter(stream))
                    writer.Write(Body);
            }

            HttpResponse response = send(request);

            return (IHttpResponse)response;
        }

        /// <summary>
        /// Retrieves the Response for the specified HTTP Request
        /// </summary>
        /// <param name="request">The Http Request to be sent.</param>
        /// <returns>The HttpResponse generated by teh Http Request</returns>
        private HttpResponse send(HttpWebRequest request)
        {
            HttpResponse response;
            try
            {
                response = (HttpResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpResponse)e.Response;
            }

            return response;
        }
    }
}
