namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities.RequestBody.Filters.MeasureGroup
{
    /// <summary>
    /// Тип фильтрации
    /// </summary>
    public enum NamedFilterType : short
    {
        /// <summary>
        /// По наименованию измерения
        /// </summary>
        DimensionName,

        /// <summary>
        /// По уникальному идентификатору измерения
        /// </summary>
        DimensionId,

        /// <summary>
        /// По уникальному идентификатору группы показателей
        /// </summary>
        MeasureId,

        /// <summary>
        /// По наименованию группы показателей
        /// </summary>
        MeasureName
    }
}
