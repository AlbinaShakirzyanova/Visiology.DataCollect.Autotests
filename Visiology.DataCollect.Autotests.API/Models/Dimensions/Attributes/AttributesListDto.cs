using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Attributes
{
    /// <summary>
    /// Список атрибутов измерения
    /// </summary>
    public class AttributesListDto : IResponseContentList<AttributeDto>
    {
        /// <summary>
        /// Список атрибутов
        /// </summary>
        [JsonProperty(PropertyName = "Attributes")]
        public IEnumerable<AttributeDto> Entities { get; set; } = new List<AttributeDto>();
    }
}
