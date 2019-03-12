using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArtefactEntities;
using ArtefactEntities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtefactGeneratorWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtefactController : ControllerBase
    {
        private readonly DataContext _db;
        public ArtefactController(DataContext db)
        {
            _db = db;
        }

        // GET: api/Artefact
        [HttpGet]
        public IEnumerable<Artefacts> Get()
        {
            return _db.Artefacts;
        }

        // GET: api/Artefact/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var artefacts = await _db.Artefacts.FindAsync(id);

            if (artefacts == null)
            {
                return NotFound();
            }

            return Ok(artefacts);
        }

        [HttpGet("search/{category}/{type}/{setpiece}")]
        public IActionResult Get([FromRoute] string category, [FromRoute] string type, [FromRoute] bool setpiece)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = _db.Artefacts.Where(artefacts => (artefacts.Category == category || category == "NULL") && (artefacts.Type == type || type == "NULL") && artefacts.setpiece == setpiece);

            if (response.Count() == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var artefactCategories = _db.Artefacts.Select(c => c.Category).Distinct().ToList();
            return Ok(artefactCategories);
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTypes()
        {
            var artefactsTypes = _db.Artefacts.Select(c => c.Type).Distinct().ToList();
            return Ok(artefactsTypes);
        }
        // POST: api/Artefact
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Artefacts artefacts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Artefacts.Add(artefacts);
            await _db.SaveChangesAsync();

            CreatedAtAction("Get", new { id = artefacts.Id }, artefacts);
            return Ok();
        }

        // PUT: api/Artefact/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] Artefacts artefacts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != artefacts.Id)
            {
                return BadRequest();
            }

            _db.Entry(artefacts).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (id != artefacts.Id)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var artefacts = _db.Artefacts.First(c => c.Id == id);

            _db.Entry(artefacts).State = EntityState.Deleted;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (id != artefacts.Id)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    
    }
}
