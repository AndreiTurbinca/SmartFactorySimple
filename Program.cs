using SmartFactorySimple;
using System;

class Program
{
    static Factory fabrica = new Factory("TOYS R US");
    static Login.EmployeeCredential loggedInUser;
    static Login loginManager;

    static void Main()
    {
        // Authentication - Login required
        loginManager = new Login();
        loggedInUser = loginManager.LoginWithAttempts(3);

        if (loggedInUser == null)
        {
            return; // Exit if authentication fails
        }

        DateDemo();
        // Load persisted orders from orders.txt if present
        fabrica.LoadOrdersFromFile();
        fabrica.IncarcaMasini();
        fabrica.IncarcaProduse();

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== SMART FACTORY ===========");
            Console.WriteLine($"Logged in as: {loggedInUser.Username} ({loggedInUser.Role})");
            EmployeeRole rolCurent;
            if (!Enum.TryParse(loggedInUser.Role, out rolCurent))
            {
                Console.WriteLine("Rol necunoscut in baza de date!");
                return;
            }


            switch (rolCurent)
            {
                case EmployeeRole.Director:
                    running = MeniuDirector();
                    break;
                case EmployeeRole.ProductionManager:
                    running = MeniuProductionManager1();
                    break;
                case EmployeeRole.Engineer:
                    running = MeniuEngineer();
                    break;
                case EmployeeRole.Technician:
                    running = MeniuTechnician();
                    break;
                case EmployeeRole.MachineOperator:
                    running = MeniuMachineOperator();
                    break;
                case EmployeeRole.SalesAgent:
                    running = MeniuSalesAgent();
                    break;
                default:
                    Console.WriteLine("Rol necunoscut!");
                    running = false;
                    break;
            }
        }

        Console.WriteLine("Good Bye!");
        Console.WriteLine("Se salveaza masinile si produsele...");
        fabrica.SalveazaMasini();
        fabrica.SalveazaProduse();
    }



    static bool MeniuDirector()
    {
        Console.WriteLine("1. Employees");
        Console.WriteLine("2. Machines");
        Console.WriteLine("3. Products");      
        Console.WriteLine("4. Sales");      
        Console.WriteLine("5. Show Operation Logs");
        Console.WriteLine("6. Log out");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuAngajati(); break;
            case "2":
                fabrica.AfiseazaMasini();
                Console.WriteLine("He can only see the machines");
                break;
            case "3":
                fabrica.AfiseazaProduse();
                Console.WriteLine("He can only see the products");
                break;           
            case "4":
                Console.WriteLine("Sales");
                Console.WriteLine("1.View Sales Report");
                Console.WriteLine("2.View General Report");
                string opt = Console.ReadLine();
                switch (opt)
                {
                    case "1": fabrica.AfiseazaRaportVanzari(); break;
                    case "2": fabrica.AfiseazaRaportGeneral(); break;
                    default: Console.WriteLine("Choose one of this options"); break;
                }

                break;
            case "5": ShowOperationLogs(); break;
            case "6": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }

    /*static bool MeniuProductionManager()
    {
        Console.WriteLine("1. Show all employees");
        Console.WriteLine("2. Machines");
        Console.WriteLine("3. Products");
        Console.WriteLine("4. Production");
        Console.WriteLine("5. General Report");
        Console.WriteLine("6. Log out");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": fabrica.AfiseazaAngajati(); break;
            case "2": MeniuMasiniProductionManager(); break;
            case "3": MeniuProduse(); break;
            case "4": MeniuProductie(); break;
            case "5": fabrica.AfiseazaRaportGeneral(); break;
            case "6": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }
    */
    static bool MeniuProductionManager1()
    {
        Console.WriteLine("1. Show all employees");
        Console.WriteLine("2. Machines");
        Console.WriteLine("3. Products");
        Console.WriteLine("4. Production");
        Console.WriteLine("5. General Report");
        Console.WriteLine("6. Log out");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": fabrica.AfiseazaAngajati(); break;
            case "2": MeniuMasiniProductionManager(); break;
            case "3": fabrica.AfiseazaProduse(); break;
            case "4": MeniuProductieManager(); break;
            case "5": fabrica.AfiseazaRaportGeneral(); break;
            case "6": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;

    }
    static void MeniuProductieManager()
    {
        Console.WriteLine("\n--- PRODUCTIE ---");
        Console.WriteLine("1. Create order");
        Console.WriteLine("2. Show orders");
        Console.WriteLine("3. Show orders sorted by priority");
        Console.Write("Choose: ");

        string alegere = Console.ReadLine();

        if (alegere == "1")
            CreazaComandaMan();
        else if (alegere == "2")
            fabrica.AfiseazaComenzi();
        else if (alegere == "3")
            fabrica.AfiseazaComenziSortedByPriority();

    }
    static void MeniuMasiniProductionManager()
    {
        Console.WriteLine("\n--- MACHINES ---");
        Console.WriteLine("1. Add a machine");
        Console.WriteLine("2. Show all");
        //Console.WriteLine("3. Stop a machine");
        //Console.WriteLine("4. Start a machine");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
        {
            AdaugaMasina();
        }
        else if (alegere == "2")
        {
            fabrica.AfiseazaMasini();
        }
        /*else if (alegere == "3")                                                  
        {
            fabrica.AfiseazaMasini();
            Console.Write("Serial number for the machine you want to stop: ");
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine("Machine doesn't exist!");
            else
                m.Stop();
        }
        else if (alegere == "4")
        {
            fabrica.AfiseazaMasini();
            Console.Write("Serial number for the machine you want to start: ");
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine("Machine doesn't exist!");
            else
                m.Start();
        }
        */
    }
    static bool MeniuEngineer()
    {
        Console.WriteLine("1. Show all machines");       
        Console.WriteLine("2. Show when a machine may require maintenance ");
        Console.WriteLine("3. Machine health monitoring");
        Console.WriteLine("4. Log out");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": fabrica.AfiseazaMasini(); break;
            case "2": fabrica.AfiseazaMentenantaPredictiva(); break;
            case "3": fabrica.AfiseazaStareMasini(); break;
            case "4": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }
   
    static bool MeniuTechnician()
    {
        Console.WriteLine("\n--- TECHNICIAN MENU ---");
        Console.WriteLine("1. Show all machines");
        Console.WriteLine("2. Repare a machine");
        Console.WriteLine("3. Show machines in maintenance");
        Console.WriteLine("4. View machine repair history");
        Console.WriteLine("5. Log out");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1":
                fabrica.AfiseazaMasini();
                break;
            case "2":
                ReparaMasina();
                break;
            case "3":
                fabrica.AfiseazaMasiniInMentenanta();
                break;
            case "4":
                AfiseazaIstoricReparatii();
                break;
            case "5":
                return Logout();
            case "0":
                return false;
            default:
                Console.WriteLine("Invalid option!");
                break;
        }
        return true;
    }

    static bool MeniuMachineOperator()
    {
        Console.WriteLine("1. Production");
        Console.WriteLine("2. Show all machines");
        Console.WriteLine("3. Stop a machine");
        Console.WriteLine("4. Start a machine");
        Console.WriteLine("5. Log out");

        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuProductie1(); break;
            case "2": fabrica.AfiseazaMasini(); break;
            case "3": StopMachine(); break;
            case "4": StartMachine(); break;
            case "5": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }

    static void MeniuProductie1()
    {
        Console.WriteLine("\n--- PRODUCTIE ---");
        Console.WriteLine("1. Execute Order (manualy)");
        Console.WriteLine("2. Execute the next priority order (auto)");
      
        Console.Write("Alege: ");
        string alegere = Console.ReadLine();

         if (alegere == "1")
            ExecutaComanda();
        else if (alegere == "2")
            ExecutaComanaPrioritara();
    
    }

    static bool MeniuSalesAgent()
    {
        Console.WriteLine("\n--- SALES AGENT MENU ---");
        Console.WriteLine("1. Sales");
        Console.WriteLine("2. Show all products");
        Console.WriteLine("3. Show raw materials stock"); // Opțiunea nouă adăugată
        Console.WriteLine("4. Log out");
        Console.WriteLine("0. Exit");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        switch (alegere)
        {
            case "1": MeniuVanzari(); break;
            case "2": fabrica.AfiseazaProduse(); break;
            case "3": fabrica.AfiseazaStocMaterialePrime(); break; // Apelăm funcția din Factory
            case "4": return Logout();
            case "0": return false;
            default: Console.WriteLine("Invalid option!"); break;
        }
        return true;
    }
    static void StopMachine()
    {
        fabrica.AfiseazaMasini();
        Console.Write("Serial number for the machine you want to stop: ");
        string serial = Console.ReadLine();
        Machine m = fabrica.GasesteMasina(serial);
        if (m == null)
            Console.WriteLine("Machine doesn't exist!");
        else
            m.Stop();
    }
    static void StartMachine()
    {
        fabrica.AfiseazaMasini();
        Console.Write("Serial number for the machine you want to start: ");
        string serial = Console.ReadLine();
        Machine m = fabrica.GasesteMasina(serial);
        if (m == null)
            Console.WriteLine("Machine doesn't exist!");
        else
            m.Start();
    }
    static void ShowOperationLogs()
    {


        Console.WriteLine("\n=== Operation History ===");
        string[] entries = Logging.GetAllEntries();
        if (entries.Length == 0)
        {
            Console.WriteLine("No operation logs available.");
        }
        else
        {
            foreach (string entry in entries)
            {
                Console.WriteLine(entry);
            }
        }
        Console.WriteLine("=========================");
    }

    // Logout and re-authenticate. Returns true to continue running, false to exit application.
    static bool Logout()
    {
        Console.WriteLine("\nLogging out...");
        if (loggedInUser != null)
        {
            Logging.Log(loggedInUser.Username, "User logged out");
        }

        loggedInUser = loginManager.LoginWithAttempts(3);
        if (loggedInUser == null)
        {
            Console.WriteLine("Authentication failed. Exiting application.");
            return false;
        }

        Console.WriteLine($"Successfully logged in as: {loggedInUser.Username} ({loggedInUser.Role})");
        return true;
    }

    // ===== MENIU ANGAJATI =====

    static void MeniuAngajati()
    {
        Console.WriteLine("\n--- Employees ---");
        Console.WriteLine("1. Add employee");
        Console.WriteLine("2. Show all employees");
        Console.WriteLine("3. Delete employee");
        Console.WriteLine("4. The employee is doing their duty");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
        {
            AdaugaAngajat();
        }
        else if (alegere == "2")
        {
            fabrica.AfiseazaAngajati();
        }
        else if (alegere == "3")
        {
            fabrica.AfiseazaAngajati();
            Console.Write("Employee ID to delete: ");
            string id = Console.ReadLine();
            fabrica.StergeAngajat(id);
        }
        else if (alegere == "4")
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID employee: ");
            string id = Console.ReadLine();
            Employee ang = fabrica.GasesteAngajat(id);
            if (ang == null)
                Console.WriteLine("Employee doesn't exist!");
            else
                ang.PerformDuty();
        }
    }

    static void AdaugaAngajat()
    {
        Console.Write("ID: ");
        string id = Console.ReadLine();
        if (fabrica.EmployeeIdExists(id))
        {
            Console.WriteLine($"Employee ID {id} already exists. Please choose a unique ID.");
            return;
        }

        Console.Write("Name: ");
        string nume = Console.ReadLine();
        Console.Write("Salary: ");
        decimal salariu = decimal.Parse(Console.ReadLine());

        Console.WriteLine("Employee Type:");
        Console.WriteLine("1. Director");
        Console.WriteLine("2. ProductionManager");
        Console.WriteLine("3. Engineer");
        Console.WriteLine("4. Technician");
        Console.WriteLine("5. MachineOperator");
        Console.WriteLine("6. SalesAgent");
        Console.Write("Choose: ");
        string tip = Console.ReadLine();

        Employee angajat = null;
        string role = null;

        if (tip == "1")
        {
            angajat = new Director(id, nume, salariu, DateTime.Now);
            role = "Director";
        }
        else if (tip == "2")
        {
            angajat = new ProductionManager(id, nume, salariu, DateTime.Now);
            role = "ProductionManager";
        }
        else if (tip == "3")
        {
            angajat = new Engineer(id, nume, salariu, DateTime.Now);
            role = "Engineer";
        }
        else if (tip == "4")
        {
            angajat = new Technician(id, nume, salariu, DateTime.Now);
            role = "Technician";
        }
        else if (tip == "5")
        {
            angajat = new MachineOperator(id, nume, salariu, DateTime.Now);
            role = "MachineOperator";
        }
        else if (tip == "6")
        {
            angajat = new SalesAgent(id, nume, salariu, DateTime.Now);
            role = "SalesAgent";
        }
        else
        {
            Console.WriteLine("Invalid user!");
            return;
        }

        // Ask for login credentials
        Console.Write("Username for login: ");
        string username = Console.ReadLine();
        Console.Write("Password for login: ");
        string password = Console.ReadLine();

        if (fabrica.AdaugaAngajat(angajat))
        {
            // Save credentials to file
            if (loginManager.SaveEmployeeCredential(id, username, password, role))
            {
                Console.WriteLine("Employee added successfully!");
            }
            else
            {
                Console.WriteLine("Employee added but failed to save credentials!");
            }
        }
    }
    static void AfiseazaIstoricReparatii()
    {
        Console.WriteLine("\n=== REPAIR HISTORY ===");
        Console.Write("Enter Machine Serial Number (or press Enter to see all): ");
        string serialCautat = Console.ReadLine()?.Trim();

       
        string[] entries = Logging.GetAllEntries();
        bool found = false;

        if (entries != null && entries.Length > 0)
        {
            foreach (string entry in entries)
            {
               
                bool isRepairLog = entry.Contains("Repaired", StringComparison.OrdinalIgnoreCase);



                
                bool matchesSerial = string.IsNullOrEmpty(serialCautat) ||
                                     entry.Contains(serialCautat, StringComparison.OrdinalIgnoreCase);

                if (isRepairLog && matchesSerial)
                {
                    Console.WriteLine(entry);
                    found = true;
                }
            }
        }

        if (!found)
        {
            Console.WriteLine("No repair history found for the specified criteria.");
        }
        Console.WriteLine("======================");
    }

    // ===== MENIU MASINI =====

    static void MeniuMasini()
    {
        Console.WriteLine("\n--- MACHINES ---");
        Console.WriteLine("1. Add a machine");
        Console.WriteLine("2. Show all");
        Console.WriteLine("3. Stop a machine");
        Console.WriteLine("4. Repair a machine");
        Console.WriteLine("5. Start a machine");
        Console.WriteLine("6. Predictive maintenance");
        Console.WriteLine("7. Production efficiency dashboard");
        Console.WriteLine("8. Machine health monitoring");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
        {
            AdaugaMasina();
        }
        else if (alegere == "2")
        {
            fabrica.AfiseazaMasini();
        }
        else if (alegere == "3")
        {
            fabrica.AfiseazaMasini();
            Console.Write("Serial number for the machine you want to stop: ");
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine("Machine doesn't exist!");
            else
                m.Stop();
        }
        else if (alegere == "4")
        {
            ReparaMasina();
        }
        else if (alegere == "5")
        {
            fabrica.AfiseazaMasini();
            Console.Write("Serial number for the machine you want to start: ");
            string serial = Console.ReadLine();
            Machine m = fabrica.GasesteMasina(serial);
            if (m == null)
                Console.WriteLine("Machine doesn't exist!");
            else
                m.Start();
        }
        else if (alegere == "6")
        {
            fabrica.AfiseazaMentenantaPredictiva();
        }
        else if (alegere == "7")
        {
            fabrica.AfiseazaDashboardEficienta();
        }
        else if (alegere == "8")
        {
            fabrica.AfiseazaStareMasini();
        }
    }

    static void AdaugaMasina()
    {
        Console.Write("Serial number: ");
        string serial = Console.ReadLine();
        Console.Write("Name: ");
        string nume = Console.ReadLine();

        Console.WriteLine("Machine type:");
        Console.WriteLine("1. SewingMachine");
        Console.WriteLine("2. CuttingMachine");
        Console.Write("Choose: ");
        string tip = Console.ReadLine();

        Machine masina = null;

        if (tip == "1")
            masina = new SewingMachine(serial, nume, DateTime.Now);
        else if (tip == "2")
            masina = new CuttingMachine(serial, nume, DateTime.Now);
        else
        {
            Console.WriteLine("Invalid user!");
            return;
        }

        Console.Write("Add a part? (yes/no): ");
        string raspuns = Console.ReadLine();
        if (raspuns == "yes")
        {
            Console.Write("Part name: ");
            string numePiesa = Console.ReadLine();
            Console.Write("Type of part (Engine/Needle/Blade): ");
            string tipPiesa = Console.ReadLine();
            masina.AdaugaPiesa(new MachinePart(numePiesa, tipPiesa));
        }

        if (fabrica.AdaugaMasina(masina))
        {
            Console.WriteLine("Car added successfully!");
            
            fabrica.SalveazaMasini();
        }
    }

    static void ReparaMasina()
    {
     
        string idTeh = loggedInUser.EmployeeId;

        fabrica.AfiseazaAngajati();

       
        Console.Write("ID Engineer: ");
        string idEng = Console.ReadLine();

        fabrica.AfiseazaMasini();
        Console.Write("Serial masina: ");
        string serial = Console.ReadLine();

       
        fabrica.ReparaMasina(idTeh, idEng, serial);
    }


    // ===== MENIU PRODUSE =====

    static void MeniuProduse()
    {
        Console.WriteLine("\n--- PRODUCTS ---");
        Console.WriteLine("1. Add a product");
        Console.WriteLine("2. Show all products");
        Console.WriteLine("3. Add Stock ");
        Console.WriteLine("4. Sell a product");
        Console.WriteLine("5.Show production efficiency dashboard");
        Console.WriteLine("6. Inventory alerts");
        Console.Write("Choose: ");
        string alegere = Console.ReadLine();

        if (alegere == "1")
            AdaugaProdus();
        else if (alegere == "2")
            fabrica.AfiseazaProduse();
        else if (alegere == "3")
            AdaugaStocProdus();
        else if (alegere == "4")
            VandeProdus();
        else if (alegere == "5")
            fabrica.AfiseazaDashboardEficienta();
        else if (alegere == "6")
            fabrica.AfiseazaAlerteInventar();
    }

    static void AdaugaStocProdus()
    {
        fabrica.AfiseazaProduse();
        Console.Write("Product name: ");
        string nume = Console.ReadLine();

            Console.Write("Amount to add: ");
            int cantitate = int.Parse(Console.ReadLine());

            fabrica.AdaugaStocProduse(nume, cantitate);
        }

        static void AdaugaProdus()
        {
            Console.Write("Name: ");
            string nume = Console.ReadLine();
            Console.Write("Production Cost: ");
            decimal productionCost = decimal.Parse(Console.ReadLine());
            Console.Write("Selling Price: ");
            decimal sellingPrice = decimal.Parse(Console.ReadLine());
            Console.Write("Initial Quantity: ");
            int cantitate = int.Parse(Console.ReadLine());

            Console.WriteLine("Product Type:");
            Console.WriteLine("1. Wooden Cubes");
            Console.WriteLine("2. Teddy Bear");
            Console.WriteLine("3. FootBall");
            Console.WriteLine("4. Doll");
            Console.WriteLine("5. Frisbee");

            Console.Write("Alege: ");
            string tip = Console.ReadLine();

            Product produs = null;

            if (tip == "1")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new WoodenCubes(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "2")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new Doll(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "3")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new TedyBear(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "4")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new Ball(nume, productionCost, sellingPrice, cantitate, marime);
            }
            else if (tip == "5")
            {
                Console.Write("Size: ");
                string marime = Console.ReadLine();
                produs = new Frisbee(nume, productionCost, sellingPrice, cantitate, marime);
            }
            if (fabrica.AdaugaProdus(produs))
            {
                Console.WriteLine("Product added successfully!");
                // Persist products immediately
                fabrica.SalveazaProduse();
            }
        }

        static void VandeProdus()
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID Sales Agent: ");
            string idAgent = Console.ReadLine();

            fabrica.AfiseazaProduse();
            Console.Write("Product Name: ");
            string numeProdus = Console.ReadLine();

            Console.Write("Selling Quantity: ");
            int cantitate = int.Parse(Console.ReadLine());

            fabrica.VindeProdus(idAgent, numeProdus, cantitate);
        }

        // ===== MENIU PRODUCTIE =====

        static void MeniuProductie()
        {
            Console.WriteLine("\n--- PRODUCTIE ---");
            Console.WriteLine("1. Creeaza comanda");
            Console.WriteLine("2. Execute Order (manualy)");
            Console.WriteLine("3. Execute the next priority order (auto)");
            Console.WriteLine("4. Show orders");
            Console.WriteLine("5. Show orders sorted by priority");
            Console.Write("Alege: ");
            string alegere = Console.ReadLine();

            if (alegere == "1")
                CreazaComandaMan();
            else if (alegere == "2")
                ExecutaComanda();
            else if (alegere == "3")
                ExecutaComanaPrioritara();
            else if (alegere == "4")
                fabrica.AfiseazaComenzi();
            else if (alegere == "5")
                fabrica.AfiseazaComenziSortedByPriority();
        }

    static void CreazaComandaMan()
    {
       
        string idManager = loggedInUser.EmployeeId;

      
        fabrica.AfiseazaMasini();
        Console.Write("Serial number for a machine: ");
        string serial = Console.ReadLine();

        Console.WriteLine("\nToys available to manufacture: MagicBlocks, Barbie, Barnie, Football, OZN");
        Console.Write("Product name to manufacture (type exactly as above): ");
        string produs = Console.ReadLine();

        Console.Write("Target amount: ");
        int cantitate = int.Parse(Console.ReadLine());

        Console.WriteLine("Priority: 1.Low  2.Medium  3.High");
        Console.Write("Choose: ");
        string prio = Console.ReadLine();

        Priority prioritate;
        if (prio == "1")
            prioritate = Priority.Low;
        else if (prio == "3")
            prioritate = Priority.High;
        else
            prioritate = Priority.Medium;

        fabrica.CreazaComanda(idManager, serial, produs, cantitate, prioritate);
    }

    static void ExecutaComanda()
        {
            //fabrica.AfiseazaAngajati();
            //Console.Write("ID MachineOperator: ");
            string idOp = loggedInUser.EmployeeId;

            fabrica.AfiseazaComenzi();
            Console.Write("ID Order (ex: ORD1): ");
            string idComanda = Console.ReadLine();

            Console.Write("Units to produce now: ");
            int unitati = int.Parse(Console.ReadLine());

            fabrica.ExecutaComanda(idOp, idComanda, unitati);
        }

        static void ExecutaComanaPrioritara()
        {
            fabrica.AfiseazaAngajati();
            Console.Write("ID MachineOperator: ");
            string idOp = Console.ReadLine();

            ProductionOrder nextOrder = fabrica.GetNextPriorityOrder(idOp);
            if (nextOrder == null)
            {
                Console.WriteLine("Nu exista comenzi active sau operatorul nu este valid!");
                return;
            }

            Console.WriteLine("\nUrmatoarea comanda prioritara:");
            nextOrder.Afiseaza();

            Console.Write("Unitati de produs acum: ");
            int unitati = int.Parse(Console.ReadLine());

            fabrica.ExecutaComanda(idOp, nextOrder.Id, unitati);
        }

        static void MeniuVanzari()
        {
            Console.WriteLine("\n--- SALES ---");
            Console.WriteLine("1. Sell Product");
            Console.WriteLine("2. View Sales Report");
            Console.WriteLine("3. View General Report");
            Console.Write("Choose: ");
            string alegere = Console.ReadLine();

            if (alegere == "1")
                VindeProdus();
            else if (alegere == "2")
                fabrica.AfiseazaRaportVanzari();
            else if (alegere == "3")
                fabrica.AfiseazaRaportGeneral();
        }

        static void VindeProdus()
        {
            //fabrica.AfiseazaAngajati();
            //Console.Write("ID SalesAgent: ");
            string idAgent = loggedInUser.EmployeeId;

        Employee ang = fabrica.GasesteAngajat(idAgent);
            if (ang == null || !(ang is SalesAgent))
            {
                Console.WriteLine("Sales Agent not found!");
                return;
            }

            SalesAgent agent = (SalesAgent)ang;

            fabrica.AfiseazaProduse();
            Console.Write("Product name to sell: ");
            string produsNume = Console.ReadLine();

            Product produs = fabrica.GasesteProdus(produsNume);
            if (produs == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.Write("Quantity to sell: ");
            int cantitate = int.Parse(Console.ReadLine());

            agent.VindeProdus(produs, cantitate, fabrica);
        }

        static void DateDemo()
        {
            fabrica.AdaugaAngajat(new Director("DIR001", "Alex Popescu", 8000, DateTime.Now.AddYears(-5)));
            fabrica.AdaugaAngajat(new ProductionManager("PM001", "Maria Ionescu", 5500, DateTime.Now.AddYears(-3)));
            fabrica.AdaugaAngajat(new Engineer("ENG001", "Ion Vasile", 5000, DateTime.Now.AddYears(-2)));
            fabrica.AdaugaAngajat(new Technician("TH001", "Andrei Marin", 4000, DateTime.Now.AddYears(-1)));
            fabrica.AdaugaAngajat(new MachineOperator("OP001", "Elena Dumitru", 3500, DateTime.Now.AddMonths(-8)));
            fabrica.AdaugaAngajat(new SalesAgent("SA001", "Ioana Radu", 3300, DateTime.Now.AddMonths(-4)));

            SewingMachine s1 = new SewingMachine("M001", "Juki Sewing", DateTime.Now.AddYears(-3));
            s1.AdaugaPiesa(new MachinePart("Industrial Needle", "Needle"));
            s1.AdaugaPiesa(new MachinePart("Polyester Thread", "Thread"));
            fabrica.AdaugaMasina(s1);

            CuttingMachine c1 = new CuttingMachine("M002", "Auto Cutter", DateTime.Now.AddYears(-2));
            c1.AdaugaPiesa(new MachinePart("Steel Blade", "Blade"));
            fabrica.AdaugaMasina(c1);

            fabrica.AdaugaProdus(new WoodenCubes("MagicBlocks", 15, 30, 3, "S"));
            fabrica.AdaugaProdus(new Doll("Barbie", 12, 50, 7, "S"));
            fabrica.AdaugaProdus(new TedyBear("Barnie", 20, 60, 15, "M"));
            fabrica.AdaugaProdus(new Ball("Football", 13, 50, 5, "Normal"));
            fabrica.AdaugaProdus(new Frisbee("OZN", 10, 25, 7, "S"));
        fabrica.InitializeazaMaterialeSiRetete();

        

        Console.WriteLine("Demo data loaded! Press Enter to continue...");
            Console.ReadLine();
        }
       
    }
