using HCL.ArticleService.API.Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.Domain.Entities
{
    public class Article
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Theme{ get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime CreateDate { get; set; }

        public Article(ArticleDTO articleDTO)
        {
            Id = "";
            Title = articleDTO.Title;
            Theme = articleDTO.Theme;
            Content = articleDTO.Content;
            Author = articleDTO.Author;
            CreateDate= DateTime.Now;
        }
    }
}
