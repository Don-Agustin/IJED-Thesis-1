using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Models
{
    /// <summary>
    /// Populates the Admin / Employee / Teacher / Student dashboard views
    /// with aggregate statistics and recent activity data.
    /// </summary>
    public class DashboardViewModel
    {
        // ── Stat cards ───────────────────────────────────────────────
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalClasses { get; set; }
        public int TotalSubjects { get; set; }
        public int UnresolvedAlerts { get; set; }

        /// <summary>Overall attendance rate across all students (0–100).</summary>
        public double AttendanceRate { get; set; }

        // ── Recent activity ──────────────────────────────────────────
        /// <summary>5 most recently enrolled students.</summary>
        public IEnumerable<Student> RecentStudents { get; set; } = Enumerable.Empty<Student>();

        /// <summary>5 most recent unresolved alerts (Admin/Employee only).</summary>
        public IEnumerable<Alert> RecentAlerts { get; set; } = Enumerable.Empty<Alert>();

        // ── Per-role context (Teacher dashboard) ─────────────────────
        public int MyClasses { get; set; }
        public int MyStudents { get; set; }
        public string WelcomeName { get; set; } = string.Empty;
    }
}