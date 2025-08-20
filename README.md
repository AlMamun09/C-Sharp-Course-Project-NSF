# 🏘️ Neighborhood Service Finder

**Neighborhood Service Finder** is a web application designed to connect local users with service providers in their neighborhood.  
This platform allows users to **discover, search, and view profiles of service providers**, while also giving providers a **dashboard to manage their business and services**.  

---

## 🚀 Features Implemented (As of August 20, 2025)

This project has a solid foundation with:  
- A **secure administrator panel**  
- A **fully functional authentication system** with a polished user experience  
- Integrated **image handling**  
- Core **user/provider dashboard functionality**

---

### 1️⃣ Administrator Panel
- **Role-Based Security**: Secured with ASP.NET Core Identity Roles. Only admins can access.  
- **Service Category Management**: Full CRUD (Create, Read, Update, Delete) with soft-delete, hard-delete confirmation, and smart sorting.  
- **User Management**:  
  - Tabbed interface to separate "Regular Users" and "Service Providers"  
  - Admin’s own account is hidden from the list (prevent self-deletion)  
  - View detailed profiles (including provider business info)  
  - Permanently delete users  
  - Stateful UI: remembers active tab after admin actions  

---

### 2️⃣ Core Authentication & User Experience
- Built with **ASP.NET Core Identity**  
- **Custom User Profiles** with extended fields:  
  - Personal details: FirstName, Address, ProfilePictureUrl (optional)  
  - Business details: BusinessName, BusinessAddress, etc.  
- **Unified Registration**: One streamlined process for all users  
- **Login & Logout**: Fully functional and secure  
- **Role-Based Redirects**:  
  - Admins → Admin Panel  
  - Users → User Dashboard  
- **User Feedback**: Success messages after key actions  
- **Dynamic UI**: Shows correct links ("Dashboard", "Admin") based on role  

---

### 3️⃣ User & Service Provider Dashboards
- **Secure User Dashboard**: Personal area for all logged-in users  
- **Provider Upgrade Path**: Users can fill out a detailed form to become service providers  
- **Profile Management (CRUD)**: Both users and providers can edit profile info (with pre-filled forms & image updates)  
- **Dedicated Provider Dashboard**: A business hub for users with the "ServiceProvider" role  

---

### 4️⃣ Image & Media Management
- Integrated with **Cloudinary**  
- **Smart Crop (fill, gravity:auto)**: Perfect square, subject-focused profile pictures  
- **Fit Inside (fit)**: Resizes gallery images without cropping  

---

### 5️⃣ Project Foundation & Architecture
- **Framework**: ASP.NET Core 8  
- **Dual-Database Architecture**:  
  - SQLite → Identity (users, roles, passwords)  
  - Google Firestore → Application data (service categories, etc.)  

---

## 📌 Future Roadmap (Next Steps)

### 🧑‍🔧 Service Provider Module (Next Up)
- **Service Management** workflow:  
  - **Add New Service** → Choose from admin-approved categories, add details (description, price, gallery)  
  - **Request New Category** → Request unlisted service types (admin approval required)  
- **Admin Approval Page**: Review/approve/deny new category requests  
- **Provider Services CRUD**: Full control over services offered by providers  

---

### 👥 User-Facing Features
- Public **homepage** with sorted service categories  
- **Search & Filter** to find providers by service type and location  
- Public **provider profile pages** with business details, services offered, and gallery  

---

### 💳 Payments
- **Stripe Integration** (future):  
  - User payments for bookings  
  - Provider subscriptions for premium plans  

---

## 🛠️ Tech Stack
- **Backend**: ASP.NET Core 8  
- **Databases**: SQLite (Identity), Google Firestore (App Data)  
- **Authentication**: ASP.NET Core Identity  
- **Image Management**: Cloudinary  
- **Future Integration**: Stripe (Payments)  

---

## 👨‍💻 Author
**Abdullah Al Mamun**  

---
