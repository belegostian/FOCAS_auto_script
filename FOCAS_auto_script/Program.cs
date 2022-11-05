using System.Text;
using System.Threading.Tasks;

namespace FOCAS_API_Server
{
    class Program
    {
        static ushort _handle = 0;
        static short _rep = 0;

        static void Main(string[] args)
        {
            //建立連線
            _rep = Focas1.cnc_allclibhndl3("192.168.1.114", 8193, 1, out _handle);

            //
            if (_rep != Focas1.EW_OK)
            {
                Console.WriteLine($"Unable to connect to 192.168.1.114:8193\n\nReturn Code: {_rep}\n\nExiting...");
                Console.Read();
            }
            else
            {
                Console.WriteLine($"Our Focas handle is {_handle}\n");
                Console.WriteLine($"Our Focas controller mode is {GetMode()}");

                Console.WriteLine($"Tool no.4 offset is {WriteToolOffSet()}");
                Console.WriteLine($"Confirm tool no.4 offset is {ReadToolOffSet(4, 3)}");
                Console.Read();
            }

            Focas1.cnc_freelibhndl(_handle);
        }

        public static string GetMode()
        {
            // Check we have a valid handle
            if (_handle == 0)
                return "UNAVAILABLE";

            // Creat an instance of our stucture
            Focas1.ODBST mode = new Focas1.ODBST();

            // Ask Fanuc for the status information
            _rep = Focas1.cnc_statinfo(_handle, mode);

            // Check to make sure the call was successfull and convert the mode to a string and return it.
            if (_rep == Focas1.EW_OK)
                return GetModeString(mode.aut);
            return "UNAVAILABLE";
        }

        private static string GetModeString(short mode)
        {
            switch (mode)
            {
                case 0:
                    return "MDI";
                case 1:
                    return "MEM";
                case 2:
                    return "****";
                case 3:
                    return "EDIT";
                case 4:
                    return "HND";
                case 5:
                    return "JOG";
                case 6:
                    return "T-JOG";
                case 7:
                    return "T-HND";
                case 8:
                    return "INC";
                case 9:
                    return "REF";
                case 10:
                    return "RMT";
                default:
                    return "UNAVAILABLE";
            }
        }

        public static string ReadToolOffSet(short no, short arr)  //arr 一般刀常補正
        {
            // Creat an instance of our stucture
            Focas1.ODBTOFS tool_offset = new Focas1.ODBTOFS();

            // Ask Fanuc for the status information
            _rep = Focas1.cnc_rdtofs(_handle, no, arr, 8, tool_offset);

            // Check to make sure the call was successfull and convert the mode to a string and return it.
            if (_rep == Focas1.EW_OK)
                return tool_offset.data.ToString();
            return "UNAVAILABLE";
        }

        public static string WriteToolOffSet()
        {
            // Creat an instance of our stucture
            Focas1.ODBTOFS tool_offset = new Focas1.ODBTOFS();

            // Ask Fanuc for the status information
            _rep = Focas1.cnc_wrtofs(_handle, 4, 3, 8, -2000000);

            // Check to make sure the call was successfull and convert the mode to a string and return it.
            if (_rep == Focas1.EW_OK)
                return tool_offset.data.ToString();
            return "UNAVAILABLE";
        }
    }
}