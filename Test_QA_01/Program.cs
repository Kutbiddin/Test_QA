using System;

namespace ProcessWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            /*foreach (string input in args)
            {
                Console.WriteLine(input);

            }*/
            Watcher watcher = new Watcher(args[0], args[1], args[2]);
        }
    }
}