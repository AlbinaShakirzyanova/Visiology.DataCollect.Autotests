using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Attributes;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Get.v2
{
    /// <summary>
    /// Класс тестирования метода получения атрибутов измерения
    /// </summary>
    [XApiVersion("2.0")]
    public class AttributesTests : BaseTests<AttributesListDto, AttributeDto>
    {
        /// <summary>
        /// Измерение для тестирования получения атрибутов 
        /// В дампе - "Измерение для тестирования получения уровней"
        /// </summary>
        private const string dimensionId = "dim_poluchenie_urovnei";

        public AttributesTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = GetUrl(dimensionId);
        }

        public override string Url { get; set; }

        /// <summary>
        /// Тест получения атрибутов измерения при невалидном или несуществующем идентификаторе измерения
        /// </summary>
        /// <param name="dimensionId">Идентификато измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(null)]
        public async Task GetAll_WithInvalidDimensionId(string dimensionId)
        {
            Url = GetUrl(dimensionId);
            var result = await ExecuteGet(TokenRoleType.UserWithRole, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения атрибутов измерения при отсутствии у роли пользователя разрешения на это измерение
        /// </summary>
        /// <param name="tokenRoleType">Тип токена пользоателя(по роли)</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TokenRoleType.UserWithFakeRole)]
        public async Task GetAll_WithInvalidTokenType(TokenRoleType tokenRoleType)
        {
            var result = await ExecuteGet(tokenRoleType, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения атрибутов измерения без параметров
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task GetAll_WithoutParameters()
        {
            //Ожидаемое количество атрибутов в измерении
            var attributesCount = 12;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, attributesCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения атрибутов измерения с параметром getAll
        /// </summary>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Success_GetAll_WithParamGetAll(bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.GetAll, getAll }
            };

            //Ожидаемое количество атрибутов в измерении
            var attributesCount = 12;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, attributesCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения атрибутов измерения с параметром limit
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task Success_GetAll_WithParamLimit(int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit }
            };

            var result = await ExecuteGet(TokenRoleType.UserWithRole, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения атрибутов измерения с параметром skip
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.ZeroValue)]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task Success_GetAll_WithParamSkip(int skip)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip }
            };

            //Ожидаемое количество атрибутов в измерении
            var attributesCount = 12;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, attributesCount - skip, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения атрибутов измерения одновременно с двумя параметрами 
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.MoreThanZeroValue, TestValues.MoreThanZeroValue)]
        public async Task Success_GetAll_WithParamsSkipAndLimit(int skip, int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip },
                    { Parameters.Limit, limit }
            };

            var result = await ExecuteGet(TokenRoleType.UserWithRole, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetDimensionAttributesPath")}", dimensionId);
        }
    }
}
