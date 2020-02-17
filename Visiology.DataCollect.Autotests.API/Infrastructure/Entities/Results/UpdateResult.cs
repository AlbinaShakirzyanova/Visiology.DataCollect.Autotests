using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Класс, описываюий результаты обновления
    /// </summary>
    public class UpdateResult : IResponseResult
    {
        /// <summary>
        /// Количество обновленных сущностей
        /// </summary>
        public int updated { get; set; }

        /// <summary>
        /// Количество сущностей, которые нельзя изменить
        /// </summary>
        public int restricted { get; set; }

        /// <summary>
        /// Количество найденных по условию, но неизмененных сущностей
        /// </summary>
        public int notChanged { get; set; }
    }
}
