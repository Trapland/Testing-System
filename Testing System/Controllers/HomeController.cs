using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;
using Testing_System.Data;
using Testing_System.Data.Entity;
using Testing_System.Models;
using Testing_System.Models.Test;
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
            TestsPageViewModel model = new()
            {
                Tests = _dataContext.Tests
                .Where(t => t.IsCompleted)
                .AsEnumerable()
                .Select(t => new TestViewModel()
                {
                    Name = t.Name,
                    Id = t.Id,
                    Description = t.Description,
                    Count = t.StartCount
                })
                .ToList()
            };
            return View(model);
        }

        public IActionResult Tests()
        {
            TestsPageViewModel model = new()
            {
                userStatus = HttpContext.Session.GetString("userStatus"),
                Tests = _dataContext.Tests
                .Where(t => t.IsCompleted)
                .AsEnumerable()
                .Select(t => new TestViewModel()
                {
                    Name = t.Name,
                    Id= t.Id,
                    Description = t.Description,
                    Count = t.StartCount
                })
                .ToList()
            };
            return View(model);
        }

        public IActionResult CreateTest()
        {
            return View();
        }

        public RedirectToActionResult CreateNewTest(CreateTestModel createTestModel)
        {
            Test NewTest = new Test()
            {
                Id = Guid.NewGuid(),
                Description = createTestModel.Description,
                Name = createTestModel.Name,
                TeacherId = Guid.Parse(HttpContext.Session.GetString("authUserId")),
                Count = 0,
                StartCount = 0,
                Time = createTestModel.Time
            };
            _dataContext.Tests.Add(NewTest);
            _dataContext.SaveChanges();
            HttpContext.Session.SetString("testId", NewTest.Id.ToString());
            HttpContext.Session.SetInt32("quesCount", 1);
            return RedirectToAction(nameof(CreateQuestion));
        }
        public ActionResult CreateQuestion() 
        {
            ViewData["CounterQuestions"] = HttpContext.Session.GetInt32("quesCount");
            return View();
        }

        public ActionResult FinishTest(CreateTestModel createTestModel)
        {
            HttpContext.Session.SetInt32("quesCount", HttpContext.Session.GetInt32("quesCount").Value - 1);
            ViewData["CounterQuestions"] = HttpContext.Session.GetInt32("quesCount").Value;
            return View();
        }

        public IActionResult Finish(CreateTestModel createTestModel)
        {
            Guid testId = Guid.Parse(HttpContext.Session.GetString("testId"));
            Test test = _dataContext.Tests.FirstOrDefault(t => t.Id == testId);
            test.Count = HttpContext.Session.GetInt32("quesCount").Value;
            test.StartCount = createTestModel.StartCount;
            test.IsCompleted = true;
            _dataContext.SaveChanges();
            HttpContext.Session.Remove("quesCount");
            HttpContext.Session.Remove("testId");
            return RedirectToAction(nameof(Tests));
        }

        public IActionResult AddQuestion(CreateQuestionModel addQuestionModel)
        {
            if (!String.IsNullOrEmpty(addQuestionModel.Description))
            {
                Dictionary<string, Difficulty> difficultyMap = new Dictionary<string, Difficulty>
                {
                    { "Beginner", Difficulty.Beginner },
                    { "Easy", Difficulty.Easy },
                    { "Medium", Difficulty.Medium },
                    { "Hard", Difficulty.Hard },
                    { "Advanced", Difficulty.Advanced },
                    { "Deep", Difficulty.Deep }
                };
                Question NewQuestion = new Question()
                {
                    Id = Guid.NewGuid(),
                    Description = addQuestionModel.Description,
                    Difficulty = difficultyMap[$"{addQuestionModel.Difficulty}"],
                    ImageURL = addQuestionModel.ImageURL,
                    TestId = Guid.Parse(HttpContext.Session.GetString("testId"))
            };
                _dataContext.Questions.Add(NewQuestion);
                Answer answer = new Answer()
                {
                    Id = Guid.NewGuid(),
                    Description = addQuestionModel.Answer1,
                    Value = addQuestionModel.ValueAnswer1,
                    QusetionId = NewQuestion.Id
                };
                _dataContext.Answers.Add(answer);
                answer = new Answer()
                {
                    Id = Guid.NewGuid(),
                    Description = addQuestionModel.Answer2,
                    Value = addQuestionModel.ValueAnswer2,
                    QusetionId = NewQuestion.Id
                };
                _dataContext.Answers.Add(answer);
                answer = new Answer()
                {
                    Id = Guid.NewGuid(),
                    Description = addQuestionModel.Answer3,
                    Value = addQuestionModel.ValueAnswer3,
                    QusetionId = NewQuestion.Id
                };
                _dataContext.Answers.Add(answer);
                answer = new Answer()
                {
                    Id = Guid.NewGuid(),
                    Description = addQuestionModel.Answer4,
                    Value = addQuestionModel.ValueAnswer4,
                    QusetionId = NewQuestion.Id
                };
                _dataContext.Answers.Add(answer);
                _dataContext.SaveChanges();
                HttpContext.Session.SetInt32("quesCount", HttpContext.Session.GetInt32("quesCount").Value + 1);
                return RedirectToAction(nameof(CreateQuestion));

            }
            else
            {
                addQuestionModel.counter = 1;
            }
            return View("CreateQuestion");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult StartTestPage([FromRoute] String id) 
        {
            HttpContext.Session.SetString("Testid", id);
            return View();
        }

        public IActionResult ViewTestPage([FromRoute] String id)
        {
            Guid TestId = Guid.Parse(id);
            Test test = _dataContext.Tests.FirstOrDefault(t => t.Id == TestId);

            TestModel model = new()
            {
                Id = TestId.ToString(),
                TeacherId = test.TeacherId.ToString(),
                Count = test.Count,
                StartCount = test.StartCount,
                Questions = _dataContext.Questions
                .Where(q => q.TestId == TestId)
                .AsEnumerable()
                .Select(q => new QuestionModel()
                {
                    Description = q.Description,
                    Id = q.Id.ToString(),
                }).ToList()
            };

            for (int i = 0; i < model.Questions.Count; i++) // по нормальному одним запитом не працює із-за ассінхронності
            {
                model.Questions[i].Answers = _dataContext.Answers
                    .Where(a => a.QusetionId.ToString() == model.Questions[i].Id)
                    .AsEnumerable()
                    .Select(a => new AnswerModel()
                    {
                        Value = a.Value,
                        Description = a.Description,
                        Id = a.Id.ToString(),
                    })
                    .ToList();
            }
            for (int i = 0; i < model.Questions.Count; i++)
            {
                int value = 0;
                for (int j = 0; j < model.Questions[i].Answers.Count; j++)
                {
                    if (value < model.Questions[i].Answers[j].Value)
                        value = model.Questions[i].Answers[j].Value;
                }
                for (int j = 0; j < model.Questions[i].Answers.Count; j++)
                {
                    if (value == model.Questions[i].Answers[j].Value)
                        model.Questions[i].Answers[j].isMax = true;
                }
            }
            return View(model);
        }

        public IActionResult ViewHistoryTestPage([FromRoute] String id)
        {
            Guid SessionId = Guid.Parse(id);
            List<History> history = _dataContext.History.Where(h => h.SessionId == SessionId).ToList(); 
            Test test = _dataContext.Tests.FirstOrDefault(t => t.Id == history[0].TestId);

            TestModel model = new()
            {
                Id = test.Id.ToString(),
                TeacherId = test.TeacherId.ToString(),
                Count = test.Count,
                StartCount = test.StartCount,
                Questions = _dataContext.Questions
                .Where(q => q.TestId == test.Id)
                .AsEnumerable()
                .Select(q => new QuestionModel()
                {
                    Description = q.Description,
                    Id = q.Id.ToString(),
                }).ToList()
            };

            for (int i = 0; i < model.Questions.Count; i++) // по нормальному одним запитом не працює із-за ассінхронності
            {
                model.Questions[i].Answers = _dataContext.Answers
                    .Where(a => a.QusetionId.ToString() == model.Questions[i].Id)
                    .AsEnumerable()
                    .Select(a => new AnswerModel()
                    {
                        Value = a.Value,
                        Description = a.Description,
                        Id = a.Id.ToString(),
                    })
                    .ToList();
            }
            for (int i = 0; i < model.Questions.Count; i++)
            {
                int value = 0;
                for (int j = 0; j < model.Questions[i].Answers.Count; j++)
                {
                    if (value < model.Questions[i].Answers[j].Value)
                        value = model.Questions[i].Answers[j].Value;
                }
                for (int j = 0; j < model.Questions[i].Answers.Count; j++)
                {
                    if (value == model.Questions[i].Answers[j].Value)
                        model.Questions[i].Answers[j].isMax = true;
                }
            }
            for (int i = 0; i < model.Questions.Count; i++)
            {
                for (int j = 0; j < model.Questions[i].Answers.Count; j++)
                {
                    for (int k = 0; k < history.Count; k++)
                    {
                        if (model.Questions[i].Answers[j].Id == history[k].AnswerId.ToString())
                        {
                            model.Questions[i].Answers[j].isMarked = true;
                            break;
                        }
                    }

                }
            }
            return View(model);
        }

        public IActionResult Test()
        {
            if(HttpContext.Session.GetString("Testid") == null)
            {
                return View("Index");
            }
            Guid TestId = Guid.Parse(HttpContext.Session.GetString("Testid"));
            Test test = _dataContext.Tests.FirstOrDefault(t => t.Id == TestId);

            TestModel model = new()
            {
                Id = TestId.ToString(),
                TeacherId = test.TeacherId.ToString(),
                StudentId = _dataContext.Students.FirstOrDefault(s => s.Id.ToString() == HttpContext.Session.GetString("authUserId")).Id.ToString(),
                Count = test.Count,
                StartCount = test.StartCount,
                Questions = _dataContext.Questions
                .Where(q => q.TestId == TestId)
                .AsEnumerable()
                .Select(q => new QuestionModel()
                {
                    Description = q.Description,
                    Id = q.Id.ToString(),
                }).ToList()
            };

            for (int i = 0; i < model.Questions.Count; i++) // по нормальному одним запитом не працює із-за ассінхронності
            {
                model.Questions[i].Answers = _dataContext.Answers
                    .Where(a => a.QusetionId.ToString() == model.Questions[i].Id)
                    .AsEnumerable()
                    .Select(a => new AnswerModel()
                    {
                        Value = a.Value,
                        Description = a.Description,
                        Id = a.Id.ToString(),
                    })
                    .ToList();
                RandomSort(model.Questions[i].Answers);
            }
            RandomSort(model.Questions);
            int sum = 0;
            for (int i = 0; i < model.Questions.Count; i++)
            {
                int value = 0;
                for (int j = 0; j < model.Questions[i].Answers.Count; j++)
                {
                    if (value < model.Questions[i].Answers[j].Value)
                        value = model.Questions[i].Answers[j].Value;
                }
                sum += value;
            }
            HttpContext.Session.SetInt32("Total", sum);
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveAnswers(TestModel testModel)
        {
            Guid sessionId = Guid.NewGuid();
            List<AnswerViewModel> answers = testModel.Answers;
            for (int i = 0; i < answers.Count; i++)
            {
                History history = new()
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    TestId = Guid.Parse(HttpContext.Session.GetString("Testid")),
                    StudentId = Guid.Parse(HttpContext.Session.GetString("authUserId")),
                    AnswerId = Guid.Parse(answers[i].AnswerId),
                    QuestionId = Guid.Parse(answers[i].QuestionId),
                    Marked = true
                };
                _dataContext.History.Add(history);
            }
            _dataContext.SaveChanges();
            HttpContext.Session.SetString("SessionId",sessionId.ToString());
            return RedirectToAction("Result");
        }

        public IActionResult Result()
        {
            Guid sessionId = Guid.Parse(HttpContext.Session.GetString("SessionId"));
            List<History> histories = _dataContext.History.Where(h => h.SessionId == sessionId).ToList();
            int sum = 0;

            for (int i = 0; i < histories.Count; i++)
            {
                Answer answer = _dataContext.Answers.FirstOrDefault(a => a.Id == histories[i].AnswerId);
                sum += answer.Value;
            }
            ViewData["result"] = $"You have gain {sum} of {HttpContext.Session.GetInt32("Total")}, Congratulations!";

            HttpContext.Session.Remove("Total");
            HttpContext.Session.Remove("SessionId");
            HttpContext.Session.Remove("Testid");
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
            else if (optionValues == "teacher")
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
            if (userStatus == "student")
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
            else if (userStatus == "teacher")
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

        public IActionResult History(HistoryViewModel historyViewModel)
        {
            historyViewModel.userStatus = HttpContext.Session.GetString("userStatus");
            if(historyViewModel.userStatus == "teacher")
            {
                Guid teacherId = Guid.Parse(HttpContext.Session.GetString("authUserId"));
                List<Test> tests = _dataContext.Tests.Where(t => t.TeacherId == teacherId).ToList();
                List<Guid> sessionIds = new();
                for (int i = 0; i < tests.Count; i++)
                {
                   sessionIds.AddRange(_dataContext.History.Where(t => t.TestId == tests[i].Id).Select(t => t.SessionId).Distinct().ToList());
                }
                List<History> history = new ();
                for (int i = 0; i < sessionIds.Count; i++)
                {
                    history.Add(_dataContext.History.FirstOrDefault(h => h.SessionId == sessionIds[i]));
                }
                List<Student> students = new();

                List<Test> students_tests = new();


                for (int i = 0; i < history.Count; i++)
                {
                    students.Add(_dataContext.Students.FirstOrDefault(s => s.Id == history[i].StudentId));

                    students_tests.Add(_dataContext.Tests.FirstOrDefault(t => t.Id == history[i].TestId));
                }
                historyViewModel.Tests = new();
                for (int i = 0; i < students.Count; i++)
                {
                    historyViewModel.Tests.Add(new TestHistoryViewModel()
                    {
                        Name= students_tests[i].Name,
                        Description = students_tests[i].Description,
                        SessionId = sessionIds[i].ToString(),
                        student = new() { 
                        Name = students[i].Name,
                        Surname = students[i].Surname,
                        Id = students[i].Id.ToString(),
                        }

                    });
                }

            }

            return View(historyViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static void RandomSort<T>(List<T> list)
        {
            Random random = new Random();
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}