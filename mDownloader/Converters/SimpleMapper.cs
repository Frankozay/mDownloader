using mDownloader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mDownloader.Converters
{
    public static class SimpleMapper
    {
        public static TTarget Map<TSource, TTarget>(TSource source, Boolean expectedId=false)
            where TSource : class
            where TTarget : class, new()
        {
            if (source == null) throw new ArgumentNullException();
            var target = new TTarget();

            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties();

            foreach ( var sourceProp in sourceProperties )
            {
                if (expectedId && sourceProp.Name == "Id") { 
                    continue;
                }
                var targetProp = targetProperties.FirstOrDefault(p =>
                p.Name == sourceProp.Name &&
                (p.PropertyType == sourceProp.PropertyType ||
                p.PropertyType == Nullable.GetUnderlyingType(sourceProp.PropertyType)));
                if(targetProp != null && targetProp.CanWrite && sourceProp.CanRead)
                {
                    Debug.WriteLine(targetProp.Name);
                    var value = sourceProp.GetValue(source);
                    if (value != null || Nullable.GetUnderlyingType(sourceProp.PropertyType) != null)
                    {
                        targetProp.SetValue(target, value);
                    }
                }

            }
            return target;
        }
    }
}
