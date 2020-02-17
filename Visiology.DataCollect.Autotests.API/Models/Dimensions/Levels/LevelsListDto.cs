using Newtonsoft.Json;
using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Levels
{
    /// <summary>
    /// Список уровней измерения
    /// </summary>
    public class LevelsListDto : IResponseContentList<LevelDto>
    {
        /// <summary>
        /// Список уровней
        /// </summary>
        [JsonProperty(PropertyName="Levels")]
        public IEnumerable<LevelDto> Entities { get; set; } = new List<LevelDto>();
    }   
}
