using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using SimpleRestApiQuestions.Dto;
using SimpleRestApiQuestions.Service;
using App.Metrics;
using App.Metrics.Counter;
using SimpleRestApiQuestions.Metrics;

namespace SimpleRestApiQuestions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IMetrics _metrics;

        public QuestionController(IQuestionService service, IMetrics metrics)
        {
            _questionService = service;
            _metrics = metrics; 
        }

        /// <summary>
        /// Get all questions in the repository of specific category.
        /// </summary>
        /// <response code="200">Returns all questions'content of the category specified</response>
        [HttpGet]
        [Route("{id_category:int}/AllQuestions")]
        [AllowAnonymous]
        public ActionResult<QuestionDto> GetAllQuestions(int id_category) => Ok(_questionService.GetAllQuestions(id_category));

        /// <summary>
        /// Get max number of questions for a specific category. 
        /// </summary>
        [HttpGet]
        [Route("{id_category:int}/MaxQuestionsToRequest")]
        [AllowAnonymous]
        public ActionResult<int> GetMaxQuestionsToRequest([Required] int id_category) => Ok(_questionService.MaxQuestionsToRequest(id_category));

        /// <summary>
        /// Get random questions from specified category. 
        /// If some action call this method means that start a quiz.
        /// Metrics: Count Quiz
        /// Metris: Count what id category is it.
        /// Metrics: Count how many questions request the user. 
        /// </summary>
        /// <response code="200">Returns the number of questions requested</response>
        /// <response code="400">Some problem was occur with the category Id or the data base</response>
        /// <response code="401">Unauthorized Jason Web Token</response>
        [HttpGet]
        [Route("{id_category:int}/RandomQuestions/{quantity:int}")]
        [AllowAnonymous]
        public ActionResult<QuestionDto> GetRandomQuestions([Required] int quantity, [Required] int id_category)
        {
            if (quantity == 0 || quantity > _questionService.MaxQuestionsToRequest(id_category))
                return BadRequest($"There aren't {quantity} questions");

            try
            {
                _metrics.Measure.Counter.Increment(MetricsRegistry.NewQuizStartCounter);
                _metrics.Measure.Counter.Increment(MetricsRegistry.CategoryQuizStart(id_category));

                return Ok(_questionService.GetQuestions(quantity, id_category));
            }
            catch {
                return BadRequest("A problem was occur");
            }
        }

        /// <summary>
        /// Get specifiec Question.
        /// </summary>
        /// <response code="200">Retur question content</response>
        /// <response code="204">There aren't conent with the specified question id</response>
        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
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
        /// <response code="200">Question added</response>
        /// <response code="401">Unauthorized Jason Web Token</response>
        /// <response code="400">Quiestion not added, trying to insert invalid question data</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult AddQuestion([Required] QuestionModelRequest question)
        {
            try
            {
                if (question.WrongAnswers.Length != 3) return BadRequest("You need to add 3 wrong answers");
                _questionService.CreateQuestion(question.Question, question.WrongAnswers, question.CorrectAnswer, question.IdCategory);
                //TODO usar CREATED or CREATED AT ROUTE 
                return Ok("Question added");
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
        /// <response code="401">Unauthorized Jason Web Token</response>
        [HttpDelete]
        [Authorize(Roles = "admin")]
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