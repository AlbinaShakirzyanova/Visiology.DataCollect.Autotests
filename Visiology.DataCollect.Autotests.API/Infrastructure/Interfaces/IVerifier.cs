using System.Collections.Generic;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results;

/// TODO Вынести все вспомогательные сущности в отдельные проекты
namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces
{
    /// <summary>
    /// Верификатор списка результатов
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public interface IVerifier<U> where U : IResponseContent
    {
        /// <summary>
        /// Верификации данных
        /// </summary>
        /// <param name="contentList">Список полученных сущностей</param>
        /// <param name="expectedList">Список ожидаемых сущностей</param>
        /// <returns>Результат верификации контента</returns>
        ContentVerificationResult Verify(IEnumerable<U> contentList, IEnumerable<U> expectedList);

        /// <summary>
        /// Верификация количества данных
        /// </summary>
        /// <param name="contentList">Список полученных сущностей</param>
        /// <param name="expectedEntitiesCount">Ожидаемое количество сущностей</param>
        /// <returns></returns>
        ContentVerificationResult VerifyCount(IEnumerable<U> contentList, int expectedEntitiesCount);
    }
}
