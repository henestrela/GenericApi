using ModelContext;
using ModelContext.Models;
using Service.Helpers;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class StoreService: GenericService<Store>, IStoreService
    {
        public StoreService(WebContext context) : base(context)
        {
        }
    }
}
