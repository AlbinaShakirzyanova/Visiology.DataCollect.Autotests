using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Класс, описывающий результаты удаления
    /// </summary>
    public class DeleteResult : IResponseResult
    {
        /// <summary>
        /// Количество удалённых элементов
        /// </summary>
        public long deleted { get; set; }
    }
}
