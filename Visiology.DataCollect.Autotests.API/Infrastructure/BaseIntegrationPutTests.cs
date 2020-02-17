using Newtonsoft.Json;
using System;
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
    /// Абстрактная сущость базового интеграционного класса на тестируемый метод Put
    /// </summary>
    public abstract class BaseIntegrationPutTests<TList, TEntity> : BaseIntegrationTests<TList, TEntity>
        where TList : IResponseContentList<TEntity>
        where TEntity : IResponseContent
    {
        public BaseIntegrationPutTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService, IVerifier<TEntity> verifier = null)
            : base(iisFixture, tokenFixture, restService, verifier)
        {
        }

        /// <summary>
        /// Метод обновления и верификации контента запроса
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользователя (по роли)</param>
        /// <param name="updatedEntities">Ожидаемое количество обновленных сущностей</param>
        /// <param name="restrictedEntities">Ожидаемое количество пропущенных сущностей</param>
        /// <param name="notChangesEntities">Ожидаемое количество неизменных сущностей</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <param name="body">Тело запроса</param>
        /// <returns>Результат верификации контента</returns>
        public async Task<ContentVerificationResult> ExecutePut(
                TokenRoleType tokenRoleType,
                UpdateResult expectedResult,
                Dictionary<string, object> parameters = null,
                string body = null)
        {
            var response = await this._restService.SendRequestAsync(
                this.Method,
                this.Url,
                tokenRoleType,
                this._tokenFixture.Tokens,
                parameters, this.Headers, body);

            var isTestSuccess = response.IsSuccessful;
            var userMessage = $"{response.StatusCode} {response.StatusDescription} {response.Content}";

            if (isTestSuccess)
            {
                var verificateResult = await this.TryVerifyRequestResponse(response.Content, expectedResult);

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

        private async Task<ContentVerificationResult> TryVerifyRequestResponse(string responseContent, UpdateResult expectedResult)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            try
            {
                var updateResult = await Task.Run(() => JsonConvert.DeserializeObject<UpdateResult>(responseContent));

                if (expectedResult.updated != updateResult.updated)
                {
                    result.Message += "Результат обновления сущностей не соответсвует ожидаемому. " +
                    $"Ожидалось обновление {expectedResult.updated} сущностей, изменилось - {updateResult.updated}. " +
                    "Пожалуйста, проверьте настройки конфигурации или обратитесь к администратору";
                    result.IsSuccess = false;
                }

                if (expectedResult.restricted != updateResult.restricted)
                {
                    result.Message += "Результат обновления сущностей не соответсвует ожидаемому. " +
                   $"Ожидалось {expectedResult.restricted} недоступных для записи сущностей, по факту - {updateResult.restricted}. " +
                   "Пожалуйста, проверьте настройки конфигурации или обратитесь к администратору";
                    result.IsSuccess = false;
                }

                if (expectedResult.notChanged != updateResult.notChanged)
                {
                    result.Message += "Результат обновления сущностей не соответсвует ожидаемому. " +
                   $"Ожидалось {expectedResult.notChanged} неизмененных сущностей, по факту - {updateResult.notChanged}. " +
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
            return string.Format($"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlDimensionElementsPath")}", dimensionId);
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            return string.Format($"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlDimensionElementsSearchPath")}", dimensionId);
        }
    }
}
