using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zhouatnet.Utilities
{
    public static class ObjectExtension
    {
        public static TDestination CopyTo<TScource, TDestination>(this TScource src, TDestination dest) where TDestination : TScource
        {
            if (src == null || dest == null)
            {
                throw new ArgumentNullException();
            }

            if (ReferenceEquals(src, dest))
            {
                return dest;
            }

            Type srcType = typeof(TScource);

            foreach (var prop in srcType.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                prop.SetValue(dest, prop.GetValue(src));
            }

            return dest;
        }

        public static TDestination TransferTo<TScource, TDestination>(this TScource src, TDestination dest)
        {
            if (src == null || dest == null)
            {
                throw new ArgumentNullException();
            }

            if (ReferenceEquals(src, dest))
            {
                return dest;
            }

            Type srcType = typeof(TScource);
            Type destType = typeof(TDestination);

            var srcProps = srcType.GetProperties().Where(p => p.CanRead).ToDictionary(m => m.Name);
            var destProps = destType.GetProperties().Where(p => p.CanWrite).ToDictionary(m => m.Name);
            foreach (var prop in srcProps)
            {
                if (destProps.ContainsKey(prop.Key) && destProps[prop.Key].PropertyType.IsAssignableFrom(prop.Value.PropertyType))
                {
                    destProps[prop.Key].SetValue(dest, prop.Value.GetValue(src));
                }
            }

            return dest;
        }
    }
}
