using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.API.Tests.Dimensions.Get;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions;
using Xunit;

namespace Visiology.DataCollect.Integration.Tests.Dimensions.Get.v2
{
    /// <summary>
    /// Класс тестирования метода получения метаописаний измерения
    /// </summary>
    [XApiVersion("2.0")]
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
            this.Url = $"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlGetDimensionsPath")}";
        }

        /// <summary>
        /// Тест получения всех метаописаний измерений для пользователя с ролью "Администратор"
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task GetAll_ForAdmin()
        {
            // Количество измерений, доступных пользователю с ролью "Администратор"
            var adminDimensionsCount = 34;

            var result = await this.ExecuteGet(TokenRoleType.UserAdmin, adminDimensionsCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения всех метаописаний измерений для пользователя с ролью
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact]
        public async Task GetAll_ForUser()
        {
            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, userDimensionsCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний всех измерений с параметром getAll
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
                    { Parameters.GetAll, getAll }
            };

            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, userDimensionsCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний всех измерений с параметром limit
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
        /// Тест получения метаописаний измерений с параметром skip
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

            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, userDimensionsCount - skip, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний измерений одновременно с двумя параметрами 
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
