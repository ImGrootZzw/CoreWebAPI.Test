using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CoreWebAPI.Test.Models.Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("test",TableDescription = "111")]
    public class PiresultDet
    {
        /// <summary>
        /// 描述 : 
        /// 允许空值 : False
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "id", IsNullable = false, IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 描述 : 
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_corp_id", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultCorpId { get; set; }

        /// <summary>
        /// 描述 : 
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_domain_id", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultDomainId { get; set; }

        /// <summary>
        /// 描述 : 产线
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_prodline", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultProdline { get; set; }

        /// <summary>
        /// 描述 : 部门
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_dept", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultDept { get; set; }

        /// <summary>
        /// 描述 : 设备
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_machine", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultMachine { get; set; }

        /// <summary>
        /// 描述 : 工序
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_op", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultOp { get; set; }

        /// <summary>
        /// 描述 : 零件
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_part", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultPart { get; set; }

        /// <summary>
        /// 描述 : 二维码
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_qrcode", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultQrcode { get; set; }

        /// <summary>
        /// 描述 : 批次号
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_lot", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultLot { get; set; }

        /// <summary>
        /// 描述 : 检测项目
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_datatype", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultDatatype { get; set; }

        /// <summary>
        /// 描述 : 检测值
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_value")]
        public decimal? PiresultValue { get; set; }

        /// <summary>
        /// 描述 : 单位
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_um")]
        public string PiresultUm { get; set; }

        /// <summary>
        /// 描述 : 检测时间
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_datetime", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public DateTime? PiresultDatetime { get; set; }

        /// <summary>
        /// 描述 : 采集方式
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_mthd")]
        public string PiresultMthd { get; set; }

        /// <summary>
        /// 描述 : 判定结果
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_result", IndexGroupNameList = new string[] { "search_index_piresult_det"})]
        public string PiresultResult { get; set; }

        /// <summary>
        /// 描述 : 检测类型（inprocess/incoming/periodic）
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_type", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_piresult_det", "search_index_piresult_det"})]
        public string PiresultType { get; set; }

        /// <summary>
        /// 描述 : 已发布
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_released", IndexGroupNameList = new string[] { "search_index_piresult_det"})]
        public bool? PiresultReleased { get; set; }

        /// <summary>
        /// 描述 : NG原因类型
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_issue")]
        public string PiresultIssue { get; set; }

        /// <summary>
        /// 描述 : 检查频率
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_frequency")]
        public string PiresultFrequency { get; set; }

        /// <summary>
        /// 描述 : 创建日期
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_crt_datetime", IsOnlyIgnoreUpdate = true)]
        public DateTime? PiresultCrtDatetime { get; set; }

        /// <summary>
        /// 描述 : 程序名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_crt_prog", IsOnlyIgnoreUpdate = true)]
        public string PiresultCrtProg { get; set; }

        /// <summary>
        /// 描述 : 创建用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_crt_user", IsOnlyIgnoreUpdate = true)]
        public string PiresultCrtUser { get; set; }

        /// <summary>
        /// 描述 : 修改日期
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_mod_datetime", IsOnlyIgnoreInsert = true)]
        public DateTime? PiresultModDatetime { get; set; }

        /// <summary>
        /// 描述 : 修改程序名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_mod_prog", IsOnlyIgnoreInsert = true)]
        public string PiresultModProg { get; set; }

        /// <summary>
        /// 描述 : 修改用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_mod_user", IsOnlyIgnoreInsert = true)]
        public string PiresultModUser { get; set; }

        /// <summary>
        /// 描述 : 修改用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "piresult_mod_user", IsOnlyIgnoreInsert = true)]
        public string PiresultStudentId { get; set; }

        /// <summary>
        /// 描述 : 修改用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [Navigate(NavigateType.OneToOne, nameof(PiresultStudentId))]
        public StudentMstr PiresultStudent { get; set; }



    }
}