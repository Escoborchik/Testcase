using Microsoft.AspNetCore.Mvc;
using UssJuniorTest.DTO;
using UssJuniorTest.Infrastructure.Store;

namespace UssJuniorTest.Controllers;

[ApiController]
[Route("api/driveLog")]
public class DriveLogController : Controller 
{
    private readonly IStore _store;
    public DriveLogController(IStore store)
    {
        _store = store;
    }

    [HttpGet]
    public IActionResult GetDriveLogsAggregation([FromQuery] QueryParameters queryParameters)
    {                                                     
        var result = GetData(queryParameters);
        result = DoFilter(queryParameters,result);         

        return Ok(result);
    }

    private  IQueryable<DriveLogDTO> GetData(QueryParameters queryParameters)
    {
        var driveLogs = _store.GetAllDriveLogs().Where(log => log.StartDateTime <= queryParameters.End && log.EndDateTime >= queryParameters.Begin);
        var persons = _store.GetAllPersons();
        var cars = _store.GetAllCars();

        var personsWithTime = driveLogs.Join(persons, log => log.PersonId, person => person.Id,
                                             (log, person) => new { person.Name, person.Age, Time = log.EndDateTime - log.StartDateTime });
        var carsWithTime = driveLogs.Join(cars, log => log.CarId, car => car.Id,
                                          (log, car) => new { car.Manufacturer, car.Model, Time = log.EndDateTime - log.StartDateTime });

        return personsWithTime.Join(carsWithTime, p => p.Time, c => c.Time,
                                          (person, car) => new DriveLogDTO { Name = person.Name, Age = person.Age, Manufacturer = car.Manufacturer, Model = car.Model, Time = car.Time });
    }

    private static IQueryable<DriveLogDTO> DoFilter(QueryParameters queryParameters, IQueryable<DriveLogDTO> result )
    {
        if (!string.IsNullOrEmpty(queryParameters.FilterByName))
        {
            result = result.Where(log => log.Name.Contains(queryParameters.FilterByName));
        }

        if (!string.IsNullOrEmpty(queryParameters.FilterByCarManufacturer))
        {
            result = result.Where(log => log.Manufacturer.Contains(queryParameters.FilterByCarManufacturer));
        }

        if (!string.IsNullOrEmpty(queryParameters.SortByNameOrManufacturer))
        {
            switch (queryParameters.SortByNameOrManufacturer.ToLower())
            {
                case "name":
                    result = result.OrderBy(log => log.Name);
                    break;
                case "manufacturer":
                    result = result.OrderBy(log => log.Manufacturer);
                    break;
                default:
                    break;
            }
        }

        return result.Skip((queryParameters.Page - 1) * queryParameters.PageSize)
                                     .Take(queryParameters.PageSize);
    }
}