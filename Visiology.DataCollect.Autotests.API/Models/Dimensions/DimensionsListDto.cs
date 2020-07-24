using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Autotests.API.Models.Dimensions;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions
{
    /// <summary>
    /// Описание измерений
    /// </summary>
    public class DimensionsListDto : IResponseContentList<DimensionDto>
    {
        /// <summary>
        /// Описание измерений
        /// </summary>
        [JsonProperty(PropertyName = "Dimensions")]
        public IEnumerable<DimensionDto> Entities { get; set; } = new List<DimensionDto>();
    }
}
