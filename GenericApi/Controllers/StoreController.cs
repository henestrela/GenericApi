using Microsoft.AspNetCore.Mvc;
using ModelContext.Models;
using Service.DTO;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Interfaces;

namespace GenericApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class StoreController : GrGenericController<StoreDTO, Store>
    {
        public StoreController(IStoreService service) : base(service)
        {
        }
    }
}
