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
                    Name = "Java",
                    NumberOfQuestions = 10,
                    NumberOfVersion = 3
                }, new CategoryDto
                {
                    Id = 15,
                    Name = "CSharp",
                    NumberOfQuestions = 5,
                    NumberOfVersion = 1
                }});

            serviceMock.Setup(b => b.CreateCategory(It.IsAny<string>())).Returns(25);
            serviceMock.Setup(b => b.CreateCategory("BadRequest")).Throws(new Exception());

            serviceMock.Setup(b => b.GetCategoryVersion(5)).Returns(2);
            serviceMock.Setup(b => b.GetCategoryVersion(4)).Returns(0);

            serviceMock.Setup(b => b.DeleteCategory(5)).Returns(true);
            serviceMock.Setup(b => b.DeleteCategory(4)).Returns(false);
            serviceMock.Setup(b => b.DeleteCategory(3)).Throws(new Exception());
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

            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.NumberOfQuestions == 10));
            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.NumberOfQuestions == 5));

            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.Name == "Java"));
            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.Name == "CSharp"));

            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.NumberOfVersion == 3));
            Assert.That(listCategoriesResult, Has.Exactly(1).Matches<CategoryDto>(c => c.NumberOfVersion == 1));

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

        [Test]
        public void Test_Get_VersionCategory_Ok() 
        {
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult<int> result = controller.VersionCategory(5);
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(OkObjectResult)));

            OkObjectResult objectResult = (OkObjectResult)result.Result;
            Assert.That(objectResult.Value, Is.EqualTo(2));
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));

            serviceMock.Verify(a => a.GetCategoryVersion(5));
        }

        [Test]
        public void Test_Get_VersionCategory_NoContent()
        {
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult<int> result = controller.VersionCategory(4);
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(NoContentResult)));

            NoContentResult objectResult = (NoContentResult)result.Result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(204));

            serviceMock.Verify(a => a.GetCategoryVersion(4));
        }

        [Test]
        public void Test_Delete_Category_Ok()
        {
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult result = controller.Category(5);
            Assert.That(result.GetType(), Is.EqualTo(typeof(OkObjectResult)));

            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
            Assert.That(objectResult.Value, Is.EqualTo("Category removed")); 

            serviceMock.Verify(a => a.DeleteCategory(5));
        }

        [Test]
        public void Test_Delete_Category_NoContent()
        {
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult result = controller.Category(4);
            Assert.That(result.GetType(), Is.EqualTo(typeof(NoContentResult)));

            NoContentResult objectResult = (NoContentResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(204));

            serviceMock.Verify(a => a.DeleteCategory(4));
        }

        [Test]
        public void Test_Delete_Category_BadRequest()
        {
            CategoryController controller = new CategoryController(serviceMock.Object);

            ActionResult result = controller.Category(3);
            Assert.That(result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));

            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));
            Assert.That(objectResult.Value, Is.EqualTo("Any exception occrus calling the data base."));

            serviceMock.Verify(a => a.DeleteCategory(3));
        }
    }
}