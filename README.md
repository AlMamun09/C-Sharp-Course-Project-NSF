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

## 🚀 Core Features & Functionality (As of September 6, 2025)

### ✅ Unified Dashboard Experience
- **Consistent UI/UX**: A professional, fixed-sidebar dashboard layout has been implemented for all user roles (Admin, Service Provider, and User), creating a cohesive and application-like feel.
- **Role-Based Navigation**: Each dashboard sidebar is tailored with links and actions specific to the user's role.

### ✅ Administrator Panel
- **Secure Dashboard**: Access protected via ASP.NET Core Identity roles.
- **Service Category Management**: Full CRUD functionality for service categories, including image uploads, priority ordering, and toggling active/inactive status.
- **Category Request Workflow**: Admins can review, approve, or deny new category suggestions submitted by providers, triggering automated notifications.
- **User Management**: A comprehensive dashboard with a tabbed interface to view and manage Service Providers and Regular Users separately.

### ✅ Service Provider Portal
- **Profile Management**: Full CRUD functionality for a provider's business profile.
- **Complete Service Management**: Full CRUD for service listings, including:
    - **Multi-Image Gallery**: Providers can upload multiple images and delete specific ones.
    - **Flexible Pricing**: Services can have a fixed price or be marked as "Negotiable".
- **Booking Management**: Providers can view all incoming booking requests on their dashboard, see customer details (including profile picture), and **Approve** or **Reject** requests.

### ✅ User Dashboard & Public Pages
- **Interactive Homepage**: A modern landing page featuring a search bar and an auto-scrolling visual category browser.
- **AJAX-Powered "Load More"**: Service lists on the homepage, search results, and category pages now load dynamically without a full page refresh.
- **Booking Workflow**: Users can request a service using a user-friendly date/time picker, see the status of their requests ("Pending Approval", "Approved", etc.) on their "My Bookings" page, and receive notifications.
- **Profile Management**: Users have a dedicated "My Profile" page to view their details and access actions like editing their profile or upgrading to a provider account.

---

## 📌 Future Roadmap

### 💳 PRIORITY 1: Complete Booking & Payment System
- **Implement Customer Actions**: Wire up the "Cancel Booking" functionality for users.
- **Integrate Stripe**: Implement the "Pay Now" feature for approved bookings using Stripe Checkout.
- **Complete the Lifecycle**: Add functionality for providers to mark a confirmed booking as "Completed".
- **Payment Confirmation**: Create success/failure pages for payment redirects and update booking status via Stripe Webhooks.

### ⭐ PRIORITY 2: Trust & Credibility Features
- **Ratings & Reviews**: Allow users to leave ratings and written reviews for services they have booked.
- **Public User Profile**: Build the public-facing user profile page that providers can view, including the user's booking statistics (total requested, confirmed, etc.).

### 📍 PRIORITY 3: Location-Based Discovery
- **Geocoding Integration**: Integrate the **LocationIQ** API to convert user and provider addresses into latitude and longitude coordinates.
- **Distance-Based Sorting**: All service lists will be sorted by proximity to the logged-in user.

---

## 👨‍💻 Author

**Abdullah Al Mamun**
Final-year CSE Undergraduate | Passionate about AI, ML & Software Development

---