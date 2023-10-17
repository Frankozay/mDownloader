using System;
using System.Linq;

namespace mDownloader.Converters
{
    public static class SimpleMapper
    {
        public static TTarget Map<TSource, TTarget>(TSource source, object[]? ctorArgs = null, Boolean expectedId = false)
            where TSource : class
            where TTarget : class
        {
            if (source == null) throw new ArgumentNullException();
            ctorArgs = ctorArgs ?? new object[0];
            var target = Activator.CreateInstance(typeof(TTarget), ctorArgs) as TTarget;

            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties();

            foreach (var sourceProp in sourceProperties)
            {
                if (expectedId && sourceProp.Name == "Id")
                {
                    continue;
                }
                var targetProp = targetProperties.FirstOrDefault(p =>
                p.Name == sourceProp.Name);
                if (targetProp != null && targetProp.CanWrite && sourceProp.CanRead)
                {
                    var value = sourceProp.GetValue(source);
                    if (value != null || Nullable.GetUnderlyingType(sourceProp.PropertyType) != null)
                    {
                        targetProp.SetValue(target, value);
                    }
                }

            }
            return target!;
        }
    }
}
