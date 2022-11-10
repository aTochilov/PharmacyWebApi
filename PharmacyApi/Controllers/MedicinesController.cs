using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacyApi.Data.Entities;
using PharmacyApi.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace PharmacyApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MedicinesController : ControllerBase
    {
        private readonly PharmacyDbContext context;

        public MedicinesController(PharmacyDbContext context)
        {
            this.context = context;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Medicine>> Get()
        {
            try
            {
                var result = context.Medicines.ToArray();

                if (result == null) return NotFound();

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public ActionResult<Medicine> Get(int id)
        {
            try
            {
                var result = context.Medicines.Include(m => m.Manufacturer).FirstOrDefault(m => m.MedicineId == id);

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
        public async Task<IActionResult> Post([FromBody] Medicine value)
        {
            try
            {
                await context.Medicines.AddAsync(value);
                await context.SaveChangesAsync();
                return Ok("Saved");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error inserting data to the database");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Medicine value)
        {
            try
            {
                var existingManuf = context.Medicines.SingleOrDefault(m => m.MedicineId == id);
                if (existingManuf == null)
                    return NotFound();

                context.Entry(existingManuf).State = EntityState.Detached;
                if (id != value.MedicineId)
                    value.MedicineId = id;
                context.Medicines.Update(value);

                await context.SaveChangesAsync();
                return Ok("Saved");
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
                var existingMed = context.Medicines.SingleOrDefault(m => m.MedicineId == id);
                if (existingMed == null) return NotFound();
                context.Medicines.Remove(existingMed);
                await context.SaveChangesAsync();
                return Ok("Removed");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data from the database");
            }
        }
    }
}
