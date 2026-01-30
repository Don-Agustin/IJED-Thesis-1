# School Management System 🎓

## About the Project
The **School Management System** is a web application developed using **ASP.NET Core MVC**. It provides an efficient and secure platform for managing students, teachers, courses, grades, and attendance within a school environment.

## Features 🚀
- 🔐 **Authentication System:** Login, logout, registration, and password recovery via email.  
- ✉️ **Temporary Password:** Upon registration, a temporary password with random characters is sent to the user's email.  
- 👥 **Role Management:** Four user roles—Admin, Staff, Student, and Anonymous.  
- 🏫 **Course and Subject Management:** Full CRUD operations for courses and subjects.  
- 👨‍🏫 **Teacher Management:** Complete CRUD operations for managing teachers.  
- 📝 **Student and Grades Management:** Manage students, classes, grades, and attendance.  
- 📊 **Admin Dashboard:** Alerts and system notifications from staff members.  
- 🖼️ **Profile Pictures:** Required for students and optional for other users.  
- 🌐 **Public API:** Provides data of all students in a specific class.  
- ❌ **Custom Error Handling:** Friendly error pages and complete CRUD error management.  
- 🎨 **Responsive UI:** Custom and original front-end design.

## User Roles and Permissions 🔑
| Feature                             | Admin | Staff | Teacher | Student | Anonymous |
|-------------------------------------|:-----:|:-----:|:-------:|:-------:|:---------:|
| Login/Logout                        |   ✅   |  ✅   |   ✅    |   ✅    |     ❌     |
| Create Accounts                     |   ✅   |  ❌   |   ❌    |   ❌    |     ❌     |
| CRUD Courses                        |   ✅   |  ✅   |   ❌    |   ❌    |     ❌     |
| CRUD Subjects                       |   ✅   |  ✅   |   ❌    |   ❌    |     ❌     |
| CRUD Teachers                       |   ✅   |  ✅   |   ❌    |   ❌    |     ❌     |
| CRUD Students and Grades            |   ✅   |  ✅   |   ❌    |   ❌    |     ❌     |
| View Courses and Subjects           |   ✅   |  ✅   |   ✅    |   ✅    |     ✅     |
| View Grades and Status              |   ❌   |  ❌   |   ❌    |   ✅    |     ❌     |
| Modify Profile                      |   ✅   |  ✅   |   ✅    |   ✅    |     ❌     |
| Attendance Tracking and Management  |   ✅   |  ✅   |   ✅    |   ✅    |     ❌     |

## Technologies Used 🛠️
- **Framework:** ASP.NET Core MVC  
- **Database:** SQL Server with Entity Framework Core  
- **Architecture:** Repository Pattern  
- **Authentication:** Identity Framework with Role Management  
- **Frontend:** Bootstrap, Syncfusion Controls  
- **API:** RESTful Web API

## Visual Demonstration 🌟
### 🏠 Home Page

![2 (1)](https://github.com/user-attachments/assets/abfd7036-e579-4144-bdc5-b8f8037af9cd)
![2 (2)](https://github.com/user-attachments/assets/9773009c-39d0-4fba-a00a-fd4b78573daf)
![2 (3)](https://github.com/user-attachments/assets/4afd5bcb-2895-40c6-b6b8-891a64da2620)
![2 (4)](https://github.com/user-attachments/assets/9f394466-e25d-4df1-9284-20bf6716d556)
![2 (5)](https://github.com/user-attachments/assets/81169cab-16f7-4ab9-a3ee-a3fe543687eb)
![2 (6)](https://github.com/user-attachments/assets/ea7bca6d-6589-4644-8a05-409e37f72afa)
![2 (7)](https://github.com/user-attachments/assets/0c6c9de8-fe85-4d2a-ba88-eac0675585c0)
![2 (8)](https://github.com/user-attachments/assets/412f976b-1aa8-4e6e-8583-b939b8205a53)
![2 (9)](https://github.com/user-attachments/assets/ee6f44ee-c507-47b0-82f3-75edb4f863fa)


### 🔑 Login Page

![Captura de ecrã 2025-01-10 172203](https://github.com/user-attachments/assets/9c77f797-8972-44ee-955c-1680018d6de7)

### 📝 Registration Page

![Captura de ecrã 2025-01-10 172257](https://github.com/user-attachments/assets/7834e368-7649-4f35-96b0-92b9d7e17524)

### 📊 Admin Dashboard

![Captura de ecrã 2025-01-10 172421](https://github.com/user-attachments/assets/ec8d032c-4938-4ab7-89b5-48aef5bfb308)

### 📚 Courses List

![Captura de ecrã 2025-01-10 172546](https://github.com/user-attachments/assets/d099c1e3-c218-473c-a57a-d0c8ec550af6)

### ➕ Create Course

![Captura de ecrã 2025-01-10 172654](https://github.com/user-attachments/assets/2417c125-7cea-4d09-8028-687be9fafeba)

### 📖 Subjects List

![Captura de ecrã 2025-01-10 172727](https://github.com/user-attachments/assets/12dd4e46-ad99-4b86-aaae-849f45b2436b)

### 🏫 School Classes List

![Captura de ecrã 2025-01-10 172801](https://github.com/user-attachments/assets/a9c46d45-b6ef-47b9-b7a2-12411b999849)

### 📝 Grades List

![Captura de ecrã 2025-01-10 172825](https://github.com/user-attachments/assets/bf4d404b-6ceb-4171-a97e-42a4c6def012)

### 📄 Grade Details

![Captura de ecrã 2025-01-10 172843](https://github.com/user-attachments/assets/5e9fb94e-8bb8-4966-9589-7f9e860e2e9b)

### 📅 Attendance List

![Captura de ecrã 2025-01-10 172904](https://github.com/user-attachments/assets/0c9c3fa2-7491-4ac1-9e75-493aa39c8a7f)

### 🗒️ Attendance Details

![Captura de ecrã 2025-01-10 172925](https://github.com/user-attachments/assets/e3bc8db5-26e5-4bea-961a-8c4630dc1451)


## How to Run the Project 💻
1. Clone the repository:
   ```bash
   git clone git clone https://github.com/Don-Agustin/IJED-Thesis-1.git
"# IJED-Thesis-1" 
