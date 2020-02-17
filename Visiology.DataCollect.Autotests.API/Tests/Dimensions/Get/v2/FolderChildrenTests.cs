using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Folders;
using Xunit;

namespace Visiology.DataCollect.Integration.Tests.Dimensions.Get.v2
{
    /// <summary>
    /// Класс тестирования метода полученя дочерних каталогов каталога измерения
    /// </summary>
    [XApiVersion("2.0")]
    public class FolderChildrenTests : BaseTests<FoldersListDto, FolderDto>
    {
        public override string Url { get; set; }

        /// <summary>
        /// Измерение для тестирования получения элементов 
        /// В дампе - "Измерение для тестирования получения папок"
        /// </summary>
        private const string dimensionId = "dim_poluchenie_papok";

        /// <summary>
        /// Каталог для тестирвоания получения папок
        /// В дампе - "Измерение для тестирования получения папок"
        /// </summary>
        private const string folderId = "218";

        public FolderChildrenTests(IisFixture iisFixture, TokenFixture tokenFixture, RestService restService)
            : base(iisFixture, tokenFixture, restService)
        {
            this.Url = this.GetUrl(dimensionId);
        }

        /// <summary>
        /// Тест получения дочерних каталогов при невалидном или несуществующем идентификаторе каталога
        /// </summary>
        /// <param name="dimensionId">Идентификатор измерения</param>
        /// <returns>Ожидаемый результат - отрицательный</returns>
        [Theory]
        [InlineData(TestValues.ZeroId)]
        [InlineData(TestValues.NonExistId)]
        [InlineData(null)]
        public async Task GetAll_WithInvalidDimensionId(string folderId)
        {
            this.Url = string.Format($"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlGetDimensionFolderChildrenPath")}", dimensionId, folderId);
            var result = await this.ExecuteGet(TokenRoleType.UserAdmin, 0);

            Assert.True(!result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения дочерних каталогов с параметром getAll
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

            // Количество получаемых дочерних каталогов при тестировании метода ApiUrlGetDimensionFolderChildrenPath(принадлежат измерению DimensionForGetFoldersTests)
            var folderChildrenCount = 1001;

            var foldersCount = getAll ? folderChildrenCount : int.Parse(this.config.GetValue("DefaultEntitiesCount"));
            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, foldersCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения дочерних каталогов с параметром limit
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
        /// Тест получения дочерних каталогов с параметром skip
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

            // Количество получаемых дочерних каталогов при тестировании метода ApiUrlGetDimensionFolderChildrenPath(принадлежат измерению DimensionForGetFoldersTests)
            var folderChildrenCount = 1001;

            var foldersCount = skip == TestValues.ZeroValue
                ? int.Parse(this.config.GetValue("DefaultEntitiesCount"))
                : (folderChildrenCount - skip > int.Parse(this.config.GetValue("DefaultEntitiesCount"))
                   ? int.Parse(this.config.GetValue("DefaultEntitiesCount")) : folderChildrenCount - skip);

            var result = await this.ExecuteGet(TokenRoleType.UserWithRole, foldersCount, parameters);

            Assert.True(result.IsSuccess, result.Message);
        }

        /// <summary>
        /// Тест получения дочерних каталогов измерения одновременно с двумя параметрами 
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

        protected override string GetSearchUrl(string dimensionId)
        {
            throw new System.NotImplementedException();
        }

        protected override string GetUrl(string dimensionId)
        {
            return string.Format($"{this.config.GetValue("ApiUrl")}{this.config.GetValue("ApiUrlGetDimensionFolderChildrenPath")}", dimensionId, folderId);
        }
    }
}
