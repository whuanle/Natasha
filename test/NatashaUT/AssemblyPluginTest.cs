﻿using Natasha;
using Natasha.Operator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace NatashaUT
{

    [Trait("程序集编译测试", "插件")]
    public class AssemblyPluginTest
    {
#if !NETCOREAPP2_2

        [Fact(DisplayName = "可回收：解构插件")]
        public void Test4()
        {
            using (DomainManagment.CreateAndLock("TempDomain14"))
            { 

                var (Assembly, TypeCache) = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "Sql", "ClassLibrary1.dll");
                Assert.Equal("ClassLibrary1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Assembly.FullName);
                Assert.Equal("Class1", TypeCache["Class1"].Name);

            }
        }


        [Fact(DisplayName = "不可回收：MySql插件")]
        public void Test1()
        {

            Assert.False(GetResult1());
            for (int i = 0; i < 6; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.False(DomainManagment.IsDeleted("TempDomain11"));

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool GetResult1()
        {
            bool result=false;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib","Sql", "ClassLibrary1.dll");
            using (DomainManagment.CreateAndLock("TempDomain11"))
            {

                var domain = DomainManagment.CurrentDomain;
                var assemebly = domain.LoadFile(path);
                var action = FastMethodOperator.New
                   .Using(assemebly)
                   .MethodBody(@"Class1 a = new Class1();return  a.Show();")
                   .Complie<Func<bool>>();
                result = action();
                domain.Dispose();
                domain.Unload();
            }
            return result;
        }


        [Fact(DisplayName = "可回收：静态引用")]
        public void Test2()
        {

            Assert.Equal("11",GetResult2());
            for (int i = 0; i < 6; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.True(DomainManagment.IsDeleted("TempDomain12"));

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetResult2()
        {
            string result;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "Static", "ClassLibrary5.dll");
            using (DomainManagment.CreateAndLock("TempDomain12"))
            {

                var domain = DomainManagment.CurrentDomain;
                var assemebly = domain.LoadFile(path);
                var action = FastMethodOperator.New
                   .Using(assemebly)
                   .MethodBody(@"Test.Instance.Name=""11""; return Test.Instance.Name;")
                   .Complie<Func<string>>();
                result = action();
                domain.Dispose();
                domain.Unload();
            }
            return result;
        }



        [Fact(DisplayName = "不可回收：Json.net")]
        public void Test3()
        {

            Assert.Equal("{\"Name\":\"11\"}", GetResult3());
            for (int i = 0; i < 6; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.False(DomainManagment.IsDeleted("TempDomain13"));

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public string GetResult3()
        {
            string result;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "Json", "ClassLibrary6.dll");
            using (DomainManagment.CreateAndLock("TempDomain13"))
            {

                var domain = DomainManagment.CurrentDomain;
                var assemebly = domain.LoadFile(path);
                var action = FastMethodOperator.New
                   .Using(assemebly)
                   .MethodBody(@"Class1 obj = new Class1(); return obj.Get();")
                   .Complie<Func<string>>();
                result = action();
                domain.Dispose();
                domain.Unload();
            }
            return result;
        }

#endif

    }
}
