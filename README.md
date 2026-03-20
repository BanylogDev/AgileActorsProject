# Aggregated Api Project

A Single service that retrieves data from multiple endpoints
(news, reddit, weather) and combines them into a single
entity. It has a 3 endpoints: 1st for single entity data
retrieval, 2nd for statistics of the response times
of each api, 3rd for auth (login endpoint).
Authentication is handled using JWT.



|                     TECHNOLOGIES USED                    |
|----------------------------------------------------------|
| Backend Framework    | ASP.NET Core Web API              |
| Language             | C# 12                             |
| Auth Method          | JWT Bearer Tokens                 |
| External Clients     | Weather API, News API, Reddit     |
| Utilities            | Memory Cache, HttpClientFactory   |
| Architecture Style   | Clean / Layered Architecture      |



|                    PROJECT STRUCTURE                     |
|----------------------------------------------------------|
| /Presentation                                            |
|    - Controllers                                         |
|    - Routing & Authentication                            |                             
|                                                          |
| /Application                                             |
|    - Use Cases (business logic)                          |
|    - DTOs                                                |
|    - Interfaces                                          |
|                                                          |
| /Infrastructure                                          |
|    - External API Clients                                |
|    - JWT Service                                         |
|    - Caching                                             |
|    - Statistics Service                                  |
|                                                          |
| /Domain                                                  |
|    - Core Entities (User, etc.)                          |
|                                                          |
| /Tests                                                   |
|    - xUnit Tests                                         |



|                    ENDPOINTS OVERVIEW                    |
|----------------------------------------------------------|
| POST /api/auth/login                                     |
|    - Authenticates a user                                |
|    - Returns JWT access & refresh tokens                 |
|                                                          |
| GET /api/aggregated                                      |
|    - Requires JWT Authorization                          |
|    - Returns combined data from:                         |
|         • Weather API                                    |
|         • News API                                       |
|         • Reddit API                                     |
|         • Internal statistics                            |
|                                                          |
| GET /api/statistics                                      |
|    - Requires JWT Authorization                          |
|    - Returns stats about the endpoints usage,            |
|      Average Response Time, Counts, Fastest              |
