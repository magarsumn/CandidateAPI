using CandidateAPI.Data;
using CandidateAPI.Interfaces;
using CandidateAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CandidateAPI.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ApplicationDbContext _appDbContext;
        public CandidateService(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Candidate> AddOrUpdateCandidateAsync(Candidate candidate)
        {
            var existingCandidateByEmail = await _appDbContext.Candidates
                .FirstOrDefaultAsync(c => c.Email == candidate.Email && c.Id != candidate.Id);

            if (existingCandidateByEmail != null)
            {
                throw new Exception("A candidate with this email already exists.");
            }

            // Find the candidate by ID and update
            var existingCandidate = await _appDbContext.Candidates
                .FirstOrDefaultAsync(c => c.Id == candidate.Id);

            if (existingCandidate != null)
            {
                // Update existing candidate
                existingCandidate.FirstName = candidate.FirstName;
                existingCandidate.LastName = candidate.LastName;
                existingCandidate.Email = candidate.Email;
                existingCandidate.PhoneNumber = candidate.PhoneNumber;
                existingCandidate.PreferredCallTime = candidate.PreferredCallTime;
                existingCandidate.LinkedInProfile = candidate.LinkedInProfile;
                existingCandidate.GitHubProfile = candidate.GitHubProfile;
                existingCandidate.Comments = candidate.Comments;

                _appDbContext.Candidates.Update(existingCandidate);
            }
            else
            {
                // Create a new candidate
                await _appDbContext.Candidates.AddAsync(candidate);
            }

            await _appDbContext.SaveChangesAsync();
            return candidate;
        }
    }
}