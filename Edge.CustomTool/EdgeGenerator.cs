using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Runtime.InteropServices;

namespace Edge.CustomTool
{

    [ComVisible(true)]
    [Guid("7092B522-44B0-4260-B92B-903ABC512A42")]
    public class EdgeGenerator : BaseCodeGeneratorWithSite
    {

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            throw new NotImplementedException();
        }

        public override string GetDefaultExtension()
        {
            throw new NotImplementedException();
        }

    }

}
