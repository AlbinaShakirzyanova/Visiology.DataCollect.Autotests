using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups
{
    /// <summary>
    /// Описание групп показателей
    /// </summary>
    public class MeasureGroupsListDto : IResponseContentList<MeasureGroupDto>
    {
        /// <summary>
        /// Описание групп показателей
        /// </summary>
        [JsonProperty(PropertyName = "MeasureGroups")]
        public IEnumerable<MeasureGroupDto> Entities { get; set; } = new List<MeasureGroupDto>();
    }   
}
