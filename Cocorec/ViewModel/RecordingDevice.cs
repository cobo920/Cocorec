using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cocorec.ViewModel
{
    class RecordingDevice
    {
        public int Number { get; }
        public string Name { get; }
        public bool ShouldRecord { get; set; }

        public RecordingDevice(int number, string name, bool shouldRecord)
        {
            Number = number;
            Name = name;
            ShouldRecord = shouldRecord;
        }
    }
}
