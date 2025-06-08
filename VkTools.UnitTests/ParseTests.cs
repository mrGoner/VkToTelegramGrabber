using System.IO;
using NUnit.Framework;
using VkApi.Serializers;

namespace VkTools.UnitTests;

public class ParseTests
{
    [Test]
    public void Success()
    {
        //Arrange
        var parser = new NewsFeedItemsDeserializer();
        var jsonData = File.ReadAllText("TestData/ParsingData.json");
        
        //Act
        var (items, _) = parser.Deserialize(jsonData);

        //Asserts
        Assert.That(items, Is.Not.Null);
        Assert.That(items, Is.Not.Empty);
    }
}