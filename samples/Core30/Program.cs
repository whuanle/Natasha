﻿using Natasha;
using Natasha.Operator;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Core30
{
    class Program
    {
        public static Action action;
        static void Main(string[] args)
        {
            Console.WriteLine("隔离编译动态方法:");
            Console.WriteLine();
            Show();
            if (action!=null)
            {
                Console.WriteLine("\t静态引用动态方法，增加方法代数！");
            }
            //var a = AssmblyManagment.Remove("TempDomain");
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("第一次检测：");
            Console.WriteLine(AssemblyManagment.IsDelete("TempDomain") ? "回收成功！" : "回收失败！");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("启用GC回收方法！");

            for (int i = 0;(!AssemblyManagment.IsDelete("TempDomain")) && (i < 15); i++)
            {
                Console.WriteLine($"\t第{i}次！");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(500);
                if (i==6)
                {
                    Console.WriteLine($"\t计数为{i}，删除静态引用！");
                    //千万别再这里调用 AssemblyManagment.Get("TempDomain").Dispose();
                    action = null;
                }
                
            }
            Console.WriteLine();
            Console.WriteLine();
            //Console.WriteLine(!a.IsAlive? "回收成功！":"回收失败！");
            Console.Write("第二次检测：");
            Console.WriteLine(AssemblyManagment.IsDelete("TempDomain") ? "回收成功！" : "回收失败！");
            
            action?.Invoke();
            Console.ReadKey();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Show()
        {

            var domain = AssemblyManagment.Create("TempDomain");
            NStruct nStruct = new NStruct();
            nStruct
                .InDomain(domain)
                .Namespace("StructDomainNamespace")
                .OopName("SturctDomain")
                .Ctor(builder=>builder
                    .MemberAccess(AccessTypes.Public)
                    .Param<string>("name")
                    .Body("Name=name;"))
                .PublicField<string>("Name");
            var type =  nStruct.GetType();



            var domain1 = AssemblyManagment.Create("MethodTempDomain");
            var temp = domain1.Execute<FastMethodOperator>(builder =>
            {
                return builder
                .Using(type)
                //.MethodAttribute<MethodImplAttribute>("MethodImplOptions.NoInlining")
                .MethodBody(@"
SturctDomain obj = new SturctDomain(""Hello World!"");
Console.WriteLine(obj.Name);"
);
            });
            action = temp.Complie<Action>();
            action();
            //AssemblyManagment.Remove("TempDomain");
            //AssemblyManagment.Remove("MethodTempDomain");
        }

    }
}
