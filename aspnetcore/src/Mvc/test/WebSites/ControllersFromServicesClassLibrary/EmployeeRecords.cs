using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesClassLibrary;

public class EmployeeRecords : Controller
{
    [HttpPut("/employee/update_records")]
    public IActionResult UpdateRecords(string recordId)
    {
        return Content("Updated record " + recordId);
    }

    [HttpPost]
    // This action uses conventional routing.
    public IActionResult Save(string id)
    {
        return Content("Saved record employee #" + id);
    }
}
