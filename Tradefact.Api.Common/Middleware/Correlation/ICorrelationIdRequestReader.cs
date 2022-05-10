using System;
using Microsoft.AspNetCore.Http;

namespace Tradefact.Api.Common.Middleware.Correlation
{
    public interface ICorrelationIdRequestReader
    {
        Guid? Read(HttpContext context);
    }
}
