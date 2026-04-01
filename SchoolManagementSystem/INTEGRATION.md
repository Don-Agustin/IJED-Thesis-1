# Student Dashboard — Integration Guide

## Files delivered

| Delivered file | Drop into project at |
|---|---|
| `_StudentDashboard.cshtml` | `Views/Shared/_StudentDashboard.cshtml` |
| `StudentDashboard.cshtml` | `Views/Dashboard/StudentDashboard.cshtml` *(replaces existing)* |
| `student-sidebar.css` | `wwwroot/dashboard/css/student-sidebar.css` |
| `student-sidebar.js` | `wwwroot/dashboard/js/student-sidebar.js` |

---

## What was added

### Sidebar
- **Profile header** — avatar photo (or initials fallback), full name, Student ID.  
  All three values are read from `ViewBag` in `_StudentDashboard.cshtml` so the controller is the single source of truth.
- **Collapse / expand** — chevron `«»` button in the top-right corner of the sidebar.  
  State is persisted to `localStorage` (`ijed_student_sidebar_collapsed`).
- **Dark maroon gradient** — uses existing `--maroon-gradient` CSS variable from `theme-ijed.css`.  
  Active link = white left-border + semi-transparent fill (matches the mockup).
- **Menu items** — Profile · Home · Calendar · Courses · Catalog · Guardians · Help · Logout (bottom-pinned).

### Top navigation bar
- **Page title** — driven by `ViewBag.PageTitle` (e.g. `"Calendar"`); just set it in each action.
- **Search bar** — centered, rounded pill, focuses with maroon outline.
- **Inbox icon** — shows red badge when `ViewBag.InboxUnreadCount > 0`.
- **Notifications icon** — shows red badge when `ViewBag.NotifUnreadCount > 0`.
- **Profile avatar dropdown** — photo or initials, shows name + student ID in header, links to Profile / Change Password / Logout.

---

## Controller changes needed

In `DashboardController.StudentDashboard()` add these ViewBag assignments
after the student record is fetched:

```csharp
ViewBag.StudentFullName  = student?.FullName ?? User.Identity?.Name;
ViewBag.StudentId        = student?.StudentNumber ?? "—";
ViewBag.StudentAvatarUrl = student?.ProfilePhotoUrl;   // null → initials fallback

// DB-READY placeholders — wire up real services when available:
ViewBag.InboxUnreadCount = 0;   // await _inboxService.GetUnreadCountAsync(userId);
ViewBag.NotifUnreadCount = 0;   // await _notifService.GetUnreadCountAsync(userId);
```

---

## Marking the active nav item

Set `ViewBag.ActivePage` in every student-facing action to highlight the correct sidebar link:

| Action | Set to |
|---|---|
| `StudentDashboard` | `"Home"` |
| `ChangeUser` (profile) | `"Profile"` |
| Calendar index | `"Calendar"` |
| Courses index | `"Courses"` |
| Catalog index | `"Catalog"` |
| Guardians index | `"Guardians"` |
| Help index | `"Help"` |

---

## Stub controllers for new menu items

Calendar, Catalog, Guardians, and Help don't exist yet.
Until they're created the links fall back to `"#"` (no 404).
Create minimal controllers as needed:

```csharp
[Authorize(Roles = "Student")]
public class CalendarController : Controller
{
    public IActionResult Index()
    {
        ViewBag.PageTitle = "Calendar";
        ViewBag.ActivePage = "Calendar";
        // TODO: fetch events
        return View();
    }
}
```

---

## Dark mode note

`theme-ijed.css` already has a `@media (prefers-color-scheme: dark)` block.
The student sidebar uses semi-transparent whites and the `--maroon-gradient` variable,
so it adapts automatically when you extend the dark-mode overrides there.
