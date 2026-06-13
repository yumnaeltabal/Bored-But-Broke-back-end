# 🌍 Bored But Broke API

A scalable ASP.NET Core Web API built to power **Bored But Broke**, an application designed to help users discover affordable activities based on their location, the weather, personal preferences, age, and how far they're willing to travel.

Developed as a team project, the API integrates multiple third-party services to deliver personalised recommendations while providing secure user authentication and data management. The project focuses on clean architecture, maintainable code, and real-world API integration.

## ✨ Highlights

* Built RESTful API endpoints to support activity recommendations
* Integrated multiple external APIs to provide personalised results
* Implemented secure user authentication using cookies
* Stored user accounts with hashed passwords
* Developed a favourites system for saving activities
* Applied service-layer architecture and separation of concerns
* Organised activity categories through configurable JSON data
* Designed with scalability and maintainability in mind

## 🛠 Tech Stack

| Technology            | Purpose                        |
| --------------------- | ------------------------------ |
| C#                    | Core programming language      |
| ASP.NET Core Web API  | Backend framework              |
| Entity Framework Core | ORM and database access        |
| SQL Server            | Relational database            |
| Cookie Authentication | User authentication            |
| LINQ                  | Querying and data manipulation |
| OpenAPI / Swagger     | API documentation and testing  |
| JSON Configuration    | Activity category management   |

## 🌐 External API Integrations

### Open-Meteo

Used to retrieve weather data and help determine suitable activities based on current conditions.

### Yelp Fusion API

Used to discover local businesses, attractions, and venues that match a user's preferences.

### Geoapify

Used for location-based services, geocoding, and distance calculations to provide relevant recommendations within a user's chosen travel range.

## 🔐 Authentication & Security

The API uses cookie-based authentication to manage user sessions securely.

### Features

* User registration
* User login and logout
* Password hashing for secure credential storage
* Protected endpoints for authenticated users
* Session management using authentication cookies

## 📡 Core Functionality

### User Management

* Register new accounts
* Secure login and logout
* Manage user preferences

### Activity Recommendations

* Weather-aware recommendations
* Location-based suggestions
* Preference-driven results
* Age-appropriate activity filtering
* Travel distance filtering

### Favourites

* Save recommended activities
* Retrieve saved favourites
* Manage personalised activity lists

## 🗄 Database Integration

The application uses SQL Server and Entity Framework Core to manage user data and application persistence.

### Entity Framework Core Features Used

* DbContext configuration
* Code-first migrations
* Entity tracking
* LINQ querying
* CRUD operations
* Dependency injection integration

## 🏗 Architecture

The API follows a layered structure to encourage maintainability and separation of concerns.

### Design Principles

* Service layer architecture
* Dependency injection
* RESTful API design
* Separation of concerns
* Configurable JSON-based category management
* Clean and maintainable code practices

## 📚 Key Takeaways

This project demonstrates the ability to:

✅ Build RESTful APIs using ASP.NET Core

✅ Integrate multiple third-party APIs

✅ Implement secure authentication and authorisation

✅ Store and protect user credentials securely

✅ Develop personalised recommendation systems

✅ Work with location and weather-based data

✅ Apply service-layer architecture and clean coding principles

✅ Collaborate effectively within a development team

## 🚀 Future Improvements

* Enhanced recommendation algorithms
* User profile customisation
* Activity history tracking
* Advanced filtering options
* Improved caching and performance optimisation
* Additional third-party integrations
* Expanded activity categories

---

Bored But Broke was developed as a team project to explore real-world API integration, secure user authentication, and personalised recommendation systems while building a scalable backend using modern .NET development practices.
