using System.Collections.Generic;
using System.Linq;
using Visiology.DataCollect.Autotests.API.Infrastructure.Entities.Results;
using Visiology.DataCollect.Autotests.API.Models.MeasureGroups.Elements;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Impl
{
    public class MeasureGroupElementsVerifier : Verifier<ElementDto>
    {
        public override ContentVerificationResult Verify(IEnumerable<ElementDto> contentList, IEnumerable<ElementDto> expectedList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = contentList != null
            };

            if (result.IsSuccess)
            {
                var verificateCountResult = this.VerifyCount(contentList, expectedList.Count());

                result.IsSuccess &= verificateCountResult.IsSuccess;
                result.Message += verificateCountResult.Message;

                var verificateUniqueResult = this.VerifyUnique(contentList);

                result.IsSuccess &= verificateUniqueResult.IsSuccess;
                result.Message += verificateUniqueResult.Message;

                var verificateInstancesCoordinatesResult = this.VerifyCoordinates(contentList, expectedList);

                result.IsSuccess &= verificateInstancesCoordinatesResult.IsSuccess;
                result.Message += verificateInstancesCoordinatesResult.Message;
            }

            return result;
        }

        private ContentVerificationResult VerifyUnique(IEnumerable<ElementDto> contentList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            foreach (var item in contentList)
            {
                if (contentList.Where(i => i.Id == item.Id).Count() != 1)
                {
                    return new ContentVerificationResult
                    {
                        Message = "Найдены повторяющиеся значения в полученном контенте.",
                        IsSuccess = false
                    };
                }
            }

            return result;
        }

        private ContentVerificationResult VerifyCoordinates(IEnumerable<ElementDto> contentList, IEnumerable<ElementDto> expectedList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };


            var elements = contentList.ToList();

            foreach (var item in expectedList)
            {
                var element = elements.FirstOrDefault(f => f.Equals(item));

                if (element == null)
                {
                    result.IsSuccess = false;
                    result.Message += $"Не корректное получение элемента формы.";

                    continue;
                }

                elements.Remove(element);
            }

            if (elements.Count() > 0)
            {
                result.IsSuccess = false;
                result.Message += $"Ошибка построения экземпляров форм";
            }

            return result;
        }
    }
}
