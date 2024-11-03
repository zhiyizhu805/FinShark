using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Interfaces;
using api.Mappers;
using api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        public StockController(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepo.GetAllAsync();
            var stockDto = stocks.Select(s => s.ToStokeDto());
            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound(); 
            }

            return Ok(stock.ToStokeDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromStockDto();
            await _stockRepo.CreateAsync(stockModel);
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id}, stockModel.ToStokeDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            var stockModel = await _stockRepo.UpdateAsync(id, updateDto);
            if (stockModel == null)
            {
                return NotFound();
            }
            return Ok(stockModel.ToStokeDto());

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var stockModel = await _stockRepo.DeleteAsync(id);
            if(stockModel == null)
            {
                return NotFound();
            }
            return NoContent();
        }


    }
}

/*
Steps to create Dtos and use Mapper to map the model class to a Dto class:
1. Create DTO for a Model Class:
   - Define a Data Transfer Object (DTO) class with properties that match or are derived from those in the model class.
   -This class usually has only the necessary fields for data transfer and may exclude sensitive or unnecessary data.
2. Create Mapper as an Extension Method for the Model Class:
   - Write a static extension method in a separate static class (e.g., StockMappers) to map the model to the DTO.
   - Use this ModelType model as the first parameter to create an extension method, allowing you to call it directly on instances of the model class.
3. Map the Model to DTO in the Controller:
   - In the controller, when you retrieve a model instance (e.g., Stock), call the extension method (e.g., stock.ToStockDto()) to map the model to a DTO before returning it.
********************************************************************
ControllerBase and IActionResult:

ControllerBase is a lightweight base class for API controllers in ASP.NET Core, providing core HTTP response methods.
IActionResult is a flexible return type that lets you return different HTTP responses (Ok(), NotFound(), etc.).
Ok() and Ok(stocks):

Ok() returns HTTP 200, indicating success.
Ok(stocks) returns HTTP 200 with stocks as the response data.
Deferred Execution with ToList():

ToList() triggers immediate execution of the query, converting data to a list, which ends deferred execution (where the query runs only when needed).
Find Method:

Find(id) is optimal for retrieving items by primary key, using caching to reduce database calls if the item was recently accessed.
NotFound():

Returns HTTP 404, indicating the item wasn’t found in the database, commonly used when Find returns null.


****************************************************
Find: Use when searching by primary key; it's optimized with caching in Entity Framework.

FirstOrDefault: Ideal for custom queries (not just primary keys) and is often used in deletes for flexibility in conditions.

NoContent(): Returns HTTP 204, signaling a successful DELETE without any data.



***********************************************************

When to Use Asynchronous:

Use async for tasks that are slow or involve external systems (like databases or network calls). This approach allows other code to run without waiting for the slow operation to complete.
How to Implement Asynchronous in .NET:

1.async keyword: Marks a method as asynchronous, enabling the use of await within it.
2.await keyword: Although it seems to "wait," await actually releases the thread, allowing other code (including other requests) to be handled on available threads. When the awaited task completes, the method resumes execution without causing a blocking delay.
3.Return Task or Task<returnType>: Represents an operation in progress. Use Task if there’s no return value, and Task<returnType> if returning a result.
4.Async Suffix: Add "Async" to asynchronous method names as a convention (e.g., ToListAsync).
Example Workflow:

For database calls, use await with methods like ToListAsync() or SaveChangesAsync() to keep the application responsive, as these calls don’t block the thread and allow other requests to proceed.

***********************************************************

var stockDto = stocks.Select(s => s.ToStokeDto());
Select operates on data already retrieved, and using async for Select is unnecessary

*/