using NJsonApi.Infrastructure;
using NJsonApi.Web.MVC5.HelloWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC5.HelloWorld.Controllers
{
    [RoutePrefix("articles")]
    public class ArticlesController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IEnumerable<Article> Get()
        {
            return StaticPersistentStore.Articles;
        }

        [Route("{id}", Name = "GetArticle")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var a = StaticPersistentStore.Articles.Single(w => w.Id == id);
            var md = new MetaDataWrapper<Article>(a);
            md.MetaData.Add("response created", DateTime.Now);
            md.MetaData.Add("response created by", this.GetType());
            return Ok(md);
        }

        [Route]
        [HttpPost]
        public IHttpActionResult Post([FromBody]Delta<Article> article)
        {
            var newArticle = article.ToObject();
            newArticle.Id = StaticPersistentStore.GetNextId();
            StaticPersistentStore.Articles.Add(newArticle);

            return Created(LinkForGet(newArticle.Id), newArticle);
        }

        [Route("{id}")]
        [HttpPatch]
        public IHttpActionResult Patch([FromBody]Delta<Article> update, int id)
        {
            var article = StaticPersistentStore.Articles.SingleOrDefault(w => w.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            update.ApplySimpleProperties(article);

            return Ok(article);
        }

        [Route("{id}")]
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var article = StaticPersistentStore.Articles.SingleOrDefault(w => w.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(StaticPersistentStore.Articles.RemoveAll(x => x.Id == id));
        }

        private Uri LinkForGet(int id)
        {
            return new Uri(Url.Link("GetArticle", new { id = id }));
        }
    }
}