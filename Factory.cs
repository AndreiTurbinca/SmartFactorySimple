using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SmartFactorySimple;

public class Factory
{
    public string Nume;

    
    private EmployeeRepository _employeeRepository = new EmployeeRepository();
    private MachineRepository _machineRepository = new MachineRepository();
    private ProductRepository _productRepository = new ProductRepository();
    private ProductionOrderRepository _orderRepository = new ProductionOrderRepository();

    private int _idComandaCounter = 1;
    private decimal _totalRevenue = 0;
    private int _totalSalesQuantity = 0;

    // Stocul pentru materialele prime (ex: "Lemn" -> 100 bucăți)
    private Dictionary<string, int> _stocMateriale = new Dictionary<string, int>();

    // Rețetele jucăriilor. Fiecare NumeJucărie are un dicționar cu Materiale și Cantități
    private Dictionary<string, Dictionary<string, int>> _retete = new Dictionary<string, Dictionary<string, int>>();

    public Factory(string nume)
    {
        Nume = nume;
    }

    // File where orders are persisted. Search for orders.txt in app base dir and up to 4 parent folders.
    private string OrdersFilePath
    {
        get
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string candidate = Path.Combine(baseDir, "orders.txt");
            if (File.Exists(candidate)) return candidate;

            var dir = new DirectoryInfo(baseDir);
            for (int i = 0; i < 5 && dir != null; i++)
            {
                candidate = Path.Combine(dir.FullName, "orders.txt");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }

            // fallback to baseDir path (file may be created there)
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orders.txt");
        }
    }

    // Load orders from orders.txt. Expects lines in the format:
    // Id;MachineSerial;ProductName;Quantity;Priority;Status;CreatedBy;CreatedAt
    public void LoadOrdersFromFile()
    {
        try
        {
            if (!File.Exists(OrdersFilePath))
                return;

            string[] lines = File.ReadAllLines(OrdersFilePath);
            int maxIdSeen = 0;
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split(';');
                if (parts.Length < 8)
                {
                    Console.WriteLine($"Warning: invalid order line: {line}");
                    continue;
                }

                string id = parts[0].Trim();
                string machineSerial = parts[1].Trim();
                string productName = parts[2].Trim();
                if (!int.TryParse(parts[3].Trim(), out int qty))
                    qty = 0;

                if (!Enum.TryParse(parts[4].Trim(), true, out Priority prioritate))
                    prioritate = Priority.Medium;

                if (!Enum.TryParse(parts[5].Trim(), true, out ProductionOrderStatus status))
                    status = ProductionOrderStatus.Created;

                string createdBy = parts[6].Trim();
                DateTime createdAt = DateTime.Now;
                DateTime.TryParse(parts[7].Trim(), out createdAt);

                Machine masina = GasesteMasina(machineSerial);
                Employee emp = GasesteAngajat(createdBy);
                ProductionManager manager = emp as ProductionManager;

                if (masina == null || manager == null)
                {
                    // can't construct a valid order without machine and manager; skip
                    Console.WriteLine($"Skipping order {id}: missing machine or manager");
                    continue;
                }

                // if order already exists, update its properties, otherwise create new
                var existing = _orderRepository.FindById(id);
                if (existing != null)
                {
                    existing.Masina = masina;
                    existing.NumeProdus = productName;
                    existing.CantitateTarget = qty;
                    existing.Prioritate = prioritate;
                    existing.Status = status;
                    existing.DataCrearii = createdAt;
                }
                else
                {
                    var order = new ProductionOrder(id, masina, manager, productName, qty, prioritate);
                    order.Status = status;
                    order.CantitateProdusa = 0; // we don't persist produced amount in file currently
                    order.DataCrearii = createdAt;

                    _orderRepository.Add(order);
                }
                // track numeric suffix of ORD... ids so we can continue numbering
                if (id.StartsWith("ORD", StringComparison.OrdinalIgnoreCase))
                {
                    string numPart = id.Substring(3);
                    if (int.TryParse(numPart, out int parsed))
                    {
                        if (parsed > maxIdSeen) maxIdSeen = parsed;
                    }
                }
            }

            // ensure next generated id is higher than any existing one
            if (maxIdSeen > 0)
            {
                _idComandaCounter = maxIdSeen + 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load orders: {ex.Message}");
        }
    }

    // Persist all orders to orders.txt (overwrites file)
    public void SaveOrdersToFile()
    {
        try
        {
            var orders = _orderRepository.GetAll();
            List<string> lines = new List<string>();
            lines.Add("# Production Orders");
            lines.Add("# Format: Id;MachineSerial;ProductName;Quantity;Priority;Status;CreatedBy;CreatedAt");
            foreach (var o in orders)
            {
                string createdBy = o.CreatDe != null ? o.CreatDe.Id : "";
                string createdAt = o.DataCrearii.ToString("s");
                string line = string.Join(";", o.Id, o.Masina?.SerialNumber ?? "", o.NumeProdus, o.CantitateTarget.ToString(), o.Prioritate.ToString(), o.Status.ToString(), createdBy, createdAt);
                lines.Add(line);
            }

            File.WriteAllLines(OrdersFilePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save orders: {ex.Message}");
        }
    }

    public void InitializeazaMaterialeSiRetete()
    {
        // 1. Definim stocul inițial de materiale (poți adăuga o metodă să cumperi materiale mai târziu)
        _stocMateriale["Lemn"] = 500;
        _stocMateriale["Plastic"] = 500;
        _stocMateriale["Lana"] = 300;
        _stocMateriale["Piele"] = 200;

        // 2. Definim rețeta pentru fiecare produs (ce consumă o singură unitate din acea jucărie)
        _retete["MagicBlocks"] = new Dictionary<string, int> { { "Lemn", 2 }, { "Plastic", 1 } };
        _retete["Barbie"] = new Dictionary<string, int> { { "Plastic", 2 }, { "Lana", 1 } };
        _retete["Barnie"] = new Dictionary<string, int> { { "Lana", 3 } };
        _retete["Football"] = new Dictionary<string, int> { { "Piele", 3 }, { "Plastic", 1 } };
        _retete["OZN"] = new Dictionary<string, int> { { "Plastic", 3 } };
    }

    // O funcție ajutătoare pentru a vedea ce materiale mai ai
    public void AfiseazaStocMaterialePrime()
    {
        Console.WriteLine("\n=== STOC MATERIALE PRIME ===");
        foreach (var material in _stocMateriale)
        {
            Console.WriteLine($"{material.Key}: {material.Value} unitati disponibile");
        }
    }

    public bool AdaugaAngajat(Employee angajat)
    {
        bool added = _employeeRepository.Add(angajat);
        if (added)
        {
            Logging.Log(angajat.Id, $"Employee added: {angajat.Nume}");
        }
        return added;
    }

    public bool EmployeeIdExists(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _employeeRepository.ExistsById(id);
    }

    public void AfiseazaAngajati()
    {
        _employeeRepository.DisplayAll();
    }

    public Employee GasesteAngajat(string id)
    {
        return _employeeRepository.FindById(id);
    }

    public bool StergeAngajat(string id)
    {
        if (_employeeRepository.RemoveById(id))
        {
            Console.WriteLine("Employee successfully deleted!");
            Logging.Log(id, $"Employee removed: {id}");
            return true;
        }
        else
        {
            Console.WriteLine("The employee doesn't exist!");
            return false;
        }
    }


    public bool AdaugaMasina(Machine masina)
    {
        return _machineRepository.Add(masina);
    }

    public void AfiseazaMasini()
    {
        _machineRepository.DisplayAll();
    }

    public Machine GasesteMasina(string serial)
    {
        return _machineRepository.FindBySerialNumber(serial);
    }



    public bool AdaugaProdus(Product produs)
    {
        _productRepository.Add(produs);
        return true;
    }

    public void AfiseazaProduse()
    {
        _productRepository.DisplayAll();
    }

    public Product GasesteProdus(string nume)
    {
        return _productRepository.FindByName(nume);
    }



    public void CreazaComanda(string idManager, string serialMasina,
                              string produs, int cantitate, Priority prioritate)
    {
        Employee angajat = GasesteAngajat(idManager);
        if (angajat == null)
        {
            Console.WriteLine("The employee doesn't exist!");
            return;
        }

        if (!(angajat is ProductionManager))
        {
            Console.WriteLine(angajat.Nume + " is not ProductionManager!");
            return;
        }

        ProductionManager manager = (ProductionManager)angajat;

        Machine masina = GasesteMasina(serialMasina);
        if (masina == null)
        {
            Console.WriteLine("Machine doesn't exist!");
            return;
        }

        string idComanda = "ORD" + _idComandaCounter;
        _idComandaCounter++;
        // Create order with priority, add to repository, log and persist
        ProductionOrder comanda = manager.CreazaComanda(idComanda, masina, produs, cantitate, prioritate);
        _orderRepository.Add(comanda);
        Logging.Log(idManager, $"Order created: {idComanda} ({produs}) qty={cantitate} priority={prioritate}");
        SaveOrdersToFile();
    }
    public void AfiseazaMasiniInMentenanta()
    {
        // Preluăm toate mașinile din repository
        List<Machine> machines = _machineRepository.GetAll();

        // Filtrăm doar mașinile care au condiția "Critical"
        var masiniInMentenanta = machines.Where(m => m.Conditie.ToString() == "Critical").ToList();

        if (masiniInMentenanta.Count == 0)
        {
            Console.WriteLine("There are no machines in maintenance");
            return;
        }

        Console.WriteLine("\n=== MACHINES IN MAINTENANCE ===");
        foreach (var machine in masiniInMentenanta)
        {
            Console.WriteLine($"{machine.SerialNumber} - {machine.Nume} | Condition: {machine.Conditie}");
        }
    }
    public void ExecutaComanda(string idOperator, string idComanda, int unitati)
    {
        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null) { Console.WriteLine("Employee doesn't exist!"); return; }
        if (!(angajat is MachineOperator)) { Console.WriteLine(angajat.Nume + " is not MachineOperator!"); return; }

        MachineOperator op = (MachineOperator)angajat;
        ProductionOrder comanda = _orderRepository.FindById(idComanda);

        if (comanda == null) { Console.WriteLine("Order doesn't exist!"); return; }

        // --- INCEPUT LOGICĂ REȚETĂ ȘI MATERIALE ---
        string numeProdus = comanda.NumeProdus;

        if (_retete.ContainsKey(numeProdus))
        {
            var reteta = _retete[numeProdus];

            // 1. Verificăm mai întâi dacă avem destule materiale în stoc
            foreach (var ingredient in reteta)
            {
                string numeMaterial = ingredient.Key;
                int cantitateNecesarPentruTotal = ingredient.Value * unitati;

                if (!_stocMateriale.ContainsKey(numeMaterial) || _stocMateriale[numeMaterial] < cantitateNecesarPentruTotal)
                {
                    int stocCurent = _stocMateriale.ContainsKey(numeMaterial) ? _stocMateriale[numeMaterial] : 0;
                    Console.WriteLine($"Error! Not enough materials: {numeMaterial}. Necessary: {cantitateNecesarPentruTotal}, In stock: {stocCurent}.");
                    return; // Oprim execuția comenzii pentru că nu avem materiale
                }
            }

            // 2. Dacă a trecut de verificarea de mai sus, înseamnă că avem materiale. Le scădem din stoc!
            foreach (var ingredient in reteta)
            {
                string numeMaterial = ingredient.Key;
                int cantitateNecesarPentruTotal = ingredient.Value * unitati;
                _stocMateriale[numeMaterial] -= cantitateNecesarPentruTotal;
            }
            Console.WriteLine($"The raw materials were consumed according to the recipe for {unitati}x {numeProdus}!");
        }
        else
        {
            Console.WriteLine($"Warning: No recipe found for {numeProdus}. Production will proceed without materials.");
        }
        // --- SFÂRȘIT LOGICĂ MATERIALE ---

        op.Opereaza(comanda.Masina);

        if (comanda.Masina.Status == MachineStatus.Running)
        {
            comanda.InregistreazaProductie(unitati);
            Logging.Log(idOperator, $"Produced {unitati} units for order {idComanda} ({comanda.NumeProdus})");
        }
    }

    public void ReparaMasina(string idTehnician, string idEngineer, string serial)
    {
        Employee a1 = GasesteAngajat(idTehnician);
        Employee a2 = GasesteAngajat(idEngineer);

        if (a1 == null || a2 == null)
        {
            Console.WriteLine("One of the employees doesn't exist!");
            return;
        }

        if (a1 is not Technician)
        {
            Console.WriteLine(a1.Nume + " is not a Technician!");
            return;
        }

        if (a2 is not Engineer)
        {
            Console.WriteLine(a2.Nume + " is not a Engineer!");
            return;
        }

        Technician teh = (Technician)a1;
        Engineer eng = (Engineer)a2;

        Machine masina = GasesteMasina(serial);
        if (masina == null)
        {
            Console.WriteLine("Machine doesn't exist!");
            return;
        }

        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine("Stop the car before the repair!");
            return;
        }

        eng.Inspecteaza(masina);
        teh.Repara(masina);
    }

    public void AdaugaStocProduse(string numeProdus, int cantitate)
    {
        Product produs = GasesteProdus(numeProdus);
        if (produs == null)
        {
            Console.WriteLine("There is no such product");
            return;
        }
        produs.AdaugaStoc(cantitate);
        Console.WriteLine($"New stock added: {numeProdus} + {cantitate} pieces");
    }

    public void VindeProdus(string idAgent, string numeProdus, int cantitate)
    {
        Employee angajat = GasesteAngajat(idAgent);
        if (angajat == null)
        {
            Console.WriteLine("Employee doesn't exist!");
            return;
        }

        if (!(angajat is SalesAgent))
        {
            Console.WriteLine(angajat.Nume + " is not a SalesAgent!");
            return;
        }

        SalesAgent agent = (SalesAgent)angajat;

        Product produs = GasesteProdus(numeProdus);
        if (produs == null)
        {
            Console.WriteLine("Product doesn't exist!");
            return;
        }

        agent.VindeProdus(produs, cantitate, this);
    }

    

    public void AfiseazaRaportGeneral()
    {
        Console.WriteLine("\n=== REPORT: " + Nume + " ===");
        Console.WriteLine("Employees: " + _employeeRepository.Count);
        Console.WriteLine("Machines:   " + _machineRepository.Count);
        Console.WriteLine("Products:  " + _productRepository.Count);
        Console.WriteLine("Orders:  " + _orderRepository.Count);
        Console.WriteLine("Total Revenue: " + _totalRevenue + " RON");
        Console.WriteLine("Total Units Sold: " + _totalSalesQuantity);
        Console.WriteLine("Machines requiring maintenance: " + GetMachinesRequiringMaintenance(7).Count);
        Console.WriteLine("Products below stock threshold: " + GetLowStockProducts().Count);
        Console.WriteLine("");
    }

    

    public void RecordSale(string productName, int quantity, decimal unitPrice)
    {
        decimal saleAmount = quantity * unitPrice;
        _totalRevenue += saleAmount;
        _totalSalesQuantity += quantity;

        Product p = GasesteProdus(productName);
        if (p != null)
        {
            p.VindeStoc(quantity);
            Console.WriteLine("Sale recorded: " + quantity + "x " + productName + " = " + saleAmount + " RON");
            DisplayInventoryAlert(p);
        }
    }

    public decimal GetTotalRevenue()
    {
        return _totalRevenue;
    }

    public int GetTotalSalesQuantity()
    {
        return _totalSalesQuantity;
    }

    public decimal CalculateProfit()
    {
        decimal totalCost = _productRepository
            .GetAll()
            .Sum(product => product.ProductionCost * (1000 - product.Cantitate));

        return _totalRevenue - totalCost;
    }

    public List<Machine> GetMachinesRequiringMaintenance(int daysAhead = 7)
    {
        return _machineRepository
            .GetAll()
            .Where(machine => machine.EstimateDaysUntilMaintenance() <= daysAhead)
            .ToList();
    }
    public void IncarcaMasini()
    {
        _machineRepository.LoadMachines();
    }

    public void IncarcaProduse()
    {
        _productRepository.LoadProducts();
    }

    public void SalveazaMasini()
    {
        _machineRepository.SaveAllMachines();
    }

    public void SalveazaProduse()
    {
        _productRepository.SaveAllProducts();
    }

    public void AfiseazaMentenantaPredictiva(int daysAhead = 7)
    {
        List<Machine> machines = GetMachinesRequiringMaintenance(daysAhead);
        Console.WriteLine("\n=== PREDICTIVE MAINTENANCE ===");

        if (machines.Count == 0)
        {
            Console.WriteLine($"No machines require maintenance in the next {daysAhead} days.");
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume}: maintenance due in {machine.EstimateDaysUntilMaintenance()} day(s)."));
    }

    public void AfiseazaDashboardEficienta() 
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine("\n=== PRODUCTION EFFICIENCY DASHBOARD ===");

        if (machines.Count == 0)
        {
            Console.WriteLine("There are no machines!");
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume}: {machine.CalculateEfficiencyPercentage():F2}% efficiency, {machine.ProductionCycles} production cycle(s)."));
        Console.WriteLine($"Average efficiency: {machines.Average(machine => machine.CalculateEfficiencyPercentage()):F2}%");
    }

    public void AfiseazaStareMasini()
    {
        List<Machine> machines = _machineRepository.GetAll();
        Console.WriteLine("\n=== MACHINE HEALTH MONITORING ===");

        if (machines.Count == 0)
        {
            Console.WriteLine("There are no machines!");
            return;
        }

        machines.ForEach(machine => Console.WriteLine(
            $"{machine.SerialNumber} - {machine.Nume} | {machine.Conditie} | {machine.GetHealthAlert()}"));
    }

    public List<Product> GetLowStockProducts(int threshold = 5)
    {
        return _productRepository
            .GetAll()
            .Where(product => product.Cantitate <= threshold)
            .ToList();
    }

    public void AfiseazaAlerteInventar(int threshold = 5)
    {
        Console.WriteLine("\n=== INVENTORY ALERTS ===");
        List<Product> products = GetLowStockProducts(threshold);

        if (products.Count == 0)
        {
            Console.WriteLine($"All products are above the stock threshold of {threshold}.");
            return;
        }

        products.ForEach(product => DisplayInventoryAlert(product, threshold));
    }

    private static void DisplayInventoryAlert(Product product, int threshold = 5)
    {
        if (product.Cantitate <= threshold)
            Console.WriteLine($"ALERT: {product.Nume} stock is low ({product.Cantitate} remaining; threshold: {threshold}).");
    }

    public void AfiseazaRaportVanzari()
    {
        Console.WriteLine("\n=== SALES REPORT: " + Nume + " ===");
        Console.WriteLine("Total Revenue: " + _totalRevenue + " RON");
        Console.WriteLine("Total Units Sold: " + _totalSalesQuantity);
        Console.WriteLine("Average Price Per Unit: " + (_totalSalesQuantity > 0 ? (_totalRevenue / _totalSalesQuantity).ToString("F2") : "N/A") + " RON");
        Console.WriteLine("Estimated Profit: " + CalculateProfit() + " RON");
        Console.WriteLine("");
    }

    public void AfiseazaComenzi()
    {
        _orderRepository.DisplayAll();
    }

    public void AfiseazaComenziSortedByPriority()
    {
        List<ProductionOrder> comenziSortate = _orderRepository.GetSortedByPriority();

        if (comenziSortate.Count == 0)
        {
            Console.WriteLine("There are no orders!");
            return;
        }

        Console.WriteLine("=== Orders sorted by priority ===");
        foreach (var comanda in comenziSortate)
        {
            comanda.Afiseaza();
        }
    }

    public ProductionOrder GetNextPriorityOrder(string idOperator)
    {
        // reload orders to make sure we consider persisted orders
        LoadOrdersFromFile();

        Employee angajat = GasesteAngajat(idOperator);
        if (angajat == null)
            return null;

        if (!(angajat is MachineOperator))
            return null;

        return _orderRepository.GetNextByPriority();
    }
}
