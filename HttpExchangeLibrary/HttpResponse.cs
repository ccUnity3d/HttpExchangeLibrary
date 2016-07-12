using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HttpExchangeLibrary
{
    public interface IHttpResponse
    {

    }

    public sealed class HttpResponse : WebResponse, IHttpResponse
    {
    }
}
