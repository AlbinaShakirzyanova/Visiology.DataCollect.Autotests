using System.Collections.Generic;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements
{
    /// <summary>
    /// Описание элемента измерения
    /// </summary>
    public class ElementDto : IResponseContent
    {
        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Наименование элемента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Путь к элементу, содержащий список каталогов
        /// </summary>
        public List<ElementFolderDto> Path { get; set; } = new List<ElementFolderDto>();

        /// <summary>
        /// Атрибуты элемента
        /// </summary>
        public IList<ElementAttributeDto> Attributes { get; set; } = new List<ElementAttributeDto>();
    }
}
