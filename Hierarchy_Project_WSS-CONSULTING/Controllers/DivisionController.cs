using Hierarchy_Project_WSS_CONSULTING.Helpers;
using Hierarchy_Project_WSS_CONSULTING.Models;
using Hierarchy_Project_WSS_CONSULTING.Models.DB;
using Hierarchy_Project_WSS_CONSULTING.Models.Dtos;
using Hierarchy_Project_WSS_CONSULTING.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml;


namespace Hierarchy_Project_WSS_CONSULTING.Controllers
{
    public class DivisionController : Controller
    {
        private readonly DivisionService _divisionService;

        public DivisionController(DivisionService divisionService)
        {
            _divisionService = divisionService;
        }

        public async Task<ActionResult> Index()
        {
            var divisions = await GetAll();
            var divisionViewModels = divisions.Select(d => new DivisionViewModel
            {
                Id = d.Id,
                Name = d.Name,
                PathFromPatriarch = d.PathFromPatriarch,
            }).ToList();

            return View(divisionViewModels);
        }

        [HttpGet("get-all")]
        public async Task<List<Division>> GetAll()
        {
            return await _divisionService.GetAllAsync();
        }

        [HttpPost("add")]
        public async Task Add([FromBody]NewDivisionDto newDivision)
        {
            await _divisionService.AddAsync(newDivision);
        }

        [HttpDelete("delete")]
        public async Task Delete(string divisionToDeleteID)
        {
            await _divisionService.DeleteEntityAndChildrenAsync(divisionToDeleteID);
        }

        [HttpPut("move")]
        public async Task Move([FromBody]MoveDivisionDto moveDivision)
        {
            await _divisionService.Move(moveDivision);
        }

        [HttpPut("rewrite-from-xml-string")]
        public async Task<IActionResult> RewriteFromXmlString([FromBody] XmlDivisionListDto xmlDto)
        {
            try
            {
                var divisions = await XmlHelper.DeserializeAsync<List<Division>>(xmlDto.Xml);
                await _divisionService.RewriteDivisions(divisions);
                return Ok("Данные успешно обновлены.");
            }
            catch (XmlException xmlEx)
            {
                return BadRequest($"Ошибка при разборе XML: {xmlEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Ошибка базы данных: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }

        [HttpPost("rewrite-from-xml-file")]
        public async Task<IActionResult> RewriteFromXmlFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не был предоставлен.");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var xml = await reader.ReadToEndAsync();
                    var divisions = await XmlHelper.DeserializeAsync<List<Division>>(xml);
                    await _divisionService.RewriteDivisions(divisions);
                    return Ok("Данные успешно обновлены.");
                }
            }
            catch (XmlException xmlEx)
            {
                return BadRequest($"Ошибка при разборе XML: {xmlEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Ошибка базы данных: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Произошла ошибка: {ex.Message}");
            }
        }

        [HttpGet("get-xml")]
        public async Task<string> GetXml()
        {
            var divisions = await _divisionService.GetAllAsync();
            return await XmlHelper.SerializeAsync<List<Division>>(divisions);
        }
    }
}
