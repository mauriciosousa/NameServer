using System;
using System.IO;
using System.Collections.Generic;

namespace NameServer
{
    class TableFile
    {
        public static List<Host> Load(string filename)
        {
            List<Host> ret = new List<Host>();

            if (File.Exists(filename))
            {
                foreach (string line in File.ReadAllLines(filename))
                {
                    try
                    {
                        if (line.Length > 0 && line[0] != '%')
                        {
                            Console.WriteLine(line);

                            string[] el = line.Split('/');
                            if (el.Length == 3)
                            {
                                Host h = new Host(el[0], el[1], el[2]);
                                ret.Add(h);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return ret;
        }
    }
}
