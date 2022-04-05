using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class ExerciseResult
    {
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        public string Answer { get; set; }
        public string Comment { get; set; }
        public int Grade { get; set; }
    }
}
