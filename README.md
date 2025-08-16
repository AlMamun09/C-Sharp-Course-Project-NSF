Neighborhood Service Finder
A web application designed to connect local users with service providers in their neighborhood. This platform allows users to discover, search, and view profiles of various service providers, while providing a comprehensive dashboard for providers to manage their business and services.

Features Implemented (As of August 16, 2025)
This project has a solid foundation with a complete feature set for administrator-level management of service categories and a fully functional core authentication system.

1. Administrator Panel - Service Category Management
A secure area for administrators to manage the types of services offered on the platform. This feature includes:

Full CRUD Functionality:

Create: Add new service categories with details like name, description, and display order.

Read: View all service categories in a single, organized table.

Update: Edit the details of any existing category.

Delete:

Soft Delete: An "Activate/Deactivate" toggle allows an admin to hide a category from public view without permanently losing the data.

Hard Delete: A permanent delete option is available with a JavaScript confirmation dialog to prevent accidental data loss.

Advanced Sorting: The main category list is intelligently sorted to improve usability:

First by status (active categories appear at the top).

Then by the custom priorityOrder.

Finally, alphabetically by name to break any ties.

2. Core Authentication System
A robust and secure user authentication system built from the ground up using ASP.NET Core Identity.

Custom User Profiles: The standard user model has been extended to include fields critical to the application, such as FirstName, LastName, and Address.

User Registration: A complete, multi-field registration form with backend validation (e.g., checking for duplicate emails, ensuring passwords match).

User Login & Logout: Fully functional login and logout capabilities.

Dynamic UI: The main website layout is "authentication-aware," showing the user's first name and a "Logout" button when signed in, and "Register/Login" links when they are not.

3. Project Foundation & Architecture
Technology: Built on the modern and stable ASP.NET Core 8 framework.

Dual-Database Architecture:

SQLite: A simple, local SQL database (app.db) is used exclusively for the structured, secure data required by ASP.NET Core Identity (user accounts, passwords, roles).

Google Firestore: A flexible, cloud-based NoSQL database is used for all main application data (Service Categories, Provider Profiles, etc.), allowing for scalability and easy schema changes.

Future Roadmap (Next Steps)
With the core foundation and admin features in place, the next phases will focus on building out the functionality for Service Providers and end-users.

Secure the Admin Section: Add authorization rules to ensure only users with an "Administrator" role can access the admin pages.

Cloudinary Integration: Integrate the Cloudinary service to handle all image uploads, starting with profile pictures and business gallery images for service providers.

Service Provider Module:

Build the separate registration process for users who want to offer services.

Create the provider-specific dashboard where they can create and manage their public business profile (CRUD).

Implement "Service Management" to allow providers to add, edit, and delete the specific services they offer (e.g., a plumber adding "Drain Unblocking" and "Pipe Repair").

User-Facing Features:

Develop the public homepage to display the sorted list of service categories.

Implement search and filtering functionality so users can find providers by service type or location.

Create the public profile pages to display service provider details.

Stripe Integration:

Integrate the Stripe payment gateway.

Design and build the payment flow (e.g., users paying for a booking, or providers paying for a premium subscription).

Admin User Management:

Build the admin page to list all registered users and service providers.

Implement the "Block/Suspend" and "Delete User" functionalities that we've planned.