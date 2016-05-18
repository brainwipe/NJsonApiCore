using NJsonApi.Infrastructure;
using NJsonApi.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC5.HelloWorld.Controllers
{
    [Route("articles")]
    public class ArticlesController : ApiController
    {
        [HttpGet]
        public IEnumerable<Article> Get()
        {
            return StaticPersistentStore.Articles;
        }

        [Route("{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            return Ok(StaticPersistentStore.Articles.Single(w => w.Id == id));
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]Delta<Article> article)
        {
            var newArticle = article.ToObject();
            newArticle.Id = StaticPersistentStore.GetNextId();
            StaticPersistentStore.Articles.Add(newArticle);
            return RedirectToRoute("Get", new { id = newArticle.Id });
        }

        [Route("{id}")]
        [HttpPatch]
        public IHttpActionResult Patch([FromBody]Delta<Article> update, int id)
        {
            var article = StaticPersistentStore.Articles.Single(w => w.Id == id);
            update.ApplySimpleProperties(article);
            return RedirectToRoute("Get", new { id = id });
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            return Ok(StaticPersistentStore.Articles.RemoveAll(x => x.Id == id));
        }
    }
}