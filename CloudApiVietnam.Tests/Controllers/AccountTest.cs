using CloudApiVietnam.Controllers;
using CloudApiVietnam.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudApiVietnam.Tests.Controllers
{
    class AccountTest
    {
        AccountController accountController = new AccountController();

        [TestMethod]
        public void AddUserWithSong()
        {
            RegisterBindingModel registerBindingModel = new RegisterBindingModel
            {
                Email = "Email@adres.nl",
                Username = "TestuserAPIaanpassing",
                Password = "Test01!",
                ConfirmPassword = "Test01!",
                FavoriteSong = "LeukLiedje!"
        };
            
            HttpResponseMessage result = accountController.Post(registerBindingModel);
            var resultContent = result.Content.ReadAsAsync<dynamic>().Result;

            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
        }
    }
}
