# 🏘️ Local Scout

**Local Scout** is a web application designed to connect local users with service providers in their neighborhood. The platform allows users to discover, search, and view profiles of service providers, while also giving providers a comprehensive dashboard to manage their services and business.

---

## 🔖 Tech Stack & Tools

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)
![Google Firestore](https://img.shields.io/badge/Google%20Firestore-F6820D?style=for-the-badge&logo=firebase&logoColor=white)
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=cloudinary&logoColor=white)
![LocationIQ](https://img.shields.io/badge/LocationIQ-Geocoding-4871E8?style=for-the-badge)
![Stripe](https://img.shields.io/badge/Stripe-626CD9?style=for-the-badge&logo=stripe&logoColor=white)

---

## 🚀 Features Implemented (As of September 5, 2025)

### ✅ Administrator Panel
* **Secure Dashboard**: Access protected via ASP.NET Core Identity roles.
* **Service Category Management**: Full CRUD functionality for service categories, including image uploads, priority ordering, and the ability to toggle a category's active/inactive status.
* **Category Request Workflow**: Admins can review, approve, or deny new category suggestions submitted by providers.
* **User Management**: A comprehensive dashboard with a tabbed interface to view and manage Service Providers and Regular Users separately. Admins can view user details and perform secure deletions that cascade to remove all associated provider data (services, images, etc.).

### ✅ Service Provider Portal
* **Provider Dashboard**: A central hub displaying the provider's business profile.
* **Profile Management**: Full CRUD functionality for a provider's business profile, including business name, address, hours, description, and profile picture.
* **Complete Service Management**: Full CRUD for service listings.
    * **Image Gallery**: Support for multiple image uploads and deletion of specific images.
    * **Flexible Pricing**: Services can have a fixed price or be marked as "Negotiable," with the UI and validation logic adapting accordingly.
* **Interactive Notifications**: An automated toast notification system alerts providers when their category requests are approved or denied.

### ✅ Public User Features
* **Role-Based Authentication**: A unified registration and login system that intelligently redirects users (Admin, Provider, User) to the correct dashboard.
* **Interactive Homepage**: A modern landing page featuring a prominent search bar and an auto-scrolling visual category browser.
* **Service Discovery**: Users can find services by searching, browsing categories, or viewing featured services on the homepage.
* **Detailed Service Pages**: Each service has a full detail page featuring an **image gallery carousel**, description, provider information, and a list of other services from the same provider.
* **Modern UI/UX**:
    * **Custom Branding**: A professional and consistent look and feel with a custom color palette and typography.
    * **Enhanced Service Cards**: Services are displayed on modern cards showing the service image, provider details (name and picture), provider location, business hours, and join date.
    * **Toast Notifications**: A non-intrusive toast notification system provides feedback for successful actions.

---

## 📌 Future Roadmap

### 💳 PRIORITY 1: Booking & Payment System
* **Booking Workflow**: Implement a complete system for users to book services from providers.
* **Stripe Integration**: Integrate Stripe to handle payments for bookings securely.
* **Booking Management**: Add new sections to the User and Provider dashboards to view and manage upcoming and past bookings.

### ⭐ PRIORITY 2: Trust & Credibility Features
* **Ratings & Reviews**: Allow users to leave ratings and written reviews for services they have booked.
* **Provider Ratings**: Display average provider ratings on service cards and provider profiles.

### 📍 PRIORITY 3: Location-Based Discovery
* **Core Feature**: Replace the "Featured Services" section with "Your Nearest Services".
* **Geocoding Integration**: Integrate the **LocationIQ** API to convert user and provider addresses into latitude and longitude coordinates upon registration and profile updates.
* **Distance-Based Sorting**: All service lists (homepage, search results, category pages) will be sorted by proximity to the logged-in user, from nearest to farthest.
* **Data Model Update**: The `ApplicationUser` model will be updated to store coordinates.

---

## 👨‍💻 Author

**Abdullah Al Mamun**
Final-year CSE Undergraduate | Passionate about AI, ML & Software Development

---