using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sassafras
{
    public class SassFile
    {


        public string InputFilePath;
        public string OutputFilePath;

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(InputFilePath) && !string.IsNullOrEmpty(OutputFilePath);
            }
        }


    }
}
