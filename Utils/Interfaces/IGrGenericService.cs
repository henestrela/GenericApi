using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils.Helpers;

namespace Utils.Interfaces
{
    public interface IGrGenericService<T> : IGrService where T : class, IEntityModel
    {
        //public  List<X> GetAll<X>(IQueryCollection query) where X : IGenericDTO;
        public Task<X> GetById<X>(Guid id) where X : IBaseIdDTO;
        public Task<X> Create<X>(X entityDTO,bool ignorePermission = false) where X : IBaseIdDTO;
        public Task<X> Update<X>(X entityDTO, Guid id, List<string> ignoreProperties = null) where X : IBaseIdDTO;

        public Task<X> Delete<X>(Guid id, bool ignorePermission = false, bool ignoreOrder = false) where X : IBaseIdDTO;
        public Task<Pagination<X>> GetAll<X>(ODataQueryOptions<T> querySettings) where X : IBaseIdDTO, new();
    }
}
