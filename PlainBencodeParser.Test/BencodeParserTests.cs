using System.Collections.Generic;
using NUnit.Framework;

namespace PlainBencodeParser.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BasicStringParsing_Works()
    {
        var b = new BencodeParser();
        var (s, len) = b.DecodeString("4:spam");
        
        Assert.AreEqual("spam", s);
        Assert.AreEqual(6, len);
    }
    
    [Test]
    public void DecodeInteger_Works()
    {
        var b = new BencodeParser();

        var (num, len) = b.DecodeInteger("i90e");
        
        Assert.AreEqual(num, 90);
    }

    [Test]
    public void DecodeLargeInteger_Works()
    {
        var b = new BencodeParser();

        var (num, len) = b.DecodeInteger("i6000e");
        Assert.AreEqual(num, 6000);
    }

    [Test]
    public void DecodeList_Works()
    {
        var b = new BencodeParser();
        var (list, len) = b.DecodeList("l4:spam4:eggse");
        
        Assert.AreEqual(2, list.Count);
        Assert.AreEqual("spam", list[0]);
        Assert.AreEqual("eggs", list[1]);
    }

    [Test]
    public void DecodeEmptyList_Works()
    {
        var b = new BencodeParser();
        var (list, len) = b.DecodeList("le");
        
        Assert.AreEqual(2, len);
        Assert.AreEqual(0, list.Count);
    }

    [Test]
    public void DecodeDictionary_Works()
    {
        var b = new BencodeParser();

        var (dict, len) = b.DecodeDictionary("d4:spaml1:a1:bee");
        Assert.AreEqual(16, len);
        var value = dict["spam"];
        
        Assert.IsTrue(value is List<object>);
        var list = value as List<object>;
        
        Assert.AreEqual("a", list[0]);
        Assert.AreEqual("b", list[1]);
    }

    [Test]
    public void DecodeEmptyDictionary_Works()
    {
        var b = new BencodeParser();
        
        var (dict, len) = b.DecodeDictionary("de");
        Assert.AreEqual(2, len);
        Assert.AreEqual(0, dict.Count);
    }
    
}