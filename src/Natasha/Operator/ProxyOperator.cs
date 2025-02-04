﻿using Natasha.Builder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Natasha.Operator
{
    /// <summary>
    /// 运行时类型动态构建器
    /// </summary>
    /// <typeparam name="T">运行时类型</typeparam>
    public class ProxyOperator<T> : ProxyOperator
    {


        private readonly static ConcurrentDictionary<string, Func<T>> _ctor_mapping;
        static ProxyOperator() => _ctor_mapping = new ConcurrentDictionary<string, Func<T>>();




        public ProxyOperator() : base(typeof(T)) { }




        /// <summary>
        /// 编译生成委托
        /// </summary>
        /// <returns></returns>
        public override Delegate Compile()
        {

            return _ctor_mapping[OopNameScript] = (Func<T>)(base.Compile());

        }




        /// <summary>
        /// 生成实例
        /// </summary>
        /// <param name="class">类名</param>
        /// <returns></returns>
        public T Create(string @class)
        {

            if (!_ctor_mapping.ContainsKey(@class)) { Compile(); }


            return _ctor_mapping[@class]();

        }

    }




    /// <summary>
    /// 类构建器
    /// </summary>
    public class ProxyOperator : OopBuilder<ProxyOperator>
    {


        private readonly static ConcurrentDictionary<string, Delegate> _delegate_mapping;
        static ProxyOperator() => _delegate_mapping = new ConcurrentDictionary<string, Delegate>();


        public string Result;
        public Type TargetType;
        private readonly Type _oop_type;
        private readonly Dictionary<string, string> _oop_methods_mapping;

        public ProxyOperator(Type oopType) : base()
        {

            Link = this;
            _oop_type = oopType;
            _oop_methods_mapping = new Dictionary<string, string>();

            Using(_oop_type)
               .Namespace("NatashaProxy")
               .OopAccess(AccessTypes.Public)
               .Inheritance(_oop_type);

        }





        /// <summary>
        /// 操作当前函数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {

                return _oop_methods_mapping[key];

            }
            set
            {

                //获取反射信息
                var reflectMethodInfo = _oop_type.GetMethod(key);
                if (reflectMethodInfo == null)
                {

                    throw new Exception($"无法在{_oop_type.Name}中找到{key}函数！");

                }


                //填装引用
                Using(reflectMethodInfo);


                //使用伪造函数模板
                var template = FakeMethodOperator.New;

                if (!_oop_type.IsInterface)
                {

                    if (reflectMethodInfo.IsAbstract || reflectMethodInfo.IsVirtual)
                    {

                        template.MethodModifier(Modifiers.Override);

                    }
                    else
                    {

                        template.MethodModifier(Modifiers.New);

                    }

                }


                template.UseMethod(reflectMethodInfo).MethodContent(value).Builder();
                _oop_methods_mapping[key] = template.MethodScript;

            }

        }




        /// <summary>
        /// 组装编译
        /// </summary>
        /// <returns></returns>
        public virtual Delegate Compile()
        {

            StringBuilder sb = new StringBuilder();
            foreach (var item in _oop_methods_mapping)
            {

                sb.Append(item.Value);

            }


            //生成整类脚本
            OopBody(sb.ToString());


            //获取类型
            TargetType = GetType();


            //返回委托
            return CtorOperator.NewDelegate(TargetType);
        }




        /// <summary>
        /// 根据类名生成委托
        /// </summary>
        /// <typeparam name="T">强委托类型</typeparam>
        /// <param name="class">类名</param>
        /// <returns></returns>
        public T Create<T>(string @class)
        {

            if (!_delegate_mapping.ContainsKey(@class)) { _delegate_mapping[OopNameScript] = Compile(); }
            return ((Func<T>)_delegate_mapping[@class])();

        }

    }

}
