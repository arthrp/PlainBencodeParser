namespace PlainBencodeParser;

public class BencodeParser
{
    /// <summary>
    /// Decode bencoded string
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Resulting string, characters read</returns>
    public (string, int) DecodeString(string s)
    {
        var len = int.Parse(s.Split(':')[0]);
        var contentStartIndex = s.IndexOf(':') + 1;
        var result = s.Substring(contentStartIndex, len);

        return (result, contentStartIndex + len);
    }
    
    /// <summary>
    /// Decode bencoded integer
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Resulting int, characters read</returns>
    public (int, int) DecodeInteger(string s)
    {
        var endIndex = s.IndexOf('e');
        var result = s.Substring(1, endIndex-1);

        return (int.Parse(result), endIndex + 1);
    }
    
    /// <summary>
    /// Decode bencoded list
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Resulting list, characters read</returns>
    public (List<dynamic>, int) DecodeList(string s)
    {
        var list = new List<dynamic>();
        var index = 1;

        while (index < s.Length)
        {
            var currentChar = s[index];
            if (currentChar == 'e')
            {
                index++;
                break;
            }

            var (v, i) = Decode(s.Substring(index));
            list.Add(v);
            index += i;
        }

        return (list, index);
    }

    /// <summary>
    /// Decode bencoded dictionary
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Resulting dictionary, characters read</returns>
    public (Dictionary<dynamic, dynamic>, int) DecodeDictionary(string s)
    {
        var result = new Dictionary<dynamic, dynamic>();
        var index = 1;

        while (index < s.Length)
        {
            var currentChar = s[index];
            if (currentChar == 'e')
            {
                index++;
                break;
            }

            var (key, i1) = Decode(s.Substring(index));
            var (value, i2) = Decode(s.Substring(index + i1));
            result[key] = value;
            index += (i1 + i2);
        }

        return (result, index);
    }

    public (dynamic, int) Decode(string s)
    {
        var index = 0;
        while (index < s.Length)
        {
            var currentChar = s[index];
            switch (currentChar)
            {
                case 'i':
                {
                    var (intVal, i) = DecodeInteger(s.Substring(index));
                    index += i;
                    return (intVal, index);
                }
                case 'l':
                {
                    var (listVal, i) = DecodeList(s.Substring(index));
                    index += i;
                    return (listVal, index);
                }
                case 'd':
                {
                    var (dictVal, i) = DecodeDictionary(s.Substring(index));
                    index += i;
                    return (dictVal, index);
                }
                default:
                {
                    var (strVal, i) = DecodeString(s.Substring(index));
                    index += i;
                    return (strVal, index);
                }
            }
        }
        
        return (null, 0);
    }
}