using System.ComponentModel.DataAnnotations;
using TestNest.Admin.SharedLibrary.Common.BaseEntity;
using TestNest.Admin.SharedLibrary.StronglyTypeIds.Common;

namespace TestNest.Admin.SharedLibrary.Test.Common.BaseEntity;
// Test implementation of StronglyTypedId for testing
public record TestId : StronglyTypedId<TestId>
{
    public TestId() : base() { }
    public TestId(Guid value) : base(value) { }
}

// Test implementation of BaseEntity for testing
public class TestEntity : BaseEntity<TestId>
{
    public TestEntity() : base()
    {
    }

    public TestEntity(TestId id) : base(id)
    {
    }
}

public class BaseEntityTests
{
    [Fact]
    public void Constructor_WithId_SetsIdProperty()
    {
        // Arrange
        var testId = TestId.New();

        // Act
        var entity = new TestEntity(testId);

        // Assert
        Assert.Equal(testId, entity.Id);
    }

    [Fact]
    public void Constructor_WithNullId_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestEntity(null!));
    }

    [Fact]
    public void DefaultConstructor_SetsIdToNull()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.Null(entity.Id);
    }

    [Fact]
    public void Id_Property_HasRequiredAttribute()
    {
        // Arrange
        var property = typeof(BaseEntity<TestId>).GetProperty(nameof(BaseEntity<TestId>.Id));

        // Act
        var attribute = property?.GetCustomAttributes(typeof(RequiredAttribute), true);

        // Assert
        Assert.NotNull(attribute);
        Assert.NotEmpty(attribute);
    }

    [Fact]
    public void Equals_WithSameIdAndType_ReturnsTrue()
    {
        // Arrange
        var id = TestId.New();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        Assert.True(entity1.Equals(entity2));
        Assert.True(entity1 == entity2);
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void Equals_WithDifferentIds_ReturnsFalse()
    {
        // Arrange
        var entity1 = new TestEntity(TestId.New());
        var entity2 = new TestEntity(TestId.New());

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
        Assert.False(entity1 == entity2);
        Assert.True(entity1 != entity2);
    }

    [Fact]
    public void Equals_WithDifferentTypes_ReturnsFalse()
    {
        // Arrange
        var id = TestId.New();
        var entity1 = new TestEntity(id);

        // Different derived type
        var entity2 = new OtherTestEntity(id);

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
        Assert.False(entity1 == entity2);
        Assert.True(entity1 != entity2);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var entity = new TestEntity(TestId.New());

        // Act & Assert
        Assert.False(entity.Equals(null));
        Assert.False(entity == null);
        Assert.False(null == entity);
        Assert.True(entity != null);
        Assert.True(null != entity);
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        // Arrange
        var entity = new TestEntity(TestId.New());

        // Act & Assert
        Assert.True(entity.Equals(entity));
        Assert.True(entity == entity);
        Assert.False(entity != entity);
    }

    [Fact]
    public void GetHashCode_ReturnsConsistentValue()
    {
        // Arrange
        var id = TestId.New();
        var entity = new TestEntity(id);

        // Act
        var hash1 = entity.GetHashCode();
        var hash2 = entity.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_ForEqualObjects_ReturnsSameValue()
    {
        // Arrange
        var id = TestId.New();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var hash1 = entity1.GetHashCode();
        var hash2 = entity2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_ForDifferentObjects_ReturnsDifferentValues()
    {
        // Arrange
        var entity1 = new TestEntity(TestId.New());
        var entity2 = new TestEntity(TestId.New());

        // Act
        var hash1 = entity1.GetHashCode();
        var hash2 = entity2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    // Helper class for testing type comparison
    private class OtherTestEntity : BaseEntity<TestId>
    {
        public OtherTestEntity(TestId id) : base(id)
        {
        }
    }
}
