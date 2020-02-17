namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Условие фильтрации
    /// </summary>
    public enum FilterCondition : short
    {
        equals,
        notequals,
        greater,
        greaterorequals,
        less,
        lessorequals,
        contains
    }
}
