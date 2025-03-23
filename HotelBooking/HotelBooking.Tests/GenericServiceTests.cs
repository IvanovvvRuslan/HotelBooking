using HotelBooking.Exceptions;
using HotelBooking.Repositories;
using HotelBooking.Services;
using NSubstitute;

namespace HotelBooking.Tests;

public class GenericServiceTests
{
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class TestDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    [Fact]
    public async Task GetAllAsync_GetCurrentAsync_ShouldReturnSuccess()
    {
        //Arrange
        var repository = Substitute.For<IGenericRepository<TestEntity>>();

        var testEntites = new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Test 1" },
            new TestEntity { Id = 2, Name = "Test 2" }
        };
        repository.GetAllAsync().Returns(testEntites);
        
        var service = new GenericService<TestEntity, TestDto>(repository);
        
        //Act
        var result = await service.GetAllAsync();
        
        //Assert
        var resultList = result.ToList();
        
        Assert.Equal(testEntites.Count, resultList.Count);

        for (int i = 0; i < testEntites.Count; i++)
        {
            Assert.Equal(testEntites[i].Id, resultList[i].Id);
            Assert.Equal(testEntites[i].Name, resultList[i].Name);
        }
        
        await repository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSuccess()
    {
        //Arrange
        var repository = Substitute.For<IGenericRepository<TestEntity>>();
        var testEntity = new TestEntity { Id = 1, Name = "Test 1" };
        
        repository.GetByIdAsync(testEntity.Id).Returns(testEntity);
        
        var service = new GenericService<TestEntity, TestDto>(repository);
        
        //Act
        var result = await service.GetByIdAsync(testEntity.Id);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(testEntity.Id, result.Id);
        Assert.Equal(testEntity.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowIfEntityNotFound()
    {
        //Arrange
        var repository = Substitute.For<IGenericRepository<TestEntity>>();
        
        repository.GetByIdAsync(1).Returns(Task.FromResult<TestEntity>(null));
        
        var service = new GenericService<TestEntity, TestDto>(repository);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.GetByIdAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteSuccessfully()
    {
        //Arrange
        var repository = Substitute.For<IGenericRepository<TestEntity>>();
        var entity = new TestEntity();
        repository.GetByIdAsync(entity.Id).Returns(entity);
        
        var service = new GenericService<TestEntity, TestDto>(repository);
        
        //Act
        await service.DeleteAsync(entity.Id);
        
        //Assert
        repository.Received(1).Delete(entity);
        await repository.Received(1).Delete(entity);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowIfEntityNotFound()
    {
        //Arrange
        var repository = Substitute.For<IGenericRepository<TestEntity>>();
        
        repository.GetByIdAsync(1).Returns(Task.FromResult<TestEntity>(null));
        
        var service = new GenericService<TestEntity, TestDto>(repository);
        
        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await service.DeleteAsync(1));
    }
}