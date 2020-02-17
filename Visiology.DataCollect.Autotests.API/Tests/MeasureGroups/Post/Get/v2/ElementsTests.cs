using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Elements;
using Xunit;

namespace Visiology.DataCollect.Integration.Tests.MeasureGroups.Get.v2
{
    [XApiVersion("2.0")]
    public class ElementsTests : BaseTests<ElementsListDto, ElementDto>
    {
        public override string Url { get; set; }

        /// <summary>
        /// Группа показателей для тестирования получения элементов 
        /// В дампе - "Группа показателей для тестирования получения элементов"
        /// </summary>
        private const string measureGroupId = "measureGroup_Gruppa_pokazatel_";

        public ElementsTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            this.Url = this.GetUrl(measureGroupId);
        }

        /// <summary>
        /// Тест получения элементов группы показателей при невалидном или несуществующем идентификаторе
        /// </summary>
        /// <param name="dimensionId">Идентификато измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory]
        [InlineData(null)]
        public async Task GetAll_WithInvalidMeasureGroupId(string measureGroupId)
        {
            this.Url = this.GetUrl(measureGroupId);
            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов группы показателей при отсутствии у роли пользователя разрешения на эту группу показателей
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользоателя(по роли)</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory]
        [InlineData(TokenRoleType.UserWithFakeRole)]
        public async Task GetAll_WithInvalidTokenType(TokenRoleType tokenRoleType)
        {
            var result = await this.ExecuteGet(tokenRoleType, 0);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов с параметром getAll
        /// </summary>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAll_WithParamGetAll(bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { "getAll", getAll }
            };

            // Количество получаемых элементов при тестировании метода
            var elementsCount = 88;
            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, elementsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов с параметром limit
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamLimit(int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit }
            };

            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов с параметром skip
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory]
        [InlineData(TestValues.ZeroValue)]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamSkip(int skip)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip }
            };

            // Количество получаемых элементов при тестировании метода
            var elementsCount = 88 - skip;
            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, elementsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения одновременно с двумя параметрами 
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory]
        [InlineData(TestValues.MoreThanZeroValue, TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamsSkipAndLimit(int skip, int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip },
                    { Parameters.Limit, limit }
            };

            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        protected override string GetSearchUrl(string entityId)
        {
            return string.Format($"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlMeasureGroupElementsSearchPath")}", entityId);
        }

        protected override string GetUrl(string entityId)
        {
            return string.Format($"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlMeasureGroupElementsPath")}", entityId);
        }
    }
}
