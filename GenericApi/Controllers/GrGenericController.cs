
using Microsoft.AspNetCore.Mvc;
using Service.Helpers;
using System;
using System.Threading.Tasks;
using Utils.Interfaces;

namespace GenericApi.Controllers
{
    public abstract class GrGenericController<X,T> : GrBaseController, IGrGenericController<T> where T : class, IEntityModel where X : IBaseIdDTO, new()
    {
        private readonly IGrGenericService<T> Service;

        public GrGenericController(IGrGenericService<T> service)
        {
            Service = service;
        }
        public virtual async Task<IActionResult> Create(X newDTO)
        {
            X entity = await Service.Create<X>(newDTO);

            return Ok(entity);
        }

        public virtual async Task<IActionResult> Update(X update, Guid id)
        {
            X entity = await Service.Update<X>(update, id);

            return Ok(entity);
        }


        public virtual async Task<IActionResult> Delete(Guid id)
        {
            X entity = await Service.Delete<X>(id);

            return Ok(entity);
        }

        public virtual async Task<IActionResult> GetById(Guid id)
        {
            X entity = await Service.GetById<X>(id);

            return Ok(entity);
        }


    }
}
