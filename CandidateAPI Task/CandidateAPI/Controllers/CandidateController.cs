using CandidateAPI.Interfaces;
using CandidateAPI.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CandidateController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpPost]
    public async Task<IActionResult> AddOrUpdateCandidate(Candidate candidate)
    {
        try
        {
            var result = await _candidateService.AddOrUpdateCandidateAsync(candidate);
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
        }
    }
}