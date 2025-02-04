﻿using Natasha.Operator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Natasha
{
    public class NAssembly
    {

        public Assembly Assembly;
        private readonly HashSet<IScript> _builderCache;
        public readonly AssemblyComplier Options;
        public ConcurrentDictionary<string, Type> TypeCache;


        public NAssembly(string name) : this()
        {

            Options.Name = name;

        }


        public NAssembly()
        {

            _builderCache = new HashSet<IScript>();
            TypeCache = new ConcurrentDictionary<string, Type>();
            Options = new AssemblyComplier();

        }




        public bool Remove(IScript builder)
        {
            return _builderCache.Remove(builder);
        }




        /// <summary>
        /// 直接添加一个合法的类/接口/结构体/枚举
        /// </summary>
        /// <param name="script">脚本代码</param>
        /// <returns></returns>
        public CompilationException AddScript(string script)
        {
            return Options.Add(script);
        }




        /// <summary>
        /// 添加一个带有代码的文件
        /// </summary>
        /// <param name="path">代码文件路径</param>
        /// <returns></returns>
        public CompilationException AddFile(string path)
        {
            return Options.AddFile(path);
        }




        /// <summary>
        /// 创建一个类Operator，命名空间默认是程序集命
        /// </summary>
        /// <param name="name">类名</param>
        /// <returns></returns>
        public OopOperator CreateClass(string name = default)
        {
            var @operator = new OopOperator().OopName(name).Namespace(Options.Name).ChangeToClass();
            _builderCache.Add(@operator);
            return @operator;

        }




        /// <summary>
        /// 创建一个枚举Operator，命名空间默认是程序集命
        /// </summary>
        /// <param name="name">枚举名</param>
        /// <returns></returns>
        public OopOperator CreateEnum(string name = default)
        {

            var @operator = new OopOperator().OopName(name).Namespace(Options.Name).ChangeToEnum();
            _builderCache.Add(@operator);
            return @operator;

        }




        /// <summary>
        /// 创建一个接口Operator，命名空间默认是程序集命
        /// </summary>
        /// <param name="name">接口名</param>
        /// <returns></returns>
        public OopOperator CreateInterface(string name = default)
        {

            var @operator = new OopOperator().OopName(name).Namespace(Options.Name).ChangeToInterface();
            _builderCache.Add(@operator);
            return @operator;

        }




        /// <summary>
        /// 创建一个结构体Operator，命名空间默认是程序集命
        /// </summary>
        /// <param name="name">结构体名</param>
        /// <returns></returns>
        public OopOperator CreateStruct(string name = default)
        {

            var @operator = new OopOperator().OopName(name).Namespace(Options.Name).ChangeToStruct();
            _builderCache.Add(@operator);
            return @operator;
        }




        /// <summary>
        /// 创建一个FastMethodOperator
        /// </summary>
        /// <param name="name">类名</param>
        /// <returns></returns>
        public FastMethodOperator CreateFastMethod(string name = default)
        {

            var @operator = new FastMethodOperator().OopName(name);
            _builderCache.Add(@operator);
            return @operator;

        }




        /// <summary>
        /// 创建一个FakeMethodOperator
        /// </summary>
        /// <param name="name">类名</param>
        /// <returns></returns>
        public FakeMethodOperator CreateFakeMethod(string name = default)
        {

            var @operator = new FakeMethodOperator().OopName(name);
            _builderCache.Add(@operator);
            return @operator;

        }




        /// <summary>
        /// 进行语法检查
        /// </summary>
        /// <returns></returns>
        public List<CompilationException> Check()
        {

            foreach (var item in _builderCache)
            {
                Options.Add(item);
            }
            return Options.ComplierInfos.Exceptions;

        }




        /// <summary>
        /// 对整个程序集进行编译
        /// </summary>
        /// <returns></returns>
        public Assembly Complier()
        {

            Check();
            Assembly = Options.GetAssembly();
            var types = Assembly.GetTypes();
            foreach (var item in types)
            {
                TypeCache[item.GetDevelopName()] = item;
            }
            return Assembly;

        }




        /// <summary>
        /// 从编译后的缓存中获取类型
        /// </summary>
        /// <param name="name">类名</param>
        /// <returns></returns>
        public Type GetType(string name)
        {

            if (TypeCache.ContainsKey(name))
            {
                return TypeCache[name];
            }
            return default;

        }

    }

}
