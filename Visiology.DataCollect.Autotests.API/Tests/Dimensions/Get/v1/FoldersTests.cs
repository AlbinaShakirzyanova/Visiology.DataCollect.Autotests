using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Folders;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Get.v1
{
    /// <summary>
    /// Класс тестирования метода получения папок измерения
    /// </summary>
    [XApiVersion("1.0")]
    public class FoldersTests : BaseTests<FoldersListDto, FolderDto>
    {
        public override string Url { get; set; }

        /// <summary>
        /// Измерение для тестирования получения элементов 
        /// В дампе - "Измерение для тестирования получения папок"
        /// </summary>
        private const string dimensionId = "11";

        public FoldersTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
            Url = GetUrl(dimensionId);
        }

        /// <summary>
        /// Тест получения каталогов измерения при невалидном или несуществующем идентификаторе измерения
        /// </summary>
        /// <param name="dimensionId">Идентификато измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "Dimensions")]
        [InlineData(TestValues.ZeroId)]
        [InlineData(TestValues.NonExistId)]
        [InlineData(null)]
        public async Task GetAll_WithInvalidDimensionId(string dimensionId)
        {
            Url = GetUrl(dimensionId);
            var result = await ExecuteGet(TokenRoleType.UserAdmin, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения каталогов измерения при отсутствии у роли пользователя разрешения на это измерение
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
        /// Тест получения каталогов измерения без параметров
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task GetAll_WithoutParameters()
        {
            var result = await ExecuteGet(TokenRoleType.UserWithRole, int.Parse(config.GetValue("DefaultEntitiesCount")));

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения каталогов измерения с параметром getAll
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
                    { Parameters.GetAll, getAll }
            };

            // Количество получаемых каталогов при тестировании метода ApiUrlGetDimensionFoldersPath(принадлежат измерению "Измерение для тестирования получения папок") 
            var foldersCountInDimension = 2002;
            var foldersCount = getAll ? foldersCountInDimension : int.Parse(config.GetValue("DefaultEntitiesCount"));

            var result = await ExecuteGet(TokenRoleType.UserWithRole, foldersCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения каталогов измерения с параметром limit
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
        /// Тест получения каталогов измерения с параметром skip
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

            // Количество получаемых каталогов при тестировании метода ApiUrlGetDimensionFoldersPath(принадлежат измерению "Измерение для тестирования получения папок") 
            var foldersCountInDimension = 2002;

            var foldersCount = skip == TestValues.ZeroValue
                ? int.Parse(config.GetValue("DefaultEntitiesCount"))
                : foldersCountInDimension - skip > int.Parse(config.GetValue("DefaultEntitiesCount"))
                   ? int.Parse(config.GetValue("DefaultEntitiesCount")) : foldersCountInDimension - skip;

            var result = await ExecuteGet(TokenRoleType.UserWithRole, foldersCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения каталогов измерения одновременно с двумя параметрами 
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
            throw new System.NotImplementedException();
        }

        protected override string GetUrl(string dimensionId)
        {
            return string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetDimensionFoldersPath")}", dimensionId);
        }
    }
}
