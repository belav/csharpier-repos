using Microsoft.AspNetCore.Mvc;

namespace ControllersFromServicesClassLibrary;

public class TimeScheduleController
{
    [HttpGet("/schedule/{id:int}")]
    public IActionResult GetSchedule(int id)
    {
        return new ContentResult { Content = "No schedules available for " + id };
    }
}
