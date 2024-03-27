using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CoreWebAPI.Test.Models.ViewModels
{
    ///<summary>
    ///
    ///</summary>
    public class BasicSearchModel
    {
        /// <summary>
        /// 描述 : 页码
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 描述 : 页大小
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public int PageSize { get; set; } = 100;

        /// <summary>
        /// 描述 : 排序
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public List<SortModel> SortList { get; set; }

        /// <summary>
        /// 描述 : 排序
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public List<TableHeaderModel> TableHead { get; set; }

        /// <summary>
        /// 描述 : 查询字段列表 当字段列表为空或不填写时，默认查询全部字段
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public List<string> FieldList { get; set; }

        /// <summary>
        /// 描述 : 查询条件列表 当条件列表为空或不填时，则不设查询条件
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public List<ConditionFieldModel> ConditionList { get; set; }

    }

    /// <summary>
    /// 排序
    /// </summary>
    public class SortModel
    {
        /// <summary>
        /// 描述 : 排序字段
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string SortParam { get; set; }

        /// <summary>
        /// 描述 : 正序
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public bool Ascending { get; set; }
    }

    /// <summary>
    /// 字段条件
    /// </summary>
    public class ConditionFieldModel
    {
        /// <summary>
        /// 描述 : 字段
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 描述 : 条件
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public List<ConditionModel> Conditions { get; set; }
    }

    /// <summary>
    /// 条件
    /// </summary>
    public class ConditionModel
    {
        /// <summary>
        /// 描述 : 条件类型 eq：等于，ne：不等于，lt：小于，lte：小于等于，gt：大于，gte：大于等于，like：模糊匹配，in：包含,between：在...之间
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 描述 : 该参数可为任意值，当类型为boolean时，只可以使用eq类型。当类型为in时，需传输Array对象
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public object FirstValue { get; set; }

        /// <summary>
        /// 描述 : 该参数只在类型为between时使用，为between的第二个值
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public object SecondValue { get; set; }

        /// <summary>
        /// 描述 : 该参数设置与上一个条件的连接关系，只要and或or，默认使用and
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string Operation { get; set; } = "and";
    }
}