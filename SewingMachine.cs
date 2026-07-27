using System;

public class SewingMachine : Machine
{
    public SewingMachine(string serial, string nume, DateTime dataFabricatiei)
        : base(serial, nume, dataFabricatiei)
    {
    }

    public override void Produce()
    {
        if (Status != MachineStatus.Running)
        {
            Console.WriteLine(Nume + " isn't started!");
            return;
        }
        Console.WriteLine(Nume + " sews the material .");
        DegradeazaConditia();
        StareVerificarePiesa();
        RegisterProductionCycle();
    }

    public override string RunDiagnostics()
    {
        if (Conditie == MachineCondition.Critical)
            return "WARNING: The needle tension is irregular!";
        else
            return "Needle and thread checked. Working normally.";
    }
}
