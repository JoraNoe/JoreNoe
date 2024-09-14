using System.Collections;
using System.Text.Json.Nodes;

namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] numbers = { "4", "5", "1", "2", "9" };
            var xx= Array.FindIndex(numbers, d => d == "1");
            
            //BubbleSort(numbers);
            sort(numbers);
            Console.WriteLine(string.Join(",", numbers));
        }

        static string[] sort(string[] arr)
        {
            return sort1(arr,arr.Length);
        }

        static string[] sort1(string[] arr,int length)
        {
            if (length == 1)
                return arr;

            for (int i = 0; i < length-1; i++) {
                if (string.Compare(arr[i], arr[i + 1]) > 0)
                {
                    string temp = arr[i];
                    arr[i] = arr[i + 1];
                    arr[i + 1] = temp;
                }
            }

            return sort1(arr,length-1);

        }

        static int[] BubbleSort(int[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        arr[j] = arr[j] ^ arr[j + 1];
                        arr[j + 1] = arr[j] ^ arr[j + 1];
                        arr[j] = arr[j] ^ (arr[j + 1]);
                    }
                }
            }
            return arr;
        }
    }
}
