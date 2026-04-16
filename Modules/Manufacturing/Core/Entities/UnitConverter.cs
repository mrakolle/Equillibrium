namespace Equillibrium.Core.Helpers;

public static class UnitConverter
{
    public static decimal Convert(decimal value, string fromUnit, string toUnit)
    {
        // Example: logic for kg to g, tons to kg, etc.
        return (fromUnit.ToLower(), toUnit.ToLower()) switch
        {
            ("ton", "kg") => value * 1000,
            ("kg", "g") => value * 1000,
            ("l", "ml") => value * 1000,
            ("kg", "ton") => value / 1000,
            _ => value // If units match or are unknown, return as is
        };
    }
}
