﻿using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyModel;
using Natasha.Template;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Natasha
{

    public class AssemblyDomain : AssemblyLoadContext, IDisposable
    {

        public readonly ConcurrentDictionary<string, Assembly> ClassMapping;
        public readonly ConcurrentDictionary<string, Assembly> DynamicDlls;
        public readonly ConcurrentQueue<PortableExecutableReference> References;


#if NETCOREAPP3_0
        private readonly AssemblyDependencyResolver _resolver;
#endif




        public AssemblyDomain(string key)
#if NETCOREAPP3_0
            : base(isCollectible: true,name:key)
#endif

        {
#if NETCOREAPP3_0
            _resolver = new AssemblyDependencyResolver(AppDomain.CurrentDomain.BaseDirectory);
#endif
            ClassMapping = new ConcurrentDictionary<string, Assembly>();
            DynamicDlls = new ConcurrentDictionary<string, Assembly>();
            References = new ConcurrentQueue<PortableExecutableReference>();
            this.Unloading += AssemblyDomain_Unloading;

        }




        private void AssemblyDomain_Unloading(AssemblyLoadContext obj)
        {
            //throw new NotImplementedException();
        }




        public void Dispose()
        {

#if NETCOREAPP3_0
            References.Clear();
#endif
            ClassMapping.Clear();
            DynamicDlls.Clear();

        }




        protected override Assembly Load(AssemblyName assemblyName)
        {

#if NETCOREAPP3_0
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
            {

                return LoadFromAssemblyPath(assemblyPath);

            }
#endif
            return null;

        }





        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {

#if NETCOREAPP3_0
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }
#endif

            return IntPtr.Zero;

        }




        public void CacheAssembly(Assembly assembly,Stream stream = null)
        {

            var types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {

                ClassMapping[types[i].Name] = assembly;

            }

            if (stream != null)
            {

                stream.Position = 0;
                var reference = MetadataReference.CreateFromStream(stream);
                References.Enqueue(reference);
                AssemblyManagment.AddRererence(reference);

            }

        }




        public void LoadFile(string path)
        {

            if (!DynamicDlls.ContainsKey(path))
            {

                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    CacheAssembly(LoadFromStream(stream), stream);
                }

            }

        }




        public Assembly GetDynamicAssembly(string className)
        {

            if (ClassMapping.ContainsKey(className))
            {

                return ClassMapping[className];

            }
            return null;

        }




        /// <summary>
        /// 根据类名获取类，前提类必须是成功编译过的
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Type GetType(string name)
        {

            return ClassMapping[name].GetTypes().First(item => item.Name == name);

        }



#if NETCOREAPP3_0
        public T Execute<T>(Func<T,T> action) where T: TemplateRecoder<T>, new()
        {

            return action?.Invoke(new T()).InDomain(this);

        }
#endif

    }

}
