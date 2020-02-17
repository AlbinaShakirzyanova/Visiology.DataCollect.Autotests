using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models.Dimensions.Levels
{
    /// <summary>
    /// Описание уровня измерения
    /// </summary>
    public class LevelDto : IResponseContent
    {
        /// <summary>
        /// Наименование уровня
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор уровня
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор родительского уровня
        /// </summary>
        public string ParentLevelId { get; set; }
    }
}
