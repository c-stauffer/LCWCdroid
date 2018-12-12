using System;

namespace RssTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var x = LCWCdroid.RssFetcher.FetchFeed();
            foreach (var lollazy in x)
            {
                Console.WriteLine(lollazy.Title);
            }
            Console.ReadLine();
        }
    }
}
