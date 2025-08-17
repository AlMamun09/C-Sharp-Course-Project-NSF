# 🏘️ Neighborhood Service Finder

**Neighborhood Service Finder** is a web application designed to connect local users with service providers in their neighborhood.  
This platform allows users to **discover, search, and view profiles** of service providers, while also giving providers a **dashboard to manage their business and services**.

---

## 🚀 Features Implemented (As of August 18, 2025)

This project has a **solid foundation** with a secure **administrator panel**, a fully functional **authentication system**, and **image/media handling**.

### 1️⃣ Administrator Panel
A secure admin area for managing the application's core data.

- **Role-Based Security**:  
  Secured with ASP.NET Core Identity Roles. Only logged-in users with the `Admin` role can access admin functionality.

- **Service Category Management**:
  - Full CRUD (Create, Read, Update, Delete)
  - **Activate/Deactivate** for soft deletes  
  - **Hard Delete** with JavaScript confirmation  
  - **Smart Sorting** by status, priority, and alphabetically  

- **User Management**:
  - View all registered users  
  - Permanently delete users  

---

### 2️⃣ Core Authentication System
A robust and secure authentication system built with **ASP.NET Core Identity**.

- **Custom User Profiles**: Extended with fields like `FirstName`, `LastName`, and `Address`.  
- **Full Functionality**: Registration, Login, Logout.  
- **Dynamic UI**: Shows **Register/Login** when logged out, or **Name + Logout** when signed in.  

---

### 3️⃣ Image & Media Management
Integrated with **Cloudinary** for all image handling.

- **Smart Crop (fill, gravity:auto)** → Perfect square, subject-focused profile pictures  
- **Fit Inside (fit)** → Resize gallery images while keeping the entire image visible  

---

### 4️⃣ Project Foundation & Architecture
- **Framework**: ASP.NET Core 8  
- **Dual-Database Architecture**:
  - **SQLite** → Local SQL database for Identity (users, roles, passwords)  
  - **Google Firestore** → Cloud-based NoSQL database for service categories, provider profiles, etc.  

---

## 📌 Future Roadmap (Next Steps)

With the core foundation complete, the next phases will focus on **Service Provider features** and **User-facing functionality**.

### 🧑‍🔧 Service Provider Module (Next Up)
- Separate **registration process** for service providers  
- Provider dashboard with **profile management (CRUD)** and **profile picture upload**  
- **Service Management** → Add, edit, delete services offered  

### 👥 User-Facing Features
- Public homepage with **sorted list of service categories**  
- **Search & Filter** by service type & location  
- Public **provider profile pages** with details & image gallery  

### 💳 Payments
- **Stripe Integration** for:
  - User payments for bookings  
  - Provider subscriptions for premium plans  

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 8  
- **Databases**: SQLite (Identity), Google Firestore (Application Data)  
- **Authentication**: ASP.NET Core Identity  
- **Image Management**: Cloudinary  
- **Future Integrations**: Stripe (Payments)  

---

## 👨‍💻 Author
**Abdullah Al Mamun**  

