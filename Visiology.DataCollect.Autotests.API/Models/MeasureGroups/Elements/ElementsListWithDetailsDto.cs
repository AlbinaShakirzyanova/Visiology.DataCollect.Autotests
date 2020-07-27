using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups;

namespace Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements
{
    public class ElementsListWithDetailsDto : IResponseContentList<ElementWithDetailsDto>
    {
        /// <summary>
        /// Описание группы показателей
        /// </summary>
        public MeasureGroupDto MeasureGroup { get; set; }

        /// <summary>
        /// Список элементов
        /// </summary>
        [JsonProperty(PropertyName = "Elements")]
        public IEnumerable<ElementWithDetailsDto> Entities { get; set; } = new List<ElementWithDetailsDto>();

        public bool Equals([AllowNull] ElementsListWithDetailsDto other)
        {
            var dto = other as ElementsListWithDetailsDto;
            return dto != null &&
                   MeasureGroup.Equals(dto.MeasureGroup) &&
                   Entities.SequenceEqual(dto.Entities);
        }

        public override int GetHashCode()
        {
            var hashCode = -2007856361;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<ElementWithDetailsDto>>.Default.GetHashCode(Entities);
            hashCode = hashCode * -1521134295 + EqualityComparer<MeasureGroupDto>.Default.GetHashCode(MeasureGroup);

            return hashCode;
        }
    }
}
