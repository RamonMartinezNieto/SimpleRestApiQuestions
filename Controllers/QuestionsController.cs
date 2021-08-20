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
        public string Get() => "Hola";

        [HttpGet]
        [Route("GetAllQuestions")]
        public ActionResult<QuestionDto> GetAllQuestions() => Ok(_questionService.GetAllQuestions());

        [HttpGet]
        [Route("GetMaxQuestionsToRequest")]
        public ActionResult<int> GetMaxQuestionsToRequest() => Ok(_questionService.maxQuestionsToRequest());

        [HttpGet]
        [Route("GetNumberOfQuestions")]
        public ActionResult<QuestionDto> GetNumberOfQuestions([Required] int quantity)
        {
            if (quantity == 0) return BadRequest("Indica una cantidad válida");

            try
            {
                return Ok(_questionService.GetQuestions(quantity));
            }
            catch {
                return BadRequest("No hay tantas preguntas");
            }
        }

        [HttpGet]
        [Route("GetQuestion/{id:int}")]
        public ActionResult<int> GetMaxQuestionsToRequest([Required] int id)
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
                _questionService.CreateQuestion(question.Question, question.WrongAnswers, question.CorrectAnswer);
                return Ok("Question added");
            }
            catch (Exception)
            {
                return BadRequest("Question not added");
            }
        }

        [HttpGet]
        [Route("RemoveQuestion")]
        public ActionResult RemoveQuestion(int id) 
        {
            try
            {
                _questionService.DeleteQuestion(id);
                return Ok("Question removed");
            }
            catch (Exception)
            {
                return BadRequest("Question not removed");
            }
        }
    }
}