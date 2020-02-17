using System.Collections.Generic;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Сущность хранения токенов
    /// </summary>
    public class TokenFixture
    {
        /// <summary>
        /// Словарь хранения по ролям информации токенов 
        /// </summary>
        public Dictionary<TokenRoleType, TokenInfo> Tokens { get; set; } = new Dictionary<TokenRoleType, TokenInfo>();
    }
}
