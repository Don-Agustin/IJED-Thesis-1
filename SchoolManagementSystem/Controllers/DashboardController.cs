using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ISchoolClassRepository _classRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public DashboardController(
            IAlertRepository alertRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            ICourseRepository courseRepository,
            ISchoolClassRepository classRepository,
            ISubjectRepository subjectRepository,
            IAttendanceRepository attendanceRepository)
        {
            _alertRepository = alertRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<IActionResult> Index() => View();

        // ── Admin Dashboard ──────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var vm = await BuildAdminViewModel();
            return View(vm);
        }

        // ── Employee Dashboard ───────────────────────────────────────────
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EmployeeDashboard()
        {
            var vm = await BuildAdminViewModel();
            vm.WelcomeName = User.Identity?.Name?.Split('@')[0] ?? "Employee";
            return View(vm);
        }

        // ── Teacher Dashboard (Home Overview) ────────────────────────────
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherDashboard()
        {
            var vm = await BuildTeacherDashboardViewModel();
            return View(vm);
        }

        // ── Teacher Courses ───────────────────────────────────────────────
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherCourses(int? classId)
        {
            var vm = await BuildTeacherCoursesViewModel(classId);
            return View(vm);
        }

        // ── Student Dashboard ────────────────────────────────────────────
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = userId != null
                ? await _studentRepository.GetStudentByUserIdAsync(userId)
                : null;

            var vm = new DashboardViewModel
            {
                WelcomeName = User.Identity?.Name?.Split('@')[0] ?? "Student",
            };

            if (student != null)
            {
                var attendances = await _attendanceRepository.GetAttendancesByStudentIdAsync(student.Id);
                int total = attendances.Count;
                int present = attendances.Count(a => a.Status == "Present");
                vm.AttendanceRate = total > 0 ? Math.Round((double)present / total * 100, 1) : 0;
            }

            return View(vm);
        }

        // ── Calendar Event CRUD (Teacher) ────────────────────────────────
        // Events are stored in TempData for now (DB migration in next sprint)

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult AddCalendarEvent(string title, string date, string color, string description)
        {
            // Placeholder — will persist to DB in next sprint
            TempData["CalendarMsg"] = $"Event '{title}' saved.";
            return RedirectToAction("TeacherDashboard");
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult DeleteCalendarEvent(int id)
        {
            TempData["CalendarMsg"] = "Event deleted.";
            return RedirectToAction("TeacherDashboard");
        }

        // ── Upload course material ────────────────────────────────────────
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<IActionResult> UploadMaterial(int classId, string title,
            string category, IFormFile file)
        {
            // Placeholder — will persist to DB/blob in next sprint
            TempData["UploadMsg"] = $"'{title}' uploaded successfully.";
            return RedirectToAction("TeacherCourses", new { classId });
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult DeleteMaterial(int materialId, int classId)
        {
            TempData["UploadMsg"] = "Material deleted.";
            return RedirectToAction("TeacherCourses", new { classId });
        }

        // ─────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────

        private async Task<TeacherDashboardViewModel> BuildTeacherDashboardViewModel()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = userId != null
                ? await _teacherRepository.GetTeacherByUserIdAsync(userId)
                : null;

            // If no DB teacher record yet, build a name from the email
            var firstName = teacher?.FirstName
                ?? User.Identity?.Name?.Split('@')[0]?.Split('.')[0]
                ?? "Teacher";

            // Build advising class summaries from real DB data
            var classSummaries = new List<TeacherClassSummary>();
            if (teacher != null)
            {
                var detailed = await _teacherRepository.GetTeacherWithDetailsAsync(teacher.Id);
                if (detailed != null)
                {
                    foreach (var tsc in detailed.TeacherSchoolClasses)
                    {
                        var students = await _studentRepository
                            .GetStudentsBySchoolClassIdAsync(tsc.SchoolClassId);
                        var subjects = detailed.TeacherSubjects
                            .Select(ts => ts.Subject)
                            .Where(s => s != null)
                            .ToList();

                        classSummaries.Add(new TeacherClassSummary
                        {
                            SchoolClass = tsc.SchoolClass,
                            Subjects = subjects,
                            StudentCount = students?.Count ?? 0,
                        });
                    }
                }
            }

            // ── Example static schedule (replaced by DB in next sprint) ──
            var todaySchedule = new List<ScheduleItem>
            {
                new() { Time = "8:00",  Title = "Grade 10 – Computer Science 101, Lecture",   Color = "green"  },
                new() { Time = "9:00",  Title = "Grade 10 – Computer Science 101",            Color = "green"  },
                new() { Time = "10:00", Title = "Meeting: Dept Head",                         Color = "amber"  },
                new() { Time = "11:30", Title = "Grade 10 – Programs 101",                    Color = "green"  },
            };

            // ── Example upcoming activities ──
            var upcomingActivities = new List<ActivityItem>
            {
                new() { Title = "Prep for Web Design Lecture",            Icon = "fa-book-open",     IsDone = false },
                new() { Title = "Grade Midterm Papers – CS 101 (20 remaining)", Icon = "fa-file-alt",IsDone = false },
                new() { Title = "Create Digital Literacy Quiz",           Icon = "fa-pencil-alt",    IsDone = true  },
            };

            // ── Example calendar events ──
            var now = DateTime.Today;
            var calendarEvents = new List<CalendarEvent>
            {
                new() { Id = 1, Title = "Web Design Test – Grade 11",   Date = now.AddDays(2),  Color = "maroon",  Description = "Covers Ch 1–3"           },
                new() { Id = 2, Title = "Staff Meeting",                 Date = now.AddDays(4),  Color = "amber",   Description = "Department meeting"       },
                new() { Id = 3, Title = "Web Design Test – Grade 11",   Date = now.AddDays(9),  Color = "maroon",  Description = "Covers Ch 1–3"            },
                new() { Id = 4, Title = "Prep time",                     Date = now.AddDays(10), Color = "blue",    Description = "Lesson prep"              },
                new() { Id = 5, Title = "Staff Meeting",                 Date = now.AddDays(11), Color = "amber",   Description = "Department meeting"       },
                new() { Id = 6, Title = "Create Digital Literacy Quiz",  Date = now.AddDays(14), Color = "green",   Description = "Quiz for Grade 9 Sec C"  },
            };

            return new TeacherDashboardViewModel
            {
                Teacher = teacher,
                WelcomeName = firstName,
                AdvisingClasses = classSummaries,
                TodaySchedule = todaySchedule,
                UpcomingActivities = upcomingActivities,
                CalendarEvents = calendarEvents,
            };
        }

        private async Task<TeacherCoursesViewModel> BuildTeacherCoursesViewModel(int? classId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var teacher = userId != null
                ? await _teacherRepository.GetTeacherByUserIdAsync(userId)
                : null;

            var classSummaries = new List<TeacherClassSummary>();
            if (teacher != null)
            {
                var detailed = await _teacherRepository.GetTeacherWithDetailsAsync(teacher.Id);
                if (detailed != null)
                {
                    foreach (var tsc in detailed.TeacherSchoolClasses)
                    {
                        var students = await _studentRepository
                            .GetStudentsBySchoolClassIdAsync(tsc.SchoolClassId);
                        var subjects = detailed.TeacherSubjects
                            .Select(ts => ts.Subject)
                            .Where(s => s != null)
                            .ToList();

                        classSummaries.Add(new TeacherClassSummary
                        {
                            SchoolClass = tsc.SchoolClass,
                            Subjects = subjects,
                            StudentCount = students?.Count ?? 0,
                        });
                    }
                }
            }

            // Resolve active class
            var activeClass = classId.HasValue
                ? classSummaries.FirstOrDefault(c => c.SchoolClass.Id == classId.Value)
                : classSummaries.FirstOrDefault();

            // ── Example materials (static until DB migration) ──
            var sampleModules = new List<CourseMaterial>
            {
                new() { Id=1, Title="Chapter 1: Intro to Python",       FileType="PDF", FileName="ch1_intro_python.pdf",     UploadedOn=DateTime.Today.AddDays(-15), Category="Module"     },
                new() { Id=2, Title="Chapter 2: Variables & Data Types", FileType="PPT", FileName="ch2_variables.pptx",       UploadedOn=DateTime.Today.AddDays(-8),  Category="Module"     },
            };
            var sampleAssessments = new List<CourseMaterial>
            {
                new() { Id=3, Title="Midterm Exam – CS 101",            FileType="EXAM", FileName="midterm_cs101.pdf",        UploadedOn=DateTime.Today.AddDays(-5),  Category="Assessment" },
                new() { Id=4, Title="Quiz 1 – Python Basics",           FileType="QUIZ", FileName="quiz1_python.pdf",         UploadedOn=DateTime.Today.AddDays(-10), Category="Assessment" },
            };
            var sampleActivities = new List<CourseMaterial>
            {
                new() { Id=5, Title="Activity 1 – Hello World Program",  FileType="TASK", FileName="act1_hello_world.pdf",    UploadedOn=DateTime.Today.AddDays(-12), Category="Activity"   },
                new() { Id=6, Title="Task Performance – Final Project",  FileType="TASK", FileName="task_perf_final.pdf",     UploadedOn=DateTime.Today.AddDays(-3),  Category="Activity"   },
            };

            return new TeacherCoursesViewModel
            {
                Teacher = teacher,
                AdvisingClasses = classSummaries,
                ActiveClassId = activeClass?.SchoolClass?.Id,
                ActiveClass = activeClass,
                Modules = sampleModules,
                Assessments = sampleAssessments,
                Activities = sampleActivities,
            };
        }

        private async Task<DashboardViewModel> BuildAdminViewModel()
        {
            var allStudents = await _studentRepository.GetAllWithIncludesAsync();
            var allTeachers = await _teacherRepository.GetAllAsync();
            var allCourses = await _courseRepository.GetAllWithDetailsAsync();
            var allClasses = await _classRepository.GetAllAsync();
            var allSubjects = await _subjectRepository.GetAllSubjectsAsync();
            var activeAlerts = await _alertRepository.GetActiveAlertsAsync();
            var allAttendances = _attendanceRepository.GetAll().ToList();

            int totalAtt = allAttendances.Count;
            int presentAtt = allAttendances.Count(a => a.Status == "Present");
            double rate = totalAtt > 0 ? Math.Round((double)presentAtt / totalAtt * 100, 1) : 0;

            var recentStudents = allStudents
                .Where(s => s.EnrollmentDate.HasValue)
                .OrderByDescending(s => s.EnrollmentDate)
                .Take(5);

            return new DashboardViewModel
            {
                TotalStudents = allStudents.Count(),
                ActiveStudents = allStudents.Count(s => s.Status == Data.Entities.StudentStatus.Active),
                TotalTeachers = allTeachers.Count(),
                TotalCourses = allCourses.Count,
                TotalClasses = allClasses.Count,
                TotalSubjects = allSubjects.Count,
                UnresolvedAlerts = activeAlerts.Count,
                AttendanceRate = rate,
                RecentStudents = recentStudents,
                RecentAlerts = activeAlerts.Take(5),
            };
        }
    }
}