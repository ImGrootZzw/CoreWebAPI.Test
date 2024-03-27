using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CoreWebAPI.Test.Models.Models
{
    ///<summary>
    ///
    ///</summary>
    //[SplitTable(SplitType.Month)] //按年分表 （自带分表支持 年、季、月、周、日）
    //[SugarTable("student_mstr_{year}{month}{day}")] //3个变量必须要有，这么设计为了兼容开始按年，后面改成按月、按日
    [SugarTable("student_mstr",tableDescription: "MYDB_PGSQLNew")]
    public class StudentMstrNew
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
        [SugarColumn(ColumnName = "student_corp_id", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_student_mstr", "search_index_student_mstr"})]
        public string StudentCorpId { get; set; }

        /// <summary>
        /// 描述 : 
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_domain_id", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_student_mstr", "search_index_student_mstr"})]
        public string StudentDomainId { get; set; }

        /// <summary>
        /// 描述 : 学生号
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_id", IsOnlyIgnoreUpdate = true, IndexGroupNameList = new string[] { "uk_index_student_mstr", "search_index_student_mstr"})]
        public string StudentId { get; set; }

        /// <summary>
        /// 描述 : 姓名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_name", IsNullable = true)]
        public string StudentName { get; set; }

        /// <summary>
        /// 描述 : 年龄
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_age", ColumnDescription = "student_age", IsNullable = true)]
        public decimal? StudentAge { get; set; }

        /// <summary>
        /// 描述 : 入学时间
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        //[SplitField]
        [SugarColumn(ColumnName = "student_enrolment_date")]
        public DateTime? StudentEnrolmentDate { get; set; }

        /// <summary>
        /// 描述 : 地址
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_addr", IsNullable = true)]
        public string StudentAddr { get; set; }

        /// <summary>
        /// 描述 : 已毕业
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_graduated", IsNullable = true)]
        public bool? StudentGraduated { get; set; }

        /// <summary>
        /// 描述 : 创建日期
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_crt_datetime", IsOnlyIgnoreUpdate = true, IsNullable = true)]
        public DateTimeOffset? StudentCrtDatetime { get; set; }

        /// <summary>
        /// 描述 : 程序名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_crt_prog", IsOnlyIgnoreUpdate = true, IsNullable = true)]
        public string StudentCrtProg { get; set; }

        /// <summary>
        /// 描述 : 创建用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_crt_user", IsOnlyIgnoreUpdate = true, IsNullable = true)]
        public string StudentCrtUser { get; set; }

        /// <summary>
        /// 描述 : 修改日期
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_mod_datetime", IsOnlyIgnoreInsert = true, IsNullable = true)]
        public DateTime? StudentModDatetime { get; set; }

        /// <summary>
        /// 描述 : 修改程序名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_mod_prog", IsOnlyIgnoreInsert = true, IsNullable = true)]
        public string StudentModProg { get; set; }

        /// <summary>
        /// 描述 : 修改用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "student_mod_user", IsOnlyIgnoreInsert = true, IsNullable = true)]
        public string StudentModUser { get; set; }

        /// <summary>
        /// 标识版本字段
        /// </summary>
        [SugarColumn(ColumnName = "version", IsEnableUpdateVersionValidation = true)]
        public string Version { get; set; } = Guid.NewGuid().ToString();

    }
}