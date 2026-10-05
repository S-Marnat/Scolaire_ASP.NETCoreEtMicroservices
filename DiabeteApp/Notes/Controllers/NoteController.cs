using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.DTOs;
using Notes.Services.Interfaces;

[Authorize(Roles = "Organisateur")]
[Route("api/[controller]")]
[ApiController]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    // GET: api/Note/patient/5
    [HttpGet("patient/{idPatient}")]
    public async Task<ActionResult<List<NoteReadDto>>> GetNoteByPatient(int idPatient)
    {
        try
        {
            var result = await _noteService.GetByPatientAsync(idPatient);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }

    // GET: api/Note/5
    [HttpGet("{id}")]
    public async Task<ActionResult<NoteReadDto>> GetNoteById(string id)
    {
        try
        {
            var result = await _noteService.GetByIdAsync(id);

            if (result == null)
                return NotFound("L'Id renseigné ne correspond à aucune note.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }

    // POST: api/Note
    [HttpPost]
    public async Task<ActionResult<NoteReadDto>> PostNote(NoteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Les informations fournies pour la note sont invalides.");

        try
        {
            var result = await _noteService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetNoteById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }
}
