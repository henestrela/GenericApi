using System;
using System.Linq.Expressions;

namespace Utils.LinqHelpers
{
    public static class LinqMethods
    {
        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
        {
            return ((MemberExpression)memberAccess.Body).Member.Name;
        }
    }
}
