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

        [HttpGet]
        [Route("GetAllQuestions")]
        public ActionResult<QuestionDto> GetAllQuestions() => Ok(_questionService.GetAllQuestions());

        [HttpGet]
        [Route("GetMaxQuestionsToRequest")]
        public ActionResult<int> GetMaxQuestionsToRequest([Required] int categoryId) => Ok(_questionService.MaxQuestionsToRequest(categoryId));

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

        [HttpPost]
        [Route("AddQuestion")]
        public ActionResult AddQuestion([Required] QuestionModelRequest question)
        {
            try
            {
                if (question.WrongAnswers.Length != 3) return BadRequest("You need to add 3 wrong answers");
                _questionService.CreateQuestion(question.Question, question.WrongAnswers, question.CorrectAnswer, question.IdCategory);
                return Ok("Question added");
            }
            catch (Exception)
            {
                return BadRequest("Question not added");
            }
        }

        [HttpDelete]
        [Route("RemoveQuestion")]
        public ActionResult RemoveQuestion(int id) 
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