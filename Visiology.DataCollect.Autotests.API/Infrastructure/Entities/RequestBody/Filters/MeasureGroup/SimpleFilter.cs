using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;

namespace Visiology.DataCollect.Autotests.API.Infrastructure.Entities.RequestBody.Filters.MeasureGroup
{
    /// <summary>
    /// Класс, описывающий простой фильтр в теле запроса
    /// </summary>
    public class SimpleFilter
    {
        /// <summary>
        /// Значение для фильтрации
        /// </summary>
        public object value { get; set; }

        /// <summary>
        /// Тип фильтра
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SimpleFilterType type { get; set; }

        /// <summary>
        /// Условие фильтрации
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FilterCondition condition { get; set; }
    }
}

