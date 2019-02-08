using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ExternalLib
{
    class Program
    {
        public void fileRead()
        {
            string[,] roomsize;
            int numberOfBlocks = 0;
            Type[] blocks;
            int idx = 0;

            int state = -1;
            string[] text = System.IO.File.ReadAllLines("Map.txt");

            //string line
            //for(int idx = 0; idx < text.Length; idx++}
            //{
            //     line = text[idx];
            //}

            foreach (string line in text)
            {
                if (line == "[misc]")
                {
                    state = 0;
                }
                else if (line == "[block definitions]")
                {
                    state = 1;
                }
                else if (line == "[room definitions]")
                {
                    state = 2;
                }
                else if (line == "[end]")
                {
                    state = -1;
                }


                switch (state)
                {
                    case 0:
                        string[] lineSplit = line.Split('=');

                        if (lineSplit[0] == "numberofblocks")
                        {
                            numberOfBlocks = int.Parse(lineSplit[1]);
                        }
                        if (lineSplit[0] == "roomsize")
                        {
                            string roomsizeNumbers = lineSplit[1];
                            string[] individualNumbers = roomsizeNumbers.Split(',');
                            roomsize = new string[int.Parse(individualNumbers[0]), int.Parse(individualNumbers[1])];
                        }

                        break;

                    case 1:

                        blocks = new Type[numberOfBlocks];
                        if(line == "[block definitions]")
                        {
                            idx++;
                        }

                        for (; line != "[end]";)
                        {
                            string[] lineSplit2 = line.Split('=');

                            string namespaceFinder = lineSplit2[0];
                            string[] tempHolderNS = namespaceFinder.Split('.');

                            Console.ReadLine();

                            if (tempHolderNS[0] == "ExternalLib")
                            {
                                blocks[int.Parse(lineSplit2[1])] = Type.GetType(lineSplit2[0], false, true); 
                            }
                            else
                            {
                                string workingDir = System.IO.Directory.GetCurrentDirectory();
                                string otherDirectories = "/ExternalLib.dll";
                                string extClass = lineSplit2[0];

                                Assembly ass = Assembly.LoadFile(workingDir + otherDirectories);
                                Type typ = ass.GetType(extClass);

                                blocks[int.Parse(lineSplit2[1])] = typ;
                            }
                        }

                        for (int i = 0; i < blocks.Length; i++)
                        {
                            Console.WriteLine(blocks[i]);
                        }
                        Console.ReadLine();
                        break;

                    case 2:
                        break;
                }
                ++idx;
            }
        }

        static void Main(string[] args)
        {
            //This first section cache's the various strings we'll be using
            //this helps demonstrate that the classes & methods can all
            //be entirely data driven.
            string workingDir = System.IO.Directory.GetCurrentDirectory();
            string dllFileName = "/ExternalLib.dll";
            string extClass = "ExternalLib.MyExternalClass";
            string extMethod = "fileRead";

            //First load the external assembly
            Assembly ass = Assembly.LoadFile(workingDir + dllFileName);

            //Get th type of the desired class (including the namespace)
            Type typ = ass.GetType(extClass);

            //Get the supporting method info so we can execute a method.
            MethodInfo meth = typ.GetMethod(extMethod);

            //Create an instance of the desired object
            object obj = Activator.CreateInstance(typ);

            //And invoke the method
            meth.Invoke(obj, null);

            Program mm = new Program();
            mm.fileRead();

            Console.ReadLine();//Wait for random keyboard input & CR
        }
    }
}
