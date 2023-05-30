﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Security.Claims;
using Testing_System.Data;
using Testing_System.Data.Entity;
using Testing_System.Models;
using Testing_System.Models.User;
using Testing_System.Services.Kdf;
using Testing_System.Services.Random;
using Testing_System.Services.RandomImg;
using Testing_System.Services.Validation;

namespace Testing_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKdfService _kdfService;
        private readonly IRandomImgName _randomImgService;
        private readonly IValidationService _validationService;
        private readonly DataContext _dataContext;
        private readonly IRandomService _randomService;

        public HomeController(ILogger<HomeController> logger, IValidationService validationService, IRandomImgName randomImgService, IKdfService kdfService, IRandomService randomService, DataContext dataContext)
        {
            _logger = logger;
            _validationService = validationService;
            _randomImgService = randomImgService;
            _kdfService = kdfService;
            _randomService = randomService;
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }


        public ActionResult Register(Registration registrationModel)
        {
            bool isModelValid = true;
            RegisterValidationResult registerValidation = new();

            if (String.IsNullOrEmpty(registrationModel.Login))
            {
                registerValidation.LoginMessage = "Login can`t be empty";
                isModelValid = false;
            }
            if (registrationModel.Option == "student")
            {
                if (_dataContext.Students.Any(s => s.Login == registrationModel.Login))
                {
                    registerValidation.LoginMessage = "This login is already exists";
                    isModelValid = false;
                }

                if (_dataContext.Students.Any(s => s.Email == registrationModel.Email))
                {
                    registerValidation.EmailMessage = "This email is already exists";
                    isModelValid = false;
                }
            }
            else if (registrationModel.Option == "teacher")
            {
                if (_dataContext.Teachers.Any(t => t.Login == registrationModel.Login))
                {
                    registerValidation.LoginMessage = "This login is already exists";
                    isModelValid = false;
                }

                if (_dataContext.Teachers.Any(t => t.Email == registrationModel.Email))
                {
                    registerValidation.EmailMessage = "This email is already exists";
                    isModelValid = false;
                }
            }
            if (String.IsNullOrEmpty(registrationModel.Password))
            {
                registerValidation.PasswordMessage = "Password can`t be empty";
                isModelValid = false;
            }
            if (String.IsNullOrEmpty(registrationModel.RepeatPassword))
            {
                registerValidation.RepeatPasswordMessage = "Confirm Password can`t be empty";
                isModelValid = false;
            }
            else if (registrationModel.RepeatPassword != registrationModel.Password)
            {
                registerValidation.RepeatPasswordMessage = "Passwords are not the same";
                isModelValid = false;
            }
            if (!_validationService.Validate(registrationModel.Email, ValidationTerms.NotEmpty))
            {
                registerValidation.EmailMessage = "Email can`t be empty";
                isModelValid = false;
            }
            else if (!_validationService.Validate(registrationModel.Email, ValidationTerms.Email))
            {
                registerValidation.EmailMessage = "Email is incorrect";
                isModelValid = false;
            }

            if (String.IsNullOrEmpty(registrationModel.Name))
            {
                registerValidation.NameMessage = "Name can`t be empty";
                isModelValid = false;
            }
            if (String.IsNullOrEmpty(registrationModel.Surname))
            {
                registerValidation.SurnameMessage = "Surname can`t be empty";
                isModelValid = false;
            }
            String savedName = "";
            if (registrationModel.Avatar == null)
            {
                savedName = "";
            }
            else
            {
                savedName = _randomImgService.RandomNameImg(registrationModel.Avatar.FileName);
            }
            if (registrationModel.Avatar is not null)
            {
                if (registrationModel.Avatar.Length > 1024)
                {
                    String folderName = "wwwroot/avatars/";
                    String filePath = folderName + savedName;
                    IEnumerable<string> files = Directory.EnumerateFiles(folderName);
                    while (files.Contains(filePath))
                    {
                        savedName = _randomImgService.RandomNameImg(registrationModel.Avatar.FileName);
                        filePath = folderName + savedName;
                    }
                    String path = folderName + savedName;
                    using FileStream fs = new(path, FileMode.Create);
                    registrationModel.Avatar.CopyTo(fs);
                }
                else
                {
                    isModelValid = false;
                    registerValidation.AvatarMessage = "Avatar size is too small";
                }
            }
            if (isModelValid)
            {
                String salt = _randomService.RandomString(16);
                if (registrationModel.Option == "student")
                {
                    Student student = new()
                    {
                        Id = Guid.NewGuid(),
                        Login = registrationModel.Login,
                        Name = registrationModel.Name,
                        Surname = registrationModel.Surname,
                        Email = registrationModel.Email,
                        PasswordSalt = salt,
                        PasswordHash = _kdfService.GetDerivedKey(registrationModel.Password, salt),
                        Avatar = savedName
                    };
                    _dataContext.Students.Add(student);
                    _dataContext.SaveChangesAsync();
                }
                else if (registrationModel.Option == "teacher")
                {
                    Teacher teacher = new()
                    {
                        Id = Guid.NewGuid(),
                        Login = registrationModel.Login,
                        Name = registrationModel.Name,
                        Surname = registrationModel.Surname,
                        Email = registrationModel.Email,
                        PasswordSalt = salt,
                        PasswordHash = _kdfService.GetDerivedKey(registrationModel.Password, salt),
                        Avatar = savedName
                    };
                    _dataContext.Teachers.Add(teacher);
                    _dataContext.SaveChangesAsync();
                }

                return View("Index");
            }
            else
            {
                ViewData["RegisterValidationResult"] = registerValidation;
                return View("Registration");
            }

        }

        [HttpPost]
        public String AuthUser()
        {
            StringValues optionValues = Request.Form["user-option"];
            if (optionValues.Count == 0)
            {
                return "Missed required parameter: user-option";
            }

            StringValues loginValues = Request.Form["user-login"];
            if (loginValues.Count == 0)
            {
                return "Missed required parameter: user-login";
            }
            String login = loginValues[0] ?? "";

            StringValues passwordValues = Request.Form["user-password"];
            if (passwordValues.Count == 0)
            {
                return "Missed required parameter: user-password";
            }
            String password = passwordValues[0] ?? "";
            if (optionValues == "student")
            {
                Student? student = _dataContext.Students.Where(s => s.Login == login).FirstOrDefault();
                if (student is not null)
                {
                    if (student.PasswordHash == _kdfService.GetDerivedKey(password, student.PasswordSalt))
                    {
                        HttpContext.Session.SetString("authUserId", student.Id.ToString());
                        HttpContext.Session.SetString("userStatus", optionValues);

                        return "OK";
                    }

                }
            }
            else if( optionValues == "teacher")
            {
                Teacher? teacher = _dataContext.Teachers.Where(t => t.Login == login).FirstOrDefault();
                if (teacher is not null)
                {
                    if (teacher.PasswordHash == _kdfService.GetDerivedKey(password, teacher.PasswordSalt))
                    {
                        HttpContext.Session.SetString("authUserId", teacher.Id.ToString());
                        HttpContext.Session.SetString("userStatus", optionValues);

                        return "OK";
                    }

                }
            }    
            return "Авторизацію відхилено";
        }

        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Remove("authUserId");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile([FromRoute] String id)
        {
            _logger.LogInformation(id);
            String? userStatus = HttpContext.Session.GetString("userStatus");
            if(userStatus == "student")
            {
                Student? user = _dataContext.Students.FirstOrDefault(u => u.Login == id);
                if (user is not null)
                {
                    StudentProfileModel model = new(user);
                    if (HttpContext.User.Identity is not null &&
                        HttpContext.User.Identity.IsAuthenticated)
                    {
                        String userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    }
                    return View(model);

                }
                else
                {
                    return NotFound();
                }
            }
            else if(userStatus == "teacher")
            {
                Teacher? user = _dataContext.Teachers.FirstOrDefault(u => u.Login == id);
                if (user is not null)
                {
                    TeacherProfileModel model = new(user);
                    if (HttpContext.User.Identity is not null &&
                        HttpContext.User.Identity.IsAuthenticated)
                    {
                        String userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    }
                    return View(model);

                }
                else
                {
                    return NotFound();
                }
            }    
            else
            {
                return NotFound();
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}