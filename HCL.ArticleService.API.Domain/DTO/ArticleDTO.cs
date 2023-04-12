using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.Domain.DTO
{
    public class ArticleDTO
    {
        [Required(AllowEmptyStrings = true, ErrorMessage = "Need Title")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Need Theme")]
        public string Theme { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Need Content")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Need Author")]
        public string Author { get; set; }
    }
}
