using System;
using SmartFactorySimple;

public class Technician : Employee
{
    public Technician(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.Technician;
    }

    public bool Repara(Machine masina)
    {
        if (masina.Status == MachineStatus.Running)
        {
            Console.WriteLine(Nume + " he can't fix a running machine!");
            return false;
        }

        for (int i = 0; i < masina.NrPiese; i++)
        {
            if (!masina.Piese[i].EFunctionala)
            {
                masina.Piese[i].Inlocuieste();
                Console.WriteLine("Part " + masina.Piese[i].Nume + " was replaced.");
            }
        }

        masina.RestoreazaConditia();
        Console.WriteLine(Nume + " fixed the machine " + masina.Nume);
        Logging.Log(Id, $"Repaired machine {masina.SerialNumber}");
        return true;
    }
//com
    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Technician) repares the machines.");
    }
}
