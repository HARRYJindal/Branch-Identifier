using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BranchCleaner.Controllers;
using BranchCleaner.Models;
using BranchCleanerDll.Interfaces;
using BranchCleanerDll.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Net;

namespace unit_test_case.controller
{
    public class BranchCleanertest
    {
        private Mock<ILogger<BranchCleanerController>> _loggerMock;
        private Mock<IBranchCleaner> _branchCleanerMock;
        private Mock<IMapper> _mapperMock;
        private BranchCleanerController _controller;
        public BranchCleanertest()
        {
            _loggerMock = new Mock<ILogger<BranchCleanerController>>();
            _branchCleanerMock = new Mock<IBranchCleaner>();
            _mapperMock = new Mock<IMapper>();
            _controller = new BranchCleanerController(_branchCleanerMock.Object, _mapperMock.Object, _loggerMock.Object);
        }
       
        [Fact]
        public async Task GetBranches_ReturnsOkResult()
        {
            // Arrange
            var branches = new Dictionary<string, List<string>>
            {
                { "Repo1", new List<string> { "branch1", "branch2" } },
                { "Repo2", new List<string> { "branch3", "branch4" } }
            };
            var branchDates = new List<BranchDate>
            {
                new BranchDate { BranchName = "branch1", NumberOfDays = 2 },
                new BranchDate { BranchName = "branch2", NumberOfDays = 3 }
            };

            _branchCleanerMock.Setup(c => c.GetBranches(It.IsAny<string>())).ReturnsAsync((branches, ""));

            _branchCleanerMock.Setup(c => c.GetCommit(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync(branchDates);

            _mapperMock.Setup(m => m.Map<List<BranchDate>, List<BranchDays>>(It.IsAny<List<BranchDate>>()))
                       .Returns(new List<BranchDays>());

            // Act

            var result = await _controller.GetBranches();

            //Assert

            var jsonResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.OK, jsonResult.StatusCode);

            var data = jsonResult.Value;
            Assert.NotNull(data);
        }

        [Theory]
        [InlineData("Some error message")]
        [InlineData("Another error message")]
        public async Task GetBranches_ReturnBadRequest(string errorMessage)
        {
            //Arrage
            _branchCleanerMock.Setup(c => c.GetBranches(It.IsAny<string>())).ReturnsAsync((null,errorMessage));

            //Act
            var result = await _controller.GetBranches();

            //Assert
            var jsonResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, jsonResult.StatusCode);
            var message = jsonResult.Value;
            Assert.NotNull(message);
        }
    }
}
