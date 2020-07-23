using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Post.Get
{
    /// <summary>
    /// Абстрактный класс базовых тестов для Get-методов
    /// </summary>
    public abstract class BaseTests<TList, TEntity> : BaseIntegrationGetTests<TList, TEntity>
        where TList : IResponseContentList<TEntity>
        where TEntity : IResponseContent
    {
        public BaseTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService, IVerifier<TEntity> verifier = null)
            : base(iisFixture, tokenFixture, restService, verifier)
        {
        }

        /// <summary>
        /// Тест получения данных при отсутствии токена в запросе
        /// </summary>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Fact, Trait("Category", "MeasureGroups")]
        public async Task GetAll_WithoutToken()
        {
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, Url);

                requestMessage.Headers.Add("X-API-VERSION", XApiVersion);

                var response = await client.SendAsync(requestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                Assert.True(!response.IsSuccessStatusCode, response.ReasonPhrase);
            }
        }

        /// <summary>
        ///Тест получения сущностей с параметром getAll=null
        /// </summary>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(null)]
        public async Task GetAll_WithInvalidParamGetAll(bool? getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.GetAll, getAll }
            };

            var content = await TryGetEntities(Method, Url, TokenRoleType.UserAdmin, parameters);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            Assert.True(!isTestSuccess, userMessage);
        }

        /// <summary>
        /// Тест получения сущностей с параметром limit
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.ZeroValue)]
        [InlineData(TestValues.LessThanZeroValue)]
        [InlineData(null)]
        public async Task GetAll_WithInvalidParamLimit(int? limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit }
            };

            var content = await TryGetEntities(Method, Url, TokenRoleType.UserAdmin, parameters);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            Assert.True(!isTestSuccess, userMessage);
        }

        /// <summary>
        /// Тест получения сущностей с параметром skip
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.LessThanZeroValue)]
        [InlineData(null)]
        public async Task GetAll_WithInvalidParamSkip(int? skip)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip }
            };

            var content = await TryGetEntities(Method, Url, TokenRoleType.UserAdmin, parameters);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            Assert.True(!isTestSuccess, userMessage);
        }

        /// <summary>
        /// Тест получения сущностей с параметром skip и getAll
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.MoreThanZeroValue, true)]
        [InlineData(TestValues.MoreThanZeroValue, false)]
        public async Task GetAll_WithParamsSkipAndGetAll(int skip, bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip },
                    { Parameters.GetAll, getAll }
            };

            var content = await TryGetEntities(Method, Url, TokenRoleType.UserAdmin, parameters);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            Assert.True(!isTestSuccess, userMessage);
        }

        /// <summary>
        /// Тест получения сущностей с параметром limit и getAll
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.MoreThanZeroValue, true)]
        [InlineData(TestValues.MoreThanZeroValue, false)]
        public async Task GetAll_WithParamsLimitAndGetAll(int limit, bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit },
                    { Parameters.GetAll, getAll }
            };

            var content = await TryGetEntities(Method, Url, TokenRoleType.UserAdmin, parameters);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            Assert.True(!isTestSuccess, userMessage);
        }

        /// <summary>
        /// Тест получения сущностей с параметром limit, skip и getAll
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <param name="skip">Параметр skip</param>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.MoreThanZeroValue, TestValues.MoreThanZeroValue, true)]
        [InlineData(TestValues.MoreThanZeroValue, TestValues.MoreThanZeroValue, false)]
        public async Task GetAll_WithAllParams(int limit, int skip, bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit },
                    { Parameters.GetAll, getAll },
                    { Parameters.Skip, skip }
            };

            var content = await TryGetEntities(Method, Url, TokenRoleType.UserAdmin, parameters);
            var isTestSuccess = content.ContentVerificationResult.IsSuccess;
            var userMessage = content.ContentVerificationResult.Message;

            Assert.True(!isTestSuccess, userMessage);
        }
    }

    /// <summary>
    /// Тестовые значения для кейсов
    /// </summary>
    internal static class TestValues
    {
        /// <summary>
        /// Идентификатор не существующей в системе сущности
        /// </summary>
        public const int NonExistуEntityId = int.MaxValue;

        /// <summary>
        /// Нулевой идентификатор
        /// </summary>
        public const int ZeroId = 0;

        /// <summary>
        /// (для параметров)
        /// </summary>
        public const int ZeroValue = 0;
        public const int LessThanZeroValue = -1;
        public const int MoreThanZeroValue = 1;
    };
}
