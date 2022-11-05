using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PharmacyApi.Data;
using PharmacyApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PharmacyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManufacturersController : ControllerBase
    {
        private readonly PharmacyDbContext context;

        public ManufacturersController(IServiceProvider services)
        {
            context = services.GetRequiredService<PharmacyDbContext>();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Manufacturer>> Get()
        {
            try
            {
                var result = context.Manufacturers.ToArray();

                if (result == null) return NotFound();

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Manufacturer> Get(int id)
        {
            try
            {
                var result = context.Manufacturers.FirstOrDefault(m => m.ManufacturerId == id);

                if (result == null) return NotFound();

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Manufacturer value)
        {
            try
            {
                await context.Manufacturers.AddAsync(value);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error inserting data to the database");
            }           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Manufacturer value)
        {
            try
            {
                var existingManuf = context.Manufacturers.SingleOrDefault(m => m.ManufacturerId == id);
                if (existingManuf == null) 
                    return NotFound();

                context.Entry(existingManuf).State = EntityState.Detached;
                if (id != value.ManufacturerId)
                    value.ManufacturerId = id;
                context.Manufacturers.Update(value);

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data in the database");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingManuf = context.Manufacturers.SingleOrDefault(m => m.ManufacturerId == id);
                if (existingManuf == null) return NotFound();
                context.Manufacturers.Remove(existingManuf);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data from the database");
            }
        }
    }
}
