using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Configuration;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;


namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Impl
{
    /// <summary>
    /// Сервис для отправки авторизованных запросов
    /// </summary>
    public class RestService
    {
        /// <summary>
        /// Конфигурация
        /// </summary>
        private ConfigurationManager config;

        /// <summary>
        /// Url авторизации (берем из настроек конфигурации)
        /// </summary>
        private string authUrl;

        /// <summary>
        /// Заголовки запроса авторизации
        /// </summary>
        private Dictionary<string, string> authHeaders;

        /// <summary>
        /// Метод отправки асинхронного запроса
        /// </summary>
        /// <param name="method">Тип метода</param>
        /// <param name="requestUri">Url меода</param>
        /// <param name="tokenRoleType">Тип токена авторизации</param>
        /// <param name="tokens">Словарь доступных токенов</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <param name="headers">Заголовки запроса</param>
        /// <param name="body">Тело метода</param>
        /// <returns>Сообщение ответа запроса</returns>
        public virtual async Task<IRestResponse> SendRequestAsync(
            Method method,
            string requestUri,
            TokenRoleType tokenRoleType,
            Dictionary<TokenRoleType, TokenInfo> tokens,
            Dictionary<string, object> parameters = null,
            Dictionary<string, string> headers = null,
            string body = null)
        {
            this.config = new ConfigurationManager();

            this.authUrl = config.GetValue("ApiUrlAuthenticatePath");

            this.authHeaders = new Dictionary<string, string>
            {
                { "X-API-VERSION", config.GetValue("XApiVersionHeader") }
            };

            if (parameters != null)
            {
                requestUri += "?";
                foreach (var parameter in parameters)
                {
                    requestUri += "&" + parameter.Key + "=" + parameter.Value;
                }
            }

            var client = new RestClient(requestUri);
            var request = new RestRequest(method);

            await this.SetTokenAsync(tokenRoleType, tokens);
            request.AddHeader("Authorization", $"{tokens[tokenRoleType].TokenType} {tokens[tokenRoleType].AccessToken}");

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (body != null)
            {
                request.AddParameter("application/json; charset=utf-8", body, ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;
            }

            return client.Execute(request);
        }        

        /// <summary>
        /// Метод добавления токена в запрос по логину и паролю пользователя
        /// </summary>
        /// <param name="tokenRoleType">Тип токена (по роли)</param>
        /// <param name="tokens">Словарь доступных токенов</param>
        /// <returns></returns>
        private async Task SetTokenAsync(TokenRoleType tokenRoleType, Dictionary<TokenRoleType, TokenInfo> tokens = null)
        {
            if (!TokenRoleType.IsDefined(typeof(TokenRoleType), tokenRoleType))
            {
                throw new NotSupportedException();
            }

            if (tokens == null || !tokens.ContainsKey(tokenRoleType))
            {
                tokens[tokenRoleType] = await this.GetTokenInfo(tokenRoleType);
            }

            if (tokens[tokenRoleType].ExpiredDate < DateTime.UtcNow)
            {
                tokens[tokenRoleType] = await this.GetTokenInfo(tokenRoleType);
            }
        }

        /// <summary>
        /// Метод получения токена по логину и паролю пользователя
        /// </summary>
        /// <param name="tokenRoleType">Тип токена (по роли)</param>
        /// <param name="tokens">Словарь доступных токенов</param>
        /// <returns>Информация о токене</returns>
        private async Task<TokenInfo> GetTokenInfo(TokenRoleType tokenRoleType)
        {
            var informationForToken = new Dictionary<TokenRoleType, string[]>();

            informationForToken.Add(TokenRoleType.UserAdmin, 
                new string[] { this.config.GetValue("AdminUserLogin"), this.config.GetValue("AdminUserPassword") });
            informationForToken.Add(TokenRoleType.UserWithRole,
                new string[] { this.config.GetValue("UserWithRoleLogin"), this.config.GetValue("UserWithRolePassword") });
            informationForToken.Add(TokenRoleType.User2WithRole,
                new string[] { this.config.GetValue("User2WithRoleLogin"), this.config.GetValue("User2WithRolePassword") });
            informationForToken.Add(TokenRoleType.UserWithFakeRole,
                new string[] { this.config.GetValue("UserWithFakeRoleLogin"), this.config.GetValue("UserWithFakeRolePassword") });
            informationForToken.Add(TokenRoleType.UserForNotUniqueElements,
               new string[] { this.config.GetValue("ForNotUniqueElementsRoleLogin"), this.config.GetValue("ForNotUniqueElementsRolePassword") });
            
            var now = DateTime.UtcNow;
            var userName = informationForToken[tokenRoleType][0];
            var userPassword = informationForToken[tokenRoleType][1];

            using (var client = new HttpClient())
            {
                var contentModel = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("scope", "openid profile email roles  viewer_api core_logic_facade"),
                    new KeyValuePair<string, string>("response_type", "id_token token"),
                    new KeyValuePair<string, string>("username", $"{userName}"),
                    new KeyValuePair<string, string>("password", $"{userPassword}")
                };

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(authUrl))
                {
                    Content = new FormUrlEncodedContent(contentModel)
                };

                foreach (var header in this.authHeaders)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }

                requestMessage.Headers.Add("Authorization", "Basic cm8uY2xpZW50OmFtV25Cc3B9dipvfTYkSQ==");

                var response = await client.SendAsync(requestMessage);
                var responseString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseString))
                {
                    throw new HttpRequestException($"Ошибка получения токена для пользователя {userName}. Пожалуйста, обратитесь к администратору");
                }

                var responseStringArgs = responseString.Trim('{', '}').Split(',');

                if (responseStringArgs.Length < 3)
                {
                    throw new FormatException($"Формат полученного токена для пользователя {userName} не соответствует требованиям. Пожалуйста, обратитесь к администратору");
                }

                return new TokenInfo
                {
                    AccessToken = responseStringArgs[0]?.Split(':')[1]?.Trim('"'),
                    ExpiredDate = now.AddSeconds(Convert.ToInt32(responseStringArgs[1]?.Split(':')[1]?.Trim('"'))),
                    TokenType = responseStringArgs[2]?.Split(':')[1]?.Trim('"'),
                };
            }
        }
    }
}
