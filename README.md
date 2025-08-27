# 🏘️ Local Scout: Your Neighbourhood Service Finder

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

## 🚀 Features Implemented (As of August 27, 2025)

✅ **Administrator Panel**  
✅ **Role-Based Authentication**  
✅ **Custom User & Provider Dashboards**  
✅ **Service Management with Image Gallery**  
✅ **Cloudinary Image Handling**  
✅ **Public Search Functionality**  
✅ **SQLite + Firestore Dual Database Architecture**

---

## 📂 Core Modules & Features

### 1️⃣ Administrator Panel
- Role-Based Security with ASP.NET Core Identity Roles  
- Service Category Management (CRUD + image uploads + soft/hard delete + sorting)  
- Category Request Management (approve/deny new requests)  
- User Management with safe admin controls and profile views  

### 2️⃣ Core Authentication & UX
- ASP.NET Core Identity with custom user profiles  
- Unified Registration & Login flow  
- Role-based UI with dynamic notifications  

### 3️⃣ Dashboards
- User Dashboard for profile management  
- Provider Upgrade Path for business profiles  
- Provider Dashboard with service + gallery management  

### 4️⃣ Public-Facing Features
- Interactive Homepage (search bar + category carousel)  
- Dedicated Search Results page  

### 5️⃣ Image & Media Management
- Cloudinary Integration (Smart Crop, Fit Inside, Auto Delete)  

### 6️⃣ Project Architecture
- Framework: **ASP.NET Core 8**  
- Databases:  
  - **SQLite** → Identity (users, roles, passwords)  
  - **Firestore** → App data (categories, services, notifications)  

---

## 📌 Future Roadmap

### User Features
- Clickable category cards → results page  
- "View Details" for service cards  
- Public Provider Profiles (full details + services)  
- AJAX "Load More Services" on homepage  

### Payments
- Stripe Integration for bookings + provider subscriptions  

---

## ⚙️ Installation & Setup

Follow these steps to run **Local Scout** locally:

### 1️⃣ Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)  
- [SQLite](https://www.sqlite.org/download.html) (pre-installed in most OS)  
- A [Google Cloud Project](https://console.cloud.google.com/) with Firestore enabled  
- [Cloudinary Account](https://cloudinary.com/) for media handling  
- [Stripe Account](https://stripe.com/) for payments  

### 2️⃣ Clone the Repository
```bash
git clone https://github.com/yourusername/localscout.git
cd localscout
````

### 3️⃣ Configure App Settings

Create or edit `appsettings.json` with your credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localscout.db"
  },
  "Firestore": {
    "ProjectId": "your-firestore-project-id",
    "CredentialsFile": "path-to-your-service-account.json"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Stripe": {
    "PublishableKey": "your-publishable-key",
    "SecretKey": "your-secret-key"
  }
}
```

### 4️⃣ Apply Migrations (for Identity in SQLite)

```bash
dotnet ef database update
```

### 5️⃣ Run the Project

```bash
dotnet run
```

The app will be available at:
👉 [https://localhost:5001](https://localhost:5001)

---

## 👨‍💻 Author

**Abdullah Al Mamun**

```
