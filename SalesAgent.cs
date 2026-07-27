using System;
using SmartFactorySimple;

public class SalesAgent : Employee
{
    public SalesAgent(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.SalesAgent;
    }

    public bool VindeProdus(Product produs, int cantitate, Factory fabrica)
    {
        if (produs.Cantitate < cantitate)
        {
            Console.WriteLine("Insufficient stock! Available: " + produs.Cantitate);
            return false;
        }
        fabrica.RecordSale(produs.Nume, cantitate, produs.SellingPrice);
        Console.WriteLine(Nume + " sold " + cantitate + "x " + produs.Nume);
        Logging.Log(Id, $"Sold product {produs.Nume} x{cantitate}");
        return true;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Sales Agent) sell the factory's products.");
    }
}
