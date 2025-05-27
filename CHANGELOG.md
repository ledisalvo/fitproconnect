# 📦 Changelog

All notable changes to this project will be documented in this file.

---

## [v1.1.0] - 2025-05-11

### Added
- ✨ `SyncController` with:
  - `GET /api/sync/missing` – detect users missing in UserService
  - `POST /api/sync/reprocess` – re-publish UserRegistered events
- ✨ `EventsController` with `GET /api/events/logs` + filter support (email, status, source)
- ✨ `EventLog` entity to track event processing and synchronization
- ✨ `UsersController` with endpoints:
  - `GET /api/users/all`
  - `GET /api/users/{email}`
  - `GET /api/users/emails`
- ✨ `RetryHelper` with exponential backoff in UserRegisteredConsumer

### Changed
- ✅ Modularized API into Users, Events, Sync controllers
- ✅ Improved JSON deserialization to avoid null values in event sync
- ✅ Separated queue declaration logic for clarity and maintainability

### Fixed
- 🐛 Null Email issue in `EventLogs`
- 🐛 Migration failure due to inconsistent DB host config

---

## [v1.0.0] - Initial Release

- ✅ AuthService with registration, login, JWT + role handling
- ✅ Event publishing to RabbitMQ on successful registration
- ✅ UserService consuming events and storing user records
- ✅ Welcome email via MailKit + Mailtrap
- ✅ Dockerized microservices and PostgreSQL with Docker Compose