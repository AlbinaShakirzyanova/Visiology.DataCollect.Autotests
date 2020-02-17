using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Forms
{
    /// <summary>
    /// Список форм группы показателей
    /// </summary>
    public class FormsListDto : IResponseContentList<FormDto>
    {
        /// <summary>
        /// Список форм
        /// </summary>
        [JsonProperty(PropertyName = "Forms")]
        public IEnumerable<FormDto> Entities { get; set; } = new List<FormDto>();
    }
}
