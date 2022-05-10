using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tradefact.Portal.Controllers
{
    public class PurchaseOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
