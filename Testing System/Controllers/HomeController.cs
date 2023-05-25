using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
                registerValidation.LoginMessage = "Логін не може бути порожним";
                isModelValid = false;
            }
            if (_dataContext.Students.Any(s => s.Login == registrationModel.Login))
            {
                registerValidation.LoginMessage = "Логін вже використовується";
                isModelValid = false;
            }
            if (String.IsNullOrEmpty(registrationModel.Password))
            {
                registerValidation.PasswordMessage = "Пароль не може бути порожним";
                isModelValid = false;
            }
            if (String.IsNullOrEmpty(registrationModel.RepeatPassword))
            {
                registerValidation.RepeatPasswordMessage = "Повтор пароля не може бути порожним";
                isModelValid = false;
            }
            else if (registrationModel.RepeatPassword != registrationModel.Password)
            {
                registerValidation.RepeatPasswordMessage = "Паролі не співпадають";
                isModelValid = false;
            }
            if (!_validationService.Validate(registrationModel.Email, ValidationTerms.NotEmpty))
            {
                registerValidation.EmailMessage = "Email не може бути порожним";
                isModelValid = false;
            }
            else if (!_validationService.Validate(registrationModel.Email, ValidationTerms.Email))
            {
                registerValidation.EmailMessage = "Email введено не корректно";
                isModelValid = false;
            }
            else if(_dataContext.Students.Any(s => s.Email == registrationModel.Email))
            {
                registerValidation.EmailMessage = "Email вже використовується";
                isModelValid = false;
            }

            if (String.IsNullOrEmpty(registrationModel.Name))
            {
                registerValidation.NameMessage = "Name не може бути порожним";
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
                    ViewData["savedName"] = savedName;
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

                return View("Index");
            }
            else
            {
                ViewData["regModel"] = registrationModel;
                ViewData["RegisterValidationResult"] = registerValidation;
                return View("Registration");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}