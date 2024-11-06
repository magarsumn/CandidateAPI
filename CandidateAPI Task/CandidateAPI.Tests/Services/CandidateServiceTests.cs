using CandidateAPI.Data;
using CandidateAPI.Models;
using CandidateAPI.Services;
using Microsoft.EntityFrameworkCore;

public class CandidateServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CandidateService _candidateService;

    public CandidateServiceTests()
    {
        // Set up InMemory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("CandidateTestDb")  // Name of the in-memory database
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _candidateService = new CandidateService(_dbContext);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldThrowException_WhenCandidateWithEmailExists()
    {
        // Arrange
        var candidate = new Candidate
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

        // Add an existing candidate to the in-memory database
        await _dbContext.Candidates.AddAsync(new Candidate
        {
            Id = 2,
            FirstName = "Rajesh",
            LastName = "Hamal",
            Email = "suman@gmail.com",
            PhoneNumber = "9809155461",
            PreferredCallTime = "Evening",
            LinkedInProfile = "https://www.linkedin.com/in/suman-magar-37763a198/",
            GitHubProfile = "https://github.com/magarsumn",
            Comments = "This new comment."
        });
        await _dbContext.SaveChangesAsync();

        // Act 
        var exception = await Assert.ThrowsAsync<Exception>(() => _candidateService.AddOrUpdateCandidateAsync(candidate));

        // Assert
        Assert.Equal("A candidate with this email already exists.", exception.Message);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldUpdateCandidate_WhenCandidateExists()
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

        // Add the existing candidate to the in-memory database
        await _dbContext.Candidates.AddAsync(existingCandidate);
        await _dbContext.SaveChangesAsync();

        var updatedCandidate = new Candidate
        {
            Id = 1,
            FirstName = "Suman Updated",
            LastName = "Magar Updated",
            Email = "suman@gmail.com",
            PhoneNumber = "9812345678",
            PreferredCallTime = "Evening",
            LinkedInProfile = "https://www.linkedin.com/in/sumanupdated",
            GitHubProfile = "https://github.com/sumanupdated",
            Comments = "Updated candidate details."
        };

        // Act
        var result = await _candidateService.AddOrUpdateCandidateAsync(updatedCandidate);

        // Assert
        Assert.NotNull(result); // Ensure the result is not null
        Assert.Equal("Suman Updated", result.FirstName);
        Assert.Equal("Magar Updated", result.LastName);
        Assert.Equal("9812345678", result.PhoneNumber);
        Assert.Equal("Evening", result.PreferredCallTime);
        Assert.Equal("https://www.linkedin.com/in/sumanupdated", result.LinkedInProfile);
        Assert.Equal("https://github.com/sumanupdated", result.GitHubProfile);
        Assert.Equal("Updated candidate details.", result.Comments);

        // Verify that the updates were saved in the in-memory database
        var candidateInDb = await _dbContext.Candidates.FirstOrDefaultAsync(c => c.Id == existingCandidate.Id);
        Assert.NotNull(candidateInDb);
        Assert.Equal("Suman Updated", candidateInDb.FirstName);
        Assert.Equal("Magar Updated", candidateInDb.LastName);
        Assert.Equal("9812345678", candidateInDb.PhoneNumber);
        Assert.Equal("Evening", candidateInDb.PreferredCallTime);
    }

    [Fact]
    public async Task AddOrUpdateCandidateAsync_ShouldAddNewCandidate_WhenCandidateDoesNotExist()
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
            Comments = "Looking for new opportunities."
        };

        // Act
        var addedCandidate = await _candidateService.AddOrUpdateCandidateAsync(newCandidate);

        // Assert
        Assert.NotNull(newCandidate);
        Assert.Equal(newCandidate.Email, addedCandidate.Email);
        Assert.Equal("Suman", addedCandidate.FirstName);
        Assert.Equal("Magar", addedCandidate.LastName);


        // Verify that the candidate was actually added to the in-memory database
        var candidateInDb = await _dbContext.Candidates
            .FirstOrDefaultAsync(c => c.Email == newCandidate.Email);

        Assert.NotNull(candidateInDb); // Candidate should exist in the database
        Assert.Equal(newCandidate.Email, candidateInDb.Email);
    }
}