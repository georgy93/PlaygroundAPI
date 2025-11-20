namespace Playground.Persistence.Mongo;

using System.Reflection;

internal static class EntitiesConfigurator
{
    /// <summary>
    /// Configures all mappings for database entities
    /// </summary>
    public static void Apply() => Assembly
        .GetExecutingAssembly()
        .DefinedTypes
        .Where(x => typeof(IEntityConfiguration).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
        .Select(x => Activator.CreateInstance(x))
        .Cast<IEntityConfiguration>()
        .ToList()
        .ForEach(configuration => configuration.Apply());
}

internal interface IEntityConfiguration
{
    void Apply();
}