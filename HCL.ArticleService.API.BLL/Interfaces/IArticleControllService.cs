using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.InnerResponse;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IArticleControllService
    {
        public Task<BaseResponse<Article>> CreateArticle(Article account);
        public Task<BaseResponse<bool>> DeleteArticle(Expression<Func<Article, bool>> expression);
        public Task<BaseResponse<bool>> UpdateArticlesActualState();
        public BaseResponse<IQueryable<Article>> GetArticleOData();
    }
}