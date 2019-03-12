using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtefactEntities;
using ArtefactEntities.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json;

namespace ArtefactGeneratorASPNET.Controllers
{
    public class ArtefactsController : Controller
    {
        private readonly HttpClient _httpClient;
        private Uri BaseEndPoint { get; set; }

        public ArtefactsController()
        {
            BaseEndPoint = new Uri("https://localhost:44309/api/artefact");
            _httpClient = new HttpClient();
        }

        // GET: Artefacts
        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = JsonConvert.DeserializeObject<List<string>>
                (_httpClient.GetAsync(BaseEndPoint + "/categories", HttpCompletionOption.ResponseContentRead)
                .Result.Content.ReadAsStringAsync().Result);
            ViewBag.Types = JsonConvert.DeserializeObject<List<string>>
                (_httpClient.GetAsync(BaseEndPoint + "/types", HttpCompletionOption.ResponseContentRead)
                .Result.Content.ReadAsStringAsync().Result);

            var response = await _httpClient.GetAsync(BaseEndPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return View(JsonConvert.DeserializeObject<List<Artefacts>>(data));
        }

        //GET: Artefacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync
                (BaseEndPoint + $"/{id}", HttpCompletionOption.ResponseHeadersRead);
            var data = await response.Content.ReadAsStringAsync();
            var artefacts = JsonConvert.DeserializeObject<Artefacts>(data);
            if (artefacts == null)
            {
                return NotFound();
            }

            return View(artefacts);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]string category, [FromForm] string type, [FromForm] string setpiece)
        {
            ViewBag.Categories = JsonConvert.DeserializeObject<List<string>>
                (_httpClient.GetAsync(BaseEndPoint + "/categories", HttpCompletionOption.ResponseContentRead)
                .Result.Content.ReadAsStringAsync().Result);
            ViewBag.Types = JsonConvert.DeserializeObject<List<string>>
                (_httpClient.GetAsync(BaseEndPoint + "/types", HttpCompletionOption.ResponseContentRead)
                .Result.Content.ReadAsStringAsync().Result);
            bool setpieceBool = false;
            if (category is null)
            {
                category = "NULL";
            }
            if (type is null)
            {
                type = "NULL";
            }
            if (setpiece == "on")
            {
                setpieceBool = true;
            }
            var response = await _httpClient.GetAsync(BaseEndPoint + $"/search/{category}/{type}/{setpieceBool}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return View(JsonConvert.DeserializeObject<List<Artefacts>>(data));
        }

        // GET: Artefacts/Create
        public IActionResult Create()
        {
            ViewBag.Categories = JsonConvert.DeserializeObject<List<string>>
               (_httpClient.GetAsync(BaseEndPoint + "/categories", HttpCompletionOption.ResponseContentRead)
               .Result.Content.ReadAsStringAsync().Result);
            ViewBag.Types = JsonConvert.DeserializeObject<List<string>>
                (_httpClient.GetAsync(BaseEndPoint + "/types", HttpCompletionOption.ResponseContentRead)
                .Result.Content.ReadAsStringAsync().Result);
            return View();
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,Type,Description,setpiece")] Artefacts artefacts)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync<Artefacts>(BaseEndPoint, artefacts);
                response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            return View(artefacts);
        }

        // GET: Artefacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync(BaseEndPoint + $"/{id}", HttpCompletionOption.ResponseHeadersRead);
            var data = await response.Content.ReadAsStringAsync();
            var artefacts = JsonConvert.DeserializeObject<Artefacts>(data);
            if (artefacts == null)
            {
                return NotFound();
            }
            return View(artefacts);
        }

        // POST: Artefacts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Type,Description,setpiece")] Artefacts artefacts)
        {
            if (id != artefacts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Post the created gift as JSON to API. HttpClient handles serialization for us
                    var response = await _httpClient.PutAsJsonAsync<Artefacts>(BaseEndPoint + $"/{id}", artefacts);
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
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
                return RedirectToAction(nameof(Index));
            }
            return View(artefacts);
        }

        // GET: Artefacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync
                (BaseEndPoint + $"/{id}", HttpCompletionOption.ResponseHeadersRead);
            var data = await response.Content.ReadAsStringAsync();
            var artefacts = JsonConvert.DeserializeObject<Artefacts>(data);
            if (artefacts == null)
            {
                return NotFound();
            }

            return View(artefacts);
        }

        // POST: Artefacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, Artefacts artefacts)
        {
            if (id != artefacts.Id)
            {
                return NotFound();
            }
            var response = await _httpClient.DeleteAsync(BaseEndPoint + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ArtefactsExists(int id)
        {
            var response = await _httpClient.GetAsync(BaseEndPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var context = JsonConvert.DeserializeObject<List<Artefacts>>(data);
            return context.Any(e => e.Id == id);
        }
    }
}
