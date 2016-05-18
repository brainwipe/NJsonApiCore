using System;
using System.Collections.Generic;
using System.Linq;

namespace NJsonApi.Web.MVC5.HelloWorld.Models
{
    public class Article
    {
        public Article()
        {
        }

        public Article(string title)
        {
            Comments = new List<Comment>();
            Id = StaticPersistentStore.GetNextId();
            Title = title;
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public Person Author { get; set; }

        public List<Comment> Comments { get; set; }
    }
}