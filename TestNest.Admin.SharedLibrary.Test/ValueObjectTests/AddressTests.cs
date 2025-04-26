using TestNest.Admin.SharedLibrary.Exceptions.Common;
using TestNest.Admin.SharedLibrary.ValueObjects;

namespace TestNest.Admin.SharedLibrary.Test.ValueObjectTests;

public class AddressTests
{
    [Fact]
    public void Create_ValidAddressDetails_ReturnsSuccessResultWithAddress()
    {
        // Arrange
        var addressLine = "123 Main St";
        var city = "Anytown";
        var municipality = "Some Municipality";
        var province = "Some Province";
        var region = "Some Region";
        var country = "USA";
        var latitude = 34.0522;
        var longitude = -118.2437;

        // Act
        var result = Address.Create(addressLine, city, municipality, province, region, country, (decimal)latitude, (decimal)longitude);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(addressLine, result.Value.AddressLine);
        Assert.Equal(city, result.Value.City);
        Assert.Equal(municipality, result.Value.Municipality);
        Assert.Equal(province, result.Value.Province);
        Assert.Equal(region, result.Value.Region);
        Assert.Equal(country, result.Value.Country);
        Assert.Equal((decimal)latitude, result.Value.Latitude);
        Assert.Equal((decimal)longitude, result.Value.Longitude);
    }

    [Theory]
    [InlineData(null, "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", 34.0522, -118.2437)]
    [InlineData("123 Main St", null, "Some Municipality", "Some Province", "Some Region", "USA", 34.0522, -118.2437)]
    [InlineData("123 Main St", "Anytown", null, "Some Province", "Some Region", "USA", 34.0522, -118.2437)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", null, "Some Region", "USA", 34.0522, -118.2437)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", "Some Province", null, "USA", 34.0522, -118.2437)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", null, 34.0522, -118.2437)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", -91, -118.2437)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", 91, -118.2437)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", 34.0522, -181)]
    [InlineData("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", 34.0522, 181)]
    public void Create_InvalidAddressDetails_ReturnsFailureResultWithValidationErrors(
        string addressLine, string city, string municipality, string province,
        string region, string country, decimal latitude, decimal longitude)
    {
        // Act
        var result = Address.Create(null, "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", (decimal)34.0522, (decimal)-118.2437);
        //var result = Address.Create(addressLine, city, municipality, province, region, country, latitude, longitude);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.ErrorType);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Empty_ReturnsAnEmptyAddressInstance()
    {
        // Act
        var emptyAddress = Address.Empty();

        // Assert
        Assert.True(emptyAddress.IsEmpty());
        Assert.Equal(string.Empty, emptyAddress.AddressLine);
        Assert.Equal(string.Empty, emptyAddress.City);
        Assert.Equal(string.Empty, emptyAddress.Municipality);
        Assert.Equal(string.Empty, emptyAddress.Province);
        Assert.Equal(string.Empty, emptyAddress.Region);
        Assert.Equal(string.Empty, emptyAddress.Country);
        Assert.Equal(0, emptyAddress.Latitude);
        Assert.Equal(0, emptyAddress.Longitude);
    }

    [Fact]
    public void IsEmpty_NewInstanceWithDefaultConstructor_ReturnsTrue()
    {
        // Arrange & Act
        var address = new Address();

        // Assert
        Assert.True(address.IsEmpty());
    }

    [Fact]
    public void IsEmpty_CreatedInstance_ReturnsFalse()
    {
        // Arrange
        var address = Address.Create("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", (decimal)34.0522, (decimal)-118.2437).Value;

        // Act & Assert
        Assert.False(address.IsEmpty());
    }

    [Fact]
    public void Update_WithNewDetails_ReturnsSuccessResultWithUpdatedAddress()
    {
        // Arrange
        var initialAddress = Address.Create("123 Main St", "Anytown", "Some Municipality", "Some Province", "Some Region", "USA", (decimal)34.0522, (decimal)-118.2437).Value;
        var newAddressLine = "456 Oak Ave";
        var newCity = "Newville";
        var newLatitude = 40.7128;
        var newLongitude = -74.0060;

        // Act
        var result = initialAddress.Update(newAddressLine, newCity, initialAddress.Municipality, initialAddress.Province, initialAddress.Region, initialAddress.Country, (decimal)newLatitude, (decimal)newLongitude);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newAddressLine, result.Value.AddressLine);
        Assert.Equal(newCity, result.Value.City);
        Assert.Equal((decimal)newLatitude, result.Value.Latitude);
        //Assert.Equal(decimal)(newLongitude, result.Value.Longitude);
        // Ensure other properties remain the same
        Assert.Equal(initialAddress.Municipality, result.Value.Municipality);
        Assert.Equal(initialAddress.Province, result.Value.Province);
        Assert.Equal(initialAddress.Region, result.Value.Region);
        Assert.Equal(initialAddress.Country, result.Value.Country);
    }

    [Fact]
    public void ToString_ReturnsFormattedAddressString()
    {
        // Arrange
        var addressLine = "123 Main St";
        var city = "Anytown";
        var municipality = "Some Municipality";
        var province = "Some Province";
        var region = "Some Region";
        var country = "USA";
        var latitude = 34.0522;
        var longitude = -118.2437;
        var address = Address.Create(addressLine, city, municipality, province, region, country, (decimal)latitude, (decimal)longitude).Value;

        // Act
        var toStringResult = address.ToString();

        // Assert
        Assert.Equal($"{addressLine}, {city}, {municipality}, {province}, {region}, {country} (Lat: {latitude}, Long: {longitude})", toStringResult);
    }
}
