using Visiology.DataCollect.Autotests.Entities.Results;

namespace Visiology.DataCollect.Autotests.Infrastructure.Interfaces
{
    public interface IDCEntity
    {
        ContentVerificationResult Create();

        ContentVerificationResult Delete();

        ContentVerificationResult Edit();
    }
}
