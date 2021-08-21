using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Dto;
using WebApplication2.Service;

namespace SimpleRestApiQuestions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CategoriesController : Controller
    {
        IQuestionService _questionService;

        public CategoriesController(IQuestionService service) => _questionService = service;

        /// <summary>
        /// Get all categories that can be used to create a new questions. 
        /// </summary>
        [HttpGet]
        [Route("GetCategories")]
        [AllowAnonymous]
        public ActionResult<CategoryDto> GetCategories() => Ok(_questionService.GetCategories());

        /// <summary>
        /// Create a new category into the repository.
        /// </summary>
        [HttpPost]
        [Route("CreateCategory")]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateCategory([Required] string category)
        {
            try
            {
                int idCategory = _questionService.CreateCategory(category);
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
        [HttpDelete]
        [Route("RemoveCategory")]
        [Authorize(Roles = "Admin")]
        public ActionResult RemoveCategory(int id)
        {
            try
            {
                if (_questionService.DeleteCategory(id))
                    return Ok("Category removed");
                else return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("Question not removed, there are questions' foreign key with this category");
            }
        }
    }
}
