# 🏘️ Local Scout

**Local Scout** is a web application designed to connect local users with service providers in their neighborhood. The platform allows users to discover, search, and view profiles of service providers, while also giving providers a comprehensive dashboard to manage their services and business.

---

## 🔖 Tech Stack & Tools

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)
![Google Firestore](https://img.shields.io/badge/Google%20Firestore-F6820D?style=for-the-badge&logo=firebase&logoColor=white)
![Cloudinary](https://img.shields.io/badge/Cloudinary-3448C5?style=for-the-badge&logo=cloudinary&logoColor=white)
![SSLCommerz](https://img.shields.io/badge/SSLCommerz-Payment-2E3192?style=for-the-badge)
![LocationIQ](https://img.shields.io/badge/LocationIQ-Geocoding-4871E8?style=for-the-badge)

---

## 🚀 Core Features & Functionality (As of September 10, 2025)

### ✅ Booking & Payment System
- **Payment Gateway Integration**: Fully integrated with **SSLCommerz**, a leading payment gateway in Bangladesh, for all transactions.
- **Complete Payment Flow**: Users can pay for approved services via a secure, hosted checkout page (EasyCheckout) supporting cards, mobile banking (bKash, Nagad), and internet banking.
- **Secure Confirmation (IPN)**: Implemented a secure webhook (IPN) listener with server-side transaction validation to automatically update a booking's status to "Confirmed" in the database upon successful payment.
- **Negotiable Price Workflow**: Added a complete quote-and-approve system. Providers can approve negotiable bookings with a final, custom price, which is then presented to the customer for payment.

### ✅ Unified Experience & Core Infrastructure
- **Consistent UI/UX**: A professional, fixed-sidebar dashboard layout has been implemented for all user roles with refined headers and spacing.
- **Active Sidebar Navigation**: All dashboards now feature active link highlighting, clearly showing the user which page they are currently on for improved usability.
- **Role-Based Navigation & Redirects**: Login and post-action redirects are correctly handled for all roles and scenarios, including `returnUrl` functionality.
- **Full Timezone Conversion**: A site-wide timezone service ensures all data is stored as UTC and all displayed timestamps are correctly converted to GMT+6 (Dhaka Standard Time).

### ✅ Service Provider Portal
- **Unified Master Profile**: A single, master profile page that serves as both the public-facing profile (for customers) and the private dashboard homepage (for the owner), with content dynamically adjusted based on the viewer.
- **Dynamic Layouts**: The provider profile page intelligently loads the correct layout (public, user dashboard, admin dashboard, or full provider dashboard) based on the viewer's role.
- **Public Trust Stats**: The public profile displays key "Trust Stats" (Total Jobs, Completed, etc.) to build customer confidence.
- **Redesigned Management Pages**: All provider-facing management pages (e.g., "My Services") have been redesigned for a modern, consistent, and user-friendly experience, featuring thumbnails and modal-based confirmations.

### ✅ User Dashboard & Public Pages
- **Full Site Integration**: All public service cards and details pages now correctly link to the provider's new master profile page.
- **Interactive Homepage & Search**: A modern landing page and AJAX-powered "Load More" functionality for service discovery.
- **Profile Management**: Users have a dedicated "My Profile" page which now serves as their default dashboard homepage.

---

## 📌 Future Roadmap

### 💳 PRIORITY 1: Finalize Booking & Payment System
- **Implement Customer Actions**: Wire up the **"Cancel Booking"** functionality for users.
- **Complete the Lifecycle**: Add functionality for providers to mark a confirmed booking as **"Completed"**.
- **Transaction History**: Build pages for users and providers to view their detailed payment history.

### ⭐ PRIORITY 2: Trust & Credibility Features
- **Ratings & Reviews**: Allow users to leave ratings and written reviews for **completed** services.
- **Public User Profile**: Fully build out the `UserProfile` page that providers can view.

### 📍 PRIORITY 3: Location-Based Discovery
- **Geocoding Integration**: Integrate the **LocationIQ** API.
- **Distance-Based Sorting**: Sort all service lists by proximity.

---

## 👨‍💻 Author

**Abdullah Al Mamun**
Final-year CSE Undergraduate | Passionate about AI, ML & Software Development

---