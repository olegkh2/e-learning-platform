using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class Course_Student
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
