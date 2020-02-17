using System.Collections.Generic;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces
{
    /// <summary>
    /// Интерфейс коллекции сущностей, приходящих при запросе API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResponseContentList<T> where T : IResponseContent
    {
        /// <summary>
        /// Коллекция приходящих сущностей
        /// </summary>
        IEnumerable<T> Entities { get; set; }
    }
}
