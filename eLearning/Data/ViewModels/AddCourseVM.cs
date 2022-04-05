using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data.ViewModels
{
    public class AddCourseVM
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
              
        [Display(Name = "Students")]
        public List<string> UserIds { get; set; }
    }
}
