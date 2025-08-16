# 🏘️ Neighborhood Service Finder

**Neighborhood Service Finder** is a web application designed to connect local users with service providers in their neighborhood. This platform allows users to **discover, search, and view profiles of various service providers**, while also giving providers a **comprehensive dashboard** to manage their business and services.

---

## 🚀 Features Implemented (As of August 16, 2025)

This project currently includes a solid foundation with administrator-level management of service categories and a fully functional core authentication system.

### 1️⃣ Administrator Panel – Service Category Management
A secure admin area for managing the types of services offered on the platform.  

**Full CRUD Functionality:**
- **Create** – Add new service categories with details like name, description, and display order.
- **Read** – View all service categories in an organized table.
- **Update** – Edit details of any existing category.
- **Delete**  
  - *Soft Delete*: Toggle "Activate/Deactivate" to hide a category from public view without losing data.  
  - *Hard Delete*: Permanently delete with a JavaScript confirmation prompt to avoid accidental data loss.

**Advanced Sorting**  
Categories are sorted by:
1. Status (active categories appear first).  
2. `priorityOrder` (custom order).  
3. Alphabetically by name (for tie-breaking).  

---

### 2️⃣ Core Authentication System
A robust and secure authentication system built with **ASP.NET Core Identity**.  

- **Custom User Profiles** – Extended user model with fields like `FirstName`, `LastName`, and `Address`.  
- **User Registration** – Multi-field registration with backend validation (duplicate email check, password match check, etc.).  
- **Login & Logout** – Fully functional authentication system.  
- **Dynamic UI** – Layout adapts to authentication state:  
  - Signed in → Shows user’s first name + Logout button.  
  - Signed out → Shows Register/Login links.  

---

### 3️⃣ Project Foundation & Architecture
- **Technology** – Built with **ASP.NET Core 8**.  
- **Dual Database Setup**:  
  - **SQLite** – Local SQL database (`app.db`) for secure Identity data (users, roles, passwords).  
  - **Google Firestore** – Cloud-based NoSQL database for main application data (service categories, provider profiles, etc.) to ensure scalability and flexibility.  

---

## 📌 Future Roadmap (Next Steps)

With the core foundation in place, the next phases will expand functionality for **service providers** and **end-users**.

### 🔐 Admin Enhancements
- Add role-based authorization to secure admin-only pages.  
- Build **User Management** (list users/providers, block/suspend, delete).  

### 🖼️ Media Management
- **Cloudinary Integration** for image uploads (profile photos, business galleries).  

### 🧑‍🔧 Service Provider Module
- Separate registration for service providers.  
- Provider dashboard with **business profile CRUD**.  
- **Service Management** – Add, edit, delete services (e.g., plumber adds "Drain Unblocking").  

### 👥 User-Facing Features
- Public homepage with sorted service categories.  
- **Search & Filter** providers by service type and location.  
- Public provider profile pages with details.  

### 💳 Payments
- **Stripe Integration** for handling payments:  
  - Users paying for bookings.  
  - Providers subscribing to premium plans.  

---

## 🛠️ Tech Stack
- **Backend:** ASP.NET Core 8  
- **Databases:** SQLite (Identity), Google Firestore (Application Data)  
- **Authentication:** ASP.NET Core Identity  
- **Future Integrations:** Cloudinary (Media), Stripe (Payments)  

---

## 📅 Project Status
✅ Core authentication complete  
✅ Admin service category management complete  
🔜 Service provider + user-facing modules under development  

---

## 👨‍💻 Author
**Abdullah Al Mamun** 

