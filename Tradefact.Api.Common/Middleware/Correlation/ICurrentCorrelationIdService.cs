using System;

namespace Tradefact.Api.Common.Middleware.Correlation
{
    public interface ICurrentCorrelationIdService
    {
        CorrelationId Current();

        void SetId(Guid correlationId);
    }
}
