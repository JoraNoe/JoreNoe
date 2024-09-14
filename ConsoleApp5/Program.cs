using JoreNoe.Limit;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
namespace ConsoleApp5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            Program s = new Program();
            s.test();

        }

        [RequireMethodAttribute]
        public void test()
        {
            Console.WriteLine("测试看看行不");
        }

        public static IEnumerable<int> GenerateFibonacci()
        {
            int prev = 0, current = 1;
            yield return prev; // F(0)
            yield return current; // F(1)

            for (int i = 2; i < 10; i++)
            {
                int next = prev + current;
                yield return next;
                prev = current;
                current = next;
            }
        }


        static IEnumerable<string> Contin(IList<string> Strings)
        {
            foreach (var item in Strings)
            {
                if(item.Length>3) yield return item;
            }
        }

        static IEnumerable<int> ONumber()
        {
            for (int i = 0; i < 10; i++)
            {
                if(i%2==0)
                yield return i;
            }
        }

        static IEnumerable<int> Numbers()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return i;
            }
        }


        static IEnumerator<int> mm()
        {
            for (int i = 0; i < 100; i++)
            {
                yield return i;
            }
        }

        static IEnumerable<IList<int>> ints()
        {
            var cc = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                cc.Add(i);
                if (cc.Count == 5)
                {
                    yield return cc;
                    cc.Clear();
                }
            }
        }
    }

    public class TreeNode
    {
        public int value { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        public IEnumerable<TreeNode> traverse()
        {
            yield return this;
            foreach (var item in Children)
            {
                foreach (var child in item.traverse())
                {
                    yield return child;
                }
            }
        }
    }

}
