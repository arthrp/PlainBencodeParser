using System.Text;

namespace PlainBencodeParser;

public class BencodeBinaryParser
{
    private const byte INT_START = 0x69; // i
    private const byte STR_DELIM = 0x3A; // :
    private const byte DICT_START = 0x64; // d
    private const byte LIST_START = 0x6C; // l
    private const byte END = 0x65; // e

    public (int, int) DecodeInt(List<byte> data)
    {
        var endIndex = data.IndexOf(END);
        var intLength = endIndex - 1;
        var sublist = data.Skip(1).Take(intLength).ToArray();

        var str = Encoding.ASCII.GetString(sublist);
        return (int.Parse(str), endIndex+1);
    }

    public (byte[], int) DecodeByteString(List<byte> data)
    {
        var delimIndex = data.IndexOf(STR_DELIM);
        var lenSublist = data.Take(delimIndex).ToArray();

        var strLen = Encoding.ASCII.GetString(lenSublist);
        var len = int.Parse(strLen);
        var contentStartIndex = delimIndex + 1;

        var result = new List<byte>(data.GetRange(contentStartIndex, len));
        var charsRead = contentStartIndex + len;
        return (result.ToArray(), charsRead);
    }

    public (Dictionary<string, dynamic>, int) DecodeDictionary(List<byte> data)
    {
        var result = new Dictionary<string, dynamic>();
        var index = 1;

        while (index < (data.Count))
        {
            if (data[index] == END)
            {
                index++;
                break;
            }

            var (key, charsReadKey) = DecodeByteString(data.Skip(index).ToList());
            var (value, charsReadValue) = DecodeNext(data.Skip(index + charsReadKey).ToList());
            index += (charsReadKey + charsReadValue);

            result[Encoding.ASCII.GetString(key)] = value;
        }

        return (result, index);
    }

    public (List<dynamic>, int) DecodeList(List<byte> data)
    {
        var result = new List<dynamic>();
        var index = 1;

        while (index < data.Count)
        {
            var currentChar = data[index];
            if (currentChar == END)
            {
                index++;
                break;
            }

            var (el, len) = DecodeNext(data.Skip(index).ToList());
            result.Add(el);
            index += len;
        }

        return (result, index);
    }

    public (dynamic, int) DecodeNext(List<byte> data)
    {
        var index = 0;
        while (index < data.Count)
        {
            var nextChar = data[index];

            switch (nextChar)
            {
                case INT_START:
                {
                    var (intVal, len) = DecodeInt(data.Skip(index).ToList());
                    index += len;
                    return (intVal, index);
                }
                case DICT_START:
                {
                    var(dictVal, len) = DecodeDictionary(data.Skip(index).ToList());
                    index += len;
                    return (dictVal, index);
                }
                default:
                {
                    var (strVal, len) = DecodeByteString(data.Skip(index).ToList());
                    index += len;
                    return (strVal, index);
                }
            }
        }

        throw new ArgumentException("Nothing to decode");
    }
}