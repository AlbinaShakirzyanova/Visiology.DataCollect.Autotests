using System.Collections.Generic;
using System.Linq;
using Visiology.DataCollect.Autotests.Entities.Results;
using Visiology.DataCollect.Integration.Tests.Models;
using Visiology.DataCollect.Integration.Tests.Models.MeasureGroups.Forms;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Impl
{
    /// <summary>
    /// Верификатор инстансов шаблона форм
    /// </summary>
    public class FormInstancesVerifier : Verifier<FormDto>
    {
        public override ContentVerificationResult Verify(IEnumerable<FormDto> contentList, IEnumerable<FormDto> expectedList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = contentList != null
            };

            if (result.IsSuccess)
            {
                var verificateInstancesCountResult = this.VerifyCount(contentList, expectedList.Count());

                result.IsSuccess &= verificateInstancesCountResult.IsSuccess;
                result.Message += verificateInstancesCountResult.Message;

                var verificateInstancesUniqueResult = this.VerifyUnique(contentList);

                result.IsSuccess &= verificateInstancesUniqueResult.IsSuccess;
                result.Message += verificateInstancesUniqueResult.Message;

                var verificateInstancesCoordinatesResult = this.VerifyCoordinates(contentList, expectedList);

                result.IsSuccess &= verificateInstancesCoordinatesResult.IsSuccess;
                result.Message += verificateInstancesCoordinatesResult.Message;
            }

            return result;
        }

        private ContentVerificationResult VerifyUnique(IEnumerable<FormDto> contentList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            foreach (var item in contentList)
            {
                if (contentList.Where(i => i.UniqueIdentifier == item.UniqueIdentifier).Count() != 1)
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

        private ContentVerificationResult VerifyCoordinates(IEnumerable<FormDto> contentList, IEnumerable<FormDto> expectedList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };


            var forms = contentList.ToList();

            foreach (var item in expectedList)
            {
                var form = forms.FirstOrDefault(f => f.Equals(item));

                if (form == null)
                {
                    result.IsSuccess = false;
                    result.Message += $"Не корректное получение экземпляров форм по шаблону {item.FormTemplateName}";

                    continue;
                }

                forms.Remove(form);
            }

            if (forms.Count() > 0)
            {
                result.IsSuccess = false;
                result.Message += $"Ошибка построения экземпляров форм";
            }

            return result;
        }
    }
}
