using HCL.ArticleService.API.Domain.Enums;

namespace HCL.ArticleService.API.Domain.DTO
{
    public class ErrorDTO
    {
        public string Message { get; set; } = null!;
        public int StatusCode { get; set; }
    }
}
