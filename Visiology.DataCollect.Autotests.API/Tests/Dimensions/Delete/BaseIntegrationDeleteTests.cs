using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Delete
{
    /// <summary>
    /// Абстрактная сущость базового интеграционного класса на тестируемый метод Put
    /// </summary>
    public abstract class BaseIntegrationDeleteTests<TList, TEntity> : BaseIntegrationTests<TList, TEntity>
        where TList : IResponseContentList<TEntity>
        where TEntity : IResponseContent
    {
        public BaseIntegrationDeleteTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService, IVerifier<TEntity> verifier = null)
            : base(iisFixture, tokenFixture, restService, verifier)
        {
        }

        /// <summary>
        /// Метод удаления и верификации контента запроса
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользователя (по роли)</param>
        /// <param name="expectedResult">Ожидаемое количество удаленных сущностей</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <param name="body">Тело запроса</param>
        /// <returns>Результат верификации контента</returns>
        public async Task<ContentVerificationResult> ExecuteDelete(
                TokenRoleType tokenRoleType,
                DeleteResult expectedResult,
                Dictionary<string, object> parameters = null,
                string body = null)
        {
            var response = await _restService.SendRequestAsync(
                Method,
                Url,
                tokenRoleType,
                _tokenFixture.Tokens,
                parameters, Headers, body);

            var isTestSuccess = response.IsSuccessful;
            var userMessage = $"{response.StatusCode} {response.StatusDescription} {response.Content}";

            if (isTestSuccess)
            {
                var verificateResult = await TryVerifyRequestResponse(response.Content, expectedResult);

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

        private async Task<ContentVerificationResult> TryVerifyRequestResponse(string responseContent, DeleteResult expectedResult)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            try
            {
                var deleteResult = await Task.Run(() => JsonConvert.DeserializeObject<DeleteResult>(responseContent));

                if (expectedResult.deleted != deleteResult.deleted)
                {
                    result.Message += "Результат удаления сущностей не соответсвует ожидаемому. " +
                    $"Ожидалось удаление {expectedResult.deleted} сущностей, удалилось - {deleteResult.deleted}. " +
                    "Пожалуйста, проверьте настройки конфигурации или обратитесь к администратору";
                    result.IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message += e.Message;

                return result;
            }

            return result;
        }

        protected override string GetUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlDimensionElementsPath")}", dimensionId);
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlDimensionElementsSearchPath")}", dimensionId);
        }
    }
}
