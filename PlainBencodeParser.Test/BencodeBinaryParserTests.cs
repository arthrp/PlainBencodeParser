using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PlainBencodeParser.Test;

[TestFixture]
public class BencodeBinaryParserTests
{
    [Test]
    public void IntegerParsing_Works()
    {
        var b = new BencodeBinaryParser();

        var data = new byte[] { 0x69, 0x39, 0x30, 0x30, 0x65 }; //i900e
        var (result, charsRead) = b.DecodeInt(data.ToList());
        
        Assert.AreEqual(900, result);
        Assert.AreEqual(5, charsRead);
    }

    [Test]
    public void NegativeIntegerParsing_Works()
    {
        var b = new BencodeBinaryParser();
        var data = new byte[] {0x69, 0x2D, 0x31, 0x30, 0x65, 0x0A}; //i-10e

        var (result, charsRead) = b.DecodeInt(data.ToList());
        Assert.AreEqual(-10, result);
        Assert.AreEqual(5, charsRead);
    }

    [Test]
    public void StringParsing_Works()
    {
        var b = new BencodeBinaryParser();
        
        var data = new byte[] { 0x34, 0x3A, 0x73, 0x70, 0x61, 0x6D, 0x0A }; //4:spam 

        var (result, charsRead) = b.DecodeByteString(data.ToList());
        var str = Encoding.ASCII.GetString(result);
        
        Assert.AreEqual("spam", str);
        Assert.AreEqual(6, charsRead);
    }

    [Test]
    public void SimpleDictionaryParsing_Works()
    {
        var b = new BencodeBinaryParser();

        var data = new byte[] { 0x64, 0x33, 0x3A, 0x66, 0x6F, 0x6F, 0x33, 0x3A, 0x62, 0x61, 0x72, 0x65 }; // d3:foo3:bare

        var (result, charsCount) = b.DecodeDictionary(data.ToList());
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("foo", result.Keys.ElementAt(0));

        var value = result["foo"] as byte[];
        var text = Encoding.ASCII.GetString(value);
        
        Assert.AreEqual("bar", text);
    }

    [Test]
    public void DictionaryIntegerValueParsing_Works()
    {
        var b = new BencodeBinaryParser();
        var data = new byte[] {0x64, 0x33, 0x3A, 0x66, 0x6F, 0x6F, 0x69, 0x39, 0x30, 0x30, 0x65, 0x65}; // d3:fooi900ee

        var (result, charsCount) = b.DecodeDictionary(data.ToList());
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("foo", result.Keys.ElementAt(0));

        var val = result["foo"] as int?;

        Assert.AreEqual(900, val.Value);
    }

    [Test]
    public void DecodeLongerDictionary_Works()
    {
        var b = new BencodeBinaryParser();
        var data = Encoding.ASCII.GetBytes("d3:foo3:bar4:spami900ee");

        var (result, charsRead) = b.DecodeDictionary(data.ToList());
        Assert.AreEqual(2, result.Count);
        
        Assert.IsTrue(result.Keys.Contains("foo"));
        Assert.IsTrue(result.Keys.Contains("spam"));
        var fooVal = result["foo"] as byte[];
        var text = Encoding.ASCII.GetString(fooVal);
        Assert.AreEqual("bar", text);

        var spamVal = result["spam"] as int?;
        Assert.AreEqual(900, spamVal.Value);
    }

    [Test]
    public void EmptyDictionaryParsing_Works()
    {
        var b = new BencodeBinaryParser();
        var data = new byte[] {0x64, 0x65}; // de

        var (result, charsRead) = b.DecodeDictionary(data.ToList());
        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void SimpleListParsing_Works()
    {
        var b = new BencodeBinaryParser();
        var data = Encoding.ASCII.GetBytes("l4:spam4:eggse");

        var (result, charsRead) = b.DecodeList(data.ToList());
        
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(14, charsRead);
        var first = Encoding.ASCII.GetString(result[0] as byte[]);
        var second = Encoding.ASCII.GetString(result[1] as byte[]);
        
        Assert.AreEqual("spam", first);
        Assert.AreEqual("eggs", second);
    }

    [Test]
    public void EmptyListParsing_Works()
    {
        var b = new BencodeBinaryParser();
        var data = Encoding.ASCII.GetBytes("le");
        
        var (result, charsRead) = b.DecodeList(data.ToList());
        Assert.AreEqual(0, result.Count);
        Assert.AreEqual(2, charsRead);
    }
}