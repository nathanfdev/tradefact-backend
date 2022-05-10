using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Models.Collab
{
    public class ExtendedMessage: Message
    {
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
    }
}
