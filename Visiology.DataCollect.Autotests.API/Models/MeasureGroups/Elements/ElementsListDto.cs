using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements
{
    public class ElementsListDto : IResponseContentList<ElementDto>
    {
        /// <summary>
        /// Описание группы показателей
        /// </summary>
        public MeasureGroupDto MeasureGroup { get; set; }

        /// <summary>
        /// Список элементов
        /// </summary>
        [JsonProperty(PropertyName = "Elements")]
        public IEnumerable<ElementDto> Entities { get; set; }
    }
}
