using NJsonApi.Infrastructure;
using NJsonApi.Web.MVC5.HelloWorld.Models;
using NJsonApiCore.Web.MVC5.HelloWorld.Models.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJsonApi.Web.MVC5.HelloWorld.Controllers
{
    [RoutePrefix("testexamples")]
    public class TestExamplesController : ApiController
    {
        [Route("throwexception")]
        [HttpGet]
        public void ThrowException()
        {
            throw new NotImplementedException("An example exception thrown");
        }

        [Route("getemptylist")]
        [HttpGet]
        public IEnumerable<Article> GetEmptyList()
        {
            return new List<Article>();
        }

        [Route("models")]
        [HttpGet]
        public IEnumerable<ModelWithPascalCase> ModelsWithPascalCase()
        {
            return new List<ModelWithPascalCase>() { new ModelWithPascalCase() { Id = 1, LongPascalCaseProperty = "Original Value" } };
        }

        [Route("models/{id}", Name = "ModelGet")]
        [HttpGet]
        public IHttpActionResult ModelWithPascalCase(int id)
        {
            return Ok(new ModelWithPascalCase() { Id = id, LongPascalCaseProperty = "Original Value" });
        }

        [Route("models")]
        [HttpPost]
        public IHttpActionResult PostModelWithPascalCase([FromBody]Delta<ModelWithPascalCase> modelDelta)
        {
            var model = modelDelta.ToObject();
            model.Id = 1;
            return Created(LinkForModelGet(1), model);
        }

        private Uri LinkForModelGet(int id)
        {
            return new Uri(Url.Link("ModelGet", new { id = id }));
        }
    }
}