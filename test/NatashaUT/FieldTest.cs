﻿using Natasha;
using Natasha.Builder;
using System;
using Xunit;

namespace NatashaUT
{

    [Trait("字段构建", "")]
    public class FieldTest
    {

        [Fact(DisplayName = "静态字段1")]
        public void Test1()
        {
            FieldBuilder template = new FieldBuilder();
            var result = template
                .MemberAttribute("[Test]")
                .MemberAccess("public")
                .MemberModifier(Modifiers.Static)
                .FieldName("Name")
                .FieldType<string>()
                .Script;

                Assert.Equal($"[Test]{Environment.NewLine}public static String Name;", result);

        }



        [Fact(DisplayName = "静态字段2")]
        public void Test2()
        {
            FieldBuilder template = new FieldBuilder();
            var result = template
                .MemberAttribute("[Test][Test1]")
                .MemberAccess(AccessTypes.Public)
                .MemberModifier(Modifiers.Static)
                .FieldName("Age")
                .FieldType(typeof(int))
                .Script;

                Assert.Equal($"[Test][Test1]{Environment.NewLine}public static Int32 Age;", result);

            
        }




        [Fact(DisplayName = "普通字段1")]
        public void Test3()
        {
            FieldBuilder template = new FieldBuilder();
            var result = template
                .MemberAttribute("[Test]")
                .MemberAccess("public")
                .FieldName("Name")
                .FieldType<string>()
                .Script;

                Assert.Equal($"[Test]{Environment.NewLine}public String Name;", result);

            
        }



        [Fact(DisplayName = "普通字段2")]
        public void Test4()
        {
            FieldBuilder template = new FieldBuilder();
            var result = template
                .MemberAttribute<ClassDataAttribute>()
                .MemberAccess("public")
                .FieldName("Name")
                .FieldType<string>()
                .Script;


                Assert.Equal($"[ClassDataAttribute]{Environment.NewLine}public String Name;", result);

            
        }

    }
}
