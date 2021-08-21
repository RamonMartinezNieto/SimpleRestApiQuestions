﻿using Microsoft.AspNetCore.Mvc;
using System;
using WebApplication2.Dto;
using WebApplication2.Service;

namespace SimpleRestApiQuestions.Controllers
{
    public class CategoryController : Controller
    {
        IQuestionService _questionService;

        public CategoryController(IQuestionService service) => _questionService = service;

        [HttpGet]
        [Route("GetCategories")]
        public ActionResult<CategoryDto> GetCategories() => Ok(_questionService.GetCategories());

        [HttpPost]
        [Route("CreateCategory")]
        public ActionResult CreateCategory(string category)
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

        [HttpDelete]
        [Route("RemoveCategory")]
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
