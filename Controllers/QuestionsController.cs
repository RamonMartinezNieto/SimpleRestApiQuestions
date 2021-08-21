using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using WebApplication2.Dto;
using WebApplication2.Service;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        IQuestionService _questionService;

        public QuestionsController(IQuestionService service) => _questionService = service;

        /// <summary>
        /// Get all questions in the repository of specific category.
        /// </summary>
        [HttpGet]
        [Route("GetAllQuestions")]
        public ActionResult<QuestionDto> GetAllQuestions(int categoryId) => Ok(_questionService.GetAllQuestions(categoryId));

        /// <summary>
        /// Get max number of questions for a specific category. 
        /// </summary>
        [HttpGet]
        [Route("GetMaxQuestionsToRequest")]
        public ActionResult<int> GetMaxQuestionsToRequest([Required] int categoryId) => Ok(_questionService.MaxQuestionsToRequest(categoryId));

        /// <summary>
        /// Get random questions from specified category. 
        /// </summary>
        [HttpGet]
        [Route("GetRandomQuestions")]
        public ActionResult<QuestionDto> GetRandomQuestions([Required] int quantity, [Required] int categoryId)
        {
            if (quantity == 0 || quantity > _questionService.MaxQuestionsToRequest(categoryId)) 
                return BadRequest($"There aren't {quantity} questions");
            
            try
            {
                return Ok(_questionService.GetQuestions(quantity, categoryId));
            }
            catch {
                return BadRequest("A problem was occur");
            }
        }

        /// <summary>
        /// Get specifiec Question.
        /// </summary>
        [HttpGet]
        [Route("GetQuestion/{id:int}")]
        public ActionResult<int> GetQuestion([Required] int id)
        {
            try {
                return Ok(_questionService.GetQuestion(id));
            }
            catch (Exception) {
                return NoContent();
            }
        }

        /// <summary>
        /// Create a new question. 
        /// </summary>
        /// <response code="201">Returns the newly created item</response>
        ///  <response code="400">If the question was not created</response>            
        [HttpPost]
        [Route("AddQuestion")]
        public ActionResult AddQuestion([Required] QuestionModelRequest question)
        {
            try
            {
                if (question.WrongAnswers.Length != 3) return BadRequest("You need to add 3 wrong answers");
                int idQuestion = _questionService.CreateQuestion(question.Question, question.WrongAnswers, question.CorrectAnswer, question.IdCategory);
                return CreatedAtRoute($"GetQuestion/{idQuestion}", _questionService.GetQuestion(idQuestion));
            }
            catch (Exception)
            {
                return BadRequest("Question not added");
            }
        }

        /// <summary>
        /// Delete specific question.
        /// </summary>
        /// <response code="200">If the question was deleted</response>
        /// <response code="400">If the question was not deleted</response>
        [HttpDelete]
        [Route("RemoveQuestion")]
        public ActionResult RemoveQuestion([Required] int id) 
        {
            try
            {
                if (_questionService.DeleteQuestion(id))
                    return Ok("Question removed");
                else return NoContent();
            }
            catch (Exception)
            {
                return BadRequest("Question not removed");
            }
        }
    }
}