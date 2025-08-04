# CommentsApp

A full-stack .NET + Angular application with nested comments, CAPTCHA protection, file uploads, and SignalR updates.

## 🧰 Stack

- **Backend:** ASP.NET Core (.NET 9)
- **Frontend:** Angular 20 + Vite + Bootstrap
- **Database:** MS SQL Server
- **Real-time:** SignalR
- **Other:** CAPTCHA, Lightbox2, Docker

---

## 🚀 Features

- Submit and view nested comments
- CAPTCHA validation for spam protection
- Upload and preview `.txt` and image files (with lightbox for images)
- Frontend pagination, sorting, and validation
- Realtime notification for new comments via SignalR
- Automatic resizing for large images
- Sanitization of HTML content

---

## 🐳 Docker Setup

### Prerequisites

- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### Clone the repository

```bash
git clone https://github.com/your-username/CommentsApp.git
cd CommentsApp
```

### Start the application

```bash
docker-compose up --build
```

- Backend: [http://localhost:5000](http://localhost:5000)
- Frontend: [http://localhost:8080](http://localhost:8080)

📝 EF Core migrations are applied automatically when the backend container starts.

---

##🎥 \[Watch demo\](App%20functionality%20showcase.mp4)