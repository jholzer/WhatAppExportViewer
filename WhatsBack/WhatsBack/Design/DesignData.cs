using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using WhatsBack.Model;

namespace WhatsBack.Design
{
    class DesignData
    {
        public static IEnumerable<FileContent> GetFileContent()
        {
            return Enumerable.Range(0, 5)
                .Select(x => new FileContent($"File_{x}.txt", $"/abd/def{x}/File_{x}.txt", ".txt"))
                .ToArray();
        }
    }

    public class DesignHostScreen : IScreen
    {
        public RoutingState Router { get; }
    }
}
