﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Configuration;
using Visiology.DataCollect.Autotests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Entities;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Impl;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Visiology.DataCollect.Integration.Tests.Models;
using Xunit;

namespace Visiology.DataCollect.Autotests.API.Tests
{
    public abstract class BaseIntegrationTests<TList, TEntity> : IClassFixture<TokenFixture>, IClassFixture<RestService>
        where TList : IResponseContentList<TEntity>
        where TEntity : IResponseContent
    {
        protected TokenFixture _tokenFixture;
        protected RestService _restService;
        protected ConfigurationManager config;

        /// <summary>
        /// Верификатор результатов
        /// </summary>
        protected IVerifier<TEntity> Verifier { get; }

        /// <summary>
        /// Значение заголовка X-API-VERSION (берем из атрибута)
        /// </summary>
        public string XApiVersion
        {
            get
            {
                Type t = GetType();
                object[] attrs = t.GetCustomAttributes(false);

                return (attrs.First() as XApiVersionAttribute)?.Value;
            }
        }

        /// <summary>
        /// Тип тестируемого метода
        /// </summary>
        public virtual Method Method { get; set; }

        /// <summary>
        /// Url тестируемого метода
        /// </summary>
        public abstract string Url { get; set; }

        /// <summary>
        /// Заголовки запроса
        /// </summary>
        public virtual Dictionary<string, string> Headers { get; set; }

        public BaseIntegrationTests(TokenFixture tokenFixture, RestService restService, IVerifier<TEntity> verifier = null)
        {
            _tokenFixture = tokenFixture;
            _restService = restService;
            config = new ConfigurationManager();

            Verifier = verifier ?? new Verifier<TEntity>();

            Headers = new Dictionary<string, string>
                {
                    { "X-API-VERSION", XApiVersion }
                };
        }

        protected async Task<GetContentResult<TList, TEntity>> TryGetEntities(
            Method method,
            string requestUri,
            TokenRoleType tokenRoleType,
            Dictionary<string, object> parameters = null,
            Dictionary<string, string> headers = null,
            string body = null)
        {
            var result = new GetContentResult<TList, TEntity>();
            var response = await _restService.SendRequestAsync(
                method,
                requestUri,
                tokenRoleType,
                _tokenFixture.Tokens,
                parameters, headers, body);

            result.ContentVerificationResult.IsSuccess = response.IsSuccessful;
            result.ContentVerificationResult.Message = $"{response.StatusCode} {response.StatusDescription} {response.Content}";

            if (result.ContentVerificationResult.IsSuccess)
            {
                try
                {
                    result.Content = await Task.Run(() => JsonConvert.DeserializeObject<TList>(response.Content));
                }
                catch (Exception e)
                {
                    result.ContentVerificationResult.IsSuccess = false;
                    result.ContentVerificationResult.Message += e.Message;

                    return result;
                }
            }

            return result;
        }

        protected abstract string GetUrl(string entityId);

        protected abstract string GetSearchUrl(string entityId);
    }
}
