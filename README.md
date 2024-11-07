# CandidateAPI

This project is designed to manage candidates for a job application system. The solution uses a modular and database-agnostic architecture, making it easy to migrate to different database types in the future.

---

# Project Enhancements

## Enhanced Caching Strategy
Distributed caching system (e.g., Redis) instead of in-memory caching can be used. Distributed caching would improve performance in a load-balanced environment by providing a single source of cache for all instances.

## Improved Error Handling and Logging
Structured logging (e.g., with Serilog or NLog) for better debugging and monitoring can be implemented.

## 📌 Assumptions
The following assumptions were made during the development of this project:

- **Database Agnosticism**: The application was designed to work with SQL-based databases initially but can be extended to NoSQL databases with additional development. Current optimizations assume SQL compatibility.
  
- **Caching Mechanism**: In-memory caching is used to enhance performance, with the assumption that it may later be swapped for a distributed cache if usage scales.
  
- **Controlled API Usage**: The API is expected to serve a limited number of clients initially. Scaling requirements may necessitate enhancements like rate limiting or a different caching strategy.

## ⏳ Total Time Spent
Total time spent on this task: 8 hours
Initial Projest setup: 2 hours
Controller and Services: 2.5 hours
Unit Test: 2.5 hours
Implementing Caching: 1

