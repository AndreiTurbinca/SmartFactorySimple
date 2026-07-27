using System;

public class Director : Employee
{
    public Director(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.Director;
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (The director) checks the factory reports.");
    }

    public override void Afiseaza()
    {
        Console.WriteLine("[" + Id + "] " + Nume +
                          " - Director" +
                          " - Salary " + Salariu +
                          " - Perios of Activity: " + GetVechimeZile() + " days");
    }
}
