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

## 🚀 Core Features & Functionality (As of September 7, 2025)

### ✅ Unified Experience & Core Infrastructure
- **Consistent UI/UX**: A professional, fixed-sidebar dashboard layout has been implemented for all user roles (Admin, Service Provider, and User) with refined headers and spacing.
- **Role-Based Navigation**: Login redirects are now correctly handled for all roles. Admins, Providers (to their new master dashboard), and Users (to their profile) are sent to the correct location. Logic also correctly handles post-login `returnUrl` redirects.
- **Full Timezone Conversion**: Implemented a site-wide timezone service. All data is correctly stored as UTC, and all displayed timestamps (booking dates, member since dates, etc.) are correctly converted to GMT+6 (Dhaka Standard Time) before display.

### ✅ Administrator Panel
- **Secure Dashboard**: Access protected via ASP.NET Core Identity roles.
- **Service Category Management**: Full CRUD functionality for service categories.
- **Category Request Workflow**: Admins can review, approve, or deny new category suggestions submitted by providers.
- **User Management**: A comprehensive dashboard to view and manage all Service Providers and Regular Users.

### ✅ Service Provider Portal
- **Unified Master Profile**: Replaced the static dashboard index with a master profile page (`/ServiceProvider/Index/{id}`) that serves as **both** the public-facing profile *and* the private dashboard homepage.
- **Dynamic Layouts**: The provider profile page intelligently loads the correct layout: the public layout (`_Layout`) for anonymous users, the dashboard layout (`_DashboardLayout`) for logged-in customers, and the full provider sidebar (`_ProviderLayout`) only for the owner.
- **Public Trust Stats**: The master profile page publicly displays key "Trust Stats" (Total Jobs, Completed, Ongoing, etc.) to build customer confidence.
- **Redesigned 'My Services' Page**: Replaced the original basic list with a redesigned, modern table that includes service thumbnails and a secure, modal-based confirmation for deleting services.
- **Booking Management**: Providers can view all incoming booking requests, approve, or reject them.

### ✅ User Dashboard & Public Pages
- **Full Site Integration**: All public service cards (on the homepage, search results, and category pages) and the service details page now correctly link to the provider's new master profile page.
- **Interactive Homepage**: A modern landing page featuring a search bar and an auto-scrolling visual category browser.
- **AJAX-Powered "Load More"**: Service lists load dynamically without a full page refresh.
- **Profile Management**: Users have a dedicated "My Profile" page (`/Dashboard/MyProfile`) which now serves as their default dashboard homepage.
- **Booking Workflow**: Users can request services and view the status on their "My Bookings" page.
- **Bug Fix (Negotiable Price)**: Corrected a display bug where "TK 0" was incorrectly showing for negotiable services in the user's booking list.

---

## 📌 Current issue
- **Provider profile from service details page**: When tries to visit providers profile through service details page, shows sidebar also.

## 📌 Future Roadmap

(Our existing roadmap remains the priority.)

### 💳 PRIORITY 1: Complete Booking & Payment System
- **Implement Customer Actions**: Wire up the "Cancel Booking" functionality for users.
- **Integrate Stripe**: Implement the "Pay Now" feature for approved bookings using Stripe Checkout.
- **Complete the Lifecycle**: Add functionality for providers to mark a confirmed booking as "Completed".
- **Payment Confirmation**: Create success/failure pages for payment redirects and update booking status via Stripe Webhooks.

### ⭐ PRIORITY 2: Trust & Credibility Features
- **Ratings & Reviews**: Allow users to leave ratings and written reviews for services they have booked (this will be added to the master provider profile page).
- **Public User Profile**: Fully build out the `UserProfile` page that providers can view (we have already built the stats section).

### 📍 PRIORITY 3: Location-Based Discovery
- **Geocoding Integration**: Integrate the **LocationIQ** API.
- **Distance-Based Sorting**: Sort all service lists by proximity.

---

## 👨‍💻 Author

**Abdullah Al Mamun**
Final-year CSE Undergraduate | Passionate about AI, ML & Software Development

---