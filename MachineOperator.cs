using System;
using SmartFactorySimple;

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
            return;
        }

        if (masina.Status == MachineStatus.Maintenance)
        {
            Console.WriteLine(Messages.MachineOperatorMaintenanceMessage);
            return;
        }

        Console.WriteLine(Messages.MachineOperatorOffMessage);
        Console.WriteLine(Messages.OrderCannotExecuteUntilMachineStarted);

        if (masina.ArePieseComplete())
        {
            masina.Start();
            if (masina.Status == MachineStatus.Running)
            {
                masina.Produce();
            }
        }
        else
        {
            Console.WriteLine(Messages.MachineHasBrokenParts(masina.Nume));
        }
    }

    public override void PerformDuty()
    {
        Console.WriteLine(Messages.RoleDuty(Nume, "Machine Operator") + " operates the machines.");
    }
}
