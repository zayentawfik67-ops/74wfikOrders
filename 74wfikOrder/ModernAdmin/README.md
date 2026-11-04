# KayuSushi Admin - Modern Panel

This is a modern, secure admin panel for the KayuSushi system, built with a decoupled architecture.

## Tech Stack
- **Backend**: Node.js (Express), JWT, Sequelize (SQLite/PostgreSQL), Helmet, Rate Limiting.
- **Frontend**: React (Vite), TypeScript, Tailwind CSS, Zustand, React Query, Lucide Icons.

## Setup Instructions

### 1. Backend Setup
```bash
cd backend
npm install
npm run seed  # To populate initial data (Admin: admin@74wfikorder.com / password123)
npm run dev   # Starts API on http://localhost:5000
```

### 2. Frontend Setup
```bash
cd frontend
npm install
npm run dev   # Starts Dashboard on http://localhost:5173
```

## Core Security Features
- **JWT Auth**: Secure login with access and refresh tokens.
- **RBAC**: Admin vs Staff roles.
- **Audit Logs**: Every admin action is logged in the database.
- **2FA Support**: Logic and UI ready for TOTP/OTP.
- **Sensitive Data Protection**: Emails and phones cannot be changed directly. A verification OTP is sent to the customer for every update request.
- **Rate Limiting**: Protection against brute force on the API.

## Demo Credentials
- **Admin**: `admin@74wfikorder.com` / `password123` (Requires 2FA code: `123456`)
- **Staff**: `staff@74wfikorder.com` / `password123`
