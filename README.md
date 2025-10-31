# Simple Marketplace

A simple online marketplace application built with ASP.NET Core MVC where users can post items for sale and buyers can mark their interest in items.

## Project Overview

**Student:** GARCIA, ERICH MAE A.
**Section:** BSIT-31A1

## Features

- **User Authentication**: Registration and login with ASP.NET Core Identity
- **Password Requirements**: At least 2 uppercase letters, 3 numbers, and 3 symbols
- **Post Items**: Authenticated users can post items for sale with title, description, and price
- **Browse Items**: View all available items in the marketplace
- **Mark Interest**: Buyers can mark their interest in items
- **My Items**: Sellers can view and manage their posted items
- **My Interests**: Buyers can view all items they've marked interest in
- **Seller Dashboard**: Sellers can see interested buyers' contact information

## Technical Implementation

### Architecture
- **Presentation Layer**: ASP.NET Core MVC with Razor Views
- **Infrastructure Layer**: Entity Framework Core with DbContext and Entities
- **Service Layer**: Interface-based services for business logic

### Technologies Used
- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core (In-Memory Database)
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, jQuery
- **Architecture Pattern**: MVC (Model-View-Controller)

### Project Structure
```
SimpleMarketplace/
├── Controllers/
│   ├── AccountController.cs       # Authentication and authorization
│   ├── ItemsController.cs         # Marketplace operations
│   └── HomeController.cs          # Home/redirect controller
├── Models/
│   ├── ApplicationUser.cs         # User entity (extends IdentityUser)
│   ├── Item.cs                    # Item entity
│   ├── Interest.cs                # Interest entity (many-to-many)
│   └── ErrorViewModel.cs          # Error handling
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Services/
│   ├── IItemService.cs           # Item service interface
│   ├── ItemService.cs            # Item service implementation
│   ├── IInterestService.cs       # Interest service interface
│   └── InterestService.cs        # Interest service implementation
├── ViewModels/
│   ├── RegisterViewModel.cs      # Registration form model
│   ├── LoginViewModel.cs         # Login form model
│   └── CreateItemViewModel.cs    # Item creation form model
└── Views/
    ├── Account/                   # Authentication views
    ├── Items/                     # Marketplace views
    └── Shared/                    # Shared layouts and partials
```

## Database Schema

### Entities and Relationships
- **ApplicationUser** (extends IdentityUser)
  - FullName, Email, UserName
  - Navigation: ItemsPosted, Interests
  
- **Item**
  - Id, Title, Description, Price, DatePosted, IsSold, SellerId
  - Navigation: Seller, InterestedBuyers
  
- **Interest**
  - Id, BuyerId, ItemId, DateMarked
  - Navigation: Buyer, Item

### Relationships
- User → Items (One-to-Many): A user can post multiple items
- User → Interests (One-to-Many): A user can mark interest in multiple items
- Item → Interests (One-to-Many): An item can have multiple interested buyers

## Usage

### Registration
1. Click "Register" in the navigation menu
2. Enter your full name, email, and password
3. Password must meet requirements: minimum 8 characters, 2 uppercase, 3 numbers, 3 symbols
4. Example valid password: `ABcd123!!!`

### Posting an Item
1. Log in to your account
2. Click "Post Item" in the navigation menu
3. Enter item title, description, and price
4. Submit the form

### Browsing and Marking Interest
1. Browse available items on the home page
2. Click "View Details" on any item
3. Click "Mark Interest" to show interest in the item
4. Sellers will see your contact information

### Managing Your Items
1. Click "My Items" to see all items you've posted
2. View interested buyers' contact information
3. Mark items as sold or delete them

### Managing Your Interests
1. Click "My Interests" to see items you're interested in
2. View seller contact information
3. Remove interest if no longer interested

## Key Features Implementation

### EF Core Code-First Approach
- In-memory database for development
- DbContext with proper entity configurations
- Navigation properties and relationships

### MVC Identity with Custom Password Requirements
```csharp
options.Password.RequireDigit = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequiredLength = 8;
```

### Clean Architecture with Layered Structure
- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic with interface abstraction
- **Data**: Entity Framework Core DbContext
- **Models**: Domain entities
- **ViewModels**: Data transfer objects for views

### Service Layer with Interfaces
- `IItemService` / `ItemService`: Item operations
- `IInterestService` / `InterestService`: Interest operations
- Dependency injection for loose coupling

## Learning Objectives Demonstrated

✅ **Entity Framework Core Implementation**: In-memory database with proper relationships  
✅ **MVC Architecture**: Clean separation of concerns  
✅ **Appropriate Layers**: Presentation, Infrastructure (DbContext/Entities), Service Layer  
✅ **Task Decomposition**: Modular services and controllers  
✅ **GitHub for Version Control**: Project tracked with Git  
✅ **Individual Presentation**: Complete documentation and working demo

## Notes

- This application uses an **in-memory database** for demonstration purposes
- Data is not persisted between application restarts
- For production, configure a persistent database (SQL Server, PostgreSQL, etc.)
- Commits should include maximum of 5 files per commit as per requirements

## Future Enhancements

- Image uploads for items
- Search and filtering functionality
- Categories for items
- Messaging system between buyers and sellers
- Payment integration
- Rating and review system

