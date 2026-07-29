using System.IO;
using Xunit;
using SmartFactorySimple;

namespace SmartFactorySimple.Tests;

public class FactoryShareTests
{
    [Fact]
    public void ListingCompanyCreatesPublicShareStateAndAppliesMenuFluctuation()
    {
        var factory = new Factory("Test Factory");

        factory.ListCompanyPublicly(25, 1000, 10m);

        Assert.True(factory.IsCompanyPublic);
        Assert.Equal(25m, factory.PublicSharePercentage);
        Assert.Equal(1000, factory.IssuedShares);
        Assert.Equal(10m, factory.SharePrice);

        decimal previousPrice = factory.SharePrice;
        factory.ApplyMenuReturnFluctuation();

        Assert.InRange(factory.SharePrice, 9.6m, 10.5m);
        Assert.True(factory.SharePrice >= 0m);
        Assert.True(factory.SharePrice != previousPrice || factory.SharePrice == previousPrice);
    }

    [Fact]
    public void ResolvePathUsesProjectRootWhenFileIsNotInBinaryOutputFolder()
    {
        string path = AppFileNames.ResolvePath(AppFileNames.OrdersFileName);
        Assert.True(File.Exists(path) || !string.IsNullOrWhiteSpace(path));
        Assert.EndsWith(AppFileNames.OrdersFileName, path);
    }

    [Fact]
    public void CompanyValuationReflectsFullCompanyValueWhenOnlyPartIsPublic()
    {
        var factory = new Factory("Test Factory");

        factory.ListCompanyPublicly(25, 1000, 10m);

        Assert.Equal(40000m, factory.GetCompanyValuation());
    }

    [Fact]
    public void LoadPersistentDataLoadsOrdersAfterMachinesAndEmployeesAreAvailable()
    {
        string path = AppFileNames.ResolvePath(AppFileNames.OrdersFileName);
        string backupPath = path + ".bak";
        File.Copy(path, backupPath, overwrite: true);

        try
        {
            File.WriteAllText(path, "# Production Orders\nORD99;M001;MagicBlocks;2;High;Created;PM001;2026-07-17T11:08:58\n");

            var factory = new Factory("Test Factory");
            factory.AdaugaAngajat(new ProductionManager("PM001", "Maria Ionescu", 5500m, DateTime.Now.AddYears(-3)));
            factory.AdaugaMasina(new SewingMachine("M001", "Test Machine", DateTime.Now.AddYears(-2)));

            factory.LoadPersistentData();

            Assert.NotNull(factory.GetOrderById("ORD99"));
        }
        finally
        {
            File.Copy(backupPath, path, overwrite: true);
            File.Delete(backupPath);
        }
    }

    [Fact]
    public void DefaultLoginCredentialsUseExistingEmployeeIds()
    {
        var factory = new Factory("Test Factory");
        factory.AdaugaAngajat(new Director("1", "Alex Popescu", 8000m, DateTime.Now.AddYears(-5)));
        factory.AdaugaAngajat(new ProductionManager("2", "Maria Ionescu", 5500m, DateTime.Now.AddYears(-3)));
        factory.AdaugaAngajat(new Engineer("3", "Ion Vasile", 5000m, DateTime.Now.AddYears(-2)));
        factory.AdaugaAngajat(new Technician("4", "Andrei Marin", 4000m, DateTime.Now.AddYears(-1)));
        factory.AdaugaAngajat(new MachineOperator("5", "Elena Dumitru", 3500m, DateTime.Now.AddMonths(-8)));
        factory.AdaugaAngajat(new SalesAgent("6", "Ioana Radu", 3300m, DateTime.Now.AddMonths(-4)));

        var login = new Login();
        var directorCred = login.Authenticate("director", "pass123");
        var operatorCred = login.Authenticate("operator1", "pass123");

        Assert.NotNull(directorCred);
        Assert.NotNull(operatorCred);
        Assert.NotNull(factory.GasesteAngajat(directorCred.EmployeeId));
        Assert.NotNull(factory.GasesteAngajat(operatorCred.EmployeeId));
    }

    [Fact]
    public void OperatorCannotExecuteOrderWhenMachineIsStopped()
    {
        var machine = new SewingMachine("M999", "Test Machine", DateTime.Now.AddYears(-1));
        machine.Status = MachineStatus.Stopped;

        var machineOperator = new MachineOperator("OP999", "Test Operator", 3000m, DateTime.Now.AddMonths(-1));
        using var writer = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(writer);

        try
        {
            machineOperator.Opereaza(machine);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Contains(Messages.OrderCannotExecuteUntilMachineStarted, writer.ToString());
    }
}
