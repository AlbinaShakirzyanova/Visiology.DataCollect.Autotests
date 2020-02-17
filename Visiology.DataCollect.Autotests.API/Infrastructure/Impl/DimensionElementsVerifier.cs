using System.Collections.Generic;
using System.Linq;
using Visiology.DataCollect.Autotests.Entities.Results;
using Visiology.DataCollect.Integration.Tests.Models;
using Visiology.DataCollect.Integration.Tests.Models.Dimensions.Elements;

namespace Visiology.DataCollect.Integration.Tests.Infrastructure.Impl
{
    /// <summary>
    /// Верификатор элементов измерения
    /// </summary>
    public class DimensionElementsVerifier : Verifier<ElementDto>
    {
        public override ContentVerificationResult Verify(IEnumerable<ElementDto> contentList, IEnumerable<ElementDto> expectedList)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = contentList != null
            };

            if (result.IsSuccess)
            {
                var verificateElementsCountResult = this.VerifyCount(contentList, expectedList.Count());

                result.IsSuccess &= verificateElementsCountResult.IsSuccess;
                result.Message += verificateElementsCountResult.Message;

                var verificateElementsUniqueResult = this.VerifyUnique(contentList);

                result.IsSuccess &= verificateElementsUniqueResult.IsSuccess;
                result.Message += verificateElementsUniqueResult.Message;

                var verificateElementsNameResult = this.VerifyName(contentList, expectedList);

                result.IsSuccess &= verificateElementsNameResult.IsSuccess;
                result.Message += verificateElementsNameResult.Message;


                var verificateElementsAttributesResult = this.VerifyAttributes(contentList, expectedList);

                result.IsSuccess &= verificateElementsAttributesResult.IsSuccess;
                result.Message += verificateElementsAttributesResult.Message;

                var verificateElementsPathResult = this.VerifyPath(contentList, expectedList);

                result.IsSuccess &= verificateElementsPathResult.IsSuccess;
                result.Message += verificateElementsPathResult.Message;
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

        private ContentVerificationResult VerifyName(IEnumerable<ElementDto> contentList, IEnumerable<ElementDto> expectedElements)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            foreach (var item in expectedElements)
            {
                var element = contentList.FirstOrDefault(e => e.Id == item.Id);

                if (element == null)
                {
                    result.IsSuccess = false;
                    result.Message += $"По запросу не пришел ожидаемый элемент {item.Name} папки {item.Path.ToArray()}";

                    continue;
                }

                if (item.Name != element.Name)
                {

                    result.Message += "Имя элемента не соответствует указанному в автотесте." +
                        $"Ожидалось - {item.Name}, а пришло - {element.Name}.";
                    result.IsSuccess = false;

                    continue;
                }
            }

            return result;
        }

        private ContentVerificationResult VerifyPath(IEnumerable<ElementDto> contentList, IEnumerable<ElementDto> expectedElements)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            foreach (var item in expectedElements)
            {
                var element = contentList.FirstOrDefault(e => e.Id == item.Id);

                if (element == null)
                {
                    result.IsSuccess = false;
                    result.Message += $"По запросу не пришел ожидаемый элемент {item.Name} папки {item.Path.ToArray()}";

                    continue;
                }

                if (item.Path.Count() != element.Path.Count())
                {

                    result.Message += "Расположение элемента не соответствует указанному в автотесте." +
                        $"Ожидалось - {item.Path.ToArray()}, а пришло - {element.Path.ToArray()}.";
                    result.IsSuccess = false;

                    continue;
                }

                var i = 0;
                while (i < item.Path.Count())
                {
                    if (item.Path[i].FolderName != element.Path[i].FolderName)
                    {
                        result.Message += "Расположение элемента не соответствует указанному в автотесте.";
                        result.IsSuccess = false;
                    }

                    i++;
                }
            }

            return result;
        }

        private ContentVerificationResult VerifyAttributes(IEnumerable<ElementDto> contentList, IEnumerable<ElementDto> expectedElements)
        {
            var result = new ContentVerificationResult
            {
                IsSuccess = true
            };

            foreach (var item in expectedElements)
            {
                var element = contentList.FirstOrDefault(e => e.Id == item.Id);

                if (element == null)
                {
                    result.IsSuccess = false;
                    result.Message += $"По запросу не пришел ожидаемый элемент {item.Name} папки {item.Path.ToArray()}";

                    continue;
                }

                foreach (var attribute in item.Attributes)
                {
                    var elemAttribute = element.Attributes.FirstOrDefault(a => a.AttributeId == attribute.AttributeId);
                    if (elemAttribute == null)
                    {

                        result.Message += $"У элемента {element.Name} не найден атрибут с идентификатором {attribute.AttributeId}";
                        result.IsSuccess = false;

                        continue;
                    }

                    if (attribute.Value == null && elemAttribute.Value == null)
                    {
                        continue;
                    }

                    if (attribute.Value?.ToString() != elemAttribute?.Value?.ToString())
                    {
                        result.Message += $"У атрибута {attribute.AttributeId} элемента {element.Name} значение {elemAttribute.Value}, а ожидалось - {attribute.Value}.";
                        result.IsSuccess = false;
                    }
                }
            }

            return result;
        }
    }
}
