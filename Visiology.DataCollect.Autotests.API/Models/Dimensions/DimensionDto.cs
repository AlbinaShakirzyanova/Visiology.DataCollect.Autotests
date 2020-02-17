using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Attributes;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions
{
    /// <summary>
    /// Описание измерения
    /// </summary>
    public class DimensionDto : IResponseContent
    {
        /// <summary>
        /// Наименование измерения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание атрибутов измерения
        /// </summary>
        public List<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();

        /// <summary>
        /// Идентификатор измерения 
        /// </summary>
        public string Id { get; set; }
    }
}
