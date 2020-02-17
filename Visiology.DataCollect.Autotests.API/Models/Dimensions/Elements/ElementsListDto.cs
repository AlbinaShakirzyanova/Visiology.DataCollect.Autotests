using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements
{
    /// <summary>
    /// Список элементов измерения
    /// </summary>
    public class ElementsListDto : IResponseContentList<ElementDto>
    {
        /// <summary>
        /// Описание измерения
        /// </summary>
        public DimensionDto Dimension { get; set; }

        /// <summary>
        /// Список элементов
        /// </summary>
        [JsonProperty(PropertyName = "Elements")]
        public IEnumerable<ElementDto> Entities { get; set; } = new List<ElementDto>();
    }    
}
