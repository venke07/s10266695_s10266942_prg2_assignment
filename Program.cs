using System;
using System.Collections.Generic;
using System.IO;

namespace s10266695_s10266942_prg2_assignment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Load flights
            string flightsFilePath = "flights.csv";
            Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

            if (File.Exists(flightsFilePath))
            {
                string[] flightLines = File.ReadAllLines(flightsFilePath);

                
                for (int i = 1; i < flightLines.Length; i++)
                {
                    string line = flightLines[i];
                    string[] fields = line.Split(',');

                    if (fields.Length >= 5)
                    {
                        string flightNumber = fields[0].Trim();
                        string origin = fields[1].Trim();
                        string destination = fields[2].Trim();
                        DateTime expectedTime = DateTime.Parse(fields[3].Trim());
                        string status = fields[4].Trim();

                        Flight flight = new Flight(flightNumber, origin, destination, expectedTime, status);
                        flights[flightNumber] = flight;
                    }
                }

                Console.WriteLine("Flights loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {flightsFilePath}");
            }

            
            Console.WriteLine("\nLoaded Flights:");
            foreach (var flight in flights.Values)
            {
                Console.WriteLine(flight);
            }

            // Create a dictionary to store airlines and boarding gates
            Dictionary<string, Airline> airlineDictionary = new Dictionary<string, Airline>();
            Dictionary<string, BoardingGate> boardingGateDictionary = new Dictionary<string, BoardingGate>();



            // Load airlines
            string airlinesFilePath = "airlines.csv";
            if (File.Exists(airlinesFilePath))
            {
                string[] airlineLines = File.ReadAllLines(airlinesFilePath);

               
                for (int i = 1; i < airlineLines.Length; i++)
                {
                    string line = airlineLines[i];
                    string[] data = line.Split(',');

                    if (data.Length >= 2)
                    {
                        string name = data[0].Trim();
                        string code = data[1].Trim();

                        Airline airline = new Airline(name, code, new Dictionary<string, Flight>());
                        airlineDictionary[code] = airline;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid line format in airlines.csv: {line}");
                    }
                }
                Console.WriteLine("Airlines loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {airlinesFilePath}");
            }

            // Load boarding gates
            string boardingGatesFilePath = "boardinggates.csv";
            if (File.Exists(boardingGatesFilePath))
            {
                string[] boardingGateLines = File.ReadAllLines(boardingGatesFilePath);

                
                for (int i = 1; i < boardingGateLines.Length; i++)
                {
                    string line = boardingGateLines[i];
                    string[] data = line.Split(',');

                    if (data.Length >= 4)
                    {
                        string gateName = data[0].Trim();

                        if (!bool.TryParse(data[1].Trim(), out bool supportsDDJB) ||
                            !bool.TryParse(data[2].Trim(), out bool supportsCFFT) ||
                            !bool.TryParse(data[3].Trim(), out bool supportsLWTT))
                        {
                            Console.WriteLine($"Invalid boolean values in boardinggates.csv line: {line}");
                            continue;
                        }

                        BoardingGate boardingGate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT, null);
                        boardingGateDictionary[gateName] = boardingGate;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid line format in boardinggates.csv: {line}");
                    }
                }
                Console.WriteLine("Boarding gates loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {boardingGatesFilePath}");
            }

            
            Console.WriteLine("\nLoaded Airlines:");
            foreach (var airline in airlineDictionary.Values)
            {
                Console.WriteLine(airline);
            }

            Console.WriteLine("\nLoaded Boarding Gates:");
            foreach (var boardingGate in boardingGateDictionary.Values)
            {
                Console.WriteLine(boardingGate);
            }
        }
    }
}
