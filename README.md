# EspressoMachine

A console-based espresso machine simulator written in C# using an Entity Component System-inspired architecture.

This project models a small commercial espresso machine with drink ordering, cart management, inventory tracking, payments, cleaning, refilling, and sales reporting. The goal was to practice software architecture by separating data into components and behavior into systems instead of placing all logic inside one large machine class.

## Features

* Menu system with multiple drink types and cup sizes
* Custom drink options including froth, extra shots, and sugar
* Cart creation, editing, removal, and checkout
* Inventory tracking for water, beans, milk, and cups
* Account balance and payment handling
* Machine maintenance through cleaning and refilling
* Sales reporting with total drinks made and revenue
* ECS-inspired organization using entities, components, and systems

## Architecture

The project is organized around a simple ECS-style structure:

### Entities

Entities are lightweight identifiers used to represent objects in the simulation.

Current entities include:

* Espresso machine
* Customer

### Components

Components store data only.

Examples:

* `InventoryComponent` — stores water, beans, milk, and cup counts
* `AccountComponent` — stores customer balance
* `MachineStatsComponent` — tracks revenue, drinks made, and cleaning status
* `DrinkOrderComponent` — stores drink selection and customization data

### Systems

Systems contain behavior and operate on component data.

Examples:

* `PricingSystem` — calculates drink prices
* `CartSystem` — manages cart totals and cart state
* `BrewingSystem` — checks inventory and brews drinks
* `PaymentSystem` — handles account payments and revenue
* `MaintenanceSystem` — refills supplies and cleans the machine

## What I Learned

This project helped me practice:

* Separating data from behavior
* Modeling real-world state with components
* Designing modular systems with clear responsibilities
* Managing user input and validation in a console application
* Applying object-oriented programming principles without overloading a single class
* Thinking about software architecture beyond basic procedural flow

## Technologies Used

* C#
* .NET
* Console application architecture
* Entity Component System-inspired design

## How to Run

1. Clone the repository.

```bash
git clone <repository-url>
```

2. Navigate into the project folder.

```bash
cd <project-folder>
```

3. Run the project.

```bash
dotnet run
```

## Future Improvements

Planned improvements include:

* Refactoring the ECS structure into a more generic component storage system
* Adding unit tests for pricing, cart, brewing, and payment logic
* Adding save/load support
* Improving the console UI
* Adding more detailed simulation behavior
* Creating documentation or diagrams explaining the system flow

## Project Status

This project is functional and serves as an architecture-focused learning project. It is not intended to be a production espresso system, but rather a demonstration of ECS-style design, modular systems, and state management in C#.
