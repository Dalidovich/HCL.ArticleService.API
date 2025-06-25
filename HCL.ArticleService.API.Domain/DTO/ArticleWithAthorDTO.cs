using HCL.ArticleService.API.Domain.Entities;

namespace HCL.ArticleService.API.Domain.DTO
{
    public class ArticleWithAthorDTO
    {
        public string Title { get; set; }
        public string Theme { get; set; }
        public string Content { get; set; }
        public string AuthorLogin { get; set; }
        public DateTime AuthorCreateDate { get; set; }
        public string AuthorStatus { get; set; }

        public ArticleWithAthorDTO(string login, string status, DateTime createDate, Article rawArticel)
        {
            Title = rawArticel.Title;
            Theme = rawArticel.Theme;
            Content = rawArticel.Content;
            AuthorLogin = login;
            AuthorCreateDate = createDate;
            AuthorStatus = status;
        }
    }
}