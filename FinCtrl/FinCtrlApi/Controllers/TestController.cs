using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinCtrlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet, Route("GetList")]
        public ActionResult<Category> GetList([FromServices] ICategory categoryRepo)
        {
            return Ok(categoryRepo.GetList());
        }

        [HttpPost, Route("InsertNewMock")]
        public ActionResult InsertNewMock([FromServices] ICategory categoryRepo)
        {
            categoryRepo.InsertNew(new Category(1, "New Fiesta"));
            return Ok();
        }

        [HttpDelete, Route("DeleteById/{id}")]
        public ActionResult InsertNewMock(int id, [FromServices] ICategory categoryRepo)
        {
            categoryRepo.DeleteById(id);
            return Ok();
        }
    }
}
