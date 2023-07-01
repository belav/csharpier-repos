using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesClassLibrary;

[NonController]
public class ClientUIStubController
{
    public object GetClientContent(int id)
    {
        return new object();
    }
}
