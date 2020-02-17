using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visiology.DataCollect.Autotests.Entities.Results;
using Visiology.DataCollect.Integration.Tests.Infrastructure.Interfaces;
using Visiology.DataCollect.Integration.Tests.Models;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Impl
{
    public class Verifier<T> : IVerifier<T>
        where T : IResponseContent
    {
        public virtual ContentVerificationResult Verify(IEnumerable<T> contentList, IEnumerable<T> expectedList)
        {
            throw new NotImplementedException();
        }

        public ContentVerificationResult VerifyCount(IEnumerable<T> contentList, int expectedEntitiesCount)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = contentList != null
            };


            if (result.IsSuccess)
            {
                if (contentList.Count() != expectedEntitiesCount)
                {
                    return new ContentVerificationResult
                    {
                        Message = "При выполнении автотеста пользователь получил список не всех доступных ему сущностей." +
                        $"Ожидалось - {expectedEntitiesCount}, пришло - {contentList.Count()}" +
                        "Пожалуйста, проверьте настройки конфигурации или обратитесь к администратору",
                        IsSuccess = false
                    };
                }
            }

            return result;
        }
    }
}
