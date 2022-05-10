using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Api.Model.Collab
{
    public class MessagePost
    {
        public string Comment { get; set; }
        public ActivityEntityTypeEnum Entity { get; set; }
    }
}
