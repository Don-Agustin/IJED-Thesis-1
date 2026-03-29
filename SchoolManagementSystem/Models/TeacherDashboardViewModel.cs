using SchoolManagementSystem.Data.Entities;

namespace SchoolManagementSystem.Models
{
    // ─────────────────────────────────────────────────────────────────────────
    // Teacher Home Dashboard (Overview tab)
    // ─────────────────────────────────────────────────────────────────────────
    public class TeacherDashboardViewModel
    {
        public Teacher Teacher { get; set; }
        public string WelcomeName { get; set; } = string.Empty;

        // Assigned classes with their subjects
        public IEnumerable<TeacherClassSummary> AdvisingClasses { get; set; }
            = Enumerable.Empty<TeacherClassSummary>();

        // Today's schedule (static / seeded for now — DB-backed in next sprint)
        public IEnumerable<ScheduleItem> TodaySchedule { get; set; }
            = Enumerable.Empty<ScheduleItem>();

        // Upcoming activities (static for now)
        public IEnumerable<ActivityItem> UpcomingActivities { get; set; }
            = Enumerable.Empty<ActivityItem>();

        // Calendar events the teacher manages
        public IEnumerable<CalendarEvent> CalendarEvents { get; set; }
            = Enumerable.Empty<CalendarEvent>();
    }

    // Summary of one class + its subjects for a teacher
    public class TeacherClassSummary
    {
        public SchoolClass SchoolClass { get; set; }
        public IEnumerable<Subject> Subjects { get; set; } = Enumerable.Empty<Subject>();
        public int StudentCount { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Schedule / Activity items (in-memory until DB migration)
    // ─────────────────────────────────────────────────────────────────────────
    public class ScheduleItem
    {
        public string Time { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        /// <summary>green = class, amber = meeting, gray = free</summary>
        public string Color { get; set; } = "gray";
    }

    public class ActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-tasks";
        public bool IsDone { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Calendar events (teacher-managed: add / edit / delete in-memory for now)
    // ─────────────────────────────────────────────────────────────────────────
    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        /// <summary>maroon | green | amber | blue</summary>
        public string Color { get; set; } = "maroon";
        public string? Description { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Teacher Courses page
    // ─────────────────────────────────────────────────────────────────────────
    public class TeacherCoursesViewModel
    {
        public Teacher Teacher { get; set; }
        public IEnumerable<TeacherClassSummary> AdvisingClasses { get; set; }
            = Enumerable.Empty<TeacherClassSummary>();

        // Active class being viewed
        public int? ActiveClassId { get; set; }
        public TeacherClassSummary? ActiveClass { get; set; }

        // Materials for the active class, grouped by tab
        public IEnumerable<CourseMaterial> Modules { get; set; } = Enumerable.Empty<CourseMaterial>();
        public IEnumerable<CourseMaterial> Assessments { get; set; } = Enumerable.Empty<CourseMaterial>();
        public IEnumerable<CourseMaterial> Activities { get; set; } = Enumerable.Empty<CourseMaterial>();
    }

    // A single uploaded course material (PDF / PPT / DOC / etc.)
    public class CourseMaterial
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        /// <summary>PDF | PPT | DOC | QUIZ | EXAM | TASK</summary>
        public string FileType { get; set; } = "PDF";
        public string FileName { get; set; } = string.Empty;
        public string? FilePath { get; set; }
        public DateTime UploadedOn { get; set; }
        public int SchoolClassId { get; set; }
        /// <summary>Module | Assessment | Activity</summary>
        public string Category { get; set; } = "Module";
    }
}