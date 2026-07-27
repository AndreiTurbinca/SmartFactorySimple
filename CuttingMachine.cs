using System;

public class CuttingMachine : Machine
{
    public CuttingMachine(string serial, string nume, DateTime dataFabricatiei)
        : base(serial, nume, dataFabricatiei)
    {
    }

    public override void Produce()
    {
        if (Status != MachineStatus.Running)
        {
            Console.WriteLine(Nume + " it's not on, it can't produce!");
            return;
        }
        Console.WriteLine(Nume + "Cuts the material according to the patterns.");
        DegradeazaConditia();
        StareVerificarePiesa();
        RegisterProductionCycle();

    }

    public override string RunDiagnostics()
    {
        if (Conditie == MachineCondition.Critical)
            return "WARNING: The blade is dull, needs replacing!";
        else
            return "Sharp blade. Normal operation.";
    }
}
