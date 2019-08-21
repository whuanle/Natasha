using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Natasha
{

    public class AssemblyManagment
    {

        public readonly static HashSet<PortableExecutableReference> References;
        public readonly static ConcurrentDictionary<string, WeakReference> Cache;
        public readonly static AssemblyDomain Default;
        static AssemblyManagment()
        {

            Cache = new ConcurrentDictionary<string, WeakReference>();
            var _ref = DependencyContext.Default.CompileLibraries
                               .SelectMany(cl => cl.ResolveReferencePaths())
                               .Select(asm => MetadataReference.CreateFromFile(asm));
            References = new HashSet<PortableExecutableReference>(_ref);
            Default = Create("Default");

        }





        public static AssemblyDomain Create(string key)
        {
            var instance = new AssemblyDomain(key);
            Add(key, instance);
            return instance;
        }




        public static AssemblyDomain Create(string key, string path)
        {
            var instance = new AssemblyDomain(path);
            Add(key, instance);
            return instance;
        }




        public static void Add(string key, AssemblyDomain domain)
        {

            if (Cache.ContainsKey(key))
            {

                ((AssemblyDomain)(Cache[key].Target)).Dispose();
                if (!Cache[key].IsAlive)
                {
                    Cache[key] = new WeakReference(domain);
                }

            }
            else
            {

                Cache[key] = new WeakReference(domain, trackResurrection: true);

            }

        }




        public static WeakReference Remove(string key)
        {

            if (Cache.ContainsKey(key))
            {

                var domain = ((AssemblyDomain)(Cache[key].Target));
                References.ExceptWith(domain.References);
                domain.Dispose();
                Cache.TryRemove(key, out var result);
                return result;

            }

            throw new Exception($"Can't find key : {key}!");
        }




        public static bool IsDelete(string key)
        {

            if (Cache.ContainsKey(key))
            {
                return !Cache[key].IsAlive;
            }
            return true;

        }




        public static AssemblyDomain Get(string key)
        {

            if (Cache.ContainsKey(key))
            {
                return (AssemblyDomain)Cache[key].Target;
            }
            return null;

        }

    }

}
