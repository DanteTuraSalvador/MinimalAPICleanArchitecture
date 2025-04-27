using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.Tests.StronglyTypeIds.Common;

// Mock implementation of StronglyTypedId for testing
public record TestStronglyTypedId : StronglyTypedId<TestStronglyTypedId>
{
    public TestStronglyTypedId(Guid value) : base(value) { }
    public static TestStronglyTypedId New() => new TestStronglyTypedId(Guid.NewGuid());
    public override string ToString() => Value.ToString(); // Override ToString()
}

public class StronglyTypedIdTests
{
    [Fact]
    public void Constructor_Default_SetsNewGuid()
    {
        // Act
        var id = new TestStronglyTypedId(Guid.NewGuid()); // Using the non-default constructor for explicit control in this test

        // Assert
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void Constructor_WithValue_SetsProvidedValue()
    {
        // Arrange
        var testGuid = Guid.NewGuid();

        // Act
        var id = new TestStronglyTypedId(testGuid);

        // Assert
        Assert.Equal(testGuid, id.Value);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_SetsEmptyGuid()
    {
        // Act
        var id = new TestStronglyTypedId(Guid.Empty);

        // Assert
        Assert.Equal(Guid.Empty, id.Value);
    }

    [Fact]
    public void ToString_ReturnsGuidString()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var id = new TestStronglyTypedId(testGuid);

        // Act
        string result = id.ToString();

        // Assert
        Assert.Equal(testGuid.ToString(), result);
    }

    [Fact]
    public void ImplicitOperatorGuid_ReturnsInnerGuidValue()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var id = new TestStronglyTypedId(testGuid);

        // Act
        Guid implicitGuid = id;

        // Assert
        Assert.Equal(testGuid, implicitGuid);
    }

    [Fact]
    public void Empty_ReturnsNewInstanceWithEmptyGuid()
    {
        // Act
        var emptyId = TestStronglyTypedId.Empty();

        // Assert
        Assert.Equal(Guid.Empty, emptyId.Value);
    }

    [Fact]
    public void ImplicitOperatorString_ValidGuidString_ReturnsNewInstance()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        string guidString = testGuid.ToString();

        // Act
        TestStronglyTypedId? id = (TestStronglyTypedId?)guidString;

        // Assert
        Assert.NotNull(id);
        Assert.Equal(testGuid, id!.Value);
    }

    [Fact]
    public void ImplicitOperatorString_InvalidGuidString_ReturnsNull()
    {
        // Arrange
        string invalidGuidString = "not-a-guid";

        // Act
        TestStronglyTypedId? id = (TestStronglyTypedId?)invalidGuidString;

        // Assert
        Assert.Null(id);
    }

    [Fact]
    public void New_ReturnsNewInstanceWithNewGuid()
    {
        // Act
        var newId = TestStronglyTypedId.New();

        // Assert
        Assert.NotEqual(Guid.Empty, newId.Value);
    }

    [Fact]
    public void TryParse_ValidGuidString_ReturnsTrueAndSetsResult()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        string guidString = testGuid.ToString();

        // Act
        bool success = TestStronglyTypedId.TryParse(guidString, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(testGuid, result!.Value);
    }

    [Fact]
    public void TryParse_InvalidGuidString_ReturnsFalseAndSetsNullResult()
    {
        // Arrange
        string invalidGuidString = "not-a-guid";

        // Act
        bool success = TestStronglyTypedId.TryParse(invalidGuidString, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_ValidGuid_ReturnsTrueAndSetsResult()
    {
        // Arrange
        var testGuid = Guid.NewGuid();

        // Act
        bool success = TestStronglyTypedId.TryParse(testGuid, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(testGuid, result!.Value);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var id1 = new TestStronglyTypedId(testGuid);
        var id2 = new TestStronglyTypedId(testGuid);

        // Act
        bool result = id1.Equals(id2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        // Arrange
        var id1 = new TestStronglyTypedId(Guid.NewGuid());
        var id2 = new TestStronglyTypedId(Guid.NewGuid());

        // Act
        bool result = id1.Equals(id2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_NullOther_ReturnsFalse()
    {
        // Arrange
        var id = new TestStronglyTypedId(Guid.NewGuid());

        // Act
        bool result = id.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHashCode()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var id1 = new TestStronglyTypedId(testGuid);
        var id2 = new TestStronglyTypedId(testGuid);

        // Act
        int hashCode1 = id1.GetHashCode();
        int hashCode2 = id2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHashCode()
    {
        // Arrange
        var id1 = new TestStronglyTypedId(Guid.NewGuid());
        var id2 = new TestStronglyTypedId(Guid.NewGuid());

        // Act
        int hashCode1 = id1.GetHashCode();
        int hashCode2 = id2.GetHashCode();

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }

    [Fact]
    public void CompareTo_SameValue_ReturnsZero()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var id1 = new TestStronglyTypedId(testGuid);
        var id2 = new TestStronglyTypedId(testGuid);

        // Act
        int result = id1.CompareTo(id2);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareTo_SmallerValue_ReturnsNegativeOne()
    {
        // Arrange
        var guid1 = new Guid("00000000-0000-0000-0000-000000000001");
        var guid2 = new Guid("00000000-0000-0000-0000-000000000002");
        var id1 = new TestStronglyTypedId(guid1);
        var id2 = new TestStronglyTypedId(guid2);

        // Act
        int result = id1.CompareTo(id2);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void CompareTo_LargerValue_ReturnsPositiveOne()
    {
        // Arrange
        var guid1 = new Guid("00000000-0000-0000-0000-000000000002");
        var guid2 = new Guid("00000000-0000-0000-0000-000000000001");
        var id1 = new TestStronglyTypedId(guid1);
        var id2 = new TestStronglyTypedId(guid2);

        // Act
        int result = id1.CompareTo(id2);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void CompareTo_OtherIsNull_ReturnsPositiveOne()
    {
        // Arrange
        var id = new TestStronglyTypedId(Guid.NewGuid());

        // Act
        int result = id.CompareTo(null);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void CompareToObject_SameValue_ReturnsZero()
    {
        // Arrange
        var testGuid = Guid.NewGuid();
        var id1 = new TestStronglyTypedId(testGuid);
        object id2 = new TestStronglyTypedId(testGuid);

        // Act
        int result = id1.CompareTo((TestStronglyTypedId?)id2);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CompareToObject_DifferentValue_ReturnsNonZero()
    {
        // Arrange
        var id1 = new TestStronglyTypedId(Guid.NewGuid());
        object id2 = new TestStronglyTypedId(Guid.NewGuid());

        // Act
        int result = id1.CompareTo((TestStronglyTypedId?)id2);

        // Assert
        Assert.NotEqual(0, result);
    }

    [Fact]
    public void CompareToObject_NullObject_ReturnsPositiveOne()
    {
        // Arrange
        var id = new TestStronglyTypedId(Guid.NewGuid());
        object? other = null;

        // Act
        int result = id.CompareTo((TestStronglyTypedId?)other);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void CompareToObject_DifferentObjectType_ThrowsInvalidCastException()
    {
        // Arrange
        var id = new TestStronglyTypedId(Guid.NewGuid());
        object other = new object();

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => id.CompareTo((TestStronglyTypedId?)other));
    }
}
