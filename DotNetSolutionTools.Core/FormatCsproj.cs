using System.Text;
using System.Xml;

namespace DotNetSolutionTools.Core;

public static class FormatCsproj
{
    public static void FormatCsprojFile(string csprojFilePath)
    {
        using TextReader rd = new StreamReader(csprojFilePath, Encoding.Default);

        XmlDocument doc = new XmlDocument();
        doc.Load(rd);

        if (rd != Console.In)
        {
            rd.Close();
        }

        using var wr = new StreamWriter(csprojFilePath, false, Encoding.Default);

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "\t",
            NewLineOnAttributes = false,
            OmitXmlDeclaration = true
        };

        using (var writer = XmlWriter.Create(wr, settings))
        {
            doc.WriteContentTo(writer);
            writer.Close();
        }
    }
}
