﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace EventWay.Infrastructure.CosmosDb
{
    internal static class PartitionKeyGenerator
    {
        private static MD5 _md5;
        static PartitionKeyGenerator()
        {
            _md5 = MD5.Create();
        }

        internal static string Generate(Guid id, int noOfPartitions)
        {
            try
            {
                return TryToGenerate(id, noOfPartitions);
            }
            catch (ObjectDisposedException)
            {
                _md5 = MD5.Create();
                return TryToGenerate(id, noOfPartitions);
            }
        }

        internal static string TryToGenerate(Guid id, int noOfPartitions)
        {
            var hashedValue = _md5.ComputeHash(Encoding.UTF8.GetBytes(id.ToString()));
            var asInt = BitConverter.ToInt32(hashedValue, 0);
            asInt = asInt == int.MinValue ? asInt + 1 : asInt;

            return $"{Math.Abs(asInt) % noOfPartitions}";
        }
    }
}
