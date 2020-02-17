using Visiology.DataCollect.Autotests.Entities.Results;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Integration.Tests.Models
{
    /// <summary>
    /// Сущность, описывающая результат получения контента
    /// </summary>
    public class GetContentResult<T, U> 
        where T : IResponseContentList<U>
        where U : IResponseContent
    {
        public GetContentResult()
        {
            ContentVerificationResult = new ContentVerificationResult();
        }

        /// <summary>
        /// Результаты верификации контента
        /// </summary>
        public ContentVerificationResult ContentVerificationResult { get; set; }

        /// <summary>
        /// Контент
        /// </summary>
        public T Content { get; set; }
    }
}
