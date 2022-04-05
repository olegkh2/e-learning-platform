using eLearning.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class Exercise : CourseEntity
    {

        public string Text { get; set; }

        public List<ExerciseResult> ExerciseResults { get; set; }

    }
}
