using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Entities.Results;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Visiology.DataCollect.Integration.Tests.Models;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure
{
    /// <summary>
    /// Абстрактная сущность базового интеграционного класса на тестируемый метод Get
    /// </summary>
    public abstract class BaseIntegrationGetTests<TList, TEntity> : BaseIntegrationTests<TList, TEntity>
        where TList : IResponseContentList<TEntity>
        where TEntity : IResponseContent
    {
        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public override Method Method { get; set; } = Method.GET;

        public BaseIntegrationGetTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService, IVerifier<TEntity> verifier = null)
            : base(iisFixture, tokenFixture, restService, verifier)
        {
        }

        /// <summary>
        /// Метод получения и верификации контента запроса
        /// </summary>
        /// <param name="tokenRoleType">Тип токена (по роли)</param>
        /// <param name="entitiesCount">Ожидаемое количество сущностей</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <param name="body">Тело запроса</param>
        /// <returns>Результат верификации контента</returns>
        public async Task<ContentVerificationResult> ExecuteGet(
            TokenRoleType tokenRoleType,
            int entitiesCount,
            Dictionary<string, object> parameters = null,
            string body = null)
        {
            var content = await this.TryGetEntities(this.Method, this.Url, tokenRoleType, parameters, this.Headers, body);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            if (isTestSuccess)
            {
                var verificateResult = this.Verifier.VerifyCount(content.Content.Entities, entitiesCount);

                if (!verificateResult.IsSuccess)
                {
                    isTestSuccess = verificateResult.IsSuccess;
                    userMessage += verificateResult.Message;
                }
            }

            return new ContentVerificationResult
            {
                IsSuccess = isTestSuccess,
                Message = userMessage
            };
        }        
    }
}
