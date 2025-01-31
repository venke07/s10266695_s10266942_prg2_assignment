using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace s10266695_s10266942_prg2_assignment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string flightsFilePath = "flights.csv";
            string airlinesFilePath = "airlines.csv";
            string boardingGatesFilePath = "boardinggates.csv";

            Dictionary<string, Flight> flights = LoadFlights(flightsFilePath);
            Dictionary<string, Airline> airlines = LoadAirlines(airlinesFilePath, flights);
            Dictionary<string, BoardingGate> boardingGates = LoadBoardingGates(boardingGatesFilePath, null);

            while (true)
            {
                Console.WriteLine("\n--- Flight Management Menu ---");
                Console.WriteLine("1. List all flights");
                Console.WriteLine("2. Assign a boarding gate to a flight");
                Console.WriteLine("3. Create a new flight");
                Console.WriteLine("4. Display scheduled flights (chronological order)");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListFlights(flights);
                        break;
                    case "2":
                        AssignBoardingGate(flights, boardingGates);
                        break;
                    case "3":
                        CreateNewFlight(flights, flightsFilePath);
                        break;
                    case "4":
                        DisplayScheduledFlights(flights);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static Dictionary<string, Flight> LoadFlights(string filePath)
        {
            var flights = new Dictionary<string, Flight>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 5 && DateTime.TryParse(fields[3].Trim(), out DateTime expectedTime))
                    {
                        var flight = new Flight(
                            fields[0].Trim(),
                            fields[1].Trim(),
                            fields[2].Trim(),
                            expectedTime,
                            fields[4].Trim()
                        );
                        flights[fields[0].Trim()] = flight;
                    }
                }
                Console.WriteLine("Flights loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            return flights;
        }

        static Dictionary<string, Airline> LoadAirlines(string filePath, Dictionary<string, Flight> flights)
        {
            var airlines = new Dictionary<string, Airline>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 2)
                    {
                        airlines[fields[1].Trim()] = new Airline(fields[0].Trim(), fields[1].Trim(), flights);
                    }
                }
                Console.WriteLine("Airlines loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            return airlines;
        }

        static Dictionary<string, BoardingGate> LoadBoardingGates(string filePath, Flight? initialFlight)
        {
            var boardingGates = new Dictionary<string, BoardingGate>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 4 &&
                        bool.TryParse(fields[1].Trim(), out bool supportsCFFT) &&
                        bool.TryParse(fields[2].Trim(), out bool supportsDDJB) &&
                        bool.TryParse(fields[3].Trim(), out bool supportsLWTT))
                    {
                        boardingGates[fields[0].Trim()] = new BoardingGate(
                            fields[0].Trim(),
                            supportsCFFT,
                            supportsDDJB,
                            supportsLWTT,
                            initialFlight
                        );
                    }
                }
                Console.WriteLine("Boarding gates loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            return boardingGates;
        }

        static void ListFlights(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\n--- List of Flights ---");
            foreach (var flight in flights.Values)
            {
                Console.WriteLine(flight.ToString());
            }


            while (true)
            {
                Console.WriteLine("\nAvailable Airlines:");
                foreach (var airline in airlineDictionary.Values)
                {
                    Console.WriteLine($"- {airline.Code}: {airline.Name}");
                }

                Console.Write("\nEnter a 2-Letter Airline Code (or type 'exit' to quit): ");
                string airlineCode = Console.ReadLine()?.Trim().ToUpper();

                if (airlineCode == "EXIT")
                {
                    break;
                }

                if (!airlineDictionary.TryGetValue(airlineCode, out Airline selectedAirline))
                {
                    Console.WriteLine("Invalid airline code. Please try again.");
                    continue;
                }

                Console.WriteLine($"\nFlights for {selectedAirline.Name} ({selectedAirline.Code}):");
                foreach (var flight in flights.Values)
                {
                    if (flight.AirlineCode == airlineCode)
                    {
                        Console.WriteLine($"- {flight.FlightNumber}: {flight.Origin} to {flight.Destination}");
                    }
                }

                Console.Write("\nEnter a Flight Number to view details: ");
                string flightNumber = Console.ReadLine()?.Trim();

                if (!flights.TryGetValue(flightNumber, out Flight selectedFlight))
                {
                    Console.WriteLine("Invalid flight number. Please try again.");
                    continue;
                }

                Console.WriteLine("\nFlight Details:");
                Console.WriteLine($"- Flight Number: {selectedFlight.FlightNumber}");
                Console.WriteLine($"- Airline Name: {selectedAirline.Name}");
                Console.WriteLine($"- Origin: {selectedFlight.Origin}");
                Console.WriteLine($"- Destination: {selectedFlight.Destination}");
                Console.WriteLine($"- Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
                Console.WriteLine($"- Special Request Code: {selectedFlight.SpecialRequestCode ?? "None"}");
                Console.WriteLine($"- Boarding Gate: {selectedFlight.BoardingGate?.GateName ?? "Not Assigned"}");
            }
        }

        static void AssignBoardingGate(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
        {
            Console.Write("Enter Flight Number: ");
            string? flightNumber = Console.ReadLine();

            if (flightNumber != null && flights.TryGetValue(flightNumber, out Flight? flight))
            {
                Console.WriteLine($"Selected Flight: {flight}");
                Console.Write("Enter Boarding Gate: ");
                string? gateName = Console.ReadLine();

                if (gateName != null && boardingGates.TryGetValue(gateName, out BoardingGate? gate))
                {
                    // Update the gate's flight property using your class's method
                    // Note: Implementation depends on your BoardingGate class structure
                    Console.Write("Would you like to update flight status? (Y/N): ");
                    if (Console.ReadLine()?.ToUpper() == "Y")
                    {
                        Console.Write("Enter new status: ");
                        string? newStatus = Console.ReadLine();
                        if (newStatus != null)
                        {
                            flight.Status = newStatus;
                        }
                    }
                    Console.WriteLine("Boarding Gate assigned successfully.");
                }
                else
                {
                    Console.WriteLine("Gate is invalid or not found.");
                }
            }
            else
            {
                Console.WriteLine("Flight not found.");
            }
        }

        static void CreateNewFlight(Dictionary<string, Flight> flights, string filePath)
        {
            Console.Write("Enter Flight Number: ");
            string? flightNumber = Console.ReadLine();

            if (string.IsNullOrEmpty(flightNumber))
            {
                Console.WriteLine("Flight number cannot be empty.");
                return;
            }

            Console.Write("Enter Origin: ");
            string? origin = Console.ReadLine();

            Console.Write("Enter Destination: ");
            string? destination = Console.ReadLine();

            Console.Write("Enter Expected Departure/Arrival Time (yyyy-MM-dd HH:mm): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime expectedTime))
            {
                var flight = new Flight(flightNumber, origin ?? "", destination ?? "", expectedTime, "On Time");
                flights[flightNumber] = flight;
                File.AppendAllText(filePath, $"\n{flightNumber},{origin},{destination},{expectedTime:yyyy-MM-dd HH:mm},On Time");
                Console.WriteLine("Flight added successfully.");
            }
            else
            {
                Console.WriteLine("Invalid date/time format.");
            }
        }

        static void DisplayScheduledFlights(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\n--- Scheduled Flights ---");
            foreach (var flight in flights.Values.OrderBy(f => f.ExpectedTime))
            {
                Console.WriteLine(flight.ToString());
            }
        }
    }
}