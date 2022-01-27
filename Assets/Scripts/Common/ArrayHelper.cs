using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /* C#的扩展方法：在不修改代码的情况下，为其添加新功能
     * 但不会改变微软的数组类， 为其添加新方法
     *
     * 三要素：
     * 1.扩展方法所在的类必须是静态类
     * 2.在第一个参数上，使用this关键字修饰被扩展的类型
     * 3.在另一个命名空间下
     *
     * 作用：让调用者方便调用该方法（就好像在调用自身类型的方法一样）
     * 被扩展类型.方法名
     */

    ///<summary>
    ///数组助手类：提供数组常用功能
    ///</summary>
    public static class ArrayHelper
    {
        //7个方法 查找 查找满足条件的所有对象
        //排序【升序、降序】最大值 最小值 筛选

        /// <summary>
        /// 查找满足条件的单个元素
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="condition">查找条件</param>
        /// <returns></returns>
        public static T Find<T>(this T[] array, Func<T, bool> condition)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (condition(array[i]))
                {
                    return array[i];
                }
            }

            return default;
        }

        /// <summary>
        /// 查找满足条件的所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Func<T, bool> condition)
        {
            //集合存储满足条件的元素
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                //查找条件
                if (condition(array[i]))
                {
                    list.Add(array[i]);
                }
            }
            //ToArray将集合转换成数组
            return list.ToArray();
        }

        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="Q">返回值类型</typeparam>
        /// <param name="array">比较数组</param>
        /// <param name="condition">比较条件</param>
        /// <returns></returns>
        public static T GetMax<T, Q>(this T[] array, Func<T, Q> condition) where Q : IComparable
        {
            T max = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (condition(max).CompareTo(condition(array[i])) < 0)
                    max = array[i];
            }
            return max;
        }

        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <typeparam name="Q">返回值类型</typeparam>
        /// <param name="array">比较数组</param>
        /// <param name="condition">比较条件</param>
        /// <returns></returns>
        public static T GetMin<T, Q>(this T[] array, Func<T, Q> condition) where Q : IComparable
        {
            T min = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (condition(min).CompareTo(condition(array[i])) > 0)
                    min = array[i];
            }
            return min;
        }


        //升序方法
        public static void SortByIncreasingOrder<T, Q>(this T[] array, Func<T, Q> condition) where Q : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (condition(array[i]).CompareTo(condition(array[j])) > 0)
                    {
                        T temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }

        //降序方法
        public static void SortByDesendingOrder<T, Q>(this T[] array, Func<T, Q> condition) where Q : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length - 1 - i; j++)
                {
                    if (condition(array[j]).CompareTo(condition(array[j + 1])) > 0)
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        //筛选方法
        public static Q[] Select<T, Q>(this T[] array, Func<T, Q> condition)
        {
            Q[] result = new Q[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                //满足筛选条件，就将该元素存到result里
                result[i] = condition(array[i]);
            }
            return result;
        }

    }       
    
    
}
