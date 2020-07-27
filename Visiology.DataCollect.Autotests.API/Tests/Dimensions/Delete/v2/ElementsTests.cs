using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;

namespace Visiology.DataCollect.Autotests.API.Tests.Dimensions.Delete.v2
{
    /// <summary>
    /// Класс тестирования метода обновления элементов измерения
    /// </summary>
    [XApiVersion("2.0")]
    public class ElementsTests : v1.ElementsTests
    {
        /// <summary>
        /// Измерение для тестирования удаления элементов пользователем с ролью 
        /// В дампе - "Тестовое измерение для тестирования удаления элементов v2.0"
        /// </summary>
        protected override string userDimensionId { get; set; } = "dim_Testovoe_izmerenie_delet01";

        /// <summary>
        /// Измерение для тестирования удаления всех элементов 
        /// В дампе "Тестовое измерение для тестирования удаления всех элементов v2.0"
        /// </summary>
        protected override string dimensionForDeleteAllId { get; set; } = "dim_Testovoe_izmerenie_delet02";

        public ElementsTests(TokenFixture tokenFixture, RestService restService)
            : base(tokenFixture, restService)
        {
        }
    }
}
