using System.Collections.Generic;
using System.Linq;

namespace Utils.Interfaces
{
    public interface ICustomQueryDTO<T>
    {
        public IQueryable<T> ApplyQuery(IQueryable<T> query, IDictionary<string, object> parameters = null);
    }
}
