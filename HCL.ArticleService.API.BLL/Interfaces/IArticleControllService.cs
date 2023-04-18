using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.InnerResponse;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IArticleControllService
    {
        public Task<BaseResponse<Article>> CreateArticle(Article account);
        public Task<BaseResponse<bool>> DeleteArticle(Expression<Func<Article, bool>> expression);
        public BaseResponse<IQueryable<Article>> GetArticleOData();
    }
}
