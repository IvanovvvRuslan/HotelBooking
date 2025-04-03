Hotel Booking API

This is a RESTful API for managing hotel room reservations, built with ASP.NET Core and C#. It allows users to check room availability, make reservations, and manage customer data. The API uses JWT-based authentication for secure access and implements role-based authorization to control user access.
Key Features:

    Room Reservation Management: Enables users to book rooms, check availability, and manage existing reservations.

    Authentication and Authorization: Utilizes JWT for user authentication and role-based access control to manage permissions for admin and user roles.

    Database Integration: The API is integrated with MS SQL Server to store reservation data.

    Testing: Ensures API reliability and performance with xUnit for unit testing and NSubstitute for mocking dependencies.

    CI/CD: Automated build and deployment pipeline using GitHub Actions to ensure seamless continuous integration and delivery to Azure.

Technology Stack:

    Backend: ASP.NET Core, C#

    Database: MS SQL Server

    Authentication: JWT

    Testing: xUnit, NSubstitute

    Containerization: Docker

    CI/CD: GitHub Actions, Azure
