using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Performance
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class StringPerformance
    {
        private const string _frase = "EEsseEUmTesteDePerformanceUsandoNetCore31"; 

        [Benchmark]
        public string ToSnakeCaseUsingRegex()
        {
            return Regex
                .Replace(
                    _frase, 
                    @"([a-z0-9])([A-Z])", 
                    "$1_$2", 
                    RegexOptions.Compiled)
                .ToLower();
        }

        [Benchmark]
        public string ToSnakeCaseUsingLinq()
        {
            return string
                .Concat(_frase
                    .Select((c, i) =>
                        i > 0 && char.IsUpper(c) 
                            ? "_" + c 
                            : c.ToString()))
                        .ToLower();
        }

        [Benchmark]
        public string ToSnakeCaseUsingSpanOnBuffer()
        {
            var undescores = 0;

            for (var i = 0; i < _frase.Length; i++)
            {
                if (char.IsUpper(_frase[i]))
                {
                    undescores++;
                }
            }

            var length = (undescores + _frase.Length) - 1;
            Span<char> buffer = new char[length];
            var possitionOfBuffer = 0;
            var letterPosition = 0;

            while (possitionOfBuffer < buffer.Length)
            {
                if (letterPosition > 0 && char.IsUpper(_frase[letterPosition]))
                {
                    buffer[possitionOfBuffer] = '_';
                    buffer[possitionOfBuffer + 1] = _frase[letterPosition];
                    possitionOfBuffer += 2;
                    letterPosition++;
                    continue;
                }

                buffer[possitionOfBuffer] = _frase[letterPosition];

                possitionOfBuffer++;
                letterPosition++;
            }

            return buffer.ToString().ToLower();
        }

        [Benchmark]
        public string ToSnakeCaseUsingStringBuildAndSpan()
        {
            ReadOnlySpan<char> frase = _frase;

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < frase.Length; i++)
            {
                if (char.IsUpper(frase[i]) && frase[0] != frase[i])
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(frase[i]);
                }
                else
                {
                    stringBuilder.Append(frase[i]);
                }
            }

            return stringBuilder
                .ToString()
                .ToLower();
        }
    }
}
