﻿using System;
using System.Collections.Concurrent;

namespace Natasha
{
    /// <summary>
    /// 文件可用名反解
    /// </summary>
    public class AvailableNameReverser
    {



        /// <summary>
        /// 根据类型获取可用名，检查缓存
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetName(Type type)
        {

            if (type == null)
            {

                return "";

            }

            return GetAvailableName(type);

        }




        /// <summary>
        /// 根据类型获取可用名
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static string GetAvailableName(Type type)
        {

            return type.GetDevelopName().Replace('<', '_').Replace('>', '_').Replace(',', '_').Replace("[", "@").Replace("]", "@");

        }

    }

}
