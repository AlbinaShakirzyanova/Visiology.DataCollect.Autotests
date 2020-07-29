using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results.Dimensions;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Post
{
    /// <summary>
    /// Абстрактная сущyость базового интеграционного класса на тестируемый метод Post
    /// </summary>
    public abstract class BaseIntegrationPostTests<TList, TEntity> : BaseIntegrationTests<TList, TEntity>
        where TList : IResponseContentList<TEntity>
        where TEntity : IResponseContent
    {
        public BaseIntegrationPostTests(TokenFixture tokenFixture, RestService restService, IVerifier<TEntity> verifier = null)
            : base(tokenFixture, restService, verifier)
        {
        }

        /// <summary>
        /// Метод удаления и верификации контента запроса
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользователя (по роли)</param>
        /// <param name="expectedResult">Ожидаемое количество созданных сущностей</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <param name="body">Тело запроса</param>
        /// <returns>Результат верификации контента</returns>
        public async Task<ContentVerificationResult> ExecuteCreate(
                TokenRoleType tokenRoleType,
                CreateResult expectedResult,
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

        private async Task<ContentVerificationResult> TryVerifyRequestResponse(string responseContent, CreateResult expectedResult)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            try
            {
                var createResult = await Task.Run(() => JsonConvert.DeserializeObject<CreateResult>(responseContent));

                if (expectedResult.Added != createResult.Added)
                {
                    result.Message += "Результат создания сущностей не соответсвует ожидаемому. " +
                    $"Ожидалось создание {expectedResult.Added} сущностей, создалось - {createResult.Added}. " +
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
