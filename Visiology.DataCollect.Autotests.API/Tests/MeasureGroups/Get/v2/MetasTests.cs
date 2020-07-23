using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests.MeasureGroups.Get.v2
{
    /// <summary>
    /// Класс тестирования метода получения метаописаний групп показателей
    /// </summary>
    [XApiVersion("2.0")]
    public class MetasTests : BaseTests<MeasureGroupsListDto, MeasureGroupDto>
    {
        /// <summary>
        /// Количество групп показателей, доступных пользователю с ролью
        /// </summary>
        private const int measureGroupsCount = 13;

        /// <summary>
        /// Группа показателей для тестирования метода получения одной группы 
        /// </summary>
        private const string measureGroupId = "measureGroup_testirovanie_polu";

        /// <summary>
        /// Url запроса на получение описаний групп показателей
        /// </summary>
        public override string Url { get; set; }

        public MetasTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            Url = $"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetMeasureGroupsPath")}";
        }

        /// <summary>
        /// Тест получения всех метаописаний групп показателей без параметров
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups")]
        public async Task GetAll()
        {
            var result = await ExecuteGet(TokenRoleType.UserWithRole, measureGroupsCount);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения всех метаописаний групп показателей с параметром getAll
        /// </summary>
        /// <param name="getAll">Параметр getAll</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAll_WithParamGetAll(bool getAll)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.GetAll, getAll }
            };

            var measureGroupCount = getAll ? measureGroupsCount :
                measureGroupsCount < int.Parse(config.GetValue("DefaultEntitiesCount")) ?
                 measureGroupsCount : int.Parse(config.GetValue("DefaultEntitiesCount"));
            var result = await ExecuteGet(TokenRoleType.UserAdmin, measureGroupCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний всех групп показателей с параметром limit
        /// </summary>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamLimit(int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Limit, limit }
            };

            var result = await ExecuteGet(TokenRoleType.UserAdmin, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний групп показателей с параметром skip
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.ZeroValue)]
        [InlineData(TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamSkip(int skip)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip }
            };

            var result = await ExecuteGet(TokenRoleType.UserAdmin, measureGroupsCount - skip, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний групп показателей одновременно с двумя параметрами 
        /// </summary>
        /// <param name="skip">Параметр skip</param>
        /// <param name="limit">Параметр limit</param>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.MoreThanZeroValue, TestValues.MoreThanZeroValue)]
        public async Task GetAll_WithParamsSkipAndLimit(int skip, int limit)
        {
            var parameters = new Dictionary<string, object>
            {
                    { Parameters.Skip, skip },
                    { Parameters.Limit, limit }
            };

            var result = await ExecuteGet(TokenRoleType.UserAdmin, limit, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний одной группы показателей при невалидном или несуществующем идентификаторе группы показателей
        /// </summary>
        /// <param name="measureGroupId">Идентификатор группы показателей</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory, Trait("Category", "MeasureGroups")]
        [InlineData(TestValues.NonExistуEntityId)]
        [InlineData(TestValues.ZeroId)]
        public async Task Get_WithInvalidMgId(int? measureGroupId)
        {
            Url = string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetMeasureGroupPath")}", measureGroupId);
            var result = await ExecuteGet(TokenRoleType.UserAdmin, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения метаописаний одной группы показателей 
        /// </summary>
        /// <returns>Ожидаемый результат - положительный</returns>
        [Fact, Trait("Category", "MeasureGroups")]
        public async Task Get_WithMgId()
        {
            Url = string.Format($"{config.GetValue("ApiUrl")}{config.GetValue("ApiUrlGetMeasureGroupPath")}", measureGroupId);

            var response = await _restService.SendRequestAsync(Method, Url, TokenRoleType.UserAdmin, _tokenFixture.Tokens, null, Headers);
            var isSuccess = response.IsSuccessful;
            var message = $"{response.StatusCode} {response.StatusDescription}";

            if (isSuccess)
            {
                try
                {
                    var content = await Task.Run(() => JsonConvert.DeserializeObject<MeasureGroupDto>(response.Content));

                    /// TODO В зависимости от версии
                    if (content.Id != measureGroupId)
                    {
                        isSuccess = false;
                        message += "Ошибка получения метаописания группы показателей." +
                            $"Ожидаемый идентификатор - {measureGroupId}, полученный - {content.Id}";
                    }
                    else if (content.Dimensions.Count != 6)
                    {
                        isSuccess = false;
                        message += "Ошибка получения описания измерений группы показателей." +
                            $"Ожидаемое количество - 6, полученное - content.Dimensions.Count";
                    }
                    else if (content.Measure == null)
                    {
                        isSuccess = false;
                        message += "Ошибка получения описания показателя группы показателей.";
                    }
                    else if (content.Calendar == null)
                    {
                        isSuccess = false;
                        message += "Ошибка получения описания календаря группы показателей.";
                    }
                }
                catch (Exception e)
                {
                    isSuccess = false;
                    message += e.Message;
                }
            }

            Assert.True(isSuccess, message);
        }

        protected override string GetUrl(string dimensionId)
        {
            throw new NotImplementedException();
        }

        protected override string GetSearchUrl(string dimensionId)
        {
            throw new NotImplementedException();
        }
    }
}
