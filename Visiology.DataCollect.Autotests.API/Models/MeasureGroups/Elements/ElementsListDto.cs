using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
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
        public IEnumerable<ElementDto> Entities { get; set; } = new List<ElementDto>();

        public bool Equals([AllowNull] ElementsListDto other)
        {
            var dto = other as ElementsListDto;
            return dto != null &&
                   MeasureGroup.Equals(dto.MeasureGroup) &&
                   Entities.SequenceEqual(dto.Entities);
        }

        public override int GetHashCode()
        {
            var hashCode = -2007856361;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<ElementDto>>.Default.GetHashCode(Entities);
            hashCode = hashCode * -1521134295 + EqualityComparer<MeasureGroupDto>.Default.GetHashCode(MeasureGroup);

            return hashCode;
        }
    }
}
