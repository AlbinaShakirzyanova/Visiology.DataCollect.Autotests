using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Forms
{
    /// <summary>
    /// Описание формы группы показателей
    /// </summary>
    public class FormDto : IResponseContent
    {
        /// <summary>
        /// Идентификатор формы
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// Идентификатор шаблона формы
        /// </summary>
        public long FormTemplateId { get; set; }

        /// <summary>
        /// Наименование шаблона формы
        /// </summary>
        public string FormTemplateName { get; set; }
    }

    public class CoordinateDescription
    {
        public object Id { get; set; }

        public string UniqueName { get; set; }

        public string Name { get; set; }
    }

    public class CoordinateValue
    {
        public object Id { get; set; }

        public string Name { get; set; }
    }

    public class Coordinate
    {
        public CoordinateDescription Description { get; set; }

        public CoordinateValue Value { get; set; }
    }
}
