using System;

namespace CoreWebAPI.Test.Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class PiresultDetSearchModel : BasicSearchModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string PiresultCorpId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PiresultDomainId { get; set; }

        /// <summary>
        /// 产线
        /// </summary>
        public string PiresultProdline { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string PiresultDept { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public string PiresultMachine { get; set; }

        /// <summary>
        /// 工序
        /// </summary>
        public string PiresultOp { get; set; }

        /// <summary>
        /// 零件
        /// </summary>
        public string PiresultPart { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        public string PiresultQrcode { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string PiresultLot { get; set; }

        /// <summary>
        /// 检测项目
        /// </summary>
        public string PiresultDatatype { get; set; }

        /// <summary>
        /// 描述 : 检测开始时间
        /// </summary>        
        public DateTime? PiresultDatetimeFrom { get; set; }

        /// <summary>
        /// 描述 : 检测结束时间
        /// </summary>        
        public DateTime? PiresultDatetimeTo { get; set; }

        /// <summary>
        /// 判定结果
        /// </summary>
        public string PiresultResult { get; set; }

        /// <summary>
        /// 检测类型（inprocess/incoming/periodic）
        /// </summary>
        public string PiresultType { get; set; }

        /// <summary>
        /// NG原因类型
        /// </summary>
        public string PiresultIssue { get; set; }

        /// <summary>
        /// 已发布
        /// </summary>
        public bool? PiresultReleased { get; set; }

    }
}