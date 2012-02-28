//Copyright (c) 2012 William Dollins - bill@geomusings.com

//Permission is hereby granted, free of charge, to any person obtaining a copy 
//of this software and associated documentation files (the "Software"), to deal 
//in the Software without restriction, including without limitation the rights 
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
//of the Software, and to permit persons to whom the Software is furnished to do 
//so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all 
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoIQ.Net;


namespace geoiqdelete
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "/dataset" :
                        handleDataset(args);
                        break;
                    case "/map" :
                        handleMaps(args);
                        break;
                    default :
                        Usage();
                        break;
                }
            }
            else
            {
                Usage();
            }
        }

        private static void Usage()
        {
            Console.WriteLine("This utility is used to delete existing data sets or maps in a GeoIQ library...");
            Console.WriteLine("Usage (data sets): geoiqdelete /dataset <Library URL> <User name> <Password> <Data set ID in GeoIQ>");
            Console.WriteLine("Example (data sets): geoiqdelete /dataset http://mylibrary.geoiq.com jdoe mypassword 1234");
            Console.WriteLine("Usage (single map): geoiqdelete /map <Library URL> <User name> <Password> <Map ID in GeoIQ>");
            Console.WriteLine("Example (single map): geoiqdelete /map http://mylibrary.geoiq.com jdoe mypassword 1234");
            Console.WriteLine("Usage (map range): geoiqdelete /map <Library URL> <User name> <Password> <Starting Map ID in GeoIQ> <Ending Map ID in GeoIQ>");
            Console.WriteLine("Example (map range): geoiqdelete /map http://mylibrary.geoiq.com jdoe mypassword 1234 1244");
        }

        private static void handleMaps(string[] args)
        {
            string library = "";
            string username = "";
            string password = "";
            int startmapid = 0;
            int endmapid = 0;
            if (args.Length == 5)
            {
                if (int.TryParse(args[4], out startmapid))
                {
                    library = args[1];
                    username = args[2];
                    password = args[3];
                    try
                    {
                        deleteSingleMap(library, username, password, startmapid);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error occured: " + ex.Message);
                        Usage();
                    }
                }
                else
                {
                    Usage();
                }
            }
            else if (args.Length == 6)
            {
                if ((int.TryParse(args[4], out startmapid)) && (int.TryParse(args[5], out endmapid)))
                {
                    library = args[1];
                    username = args[2];
                    password = args[3];
                    int start = Math.Min(startmapid, endmapid);
                    int end = Math.Max(startmapid, endmapid);
                    for (int i = start; i < end + 1; i++)
                    {
                        try
                        {
                            deleteSingleMap(library, username, password, i);
                        }
                        catch { }
                    }
                }
            }
            else
            {
                Usage();
            }
        }

        private static void deleteSingleMap(string library, string username, string password, int mapid)
        {
            try
            {
                var maps = new GeoIQ.Net.MapsApi(library, username, password);
                var result = maps.deleteMap(mapid);
                if (result.Result.ToLower() == "204")
                {
                    Console.WriteLine("Map " + mapid.ToString() + " deleted from " + library);
                }
                else
                {
                    Console.WriteLine("Delete unsucessful. Response code " + result.Result + " received from GeoIQ library.");
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void handleDataset(string[] args)
        {
            string library = "";
            string username = "";
            string password = "";
            int datasetid = 0;
            if (args.Length == 5)
            {
                if (int.TryParse(args[4], out datasetid))
                {
                    library = args[1];
                    username = args[2];
                    password = args[3];
                    var finder = new GeoIQ.Net.Finder(library, username, password);
                    try
                    {
                        var result = finder.Delete(datasetid);
                        if (result.Result.ToLower() == "204")
                        {
                            Console.WriteLine("Dataset " + args[4] + " deleted from " + library);
                        }
                        else
                        {
                            Console.WriteLine("Delete unsucessful. Response code " + result.Result + " received from GeoIQ library.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error occured: " + ex.Message);
                        Usage();
                    }
                }
                else
                {
                    Usage();
                }
            }
            else
            {
                Usage();
            }
        }
    }
}
