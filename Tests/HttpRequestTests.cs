using System;
using System.Net;
using HttpExchangeLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HttpRequestTests
    {
        [TestMethod]
        public void Construct_With_Url()
        {
            HttpRequest request = new HttpRequest("test uri");

            Assert.AreEqual("test uri", request.Url);
            Assert.AreEqual(WebRequestMethods.Http.Get, request.Method);
            Assert.AreEqual(0, request.Headers.Count);
            Assert.AreEqual(String.Empty, request.Body);
            Assert.AreEqual(false, request.AllowRedirects);
            Assert.AreEqual(0, request.MaxRedirects);
        }

        [TestMethod]
        public void Construct_With_Url_And_Method()
        {
            HttpRequest request = new HttpRequest("test uri", WebRequestMethods.Http.Post);

            Assert.AreEqual("test uri", request.Url);
            Assert.AreEqual(WebRequestMethods.Http.Post, request.Method);
            Assert.AreEqual(0, request.Headers.Count);
            Assert.AreEqual(String.Empty, request.Body);
            Assert.AreEqual(false, request.AllowRedirects);
            Assert.AreEqual(0, request.MaxRedirects);
        }

        [TestMethod]
        public void Construct_With_Url_Method_And_Headers()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("key1", "val1");

            HttpRequest request = new HttpRequest("test uri", WebRequestMethods.Http.Put, headers);

            Assert.AreEqual("test uri", request.Url);
            Assert.AreEqual(WebRequestMethods.Http.Put, request.Method);
            Assert.AreEqual("val1", request.Headers["key1"]);
            Assert.AreEqual(String.Empty, request.Body);
            Assert.AreEqual(false, request.AllowRedirects);
            Assert.AreEqual(0, request.MaxRedirects);
        }

        [TestMethod]
        public void Construct_With_Url_Method_Headers_And_Body()
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("key1", "val1");

            HttpRequest request = new HttpRequest("test uri", WebRequestMethods.Http.Get, headers, "request body");

            Assert.AreEqual("test uri", request.Url);
            Assert.AreEqual(WebRequestMethods.Http.Get, request.Method);
            Assert.AreSame("val1", request.Headers["key1"]);
            Assert.AreEqual("request body", request.Body);
            Assert.AreEqual(false, request.AllowRedirects);
            Assert.AreEqual(0, request.MaxRedirects);
        }

        [TestMethod]
        public void addHeader()
        {
            HttpRequest request = new HttpRequest("Url");
            request.addHeader("key1", "val1");

            Assert.AreEqual("val1", request.Headers["key1"]);
        }

        [TestMethod]
        public void addHeader_DuplicateKey()
        {
            HttpRequest request = new HttpRequest("Url");
            request.addHeader("key1", "val1");
            request.addHeader("key1", "val2");

            var headVals = request.Headers["key1"];
            Assert.AreEqual("val1,val2", request.Headers["key1"]);
        }

        [TestMethod]
        public void addHeader_MultipleHeaders()
        {
            HttpRequest request = new HttpRequest("Url");
            request.addHeader("key1", "val1");
            request.addHeader("key2", "val2");

            Assert.AreEqual(2, request.Headers.Count);
            Assert.AreEqual("val1", request.Headers["key1"]);
            Assert.AreEqual("val2", request.Headers["key2"]);
        }

        [TestMethod]
        public void clearHeaders()
        {
            HttpRequest request = new HttpRequest("url");
            request.addHeader("key1", "val1");
            request.addHeader("key2", "val2");

            Assert.AreEqual(2, request.Headers.Count);

            request.clearHeaders();
            
            Assert.AreEqual(0, request.Headers.Count);
        }

        [TestMethod]
        public void removeHeader()
        {
            HttpRequest request = new HttpRequest("Url");
            request.addHeader("key1", "val1");

            Assert.AreEqual(1, request.Headers.Count);

            WebHeaderCollection expected = new WebHeaderCollection();
            expected.Add("key1", "val1");

            WebHeaderCollection actual = request.removeHeader("key1");

            Assert.AreEqual(expected["key1"], actual["key1"]);
        }

        [TestMethod]
        public void remove_NonExistentHeader()
        {
            HttpRequest request = new HttpRequest("Url");

            var actual = request.removeHeader("nonexistent-header");

            Assert.AreEqual(String.Empty, actual["nonexistent-header"]);
        }

        [TestMethod]
        public void setHeaders()
        {
            HttpRequest request = new HttpRequest("Uri");
            request.setUserAgent();

            Assert.AreEqual("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)", request.getUserAgent());
        }

        [TestMethod]
        public void setTimeout()
        {
            HttpRequest request = new HttpRequest("Url");
            request.setTimeout(100);

            Assert.AreEqual(100, request.Timeout);
        }

        [TestMethod]    
        public void enableRedirects()
        {
            HttpRequest request = new HttpRequest("Uri");
            request.enableRedirects(10);

            Assert.IsTrue(request.AllowRedirects);
            Assert.AreEqual(10, request.MaxRedirects);
        }
    
        [TestMethod]
        public void disableRedirects()
        {
            HttpRequest request = new HttpRequest("Url");
            
            request.enableRedirects(1);
            request.disableRedirects();

            Assert.IsFalse(request.AllowRedirects);
            Assert.AreEqual(0, request.MaxRedirects);
        }
    
        [TestMethod]
        public void setBody()
        {
            HttpRequest request = new HttpRequest("Url");
            request.setBody("This is the request body");

            Assert.AreEqual("This is the request body", request.Body);
        }
    }
}
