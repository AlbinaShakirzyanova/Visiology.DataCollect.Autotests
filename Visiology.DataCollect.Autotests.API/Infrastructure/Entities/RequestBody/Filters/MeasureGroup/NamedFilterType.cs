namespace Visiology.DataCollect.Autotests.API.Infrastructure.Entities.RequestBody.Filters.MeasureGroup
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
        MeasureName,

        /// <summary>
        /// По уникальному идентификатору атрибута группы показателей
        /// </summary>
        AttributeId,

        /// <summary>
        /// По наименованию атрибута группы показателей
        /// </summary>
        AttributeName
    }
}
