using CandidateAPI.Data;
using CandidateAPI.Interfaces;
using CandidateAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CandidateAPI.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMemoryCache _memoryCache;
        private const string CandidateCacheKey = "Cache_Candidate_{0}";

        public CandidateService(ApplicationDbContext appDbContext, IMemoryCache memoryCache)
        {
            _appDbContext = appDbContext;
            _memoryCache = memoryCache;
        }

        public async Task<Candidate> AddOrUpdateCandidateAsync(Candidate candidate)
        {
            var existingCandidateByEmail = await _appDbContext.Candidates
                .FirstOrDefaultAsync(c => c.Email == candidate.Email && c.Id != candidate.Id);

            if (existingCandidateByEmail != null)
            {
                throw new Exception("A candidate with this email already exists.");
            }

            // Try to get the candidate from cache first
            string cacheKey = string.Format(CandidateCacheKey, candidate.Id);
            if (!_memoryCache.TryGetValue(cacheKey, out Candidate existingCandidate))
            {
                // If candidate not found in cache, check in database
                existingCandidate = await _appDbContext.Candidates
                    .FirstOrDefaultAsync(c => c.Id == candidate.Id);
            }

            if (existingCandidate != null)
            {
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
                await _appDbContext.Candidates.AddAsync(candidate);
                existingCandidate = candidate;
            }

            await _appDbContext.SaveChangesAsync();

            // Update the cache with the new or updated candidate
            _memoryCache.Set(cacheKey, existingCandidate, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return existingCandidate;
        }
    }
}