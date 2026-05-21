# Espresso Machine Simulator

A feature-rich C# espresso machine simulator built with an ECS-inspired architecture.

This project models a smart café-style ordering system with customizable drinks, inventory tracking, checkout workflows, editable carts, account balances, and modular gameplay-inspired systems architecture.

---

# Features

## Drink Ordering
- Espresso
- Americano
- Latte
- Cappuccino
- Mocha

## Drink Customization
- Small / Medium / Large cup sizes
- Froth add-on
- Extra espresso shot
- Sugar option

## Cart System
- Add multiple drinks
- View cart totals
- Edit drinks already in cart
- Remove drinks from cart
- Clear entire cart

## Payment System
- Customer account balance
- Add funds to account
- Checkout validation
- Revenue tracking

## Machine Simulation
- Water tracking
- Bean tracking
- Milk tracking
- Cup inventory
- Cleaning requirements
- Refill system

## ECS-Inspired Architecture

The application is structured around:
- **Entities** → IDs only
- **Components** → data only
- **Systems** → logic only

### Systems Included
- PricingSystem
- BrewingSystem
- PaymentSystem
- CartSystem
- MaintenanceSystem

---

# Architecture Overview

```text
Entity
 ├── Components
 │    ├── InventoryComponent
 │    ├── AccountComponent
 │    ├── MachineStatsComponent
 │    └── DrinkOrderComponent
 │
 └── Systems
      ├── PricingSystem
      ├── BrewingSystem
      ├── PaymentSystem
      ├── CartSystem
      └── MaintenanceSystem
```

This design separates application state from business logic and makes the simulator easier to scale and maintain.

---

# Technologies Used

- C#
- .NET
- Console Application
- ECS-inspired architecture
- Object-Oriented Programming
- Data-Oriented Design principles

---

# Running the Project

## Clone the Repository

```bash
git clone <your-repo-url>
```

## Navigate to the Project

```bash
cd EspressoMachineSimulator
```

## Run the Application

```bash
dotnet run
```

---

# Example Workflow

```text
1. View Menu
2. Add Drink to Cart
3. Customize Drink
4. Edit Existing Orders
5. Checkout
6. Brew Drinks
7. Track Inventory & Revenue
```

---

# Future Improvements

- JSON save/load system
- SQLite persistence
- GUI frontend with MAUI or WPF
- Multi-customer simulation
- Employee/admin mode
- Async brewing queue
- REST API with ASP.NET Core
- Unit testing suite
- Telemetry dashboard

---

# Why This Project?

This project started as a simple console application and evolved into a larger systems-oriented simulation focused on:
- modular architecture
- scalability
- user workflow design
- data management
- maintainability

The goal was to explore how game-engine-inspired ECS patterns can be adapted to business-style application logic in C#.

---

# License

MIT License
