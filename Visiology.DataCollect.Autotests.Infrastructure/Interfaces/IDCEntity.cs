using Visiology.DataCollect.Autotests.Entities.Results;
using Visiology.DataCollect.Autotests.Infrastructure.Models;

namespace Visiology.DataCollect.Autotests.Infrastructure.Interfaces
{
    public interface IDCEntity
    {
        ContentVerificationResult Create(DimensionInfo dimensionInfo);

        ContentVerificationResult Delete();

        ContentVerificationResult Edit();
    }
}
