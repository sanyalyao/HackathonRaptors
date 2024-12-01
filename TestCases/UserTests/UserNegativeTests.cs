﻿using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using QAHackathon.BussinesObjects.Models;
using QAHackathon.BussnessObjects.Models;
using QAHackathon.BussnessObjects.Utils;
using QAHackathon.Core.BussnessLogic;
using QAHackathon.Core.RestCore;
using RestSharp;
using System.Text.RegularExpressions;
using static QAHackathon.Core.BussnessLogic.StepsBL;

namespace QAHackathon.TestCases.UserTests
{
    [TestFixture]
    public class UserNegativeTests : TestBase
    {
        [Test]
        [Description("Update non-existent user. Checking for error in response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void UpdateNonExistentUser()
        {
            var user =  Step("Generating a user with fake UUID", () => 
            {
                var user = new UserModel()
                {
                    Uuid = UtilsBL.GetRandomUuid()
                };

                loggingBL.Info($"Fake UUID: {user.Uuid}");

                return user;
            });

            var response = Step("Updating non-existent user", () =>
            {
                var userWithChanges = new Dictionary<string, string>()
                {
                    { "name", UtilsBL.GetCorrectName() },
                    { "nickname", UtilsBL.GetCorrectNickname() }
                };

                return userService.UpdateUserWithoutException(user, userWithChanges);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex($"Could not find user with \"uuid\": {user.Uuid}").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.NotFound, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Get a user NOT by email and password. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void GetUserNotByPasswordAndEmail()
        {
            var currentUser = Step("Creating a new user", () =>
            {
                var newUser = UserGenerator.GetNewUser();

                userService.CreateNewUser(newUser);

                return newUser;
            });

            var response = Step("Getting the user by nickname and password", () =>
            {
                return userService.GetUserByNicknameAndPassword(currentUser);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex(".*property \"email\" is missing.*").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Create a new user with incorrect email format. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void CreateUserWithIncorrectEmailFormat()
        {
            var userIncorrectEmail = Step("Generating fake user with incorrect email format", () => 
            {
                return UserGenerator.GetNewUserWithIncorrectEmailFormat();
            });            

            var response = Step("Registering a new user with incorrect email", () => 
            {
                return userService.CreateNewUserWithoutException(userIncorrectEmail);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex(".*string doesn't match the regular expression.*").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Create a new user with incorrect max email length. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void CreateUserWithIncorrectMaxEmailLength()
        {
            var userIncorrectEmailMaxLength = Step("Generating fake users with incorrect max email length", () =>
            {
                return UserGenerator.GetUserIncorrectEmailLength(UserGenerator.Lengths.Max);
            });

            var response = Step("Registering a new user with incorrect max email length", () =>
            {
                return userService.CreateNewUserWithoutException(userIncorrectEmailMaxLength);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex(".*maximum string length is 100.*").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Create a new user with empty email. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void CreateUserWithEmptyEmailLength()
        {
            var userIncorrectEmailMinLength = Step("Generating fake users with empty email", () =>
            {
                var newUser = UserGenerator.GetNewUser();
                newUser.Email = string.Empty;

                loggingBL.Info("Generated fake user with empty email");

                return newUser;
            });

            var response = Step("Registering a new user with incorrect min email length", () =>
            {
                return userService.CreateNewUserWithoutException(userIncorrectEmailMinLength);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex(".*minimum string length is 5.*").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Create a new user with null email. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void CreateUserWithNullEmail()
        {
            var userIncorrectEmailMinLength = Step("Generating fake users with null email", () =>
            {
                var newUser = UserGenerator.GetNewUser();
                newUser.Email = null;

                loggingBL.Info("Generated fake user with empty email");

                return newUser;
            });

            var response = Step("Registering a new user with incorrect min email length", () =>
            {
                return userService.CreateNewUserWithoutException(userIncorrectEmailMinLength);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex(".*Value is not nullable.*").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Create a new user with incorrect min email length. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void CreateUserWithIncorrectMinEmailLength()
        {
            var userIncorrectEmailMinLength = Step("Generating fake users with incorrect min email length", () =>
            {
                var newUser = UserGenerator.GetUserIncorrectEmailLength(UserGenerator.Lengths.Min);

                loggingBL.Info("Generated fake user with incorrect min email length");

                return newUser;
            });

            var response = Step("Registering a new user with incorrect min email length", () =>
            {
                return userService.CreateNewUserWithoutException(userIncorrectEmailMinLength);
            });

            Step("Checking for error in response", () =>
            {
                var error = new ErrorModel().GetError(response);
                var isMatch = new Regex(".*minimum string length is 5.*").Match(error.Message);

                AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                AssertBL.IsFalse(response.IsSuccessful);
                AssertBL.IsTrue(isMatch.Success);

                loggingBL.Info($"There is error in response: {error.Code}");
                loggingBL.Info(error.Message);
            });
        }

        [Test]
        [Description("Update a user with incorrect null data. Checking for error response")]
        [Category("API")]
        [Category("Users")]
        [Category("Negative")]
        [AllureSeverity(SeverityLevel.critical)]
        public void UpdateUserWithIncorrectData()
        {
            var responses = Step("Updating a user with incorrect null data", () =>
            {
                var currentUser = userService.GetUsers().Users.ToList().First();

                var userWithNullName = new Dictionary<string, string>()
                {
                    { "name", null }
                };

                var userWithNullNickname = new Dictionary<string, string>()
                {
                    { "nickname", null }
                };

                var userWithNullEmail = new Dictionary<string, string>()
                {
                    { "email", null }
                };

                var userWithNullPassword = new Dictionary<string, string>()
                {
                    { "password", null }
                };

                var responses = new List<RestResponse>() { userService.UpdateUserWithoutException(currentUser, userWithNullName),
                    userService.UpdateUserWithoutException(currentUser, userWithNullNickname),
                    userService.UpdateUserWithoutException(currentUser, userWithNullEmail),
                    userService.UpdateUserWithoutException(currentUser, userWithNullPassword) };

                return responses;
            });

            Step("Checking for error in responses", () =>
            {
                foreach (var response in responses)
                {
                    var error = new ErrorModel().GetError(response);
                    var isMatch = new Regex(".*Value is not nullable.*").Match(error.Message);

                    AssertBL.AreEqual((int)HttpStatusCodes.StatusCodes.BadRequest, (int)response.StatusCode);
                    AssertBL.IsFalse(response.IsSuccessful);
                    AssertBL.IsTrue(isMatch.Success);

                    loggingBL.Info($"There is error in each response: {error.Code}");
                    loggingBL.Info(error.Message);
                }
            });
        }
    }
}