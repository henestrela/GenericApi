using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Utils.Interfaces;

namespace Utils.Helpers
{
    public static class ServiceHelper
    {
        public static IQueryable<Y> ImplementCustomQuery<X, Y>(IQueryable<Y> query, X entity, IDictionary<string, object> parameters = null) where Y : class, IEntity where X : new()
        {
            if (entity is ICustomQueryDTO<Y>)
            {
                return ((ICustomQueryDTO<Y>)entity).ApplyQuery(query, parameters);
            }

            return query;
        }


        public static string[] GetMembersToExpandNames(ODataQueryOptions options)
        {
            return GetExpandPropertyPaths(GetExpandItems(options?.SelectExpand?.SelectExpandClause)).ToArray();
        }
        public static IEnumerable<string> GetExpandPropertyPaths(IEnumerable<ExpandedNavigationSelectItem> items, string prefix = "")
        {
            foreach (ExpandedNavigationSelectItem item in items)
            {
                foreach (string res in GetExpandPropertyPaths(item, prefix))
                {
                    yield return res;
                }
            }
        }

        public static IEnumerable<string> GetExpandPropertyPaths(ExpandedNavigationSelectItem item, string prefix = "")
        {
            string curPropName = item.NavigationSource.Name;
            ExpandedNavigationSelectItem[] nestedExpand = GetExpandItems(item.SelectAndExpand).ToArray();
            if (nestedExpand.Count() > 0)
            {
                foreach (string res in GetExpandPropertyPaths(nestedExpand, $"{prefix}{curPropName}."))
                {
                    yield return res;
                }
            }
            else
            {
                yield return $"{prefix}{curPropName}";
            }
        }
        public static IEnumerable<ExpandedNavigationSelectItem> GetExpandItems(SelectExpandClause sec)
        {
            if (sec != null)
            {
                IEnumerable<ExpandedNavigationSelectItem> res = sec?.SelectedItems?.OfType<ExpandedNavigationSelectItem>();
                if (res != null)
                {
                    return res;
                }
            }
            return new ExpandedNavigationSelectItem[0];
        }

    }
}
