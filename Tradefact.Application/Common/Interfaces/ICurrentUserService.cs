using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Name { get; }
        string Email { get; }
    }
}
