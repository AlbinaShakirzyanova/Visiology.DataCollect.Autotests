using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.API.Models.Dimensions;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Get.v1
{
    /// <summary>
    /// Класс тестирования метода получения метаописаний измерения
    /// </summary>
    [XApiVersion("1.0")]
    public class MetasTests : BaseTests<DimensionsListDto, DimensionDto>
    {
        // Количество измерений, доступных пользователю с ролью
        private const int userDimensionsCount = 22;

        /// <summary>
        /// Url запроса на получение описаний измерений
        /// </summary>
        public override string Url { get; set; }

        public MetasTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetDimensionsPath")}";
        }

        /// <summary>
        /// Тест получения всех метаописаний измерений для пользователя с ролью "Администратор"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task GetAll_ForAdmin()
        {
            // Количество измерений, доступных пользователю с ролью "Администратор"
            var adminDimensionsCount = 34;

            var result = await ExecuteGet(TokenRoleType.UserAdmin, adminDimensionsCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения всех метаописаний измерений для пользователя с ролью
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "Dimensions")]
        public async Task Success_GetAll_ForUser()
        {
            var result = await ExecuteGet(TokenRoleType.UserWithRole, userDimensionsCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний всех измерений с параметром getAll
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

            var result = await ExecuteGet(TokenRoleType.UserWithRole, userDimensionsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний всех измерений с параметром limit
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
        /// Тест получения метаописаний измерений с параметром skip
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

            var result = await ExecuteGet(TokenRoleType.UserWithRole, userDimensionsCount - skip, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний измерений одновременно с двумя параметрами 
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

        protected override string GetUrl(string dimensionId)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            throw new System.NotImplementedException();
        }
    }
}
