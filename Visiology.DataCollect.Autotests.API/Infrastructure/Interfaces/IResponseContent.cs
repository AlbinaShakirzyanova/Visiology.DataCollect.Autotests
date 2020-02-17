namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces
{
    /// <summary>
    /// Интерфейс сущности, приходящей при запросе API
    /// </summary>
    public interface IResponseContent
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        string Id { get; }
    }
}
