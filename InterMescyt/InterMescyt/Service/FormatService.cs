using InterMescyt.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterMescyt.Service
{
    public interface IFormatService
    {
        Header FileLineToHeader(string line);
        TransLine FileLineToTransLine(string line);
        string HeaderToFileLine(Header header);
        string TransLineToFileLine(TransLine line);
        char HeaderId { get; }
        char DetailId { get; }
        char SummaryId { get; }
        int[] MapHeader { get; }
        int[] MapLine { get; }
        int[] MapSummary { get; }
        int MaxHeader { get; }
        int MaxDetail { get; }
        int MaxSummary { get; }
        string GetDetailField(int order, string lin);
        string GetHeaderField(int order, string lin);
        string GetSummaryField(int order, string lin);
        string SetDetailField(int order, string value);
        string SetHeaderField(int order, string value);
        string SetSummaryField(int order, string value);
        void ValidateLineStructure(string line);
        int GetFieldMaxLength(int order, string line);
        string Ensamble(string[] input);

    }
    public class FormatService : IFormatService
    {
        public char HeaderId => 'E';

        public char DetailId => 'D';

        public char SummaryId => 'S';

        public int[] MapHeader => new int[] { 0, 1, 12, 28, 59 };

        public int[] MapLine => new int[] { 0, 1, 12, 62, 73, 104, 108, 110 };

        public int[] MapSummary => new int[] { 0, 1 };

        public int MaxHeader => 68;

        public int MaxDetail => 119;

        public int MaxSummary => 7;

        public string Ensamble(string[] input)
        {
            string output = "";
            foreach (var item in input)
            {
                output = $"{output}{item}";
            }
            return output;
        }

        public Header FileLineToHeader(string line)
        => new Header
        {
            Rnc = GetHeaderField(1, line),
            Name = GetHeaderField(2, line),
            Location = GetHeaderField(3, line),
            TransDate = DateTime.Parse(GetHeaderField(4, line))
        };

        public TransLine FileLineToTransLine(string line)
        => new TransLine
        {
            Cedula = GetDetailField(1,line),
            Name = GetDetailField(2, line),
            EnrollNumber = GetDetailField(3, line),
            Career = GetDetailField(4, line),
            AcademicIndex = decimal.Parse(GetDetailField(5, line)),
            Period = int.Parse(GetDetailField(6, line)),
            Title = GetDetailField(7, line),
        };

        public string GetDetailField(int order, string line)
        => line.Substring(MapLine[order], GetFieldMaxLength(order, line));

        public int GetFieldMaxLength(int order, string line)
        {
            if (line.FirstOrDefault()==HeaderId)
            {
                if (order == (MapHeader.Length - 1))
                {
                    return MaxHeader - MapHeader[order];
                }
                return MapHeader[order + 1] - MapHeader[order];
            }
            else if (line.FirstOrDefault()==DetailId)
            {
                if (order == (MapLine.Length - 1))
                {
                    return MaxDetail - MapLine[order];
                }
                return MapLine[order + 1] - MapLine[order];
            }
            else if (line.FirstOrDefault() == SummaryId)
            {
                if (order == (MapSummary.Length - 1))
                {
                    return MaxHeader - MapSummary[order];
                }
                return MapSummary[order + 1] - MapSummary[order];
            }
            return 0;
        }

        public string GetHeaderField(int order, string line)
        => line.Substring(MapHeader[order], GetFieldMaxLength(order, line));

        public string GetSummaryField(int order, string line)
        => line.Substring(MapSummary[order], GetFieldMaxLength(order, line));

        public string HeaderToFileLine(Header header)
        {
            var data = new string[] {
                HeaderId.ToString(),
                header.Rnc,
                header.Name,
                header.Location,
                header.TransDate.ToShortDateString()
            };
            var ret = GatherData(data);
            ValidateLineStructure(ret);
            return ret;
        }       

        public string SetDetailField(int order, string value)
        {
            int max = GetFieldMaxLength(order, DetailId.ToString());
            return value.PadRight( max ).Substring(0,max);
        }

        public string SetHeaderField(int order, string value)
        {
            int max = GetFieldMaxLength(order, HeaderId.ToString());
            return value.PadRight(max).Substring(0, max);
        }

        public string SetSummaryField(int order, string value)
        {
            int max = GetFieldMaxLength(order, SummaryId.ToString());
            return value.PadRight(max).Substring(0, max);
        }

        public string TransLineToFileLine(TransLine line)
        {
            var data = new string[] {
                DetailId.ToString(),
                line.Cedula,
                line.Name,
                line.EnrollNumber,
                line.Career,
                line.AcademicIndex.ToString(),
                line.Period.ToString(),
                line.Title
            };
            var ret = GatherData(data);
            ValidateLineStructure(ret);
            return ret;
            
        }
        string GatherData(string[] data)
        {
            List<string> toEnsamble = new List<string>();
            for (int i = 0; i <= data.Length - 1; i++)
            {
                toEnsamble.Add(SetDetailField(i, data[i]));
            }
            return Ensamble(toEnsamble.ToArray());
        }

        public void ValidateLineStructure(string line)
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                throw new Exception("La linea esta vacia");
            }
            if (line.FirstOrDefault() != HeaderId || line.FirstOrDefault() != DetailId || line.FirstOrDefault() != SummaryId)
            {
                throw new Exception("La linea no esta debidamente identificada");
            }
            if (line.FirstOrDefault() != HeaderId && line.Length != MaxHeader)
            {
                throw new Exception("Encabezado invalido, la longitud es incorrecta");
            }
            if (line.FirstOrDefault() != DetailId && line.Length != MaxDetail)
            {
                throw new Exception("Detalle invalido, la longitud es incorrecta");
            }
            if (line.FirstOrDefault() != SummaryId && line.Length != MaxSummary)
            {
                throw new Exception("Sumario invalido, la longitud es incorrecta");
            }
        }
    }
}
