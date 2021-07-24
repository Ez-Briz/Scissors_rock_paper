using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Task3
{
    // class RandomNumberGeneratorRealization : RandomNumberGenerator
    // {
    //     public void GetBytes(byte[] bytes){

    //     }
    // }
    class Program
    {
        const int byteBufferSize = 16;
        static void Main(string[] playables)
        {
            if (playables.Length < 3 || playables.Length % 2 != 1)
            {
                Console.WriteLine("Invalid parameters count!");
                return;
            }
            else if (playables.Distinct().ToArray().Length != playables.Length)
            {
                Console.WriteLine("Invalid parameters! Do not use similar variants!");
                return;
            }
            byte[] key = new byte[byteBufferSize];
            RandomNumberGenerator keyGenerator;
            (key, keyGenerator) = GenerateKey();
            int ans = GenerateAns(keyGenerator, playables.Length);
            byte[] HMAC = GenerateHMAC(key, ans);

            Console.WriteLine("HMAC = " + BitConverter.ToString(HMAC).Replace("-", ""));
            int chose = -1;
            do
            {
                PrintAbles(playables);
                Console.Write("Enter your move: ");
                chose = Convert.ToInt32(Console.ReadLine()) - 1;
            }
            while(chose >= playables.Length || chose < 0);
            Console.WriteLine("Your move = " + playables[Convert.ToInt32(chose)]);
            Console.WriteLine("PC move = " + playables[ans]);
            if (ans == chose)
            {
                Console.WriteLine("Tie!");
            }
            else if ((ans - chose <= Convert.ToInt32(playables.Length/2) && ans - chose > 0) ||
                (ans - chose < 0 - Convert.ToInt32(playables.Length/2)))
            {
                Console.WriteLine("You lose!");
                Console.WriteLine("Key = " + BitConverter.ToString(key).Replace("-", ""));
            }
            else
            {
                Console.WriteLine("You win!");
            }
            Console.Read();
        }

        static (byte[], RandomNumberGenerator) GenerateKey()
        {
            var keyGenerator = RandomNumberGenerator.Create();
            byte[] key = new byte[byteBufferSize];
            keyGenerator.GetNonZeroBytes(key);
            var result = BitConverter.ToString(key);
            return (key, keyGenerator);
        }

        static int GenerateAns(RandomNumberGenerator keyGenerator, int length)
        {
            byte[] compChose = new byte[sizeof(int)];
            keyGenerator.GetBytes(compChose, 0, compChose.Length);
            return (Math.Abs(BitConverter.ToInt32(compChose, 0)) % length);
        }
        static byte[] GenerateHMAC(byte[] key, int ans)
        {
            using (var hmac = new HMACSHA256(key))
            {
                byte[] hmacBytes = new byte[key.Length];
                hmacBytes = hmac.ComputeHash(Encoding.Default.GetBytes(ans.ToString()));
                return hmacBytes;
            }
        }
        static void PrintAbles(string[] playables)
        {
            int counter = 1;
            foreach (string playable in playables)
                Console.WriteLine((counter++).ToString() + ". " + playable);
        }
    }
}
