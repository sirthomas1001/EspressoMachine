# EspressoMachine

A console-based espresso machine simulator written in **C#** using an **Entity Component System (ECS)-inspired architecture**.

EspressoMachine simulates a small commercial espresso machine with drink ordering, cart management, inventory tracking, payments, cleaning, refilling, and sales reporting.

The goal of this project was to practice software architecture by separating application data into components and organizing behavior into systems instead of placing all logic inside one large machine class.

> This is an architecture-focused learning project. It uses ECS-inspired design principles, but it is not intended to be a full generic ECS framework.

---

## Features

- Menu system with multiple drink types and cup sizes
- Custom drink options including froth, extra shots, and sugar
- Cart creation, editing, removal, and checkout
- Inventory tracking for water, beans, milk, and cups
- Account balance and payment handling
- Machine maintenance through cleaning and refilling
- Sales reporting with total drinks made and revenue
- ECS-inspired organization using entities, components, and systems

---

## Architecture

EspressoMachine uses a simple ECS-inspired structure made up of entities, components, and systems.

### Entities

Entities are lightweight identifiers used to represent objects in the simulation.

Current entities include:

- Espresso Machine
- Customer

### Components

Components store data only.

Examples:

- `InventoryComponent` — stores water, beans, milk, and cup counts
- `AccountComponent` — stores the customer balance
- `MachineStatsComponent` — tracks revenue, drinks made, and cleaning status
- `DrinkOrderComponent` — stores drink selection and customization data
- `CartComponent` — stores the customer's selected drink orders

### Systems

Systems contain behavior and operate on component data.

Examples:

- `PricingSystem` — calculates drink prices
- `CartSystem` — manages cart totals and cart state
- `BrewingSystem` — checks inventory and brews drinks
- `PaymentSystem` — handles account payments and revenue
- `MaintenanceSystem` — refills supplies and cleans the machine

This structure keeps data and behavior separate, making the program easier to understand, maintain, and extend.

---

## Design Goals

The main purpose of this project was to practice software design and architecture.

Key goals included:

- Separate data from behavior
- Avoid building one large monolithic machine class
- Practice modular system design
- Model real-world machine state using components
- Apply ECS-inspired thinking outside of game development
- Build a project that could be extended with testing, persistence, and better UI later

---

## Technologies Used

- C#
- .NET
- Console application architecture
- Object-Oriented Programming
- ECS-inspired design

---

## How to Run

Clone the repository:

```bash
git clone <repository-url>
