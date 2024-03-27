using System;

namespace CoreWebAPI.Test.Models.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class StudentMstrSearchModel : BasicSearchModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string StudentCorpId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StudentDomainId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StudentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal? StudentAge { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? StudentEnrolmentDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StudentAddr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? StudentGraduated { get; set; }

    }
}