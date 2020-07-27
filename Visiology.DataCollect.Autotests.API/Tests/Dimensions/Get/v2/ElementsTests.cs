using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Get.v2
{
    /// <summary>
    /// Класс тестирования получения элементов измерения
    /// </summary>
    [XApiVersion("2.0")]
    public class ElementsTests : BaseTests<ElementsListDto, ElementDto>
    {
        public override string Url { get; set; }

        /// <summary>
        /// Измерение для тестирования получения элементов 
        /// В дампе - "Измерение для тестирования получения элементов"
        /// </summary>
        private const string dimensionId = "dim_poluchenie_eleentov";

        public ElementsTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = GetUrl(dimensionId);
        }

        /// <summary>
        /// Тест получения элементов измерения при невалидном или несуществующем идентификаторе измерения
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
        /// Тест получения элементов измерения при отсутствии у роли пользователя разрешения на это измерение
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
        /// Тест получения элементов измерения с параметром getAll
        /// </summary>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAll_WithParamGetAll(bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { "getAll", getAll }
            };

            // Количество получаемых элементов при тестировании метода (принадлежат измерению "Измерение для тестирования получения элементов")
            var elementsCountInDimension = 1005;
            var elementsCount = getAll ? elementsCountInDimension : int.Parse(config.GetValue("DefaultEntitiesCount"));
            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения с параметром limit
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamLimit(int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit }
            };

            var result = await ExecuteGet(TokenRoleType.UserWithRole, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения с параметром skip
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.ZeroValue)]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamSkip(int skip)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip }
            };


            // Количество получаемых элементов при тестировании метода (принадлежат измерению "Измерение для тестирования получения элементов")
            var elementsCountInDimension = 1005;

            var elementsCount = skip == TestValues.ZeroValue
                ? int.Parse(config.GetValue("DefaultEntitiesCount"))
                : elementsCountInDimension - skip > int.Parse(config.GetValue("DefaultEntitiesCount"))
                   ? int.Parse(config.GetValue("DefaultEntitiesCount")) : elementsCountInDimension - skip;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, elementsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения элементов измерения одновременно с двумя параметрами 
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.MoreThanZeroValue, TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamsSkipAndLimit(int skip, int limit)
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
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlDimensionElementsSearchPath")}", dimensionId);
        }

        protected override string GetUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlDimensionElementsPath")}", dimensionId);
        }
    }
}
