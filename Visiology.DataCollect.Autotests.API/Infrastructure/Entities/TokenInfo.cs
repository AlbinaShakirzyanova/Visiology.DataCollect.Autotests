using System;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Entities
{
    /// <summary>
    /// Информация о токене
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// Токен
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Тип токена
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Срок действия токена
        /// </summary>
        public DateTime ExpiredDate { get; set; }
    }
}
