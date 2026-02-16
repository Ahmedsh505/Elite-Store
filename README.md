🖥️ Elite Store – E-Commerce & Inventory Management System

Elite Store is a production-grade e-commerce and inventory management system built for a real retail business specializing in:

Laptops

PC Components

Computer Accessories

New, Used, and Refurbished Devices

The platform supports full order lifecycle management, inventory control with serial tracking, purchasing, cashflow tracking, and integration with a local in-store system.

🌍 Business Scope

Country: Egypt

Currency: EGP

Single warehouse (physical shop)

Daily sync with local shop system (source of truth)

Supports:

Online payments

Cash on Delivery (with dynamic limit)

Installments

Wallet payments

🏗 Architecture

Backend is built using a clean layered architecture:

EliteSolution
│
├── Elite.Core            # Domain models, DTOs, interfaces, enums
├── Elite.Infrastructure  # EF Core, repositories, DbContext, migrations
├── Elite.Services        # Business logic layer
├── Elite (API)           # ASP.NET Core REST API

Principles Used

Clean architecture separation

Repository pattern

Service layer abstraction

Explicit business rule enforcement

Snapshot pricing in orders

Idempotent payment handling

Structured inventory movement tracking

📦 Core Business Modules
Catalog

Categories (parent-child)

Brands

Products

Product Variants (RAM / SSD / etc.)

Used / Refurbished condition support

Product & Variant images

Inventory

Single warehouse model

On-hand quantity tracking

Backorder support

Stock movement logging

Purchase tracking

Serial number tracking per unit

Orders

Order lifecycle:
Created → PendingPayment → Paid → Processing → Shipped → Delivered
Branches:

FailedPayment

Cancelled

ReturnRequested

ReturnedReceived

Refunded (full / partial)

Payments

Card

Wallet

Installments

COD (restricted by dynamic limit setting)

COD rule:

Configurable setting: COD_MAX_LIMIT_EGP

If order total exceeds limit → COD unavailable

Returns & Refunds

14-day return window

Partial refunds supported

Cash refund tracking

Serial reintegration into inventory

Purchasing

Supplier management

Purchase orders

Cost tracking

Cost of Goods Sold (COGS) preparation

Cashflow & Ledger

Sale entries

Refund entries

Purchase entries

Adjustment entries

🔐 Authentication & Roles

Email + Phone authentication

Guest checkout supported

Multiple admin roles:

Product Management

Orders

Finance

Support

🔄 Integration Strategy

The local shop system is the source of truth.

Daily synchronization includes:

Inventory quantities

Purchase costs

Serial status

Optional product updates

The website does not override authoritative financial or stock data without defined rules.

🗄 Database

MS SQL Server

EF Core migrations

Indexed for:

SKU

Order status

Product category/brand

Serial uniqueness

Backups:

Automated backups 6 times daily

Offsite backup storage recommended

🛡 Critical Business Rules

Stock decremented only after successful payment

COD cash enters ledger only when delivered

Serial numbers linked to:

Purchase

Order item

Return

Payment webhooks are idempotent

Order price snapshot preserved even if product price changes later

🚀 Deployment Considerations

Object storage for product images (S3-compatible)

Background jobs for:

Payment webhook retries

Daily sync

Reports

Structured logging & global exception handling

Server-side caching for catalog endpoints

📈 Roadmap

Advanced reporting dashboard

Reservation system (inventory hold at checkout)

Shipment splitting support

Enhanced fraud detection via serial analysis

⚖️ Production Notice

This repository represents a live commercial system used by Elite Store.
Changes to business logic, pricing, inventory, or payment flow must follow operational approval and version control policy.
