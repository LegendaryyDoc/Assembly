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
            Type[,] roomsize;
            int numberOfBlocks = 0;
            Type[] blocks;
            int idx = 0;
            int cIntConverterFromChar = 0;

            string[] text = System.IO.File.ReadAllLines("Map.txt");

            //string line
            //for(int idx = 0; idx < text.Length; idx++}
            //{
            //     line = text[idx];
            //}
            string[] line;
            roomsize = null;
            blocks = null;
            for (idx = 0; idx < text.Length; idx++)
            {
                line = text;

                switch (line[idx])
                {
                    case "[misc]":

                        if(line[idx] == "[misc]")
                        {
                            idx++;
                        }
                        
                        string[] lineSplit = line[idx].Split('=');

                        if (lineSplit[0] == "numberofblocks")
                        {
                            numberOfBlocks = int.Parse(lineSplit[1]);
                        }

                        idx++;
                        lineSplit = line[idx].Split('=');

                        if (lineSplit[0] == "roomsize")
                        {
                            string roomsizeNumbers = lineSplit[1];
                            string[] individualNumbers = roomsizeNumbers.Split(',');
                            roomsize = new Type[int.Parse(individualNumbers[0]), int.Parse(individualNumbers[1])];
                        }

                        break;

                    case "[block definitions]":

                        blocks = new Type[numberOfBlocks];
                        if (line[idx] == "[block definitions]")
                        {
                            idx++;
                        }

                        for (; line[idx] != "[end]"; idx++)
                        {
                            string[] lineSplit2 = line[idx].Split('=');

                            string namespaceFinder = lineSplit2[0];
                            string[] tempHolderNS = namespaceFinder.Split('.');

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
                        break;

                    case "[room definitions]":
                        if (line[idx] == "[room definitions]")
                        {
                            idx++;
                        }

                        int roomSizeY = 0;
                        int roomSizeX = 0;
                        for (; line[idx] != "[end]"; idx++)
                        {    
                            foreach(char c in line[idx])
                            {
                                cIntConverterFromChar = Convert.ToInt32(new string(c, 1));

                                roomsize[roomSizeX, roomSizeY] = blocks[cIntConverterFromChar];
                                roomSizeX++;
                            }
                            roomSizeX = 0;
                            roomSizeY++; // adds to the row every time a line goes through
                        }

                        foreach (Type grid in roomsize)
                        {
                            Console.WriteLine(grid);
                        }

                        break;
                }
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
            string extMethod = "TestHelloWorld";

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
