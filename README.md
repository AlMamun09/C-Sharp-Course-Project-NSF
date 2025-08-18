# 🏘️ Neighborhood Service Finder

**Neighborhood Service Finder** is a web application designed to connect local users with service providers in their neighborhood. This platform allows users to discover, search, and view profiles of service providers, while also giving providers a dashboard to manage their business and services.

---

## 🚀 Features Implemented (As of August 19, 2025)

This project has a solid foundation with a secure administrator panel, a fully functional authentication system with a polished user experience, integrated image handling, and the core user dashboard functionality.

### 1️⃣ Administrator Panel
A secure admin area for managing the application's core data.

- **Role-Based Security:** Secured with ASP.NET Core Identity Roles. Only logged-in users with the Admin role can access admin functionality.  
- **Service Category Management:** Full CRUD (Create, Read, Update, Delete) with soft-delete, hard-delete confirmation, and smart sorting.  
- **User Management:**  
  - Tabbed Interface to cleanly separate "Regular Users" and "Service Providers".  
  - Admin's own account is hidden from the list to prevent accidental self-deletion.  
  - View detailed profiles for each user, including business information for providers.  
  - Permanently delete users from the system.  
  - **Stateful UI:** The page correctly remembers the active tab after an admin performs an action (like deleting a user or viewing details).  

### 2️⃣ Core Authentication & User Experience
A robust and secure authentication system built with ASP.NET Core Identity.

- **Custom User Profiles:** Extended with fields for personal details (FirstName, Address, optional ProfilePictureUrl) and business details (BusinessName, BusinessAddress, etc.).  
- **Unified Registration:** A single, streamlined registration process for all users, with optional profile picture upload.  
- **Login & Logout:** Fully functional and secure authentication.  
- **Role-Based Redirects:** After logging in, users are intelligently redirected: Admins go to the admin panel, and all other users go to their personal dashboard.  
- **User Feedback:** Success messages are displayed after key actions (like registration and upgrading to a provider) to improve user experience.  
- **Dynamic UI:** The layout adapts to the authentication state, showing the correct links (e.g., "Dashboard" or "Admin") based on the user's role.  

### 3️⃣ User & Service Provider Dashboards
- **Secure User Dashboard:** A dedicated, private area for all logged-in users.  
- **Provider Upgrade Path:** A seamless flow for regular users to become a service provider. From their dashboard, they can fill out a detailed form with their business name, address, description, and profile picture to upgrade their account and gain the "ServiceProvider" role.  
- **Dedicated Provider Dashboard:** A separate, secure dashboard for users with the "ServiceProvider" role, ready for future management features.  

### 4️⃣ Image & Media Management
Integrated with Cloudinary for all image handling.

- **Smart Crop (fill, gravity:auto):** Creates perfectly square, subject-focused profile pictures.  
- **Fit Inside (fit):** Resizes gallery images while keeping the entire image visible.  

### 5️⃣ Project Foundation & Architecture
- **Framework:** ASP.NET Core 8  
- **Dual-Database Architecture:**  
  - SQLite: Local SQL database for Identity (users, roles, passwords).  
  - Google Firestore: Cloud-based NoSQL database for application data (service categories, etc.).  

---

## 📌 Future Roadmap (Next Steps)

With the core foundation and user upgrade path complete, the next phases will focus on building out the Service Provider's tools and the main public-facing features.

### 🧑‍🔧 Service Provider Module (Next Up)
- **Profile Management (CRUD):** On their new dashboard, allow providers to view and edit the business information they submitted.  
- **Service Management:** The core feature for providers. Allow them to add, edit, and delete the specific services they offer (e.g., a plumber adding "Drain Unblocking" and "Pipe Repair"). This data will be stored in Firestore.  

### 👥 User-Facing Features
- Public homepage with a sorted list of service categories.  
- Search & Filter functionality to find providers by service type and location.  
- Public provider profile pages to display all business details and an image gallery of their work.  

### 💳 Payments
- **Stripe Integration:** For handling payments, such as user payments for bookings or provider subscriptions for premium plans.  

---

## 🛠️ Tech Stack
- **Backend:** ASP.NET Core 8  
- **Databases:** SQLite (Identity), Google Firestore (Application Data)  
- **Authentication:** ASP.NET Core Identity  
- **Image Management:** Cloudinary  
- **Future Integrations:** Stripe (Payments)  

---

## 👨‍💻 Author
**Abdullah Al Mamun**
