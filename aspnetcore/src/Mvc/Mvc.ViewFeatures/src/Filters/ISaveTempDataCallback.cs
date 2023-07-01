using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Filters;

internal interface ISaveTempDataCallback : IFilterMetadata
{
    void OnTempDataSaving(ITempDataDictionary tempData);
}
