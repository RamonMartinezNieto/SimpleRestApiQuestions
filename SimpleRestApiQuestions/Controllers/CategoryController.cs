using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using SimpleRestApiQuestions.Dto;
using SimpleRestApiQuestions.Service;

namespace SimpleRestApiQuestions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CategoryController : Controller
    {
        IQuestionService _questionService;

        public CategoryController(IQuestionService service) => _questionService = service;

        /// <summary>
        /// Get all categories that can be used to create a new questions. 
        /// </summary>
        /// <response code="200">Return response with the categories</response>
        [HttpGet]
        [Route("Categories")]
        [AllowAnonymous]
        public ActionResult<CategoryDto> Categories() => Ok(_questionService.GetCategories());

        /// <summary>
        /// Get the current version of the category version passed. The is used to know if the questions were 
        /// update to know if you need to request the newest version. 
        /// </summary>
        /// <response code="200">Return response with the categories</response>
        [HttpGet]
        [Route("Version/{idCategory:int}")]
        [AllowAnonymous]
        public ActionResult<int> VersionCategory([Required] int idCategory) {

            int versionCategory = _questionService.GetCategoryVersion(idCategory); 

            if(versionCategory < 1) {
                return NoContent();
            } 
            return Ok(versionCategory);
        }

        /// <summary>
        /// Create a new category into the repository.
        /// </summary>
        /// <response code="200">If the category was added</response>
        /// <response code="401">Unauthorized Jason Web Token</response>
        /// <response code="400">If the category was not added</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Category([Required] string category)
        {
            try
            {
                int idCategory = _questionService.CreateCategory(category);
                //TODO usar create at  or create
                return Ok($"Category added with id {idCategory}");
            }
            catch (Exception)
            {
                return BadRequest("Category not added");
            }
        }

        /// <summary>
        /// Delete specific Category.
        /// </summary>
        /// <returns>BadRequest if the category can't be removed because is in use or not exist.</returns>
        /// <response code="200">If the category was removed</response>
        /// <response code="401">Unauthorized Jason Web Token</response>
        /// <response code="400">If the category was not deleted</response>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public ActionResult Category(int id)
        {
            try
            {
                if (_questionService.DeleteCategory(id))
                    return Ok("Category removed");
                else return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("Any exception occrus calling the data base.");
            }
        }
    }
}
