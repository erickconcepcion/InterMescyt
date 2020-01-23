using InterMescyt.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        int[] MapHeader { get; set; }
        int[] MapLine { get; set; }
        int[] MapSummary { get; }
        int MaxHeader { get; set; }
        int MaxDetail { get; set; }
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
        HeaderBank FileLineToHeaderBank(string line);
        TransLineBank FileLineToTransLineBank(string line);
        string HeaderBankToFileLine(HeaderBank header);
        string TransLineBankToFileLine(TransLineBank line);

    }
    public class FormatService : IFormatService
    {
        public char HeaderId => 'E';

        public char DetailId => 'D';

        public char SummaryId => 'S';

        public int[] MapHeader { get; set; } = new int[] { 0, 1, 12, 27, 57 };

        public int[] MapLine { get; set; } = new int[] { 0, 1, 12, 62, 73, 104, 108, 110 };

        public int[] MapSummary => new int[] { 0, 1 };

        public int MaxHeader { get; set; } = 67;

        public int MaxDetail { get; set; } = 120;

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
            Rnc = GetHeaderField(1, line).Trim(),
            Name = GetHeaderField(2, line).Trim(),
            Location = GetHeaderField(3, line).Trim(),
            TransDate = DateTime.ParseExact(GetHeaderField(4, line).Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
        };

        public TransLine FileLineToTransLine(string line)
        => new TransLine
        {
            Cedula = GetDetailField(1,line).Trim(),
            Name = GetDetailField(2, line).Trim(),
            EnrollNumber = GetDetailField(3, line).Trim(),
            Career = GetDetailField(4, line).Trim(),
            AcademicIndex = decimal.Parse(GetDetailField(5, line).Trim()),
            Period = int.Parse(GetDetailField(6, line).Trim()),
            Title = GetDetailField(7, line).Trim(),
        };
        public HeaderBank FileLineToHeaderBank(string line)
        => new HeaderBank
        {
            Rnc = GetHeaderField(1, line).Trim(),
            TransDate = DateTime.ParseExact(GetHeaderField(2, line).Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
        };

        public TransLineBank FileLineToTransLineBank(string line)
        => new TransLineBank
        {
            Cedula = GetDetailField(1, line).Trim(),
            BankAccount = GetDetailField(2, line).Trim(),
            NetSalary = decimal.Parse(GetDetailField(3, line).Trim()),
            TransDate = DateTime.ParseExact(GetDetailField(4, line).Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
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
                    return MaxSummary - MapSummary[order];
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
                header.TransDate.ToString("dd/MM/yyyy")
            };
            List<string> toEnsamble = new List<string>();
            for (int i = 0; i <= data.Length - 1; i++)
            {
                toEnsamble.Add(SetHeaderField(i, data[i]));
            }
            var ret = Ensamble(toEnsamble.ToArray());
            ValidateLineStructure(ret);
            return ret;
        }
        public string HeaderBankToFileLine(HeaderBank header)
        {
            var data = new string[] {
                HeaderId.ToString(),
                header.Rnc,
                header.TransDate.ToString("dd/MM/yyyy")
            };
            List<string> toEnsamble = new List<string>();
            for (int i = 0; i <= data.Length - 1; i++)
            {
                toEnsamble.Add(SetHeaderField(i, data[i]));
            }
            var ret = Ensamble(toEnsamble.ToArray());
            ValidateLineStructure(ret);
            return ret;
        }

        public string SetDetailField(int order, string value)
        {
            value = value ?? string.Empty;
            int max = GetFieldMaxLength(order, DetailId.ToString());
            return value.PadRight( max ).Substring(0,max);
        }

        public string SetHeaderField(int order, string value)
        {
            value = value ?? string.Empty;
            int max = GetFieldMaxLength(order, HeaderId.ToString());
            return value.PadRight(max).Substring(0, max);
        }

        public string SetSummaryField(int order, string value)
        {
            value = value ?? string.Empty;
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
            List<string> toEnsamble = new List<string>();
            for (int i = 0; i <= data.Length - 1; i++)
            {
                toEnsamble.Add(SetDetailField(i, data[i]));
            }
            var ret = Ensamble(toEnsamble.ToArray());
            ValidateLineStructure(ret);
            return ret;
            
        }

        public string TransLineBankToFileLine(TransLineBank line)
        {
            var data = new string[] {
                DetailId.ToString(),
                line.Cedula,
                line.BankAccount,
                line.NetSalary.ToString(),
                line.TransDate.ToString("dd/MM/yyyy")
            };
            List<string> toEnsamble = new List<string>();
            for (int i = 0; i <= data.Length - 1; i++)
            {
                toEnsamble.Add(SetDetailField(i, data[i]));
            }
            var ret = Ensamble(toEnsamble.ToArray());
            ValidateLineStructure(ret);
            return ret;

        }

        public void ValidateLineStructure(string line)
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                throw new Exception("La linea esta vacia");
            }
            if (line.FirstOrDefault() != HeaderId && line.FirstOrDefault() != DetailId && line.FirstOrDefault() != SummaryId)
            {
                throw new Exception("La linea no esta debidamente identificada");
            }
            if (line.FirstOrDefault() == HeaderId && line.Length != MaxHeader)
            {
                throw new Exception("Encabezado invalido, la longitud es incorrecta");
            }
            if (line.FirstOrDefault() == DetailId && line.Length != MaxDetail)
            {
                throw new Exception("Detalle invalido, la longitud es incorrecta");
            }
            if (line.FirstOrDefault() == SummaryId && line.Length != MaxSummary)
            {
                throw new Exception("Sumario invalido, la longitud es incorrecta");
            }
        }
    }
}
