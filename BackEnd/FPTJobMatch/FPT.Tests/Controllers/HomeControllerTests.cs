using FakeItEasy;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models.ViewModels;
using FPT.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPTJobMatch.Controllers;

namespace FPT.Tests.Controllers
{
    public class HomeControllerTests
    {
        private HomeController _controller;
        private IUnitOfWork _unitOfWork;
        public HomeControllerTests()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _controller = new HomeController(_unitOfWork);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCorrectViewModel()
        {

        }
    }
}
