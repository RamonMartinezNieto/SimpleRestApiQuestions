using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SimpleRestApiQuestions.Controllers;
using SimpleRestApiQuestions.Dto;
using SimpleRestApiQuestions.Service;
using System;
using System.Collections.Generic;

namespace TestRestSimpleRestApiQuestions.Service
{
    [TestFixture]
    public class TestCategoryController
    {
        Mock<IQuestionService> serviceMock;

        [SetUp]
        public void SetUp() 
        {
            serviceMock = new Mock<IQuestionService>();

            serviceMock.Setup(a => a.GetCategories()).Returns(new List<CategoryDto>() {
                new CategoryDto
                {
                    Id = 5,
                    Name = "Java"
                }, new CategoryDto
                {
                    Id = 15,
                    Name = "CSharp"
                }});

            serviceMock.Setup(b => b.CreateCategory(It.IsAny<string>())).Returns(25);
            serviceMock.Setup(b => b.CreateCategory("BadRequest")).Throws(new Exception());
        }

        [Test]
        public void Test_Get_Categories_Controller() 
        {
            CategoryController controller = new CategoryController(serviceMock.Object);
            
            ActionResult<CategoryDto> categoriesResutl = controller.Categories();
            
            Assert.That(typeof(OkObjectResult), Is.EqualTo(categoriesResutl.Result.GetType()));
            OkObjectResult objectResult = (OkObjectResult)categoriesResutl.Result;
            
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
            Assert.That(typeof(List<CategoryDto>), Is.EqualTo(objectResult.Value.GetType()));
            List<CategoryDto> listCategoriesResult = (List<CategoryDto>)objectResult.Value;

            Assert.That(listCategoriesResult, Is.Not.Empty);
            Assert.That(listCategoriesResult, Has.Exactly(2).Items);
            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>( c => c.Id == 5));
            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.Id == 15));

            serviceMock.Verify(a=> a.GetCategories());
        }

        [Test]
        public void Test_Create_Category() 
        {
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult result = controller.Category("Ruby");
            Assert.That(typeof(OkObjectResult), Is.EqualTo(result.GetType()));

            serviceMock.Verify(a => a.CreateCategory(It.IsAny<string>()));
        }

        [Test]
        public void Test_Create_Category_BadRequest()
        { 
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult result = controller.Category("BadRequest");
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(result.GetType()));
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
            Assert.That(objectResult.Value, Is.EqualTo("Category not added"));

            serviceMock.Verify(a => a.CreateCategory("BadRequest"));
        }
    }
}


