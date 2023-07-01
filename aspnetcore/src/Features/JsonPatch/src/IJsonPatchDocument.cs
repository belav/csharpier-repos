using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;

namespace Microsoft.AspNetCore.JsonPatch;

public interface IJsonPatchDocument
{
    IContractResolver ContractResolver { get; set; }

    IList<Operation> GetOperations();
}
