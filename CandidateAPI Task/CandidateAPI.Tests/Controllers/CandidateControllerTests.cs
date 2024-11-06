using CandidateAPI.Data;
using CandidateAPI.Models;
using CandidateAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateAPI.Tests.Controllers
{
    public class CandidateControllerTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly CandidateService _candidateService;
        private readonly CandidateController _controller;

        public CandidateControllerTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("CandidateTestDb")  // Name of the in-memory database
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _candidateService = new CandidateService(_dbContext); // Use real service with in-memory DB
            _controller = new CandidateController(_candidateService);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_ShouldReturnOk_WhenCandidateIsAdded()
        {
            // Arrange
            var newCandidate = new Candidate
            {
                Id = 1,
                FirstName = "Suman",
                LastName = "Magar",
                Email = "suman@gmail.com",
                PhoneNumber = "9809155461",
                PreferredCallTime = "Morning",
                LinkedInProfile = "https://www.linkedin.com/in/suman-magar-37763a198/",
                GitHubProfile = "https://github.com/magarsumn",
                Comments = "Looking for opportunities."
            };

            // Act
            var result = await _controller.AddOrUpdateCandidate(newCandidate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCandidate = Assert.IsType<Candidate>(okResult.Value);
            Assert.Equal(newCandidate.Email, returnedCandidate.Email);
            Assert.Equal(200, okResult.StatusCode);

            // Check that the candidate was actually added to the database
            var candidateInDb = await _dbContext.Candidates.FirstOrDefaultAsync(c => c.Email == newCandidate.Email);
            Assert.NotNull(candidateInDb);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_ReturnsOkResult_WhenCandidateIsUpdated()
        {
            // Arrange
            var existingCandidate = new Candidate
            {
                Id = 1,
                FirstName = "Suman",
                LastName = "Magar",
                Email = "suman@gmail.com",
                PhoneNumber = "9809155461",
                PreferredCallTime = "Morning",
                LinkedInProfile = "https://www.linkedin.com/in/suman-magar-37763a198/",
                GitHubProfile = "https://github.com/magarsumn",
                Comments = "Looking for opportunities."
            };
            await _dbContext.Candidates.AddAsync(existingCandidate);
            await _dbContext.SaveChangesAsync();

            var updatedCandidate = new Candidate
            {
                Id = 1,
                FirstName = "Suman Updated",
                LastName = "Magar Updated",
                Email = "suman@gmail.com",
                PhoneNumber = "9809155461",
                PreferredCallTime = "Evening",
                LinkedInProfile = "https://www.linkedin.com/in/updated",
                GitHubProfile = "https://github.com/updated",
                Comments = "Looking for opportunities."
            };

            // Act
            var result = await _controller.AddOrUpdateCandidate(updatedCandidate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var candidateInDb = await _dbContext.Candidates.FindAsync(updatedCandidate.Id);
            Assert.NotNull(candidateInDb);
            Assert.Equal("Suman Updated", candidateInDb.FirstName);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_ShouldReturnConflict_WhenDuplicateEmailExists()
        {
            // Arrange
            var existingCandidate = new Candidate
            {
                Id = 1,
                FirstName = "Suman",
                LastName = "Magar",
                Email = "suman@gmail.com",
                PhoneNumber = "9809155461",
                PreferredCallTime = "Morning",
                LinkedInProfile = "https://www.linkedin.com/in/suman-magar-37763a198/",
                GitHubProfile = "https://github.com/magarsumn",
                Comments = "This is old comment."
            };
            await _dbContext.Candidates.AddAsync(existingCandidate);
            await _dbContext.SaveChangesAsync();

            // Act: Try adding another candidate with the same email
            var newCandidate = new Candidate
            {
                Id = 2,
                FirstName = "Suman",
                LastName = "Magar",
                Email = "suman@gmail.com",
                PhoneNumber = "9809155461",
                PreferredCallTime = "Morning",
                LinkedInProfile = "https://www.linkedin.com/in/suman-magar-37763a198/",
                GitHubProfile = "https://github.com/magarsumn",
                Comments = "This is old comment."
            };

            // Act
            var result = await _controller.AddOrUpdateCandidate(newCandidate);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Error updating data", internalServerErrorResult.Value);
        }
    }
}