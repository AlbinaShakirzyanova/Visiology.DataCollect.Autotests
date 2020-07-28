using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results.Dimensions
{
    /// <summary>
    /// Результат создания элементов измерения
    /// </summary>
    public class CreateResult : IResponseResult
    {
        /// <summary>
        /// Количество добавленных элементов
        /// </summary>
        public long Added { get; set; }
    }
}
