using System;

public class MachineOperator : Employee
{
    public MachineOperator(string id, string nume, decimal salariu, DateTime dataAngajarii)
        : base(id, nume, salariu, dataAngajarii)
    {
        Rol = EmployeeRole.MachineOperator;
    }

    public void Opereaza(Machine masina)
    {

        if (masina.Status == MachineStatus.Running)
        {
            masina.Produce();
        }
        else if (masina.Status == MachineStatus.Maintenance)
        {
            Console.WriteLine("The car is in maintenance, wait for it to be repaired");
            return;
        }
        else
        {
            Console.WriteLine("The car is off, you can't execute the command");
            Console.WriteLine("Do you want to start the car? YES/NO");
            string continuare = Console.ReadLine();
            if (continuare == "YES")
            { 
                masina.Status = MachineStatus.Running;
                masina.Produce();
            }
            else if(continuare == "NO")
                {
                 return;
                }

        }
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Nume + " (Machine Operator) operates the machines.");
    }
}
