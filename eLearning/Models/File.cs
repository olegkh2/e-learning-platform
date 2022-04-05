using eLearning.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class File : CourseEntity
    {
        [StringLength(70)]
        public string FileName { get; set; }
    }
}
