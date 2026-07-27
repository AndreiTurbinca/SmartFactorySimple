using System;
public class TedyBear : Product
{
    public string Marime;
    public TedyBear(string nume, decimal productionCost, decimal sellingPrice, int cantitate, string marime)
        : base(nume, ProductCategory.PretendToys, productionCost, sellingPrice, cantitate)
    {
        Marime = marime;
    }
    public override string GetDescription()
    {
        
        return "Teddy Bear:" + Nume + " " + Category + " Size " + Marime;
    }
}


