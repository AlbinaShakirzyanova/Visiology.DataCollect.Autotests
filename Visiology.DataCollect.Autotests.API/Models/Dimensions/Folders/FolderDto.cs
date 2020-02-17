using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Folders
{
    /// <summary>
    /// Описание каталога измерения
    /// </summary>
    public class FolderDto : IResponseContent
    {
        /// <summary>
        /// Наименование каталога
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор каталога
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор родительского каталога
        /// </summary>
        public string ParentFolderId { get; set; }

        /// <summary>
        /// Идентификатор уровня
        /// </summary>
        public string LevelId { get; set; }
    }
}
