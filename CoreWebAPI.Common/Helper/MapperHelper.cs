using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.Helper
{
    public static class MapperHelper
    {
        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// 适用于初始化新实体
        /// </summary>
        /// <typeparam name="R">返回的实体</typeparam>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <param name="s">数据源实体</param>
        /// <returns>返回的新实体</returns>
        public static R Mapper<R, S>(S s)
        {
            R r = Activator.CreateInstance<R>(); //构造新实例
            try
            {
                var Types = s.GetType();//获得类型  
                var Typed = typeof(R);
                foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                        {
                            dp.SetValue(r, sp.GetValue(s, null), null);//获得s对象属性的值复制给r对象的属性  
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return r;
        }

        /// <summary>
        /// 反射实现历史表赋值
        /// </summary>
        /// <typeparam name="R">返回的实体</typeparam>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <param name="sOld">修改前数据实体</param>
        /// <param name="sNew">修改后数据实体</param>
        /// <returns>返回的新实体</returns>
        public static R MapperHist<R, S>(S sOld, S sNew)
        {
            R r = Activator.CreateInstance<R>(); //构造新实例
            try
            {
                var TypeSOld = sOld.GetType();//获得类型  
                var TypeSNew = sNew.GetType();//获得类型  
                var TypeR = typeof(R);

                foreach (PropertyInfo rp in TypeR.GetProperties())
                {
                    if (rp.Name != "Error" && rp.Name != "Item")
                        continue;

                    PropertyInfo piOld = TypeSOld.GetProperties().FirstOrDefault(p => rp.Name == p.Name && rp.PropertyType == p.PropertyType);
                    if (piOld != null)
                        rp.SetValue(r, piOld.GetValue(sOld, null), null);

                    PropertyInfo piNew = TypeSNew.GetProperties().FirstOrDefault(p => rp.Name == p.Name + "New" && rp.PropertyType == p.PropertyType);
                    if (piNew != null)
                        rp.SetValue(r, piNew.GetValue(sOld, null), null);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return r;
        }

        /// <summary>
        /// 反射实现两个类的对象之间相同属性的值的复制
        /// 适用于没有新建实体之间
        /// </summary>
        /// <typeparam name="R">返回的实体</typeparam>
        /// <typeparam name="S">数据源实体</typeparam>
        /// <param name="r">返回的实体</param>
        /// <param name="s">数据源实体</param>
        /// <param name="isContainsNull">字段为null是否的跳过</param>
        /// <param name="isContainsEmpty">字段为空字符串的是否的跳过</param>
        /// <returns></returns>
        public static R MapperToModel<R, S>(R r, S s, bool isContainsNull = true, bool isContainsEmpty = true)
        {
            try
            {
                var Types = s.GetType();//获得类型  
                var Typed = typeof(R);
                foreach (PropertyInfo sp in Types.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")//判断属性名是否相同  
                        {
                            object val = sp.GetValue(s, null);
                            if (!isContainsNull && val == null)
                            {
                                continue;
                            }
                            if (!isContainsEmpty && val != null &&
                                (val is string) && string.IsNullOrWhiteSpace(val.ToString()))
                            {
                                continue;
                            }
                            dp.SetValue(r, val, null);//获得s对象属性的值复制给r对象的属性  
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return r;
        }
    }
}
