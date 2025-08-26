# 🏘️ Neighborhood Service Finder

**Neighborhood Service Finder** is a web application designed to connect local users with service providers in their neighborhood.  
This platform allows users to **discover, search, and view service providers**, while also giving providers a **comprehensive dashboard** to manage their business and services.

---

## 🔖 Tech Stack & Tools
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)  
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)  
![Google Firestore](https://img.shields.io/badge/Google%20Firestore-F6820D?style=for-the-badge&logo=firebase&logoColor=white)  
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=cloudinary&logoColor=white)  
![Stripe](https://img.shields.io/badge/Stripe-626CD9?style=for-the-badge&logo=stripe&logoColor=white)  

---

## 🚀 Features Implemented (As of August 25, 2025)

✅ **Administrator Panel**  
✅ **Role-Based Authentication**  
✅ **Custom User & Provider Dashboards**  
✅ **Service Management with Image Gallery**  
✅ **Cloudinary Image Handling**  
✅ **SQLite + Firestore Dual Database Architecture**  

---

### 1️⃣ Administrator Panel
- **Role-Based Security:** Admin access protected via ASP.NET Core Identity Roles.  
- **Service Category Management:**
  - Full CRUD (Create, Read, Update, Delete) with image uploads.
  - *Activate/Deactivate* for soft deletes + *Hard Delete* with confirmation.
  - Smart sorting (by status, priority, and name).
- **Category Request Management:**
  - Admins can review and approve/deny category requests from providers.
  - Approvals create new categories instantly.
- **User Management:**
  - Tabbed interface separating "Regular Users" and "Service Providers".
  - Admin’s own account hidden to prevent accidental self-deletion.
  - Detailed profile view + permanent delete option.
  - Stateful UI: remembers active tab after an action.

### 2️⃣ Core Authentication & User Experience
- Built with **ASP.NET Core Identity**.  
- **Custom User Profiles:** Includes optional profile picture + business details.  
- **Unified Registration & Login:** Single streamlined flow with role-based redirects.  
- **Dynamic UI & Notifications:**
  - Layout adapts to user role (Admin, User, Provider).
  - Modern **toast notifications** for registration, requests, approvals, etc.

### 3️⃣ User & Service Provider Dashboards
- **Secure User Dashboard:** Manage personal profile (CRUD).  
- **Provider Upgrade Path:** Users can seamlessly become service providers by completing a business profile.  
- **Dedicated Provider Dashboard:**
  - **Profile Management (CRUD):** Edit and manage business details.  
  - **Service Management (CRUD):** Create, update, delete services with a **multi-image gallery**.  

### 4️⃣ Image & Media Management
- **Cloudinary Integration** for all image handling.  
- **Smart Crop:** Perfectly square, subject-focused profile pictures.  
- **Fit Inside:** Resizes gallery images while keeping full image visible.  
- **Image Deletion:** Removes images from both database + Cloudinary.  

### 5️⃣ Project Foundation & Architecture
- **Framework:** ASP.NET Core 8  
- **Databases:**
  - SQLite → Identity data (users, roles, passwords).
  - Google Firestore → Application data (categories, services, notifications).  

---

## 📌 Future Roadmap (Next Steps)

With the **admin and service provider modules complete**, the next phase is building the **public-facing user experience**.

### 👥 User-Facing Features
- **Homepage:**
  - Backend logic for main search bar.  
  - Interactive category cards linking to results.  
  - *Load More* functionality with AJAX.  
- **Search Results Page:** Display service cards matching search or category.  
- **Public Profiles:** Public-facing service provider profiles with service listings.  

### 💳 Payments
- **Stripe Integration:** Implement "Hire Me" button for:
  - Direct service bookings.  
  - Provider subscriptions.  

---

## 👨‍💻 Author
**Abdullah Al Mamun**  

---
