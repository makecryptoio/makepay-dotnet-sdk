using System;
using System.Net;

namespace MakePay;

public sealed class MakePayException : Exception
{
    public MakePayException(string message)
        : base(message)
    {
    }

    public MakePayException(string message, HttpStatusCode statusCode, string responseBody)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public HttpStatusCode? StatusCode { get; }

    public string? ResponseBody { get; }
}
