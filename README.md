# 🏘️ Local Scout  

**Local Scout** is a web application designed to connect local users with service providers in their neighborhood.  
This platform allows users to discover, search, and view profiles of service providers, while also giving providers a comprehensive dashboard to manage their business and services.  

---

## 🔖 Tech Stack & Tools  
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)  
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)  
![Google Firestore](https://img.shields.io/badge/Google%20Firestore-F6820D?style=for-the-badge&logo=firebase&logoColor=white)  
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=cloudinary&logoColor=white)  
![Stripe](https://img.shields.io/badge/Stripe-626CD9?style=for-the-badge&logo=stripe&logoColor=white)  

---

## 🚀 Features Implemented (As of August 28, 2025)  
- ✅ **Administrator Panel**  
- ✅ **Role-Based Authentication**  
- ✅ **Custom User & Provider Dashboards**  
- ✅ **Service Management with Image Gallery**  
- ✅ **Cloudinary Image Handling**  
- ✅ **Public Search & Category Browsing**  
- ✅ **SQLite + Firestore Dual Database Architecture**  

---

### 1️⃣ Administrator Panel  
- **Role-Based Security**: Admin access protected via ASP.NET Core Identity Roles.  
- **Service Category Management**: Full CRUD with image uploads, soft/hard deletes, and smart sorting.  
- **Category Request Management**: Admins can review and approve/deny provider requests for new categories, with an automated notification system.  
- **User Management**: Tabbed interface to separate Users and Providers, with detailed profile views and secure delete functionality.  

---

### 2️⃣ Core Authentication & User Experience  
- **Built with ASP.NET Core Identity**.  
- **Custom User Profiles**: Extended with fields for personal and business details.  
- **Unified Registration & Login**: Single streamlined flow with role-based redirects and modern toast notifications for user feedback.  
- **Dynamic UI**: Layout adapts to user role (Admin, User, Provider).  

---

### 3️⃣ User & Service Provider Dashboards  
- **Secure User Dashboard**: Manage personal profile (CRUD).  
- **Provider Upgrade Path**: Seamless flow for users to become service providers.  
- **Dedicated Provider Dashboard**:  
  - Profile Management (CRUD): Edit and manage business details.  
  - Service Management (CRUD): Create, update, delete services with a multi-image gallery and image removal functionality.  

---

### 4️⃣ Public-Facing Features  
- **Interactive Homepage**: Prominent search bar and a visually engaging, auto-scrolling carousel of service categories.  
- **Search & Category Functionality**: Fully functional search bar and category cards leading to results pages displaying matching services.  

---

### 5️⃣ Image & Media Management  
- **Cloudinary Integration** for all image handling.  
- **Smart Crop & Fit**: Purpose-built resizing strategies for profile pictures and gallery images.  
- **Image Deletion**: Removes images from both the database and Cloudinary.  

---

### 6️⃣ Project Foundation & Architecture  
- **Framework**: ASP.NET Core 8  
- **Databases**: SQLite (Identity) & Google Firestore (Application Data)  

---

## 📌 Future Roadmap (Next Steps)  

### 👥 User-Facing Features (Next Up)  
- **Enhance Service Cards**:  
  - Add New Data: Include business hours, provider’s location, and joined date.  
  - Redesign UI: More modern and informative card layout.  
- **Public Profiles**: Full provider details with all their services.  
- **"Load More" Functionality**: Dynamic service fetching using AJAX.  

### 💳 Payments  
- **Stripe Integration**: Implement “Hire Me” button for direct bookings or provider subscriptions.  

### ⭐ Trust & Credibility  
- **Ratings & Reviews**: Users can rate and review providers.  

---

## 👨‍💻 Author  
**Abdullah Al Mamun**  
