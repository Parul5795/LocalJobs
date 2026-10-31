using LocalJobs.Application.Services;
using LocalJobs.Contracts.Requests;
using LocalJobs.Domain.Entities;
using LocalJobs.Domain.Enums;
using LocalJobs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LocalJobs.Tests;

public class PerListingContactDetailsTests
{
    [Fact]
    public async Task JobContactDetailsMapThroughCreateUpdateAndDetailButAreHiddenFromFeed()
    {
        await using var db = CreateDatabase();
        var (userId, categoryId, cityId, _) = await SeedAsync(db);
        var service = new JobService(db);
        var created = await service.CreateJobAsync(JobRequest("First Contact", "9876543210", true, false), userId);

        Assert.NotNull(created);
        Assert.Equal("First Contact", created.ContactPersonName);
        Assert.Equal("9876543210", created.ContactMobileNumber);
        Assert.True(created.AllowPhoneCall);
        Assert.False(created.AllowWhatsApp);

        var feed = await service.GetJobsAsync(1, 20);
        Assert.Null(Assert.Single(feed.Items).ContactMobileNumber);
        var ownerFeed = await service.GetMyJobsAsync(userId, 1, 20);
        Assert.Equal("9876543210", Assert.Single(ownerFeed.Items).ContactMobileNumber);

        var updated = await service.UpdateJobAsync(created.Id, userId,
            new UpdateJobRequest("Job", "Details", 1000, categoryId, cityId, true, "Full time", "Any", "Monthly", null, null,
                "Updated Contact", "+919876543210", false, true));

        Assert.NotNull(updated);
        Assert.Equal("Updated Contact", updated.ContactPersonName);
        Assert.Equal("+919876543210", updated.ContactMobileNumber);
        Assert.False(updated.AllowPhoneCall);
        Assert.True(updated.AllowWhatsApp);
        var detail = await service.GetJobByIdAsync(created.Id);
        Assert.Equal("+919876543210", detail!.ContactMobileNumber);
    }

    [Fact]
    public async Task AdContactDetailsMapThroughCreateUpdateAndDetailButAreHiddenFromFeed()
    {
        await using var db = CreateDatabase();
        var (userId, _, cityId, adCategoryId) = await SeedAsync(db);
        var service = new AdService(db);
        var created = await service.CreateAdAsync(AdRequest("First Contact", "9876543210", true, false), userId);

        Assert.Equal("First Contact", created.ContactPersonName);
        Assert.Equal("9876543210", created.ContactMobileNumber);
        Assert.True(created.AllowPhoneCall);
        Assert.False(created.AllowWhatsApp);

        var feed = await service.GetAdsAsync(1, 20);
        Assert.Null(Assert.Single(feed.Items).ContactMobileNumber);
        var ownerFeed = await service.GetMyAdsAsync(userId, 1, 20);
        Assert.Equal("9876543210", Assert.Single(ownerFeed.Items).ContactMobileNumber);

        var updated = await service.UpdateAdAsync(created.Id,
            new UpdateAdRequest("Ad", "Details", adCategoryId, cityId, 100, true, null, null,
                "Updated Contact", "+919876543210", false, true), userId);

        Assert.NotNull(updated);
        Assert.Equal("Updated Contact", updated.ContactPersonName);
        Assert.Equal("+919876543210", updated.ContactMobileNumber);
        Assert.False(updated.AllowPhoneCall);
        Assert.True(updated.AllowWhatsApp);
        var detail = await service.GetAdByIdAsync(created.Id);
        Assert.Equal("+919876543210", detail!.ContactMobileNumber);
    }

    [Theory]
    [InlineData("1234567890", true, false)]
    [InlineData("9876543210", false, false)]
    [InlineData("+911234567890", true, false)]
    public async Task JobRejectsInvalidContactDetails(string mobile, bool allowPhoneCall, bool allowWhatsApp)
    {
        await using var db = CreateDatabase();
        var (userId, _, _, _) = await SeedAsync(db);
        await Assert.ThrowsAsync<ArgumentException>(() => new JobService(db).CreateJobAsync(
            JobRequest("Contact", mobile, allowPhoneCall, allowWhatsApp), userId));
    }

    [Theory]
    [InlineData("1234567890", true, false)]
    [InlineData("9876543210", false, false)]
    [InlineData("+911234567890", true, false)]
    public async Task AdRejectsInvalidContactDetails(string mobile, bool allowPhoneCall, bool allowWhatsApp)
    {
        await using var db = CreateDatabase();
        var (userId, _, _, _) = await SeedAsync(db);
        await Assert.ThrowsAsync<ArgumentException>(() => new AdService(db).CreateAdAsync(
            AdRequest("Contact", mobile, allowPhoneCall, allowWhatsApp), userId));
    }

    private static ApplicationDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<(Guid UserId, int CategoryId, int CityId, int AdCategoryId)> SeedAsync(ApplicationDbContext db)
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Employer", Email = "employer@example.test", Phone = "9000000000", PasswordHash = "hash", AccountType = AccountType.Employer };
        var category = new Category { Name = "Driver" };
        var city = new City { Name = "Mumbai", State = "Maharashtra" };
        var adCategory = new AdCategory { Name = "Goods" };
        db.Users.Add(user); db.Categories.Add(category); db.Cities.Add(city); db.AdCategories.Add(adCategory);
        await db.SaveChangesAsync();
        return (user.Id, category.Id, city.Id, adCategory.Id);
    }

    private static CreateJobRequest JobRequest(string name, string mobile, bool phone, bool whatsapp) =>
        new("Job", "Details", 1000, 1, 1, "Full time", "Any", "Monthly", null, null, name, mobile, phone, whatsapp);

    private static CreateAdRequest AdRequest(string name, string mobile, bool phone, bool whatsapp) =>
        new("Ad", "Details", 1, 1, 100, null, null, name, mobile, phone, whatsapp);
}
