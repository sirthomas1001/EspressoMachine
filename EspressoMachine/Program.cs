using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public readonly record struct Entity(int Id);

public enum DrinkType
{
    Espresso = 1,
    Americano,
    Latte,
    Cappuccino,
    Mocha
}

public enum CupSize
{
    Small = 1,
    Medium,
    Large
}

public class Recipe
{
    public string Name { get; }
    public int WaterMl { get; }
    public int BeansGrams { get; }
    public int MilkMl { get; }
    public Dictionary<CupSize, decimal> Prices { get; }

    public Recipe(
        string name,
        int waterMl,
        int beansGrams,
        int milkMl,
        decimal smallPrice,
        decimal mediumPrice,
        decimal largePrice)
    {
        Name = name;
        WaterMl = waterMl;
        BeansGrams = beansGrams;
        MilkMl = milkMl;

        Prices = new Dictionary<CupSize, decimal>
        {
            { CupSize.Small, smallPrice },
            { CupSize.Medium, mediumPrice },
            { CupSize.Large, largePrice }
        };
    }
}

public class InventoryComponent
{
    public int WaterMl;
    public int BeansGrams;
    public int MilkMl;
    public int Cups;
}

public class AccountComponent
{
    public decimal Balance;
}

public class MachineStatsComponent
{
    public int DrinksSinceCleaning;
    public int TotalDrinksMade;
    public decimal Revenue;
    public bool NeedsCleaning;
}

public class DrinkOrderComponent
{
    public DrinkType DrinkType;
    public CupSize Size;
    public bool WantsFroth;
    public bool ExtraShot;
    public bool Sugar;
    public decimal TotalPrice;
}

public class World
{
    private int nextId = 1;

    public Dictionary<int, InventoryComponent> Inventories { get; } = new();
    public Dictionary<int, AccountComponent> Accounts { get; } = new();
    public Dictionary<int, MachineStatsComponent> MachineStats { get; } = new();
    public Dictionary<int, List<DrinkOrderComponent>> Carts { get; } = new();

    public Entity CreateEntity()
    {
        return new Entity(nextId++);
    }
}

public static class RecipeDatabase
{
    public static readonly Dictionary<DrinkType, Recipe> Recipes = new()
    {
        { DrinkType.Espresso, new Recipe("Espresso", 40, 18, 0, 2.50m, 3.25m, 4.00m) },
        { DrinkType.Americano, new Recipe("Americano", 120, 18, 0, 3.00m, 3.75m, 4.50m) },
        { DrinkType.Latte, new Recipe("Latte", 60, 18, 150, 4.25m, 5.00m, 5.75m) },
        { DrinkType.Cappuccino, new Recipe("Cappuccino", 50, 18, 120, 4.00m, 4.75m, 5.50m) },
        { DrinkType.Mocha, new Recipe("Mocha", 70, 18, 140, 4.75m, 5.50m, 6.25m) }
    };
}

public static class PricingSystem
{
    public static decimal CalculatePrice(DrinkOrderComponent order)
    {
        Recipe recipe = RecipeDatabase.Recipes[order.DrinkType];
        decimal total = recipe.Prices[order.Size];

        if (order.WantsFroth)
            total += 0.50m;

        if (order.ExtraShot)
            total += 1.00m;

        if (order.Sugar)
            total += 0.10m;

        return total;
    }
}

public static class CartSystem
{
    public static decimal GetCartTotal(List<DrinkOrderComponent> cart)
    {
        return cart.Sum(order => order.TotalPrice);
    }

    public static bool IsCartEmpty(List<DrinkOrderComponent> cart)
    {
        return cart.Count == 0;
    }

    public static void ClearCart(List<DrinkOrderComponent> cart)
    {
        cart.Clear();
    }
}

public static class BrewingSystem
{
    public static void GetRequiredIngredients(
        DrinkOrderComponent order,
        out int waterMl,
        out int beansGrams,
        out int milkMl)
    {
        Recipe recipe = RecipeDatabase.Recipes[order.DrinkType];

        decimal multiplier = order.Size switch
        {
            CupSize.Small => 1.0m,
            CupSize.Medium => 1.35m,
            CupSize.Large => 1.75m,
            _ => 1.0m
        };

        waterMl = (int)(recipe.WaterMl * multiplier);
        beansGrams = (int)(recipe.BeansGrams * multiplier);
        milkMl = (int)(recipe.MilkMl * multiplier);

        if (order.ExtraShot)
        {
            waterMl += 40;
            beansGrams += 18;
        }

        if (order.WantsFroth)
        {
            milkMl += 50;
        }
    }

    public static bool CanBrewCart(List<DrinkOrderComponent> cart, InventoryComponent inventory)
    {
        int totalWater = 0;
        int totalBeans = 0;
        int totalMilk = 0;

        foreach (DrinkOrderComponent order in cart)
        {
            GetRequiredIngredients(order, out int water, out int beans, out int milk);

            totalWater += water;
            totalBeans += beans;
            totalMilk += milk;
        }

        return inventory.WaterMl >= totalWater &&
               inventory.BeansGrams >= totalBeans &&
               inventory.MilkMl >= totalMilk &&
               inventory.Cups >= cart.Count;
    }

    public static void BrewDrink(
        DrinkOrderComponent order,
        InventoryComponent inventory,
        MachineStatsComponent stats)
    {
        GetRequiredIngredients(order, out int water, out int beans, out int milk);

        inventory.WaterMl -= water;
        inventory.BeansGrams -= beans;
        inventory.MilkMl -= milk;
        inventory.Cups--;

        stats.DrinksSinceCleaning++;
        stats.TotalDrinksMade++;

        Console.WriteLine($"\nBrewing {order.Size} {order.DrinkType}...");
        Console.WriteLine("Grinding beans...");
        Console.WriteLine("Heating water...");
        Console.WriteLine("Pulling espresso shot...");

        if (milk > 0)
            Console.WriteLine("Steaming milk...");

        if (order.WantsFroth)
            Console.WriteLine("Adding froth...");

        if (order.Sugar)
            Console.WriteLine("Adding sugar...");

        Console.WriteLine($"Finished: {order.Size} {order.DrinkType} ☕");
    }
}

public static class PaymentSystem
{
    public static bool TryPay(AccountComponent account, MachineStatsComponent stats, decimal amount)
    {
        if (account.Balance < amount)
            return false;

        account.Balance -= amount;
        stats.Revenue += amount;

        return true;
    }

    public static void AddMoney(AccountComponent account, decimal amount)
    {
        account.Balance += amount;
    }
}

public static class MaintenanceSystem
{
    public static void Refill(InventoryComponent inventory)
    {
        inventory.WaterMl = 3000;
        inventory.BeansGrams = 700;
        inventory.MilkMl = 2000;
        inventory.Cups = 30;
    }

    public static void Clean(MachineStatsComponent stats)
    {
        stats.NeedsCleaning = false;
        stats.DrinksSinceCleaning = 0;
    }
}

public class EspressoApp
{
    private readonly World world = new();
    private readonly Entity machine;
    private readonly Entity customer;

    public EspressoApp()
    {
        machine = world.CreateEntity();
        customer = world.CreateEntity();

        world.Inventories[machine.Id] = new InventoryComponent();
        MaintenanceSystem.Refill(world.Inventories[machine.Id]);

        world.MachineStats[machine.Id] = new MachineStatsComponent
        {
            DrinksSinceCleaning = 0,
            TotalDrinksMade = 0,
            Revenue = 0m,
            NeedsCleaning = false
        };

        world.Accounts[customer.Id] = new AccountComponent
        {
            Balance = 25.00m
        };

        world.Carts[customer.Id] = new List<DrinkOrderComponent>();
    }

    public void Run()
    {
        while (true)
        {
            AccountComponent account = world.Accounts[customer.Id];
            List<DrinkOrderComponent> cart = world.Carts[customer.Id];

            Console.WriteLine("\n==============================");
            Console.WriteLine("   ECS ESPRESSO MACHINE");
            Console.WriteLine("==============================");
            Console.WriteLine($"Account Balance: {account.Balance:C}");
            Console.WriteLine($"Cart Total: {CartSystem.GetCartTotal(cart):C}");
            Console.WriteLine("------------------------------");
            Console.WriteLine("1. View Menu");
            Console.WriteLine("2. Add Drink to Cart");
            Console.WriteLine("3. View Cart");
            Console.WriteLine("4. Edit Drink in Cart");
            Console.WriteLine("5. Remove Drink from Cart");
            Console.WriteLine("6. Checkout");
            Console.WriteLine("7. Add Money to Account");
            Console.WriteLine("8. Clear Cart");
            Console.WriteLine("9. View Machine Status");
            Console.WriteLine("10. Refill Supplies");
            Console.WriteLine("11. Clean Machine");
            Console.WriteLine("12. View Sales Report");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowMenu();
                    break;
                case "2":
                    AddDrinkToCart();
                    break;
                case "3":
                    ViewCart();
                    break;
                case "4":
                    EditDrinkInCart();
                    break;
                case "5":
                    RemoveDrinkFromCart();
                    break;
                case "6":
                    Checkout();
                    break;
                case "7":
                    AddMoneyToAccount();
                    break;
                case "8":
                    ClearCart();
                    break;
                case "9":
                    ShowStatus();
                    break;
                case "10":
                    RefillMachine();
                    break;
                case "11":
                    CleanMachine();
                    break;
                case "12":
                    ShowSalesReport();
                    break;
                case "0":
                    Console.WriteLine("Machine shutting down. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid selection.");
                    break;
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("\n--- Full Menu ---");
        Console.WriteLine("Add-ons:");
        Console.WriteLine("Froth: +$0.50");
        Console.WriteLine("Extra Shot: +$1.00");
        Console.WriteLine("Sugar: +$0.10");
        Console.WriteLine();

        Console.WriteLine($"{"#", -5} {"Drink",-15} {"Small",-10} {"Medium",-10} {"Large",-10}");
        Console.WriteLine("-----------------------------------------------------");

        foreach (KeyValuePair<DrinkType, Recipe> item in RecipeDatabase.Recipes)
        {
            Recipe recipe = item.Value;

            Console.WriteLine(
                $"{(int)item.Key,-5} " +
                $"{recipe.Name,-15} " +
                $"{recipe.Prices[CupSize.Small],-10:C} " +
                $"{recipe.Prices[CupSize.Medium],-10:C} " +
                $"{recipe.Prices[CupSize.Large],-10:C}"
            );
        }
    }

    private void AddDrinkToCart()
    {
        MachineStatsComponent stats = world.MachineStats[machine.Id];
        InventoryComponent inventory = world.Inventories[machine.Id];
        List<DrinkOrderComponent> cart = world.Carts[customer.Id];

        if (stats.NeedsCleaning)
        {
            Console.WriteLine("The machine needs cleaning before more drinks can be ordered.");
            return;
        }

        ShowMenu();

        DrinkOrderComponent order = CreateOrderFromUserInput();
        cart.Add(order);

        if (!BrewingSystem.CanBrewCart(cart, inventory))
        {
            cart.Remove(order);
            Console.WriteLine("Not enough supplies for that drink.");
            return;
        }

        Console.WriteLine($"\nAdded to cart: {order.Size} {order.DrinkType}");
        Console.WriteLine($"Drink Total: {order.TotalPrice:C}");
        Console.WriteLine($"Cart Total: {CartSystem.GetCartTotal(cart):C}");
    }

    private DrinkOrderComponent CreateOrderFromUserInput()
    {
        DrinkOrderComponent order = new DrinkOrderComponent
        {
            DrinkType = SelectDrink(),
            Size = SelectCupSize(),
            WantsFroth = AskYesNo("Would you like froth?"),
            ExtraShot = AskYesNo("Would you like an extra shot?"),
            Sugar = AskYesNo("Would you like sugar?")
        };

        order.TotalPrice = PricingSystem.CalculatePrice(order);
        return order;
    }

    private void ViewCart()
    {
        List<DrinkOrderComponent> cart = world.Carts[customer.Id];

        Console.WriteLine("\n--- Cart ---");

        if (CartSystem.IsCartEmpty(cart))
        {
            Console.WriteLine("Cart is empty.");
            return;
        }

        for (int i = 0; i < cart.Count; i++)
        {
            DrinkOrderComponent order = cart[i];

            Console.WriteLine($"{i + 1}. {order.Size} {order.DrinkType} - {order.TotalPrice:C}");
            Console.WriteLine(
                $"   Froth: {(order.WantsFroth ? "Yes" : "No")}, " +
                $"Extra Shot: {(order.ExtraShot ? "Yes" : "No")}, " +
                $"Sugar: {(order.Sugar ? "Yes" : "No")}"
            );
        }

        Console.WriteLine($"Cart Total: {CartSystem.GetCartTotal(cart):C}");
    }

    private void EditDrinkInCart()
    {
        List<DrinkOrderComponent> cart = world.Carts[customer.Id];
        InventoryComponent inventory = world.Inventories[machine.Id];

        if (CartSystem.IsCartEmpty(cart))
        {
            Console.WriteLine("Cart is empty. Nothing to edit.");
            return;
        }

        ViewCart();

        Console.Write("\nChoose the cart item number to edit: ");

        if (!int.TryParse(Console.ReadLine(), out int itemNumber) ||
            itemNumber < 1 ||
            itemNumber > cart.Count)
        {
            Console.WriteLine("Invalid cart item.");
            return;
        }

        int index = itemNumber - 1;

        DrinkOrderComponent originalOrder = CloneOrder(cart[index]);
        DrinkOrderComponent editedOrder = CloneOrder(cart[index]);

        while (true)
        {
            editedOrder.TotalPrice = PricingSystem.CalculatePrice(editedOrder);

            Console.WriteLine("\n--- Edit Drink ---");
            Console.WriteLine($"1. Drink Type: {editedOrder.DrinkType}");
            Console.WriteLine($"2. Cup Size: {editedOrder.Size}");
            Console.WriteLine($"3. Froth: {(editedOrder.WantsFroth ? "Yes" : "No")}");
            Console.WriteLine($"4. Extra Shot: {(editedOrder.ExtraShot ? "Yes" : "No")}");
            Console.WriteLine($"5. Sugar: {(editedOrder.Sugar ? "Yes" : "No")}");
            Console.WriteLine($"Current Price: {editedOrder.TotalPrice:C}");
            Console.WriteLine("------------------");
            Console.WriteLine("6. Save Changes");
            Console.WriteLine("0. Cancel Editing");
            Console.Write("Choose what to edit: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowMenu();
                    editedOrder.DrinkType = SelectDrink();
                    break;

                case "2":
                    editedOrder.Size = SelectCupSize();
                    break;

                case "3":
                    editedOrder.WantsFroth = !editedOrder.WantsFroth;
                    Console.WriteLine($"Froth set to {(editedOrder.WantsFroth ? "Yes" : "No")}.");
                    break;

                case "4":
                    editedOrder.ExtraShot = !editedOrder.ExtraShot;
                    Console.WriteLine($"Extra shot set to {(editedOrder.ExtraShot ? "Yes" : "No")}.");
                    break;

                case "5":
                    editedOrder.Sugar = !editedOrder.Sugar;
                    Console.WriteLine($"Sugar set to {(editedOrder.Sugar ? "Yes" : "No")}.");
                    break;

                case "6":
                    editedOrder.TotalPrice = PricingSystem.CalculatePrice(editedOrder);
                    cart[index] = editedOrder;

                    if (!BrewingSystem.CanBrewCart(cart, inventory))
                    {
                        cart[index] = originalOrder;
                        Console.WriteLine("Edit cancelled. Not enough machine supplies for the updated cart.");
                        return;
                    }

                    Console.WriteLine("Drink updated successfully.");
                    Console.WriteLine($"New Cart Total: {CartSystem.GetCartTotal(cart):C}");
                    return;

                case "0":
                    Console.WriteLine("Edit cancelled.");
                    return;

                default:
                    Console.WriteLine("Invalid edit option.");
                    break;
            }
        }
    }

    private DrinkOrderComponent CloneOrder(DrinkOrderComponent order)
    {
        return new DrinkOrderComponent
        {
            DrinkType = order.DrinkType,
            Size = order.Size,
            WantsFroth = order.WantsFroth,
            ExtraShot = order.ExtraShot,
            Sugar = order.Sugar,
            TotalPrice = order.TotalPrice
        };
    }

    private void RemoveDrinkFromCart()
    {
        List<DrinkOrderComponent> cart = world.Carts[customer.Id];

        if (CartSystem.IsCartEmpty(cart))
        {
            Console.WriteLine("Cart is empty. Nothing to remove.");
            return;
        }

        ViewCart();

        Console.Write("\nChoose the cart item number to remove: ");

        if (!int.TryParse(Console.ReadLine(), out int itemNumber) ||
            itemNumber < 1 ||
            itemNumber > cart.Count)
        {
            Console.WriteLine("Invalid cart item.");
            return;
        }

        DrinkOrderComponent removed = cart[itemNumber - 1];
        cart.RemoveAt(itemNumber - 1);

        Console.WriteLine($"Removed: {removed.Size} {removed.DrinkType}");
        Console.WriteLine($"New Cart Total: {CartSystem.GetCartTotal(cart):C}");
    }

    private void Checkout()
    {
        List<DrinkOrderComponent> cart = world.Carts[customer.Id];
        AccountComponent account = world.Accounts[customer.Id];
        MachineStatsComponent stats = world.MachineStats[machine.Id];
        InventoryComponent inventory = world.Inventories[machine.Id];

        if (CartSystem.IsCartEmpty(cart))
        {
            Console.WriteLine("Cart is empty.");
            return;
        }

        if (stats.NeedsCleaning)
        {
            Console.WriteLine("The machine needs cleaning before checkout.");
            return;
        }

        decimal total = CartSystem.GetCartTotal(cart);

        Console.WriteLine("\n--- Checkout ---");
        ViewCart();
        Console.WriteLine($"Account Balance: {account.Balance:C}");
        Console.WriteLine($"Total Due: {total:C}");

        if (account.Balance < total)
        {
            Console.WriteLine($"Insufficient funds. You need {(total - account.Balance):C} more.");
            return;
        }

        if (!BrewingSystem.CanBrewCart(cart, inventory))
        {
            Console.WriteLine("Not enough machine supplies for the full cart.");
            return;
        }

        if (!AskYesNo("Confirm purchase?"))
        {
            Console.WriteLine("Checkout cancelled.");
            return;
        }

        bool paymentSucceeded = PaymentSystem.TryPay(account, stats, total);

        if (!paymentSucceeded)
        {
            Console.WriteLine("Payment failed.");
            return;
        }

        foreach (DrinkOrderComponent order in cart)
        {
            BrewingSystem.BrewDrink(order, inventory, stats);
        }

        if (stats.DrinksSinceCleaning >= 5)
        {
            stats.NeedsCleaning = true;
            Console.WriteLine("\nThe machine now needs cleaning.");
        }

        CartSystem.ClearCart(cart);

        Console.WriteLine("\nPurchase complete.");
        Console.WriteLine($"Remaining Balance: {account.Balance:C}");
    }

    private void AddMoneyToAccount()
    {
        AccountComponent account = world.Accounts[customer.Id];

        Console.Write("\nEnter amount to add: $");
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal amount) ||
            amount <= 0)
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        PaymentSystem.AddMoney(account, amount);

        Console.WriteLine($"Added {amount:C}.");
        Console.WriteLine($"New Balance: {account.Balance:C}");
    }

    private void ClearCart()
    {
        List<DrinkOrderComponent> cart = world.Carts[customer.Id];

        if (CartSystem.IsCartEmpty(cart))
        {
            Console.WriteLine("Cart is already empty.");
            return;
        }

        if (!AskYesNo("Are you sure you want to clear your cart?"))
        {
            Console.WriteLine("Cart was not cleared.");
            return;
        }

        CartSystem.ClearCart(cart);
        Console.WriteLine("Cart cleared.");
    }

    private void ShowStatus()
    {
        InventoryComponent inventory = world.Inventories[machine.Id];
        MachineStatsComponent stats = world.MachineStats[machine.Id];

        Console.WriteLine("\n--- Machine Status ---");
        Console.WriteLine($"Water: {inventory.WaterMl}ml");
        Console.WriteLine($"Beans: {inventory.BeansGrams}g");
        Console.WriteLine($"Milk: {inventory.MilkMl}ml");
        Console.WriteLine($"Cups: {inventory.Cups}");
        Console.WriteLine($"Drinks Since Cleaning: {stats.DrinksSinceCleaning}");
        Console.WriteLine($"Needs Cleaning: {(stats.NeedsCleaning ? "Yes" : "No")}");
    }

    private void RefillMachine()
    {
        MaintenanceSystem.Refill(world.Inventories[machine.Id]);
        Console.WriteLine("Supplies refilled.");
    }

    private void CleanMachine()
    {
        MaintenanceSystem.Clean(world.MachineStats[machine.Id]);
        Console.WriteLine("Machine cleaned.");
    }

    private void ShowSalesReport()
    {
        MachineStatsComponent stats = world.MachineStats[machine.Id];
        AccountComponent account = world.Accounts[customer.Id];

        Console.WriteLine("\n--- Sales Report ---");
        Console.WriteLine($"Total Drinks Made: {stats.TotalDrinksMade}");
        Console.WriteLine($"Machine Revenue: {stats.Revenue:C}");
        Console.WriteLine($"Customer Account Balance: {account.Balance:C}");
    }

    private DrinkType SelectDrink()
    {
        while (true)
        {
            Console.Write("Choose drink number: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) &&
                Enum.IsDefined(typeof(DrinkType), choice))
            {
                return (DrinkType)choice;
            }

            Console.WriteLine("Invalid drink choice.");
        }
    }

    private CupSize SelectCupSize()
    {
        while (true)
        {
            Console.WriteLine("\nChoose cup size:");
            Console.WriteLine("1. Small");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Large");
            Console.Write("Selection: ");

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int choice) &&
                Enum.IsDefined(typeof(CupSize), choice))
            {
                return (CupSize)choice;
            }

            Console.WriteLine("Invalid cup size.");
        }
    }

    private bool AskYesNo(string question)
    {
        while (true)
        {
            Console.Write($"{question} y/n: ");
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "y" || input == "yes")
                return true;

            if (input == "n" || input == "no")
                return false;

            Console.WriteLine("Please enter y or n.");
        }
    }
}

public class Program
{
    public static void Main()
    {
        EspressoApp app = new EspressoApp();
        app.Run();
    }
}