using CandidateAPI.Models;

namespace CandidateAPI.Interfaces;

public interface ICandidateService
{
    Task<Candidate> AddOrUpdateCandidateAsync(Candidate candidate);
}