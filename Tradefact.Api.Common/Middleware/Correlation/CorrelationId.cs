using System;

namespace Tradefact.Api.Common.Middleware.Correlation
{
    public class CorrelationId
    {
        public CorrelationId(Guid current)
        {
            Current = current;
        }

        public Guid Current { get; }
    }
}
