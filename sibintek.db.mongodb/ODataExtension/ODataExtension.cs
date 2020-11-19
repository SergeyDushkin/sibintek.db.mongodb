using System;
using System.Web;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        private static string Get(this Uri request, string name)
        {
            return HttpUtility.ParseQueryString(request.Query).Get(name);
        }
        private static T Get<T>(this Uri request, string name)
        {
            var value = HttpUtility.ParseQueryString(request.Query).Get(name);
            
            if (String.IsNullOrEmpty(value))
            {
                return default(T);
            }
            
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
