using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.Domain.DTO
{
    public class ArticleDTO
    {
        public string Title { get; set; }
        public string Theme { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
    }
}
