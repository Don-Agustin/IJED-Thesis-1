using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;

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

        public async Task<IActionResult> Index()
        {
            return View();
        }

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
            var vm = await BuildAdminViewModel();   // same data set, same view shape
            vm.WelcomeName = User.Identity?.Name?.Split('@')[0] ?? "Employee";
            return View(vm);
        }

        // ── Teacher Dashboard ────────────────────────────────────────────
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherDashboard()
        {
            // For teachers we show lighter stats — no admin totals
            var teacher = await _teacherRepository.GetTeacherByUserIdAsync(
                _studentRepository.GetAll()   // just to get user context — User.FindFirstValue below
                    .FirstOrDefault()?.UserId ?? "");

            var allStudents = await _studentRepository.GetAllWithIncludesAsync();
            var allClasses = await _classRepository.GetAllAsync();

            var vm = new DashboardViewModel
            {
                TotalStudents = allStudents.Count(),
                ActiveStudents = allStudents.Count(s =>
                    s.Status == Data.Entities.StudentStatus.Active),
                TotalClasses = allClasses.Count,
                WelcomeName = User.Identity?.Name?.Split('@')[0] ?? "Teacher",
                RecentAlerts = (await _alertRepository.GetActiveAlertsAsync()).Take(5),
            };
            return View(vm);
        }

        // ── Student Dashboard ────────────────────────────────────────────
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            // Student sees their own attendance summary
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
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

        // ── Shared helper ────────────────────────────────────────────────
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

            // 5 most recently enrolled students
            var recentStudents = allStudents
                .Where(s => s.EnrollmentDate.HasValue)
                .OrderByDescending(s => s.EnrollmentDate)
                .Take(5);

            return new DashboardViewModel
            {
                TotalStudents = allStudents.Count(),
                ActiveStudents = allStudents.Count(s =>
                    s.Status == Data.Entities.StudentStatus.Active),
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