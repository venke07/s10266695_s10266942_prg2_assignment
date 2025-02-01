using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace s10266695_s10266942_prg2_assignment
{
    internal class Program
    {
        // --- External dictionaries to track extra data (without modifying your classes) ---
        // Maps flight number to its special request code (if any)
        static Dictionary<string, string> flightSpecialRequests = new Dictionary<string, string>();
        // Maps flight number to its assigned boarding gate
        static Dictionary<string, BoardingGate> flightGateAssignments = new Dictionary<string, BoardingGate>();

        static void Main(string[] args)
        {
            // File paths (adjust if needed)
            string flightsFilePath = "flights.csv";
            string airlinesFilePath = "airlines.csv";
            string boardingGatesFilePath = "boardinggates.csv";

            // Load the data from CSV files
            Dictionary<string, Flight> flights = LoadFlights(flightsFilePath);
            Dictionary<string, Airline> airlines = LoadAirlines(airlinesFilePath, flights);
            Dictionary<string, BoardingGate> boardingGates = LoadBoardingGates(boardingGatesFilePath);

            // Main menu loop
            while (true)
            {
                Console.WriteLine("\n=============================================");
                Console.WriteLine("Welcome to Changi Airport Terminal 5");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. List All Flights");
                Console.WriteLine("2. List Boarding Gates");
                Console.WriteLine("3. Assign a Boarding Gate to a Flight");
                Console.WriteLine("4. Create Flight");
                Console.WriteLine("5. Display Airline Flights");
                Console.WriteLine("6. Modify Flight Details");
                Console.WriteLine("7. Display Flight Schedule");
                Console.WriteLine("8. Process Unassigned Flights");
                Console.WriteLine("0. Exit");
                Console.Write("Please select your option: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListFlights(flights);
                        break;
                    case "2":
                        ListBoardingGates(boardingGates);
                        break;
                    case "3":
                        AssignBoardingGate(flights, boardingGates);
                        break;
                    case "4":
                        CreateNewFlight(flights, flightsFilePath);
                        break;
                    case "5":
                        DisplayAirlineFlights(airlines);
                        break;
                    case "6":
                        ModifyFlightDetails(flights, flightsFilePath);
                        break;
                    case "7":
                        DisplayScheduledFlights(flights);
                        break;
                    case "8":
                        ProcessUnassignedFlights(flights, boardingGates);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // --- CSV Loading Methods ---
        // Expected CSV for flights:
        // FlightNumber,Origin,Destination,ExpectedTime,Status[,SpecialRequestCode]
        // (If a special request code is provided, it will be stored in flightSpecialRequests.)
        static Dictionary<string, Flight> LoadFlights(string filePath)
        {
            var flights = new Dictionary<string, Flight>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                // Assume first line is a header
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 5 && DateTime.TryParse(fields[3].Trim(), out DateTime expectedTime))
                    {
                        string flightNumber = fields[0].Trim();
                        Flight flight = new Flight(
                            flightNumber,
                            fields[1].Trim(),
                            fields[2].Trim(),
                            expectedTime,
                            fields[4].Trim()
                        );
                        flights[flightNumber] = flight;
                        // If a sixth field is provided, store it as the special request code.
                        if (fields.Length >= 6)
                        {
                            string specialReq = fields[5].Trim();
                            if (!string.IsNullOrEmpty(specialReq))
                            {
                                flightSpecialRequests[flightNumber] = specialReq;
                            }
                        }
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

        // Expected CSV for airlines:
        // Name,Code
        static Dictionary<string, Airline> LoadAirlines(string filePath, Dictionary<string, Flight> flights)
        {
            var airlines = new Dictionary<string, Airline>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                // Assume header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 2)
                    {
                        // Create airline using the provided constructor.
                        Airline airline = new Airline(fields[0].Trim(), fields[1].Trim(), flights);
                        airlines[airline.Name] = airline;
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

        // Expected CSV for boarding gates:
        // GateName,SupportsCFFT,SupportsDDJB,SupportsLWTT
        static Dictionary<string, BoardingGate> LoadBoardingGates(string filePath)
        {
            var gates = new Dictionary<string, BoardingGate>();
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                // Assume header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(',');
                    if (fields.Length >= 4 &&
                        bool.TryParse(fields[1].Trim(), out bool supportsCFFT) &&
                        bool.TryParse(fields[2].Trim(), out bool supportsDDJB) &&
                        bool.TryParse(fields[3].Trim(), out bool supportsLWTT))
                    {
                        // When loading from CSV, no flight is yet assigned so pass null.
                        BoardingGate gate = new BoardingGate(fields[0].Trim(), supportsCFFT, supportsDDJB, supportsLWTT, null);
                        gates[gate.GateName] = gate;
                    }
                }
                Console.WriteLine("Boarding gates loaded successfully.");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
            return gates;
        }

        // --- Menu Option Methods ---

        // List all flights; if a flight has an assigned gate (tracked externally), show it.
        static void ListFlights(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\n--- List of Flights ---");
            foreach (var flight in flights.Values)
            {
                string assignedGate = flightGateAssignments.ContainsKey(flight.FlightNumber)
                    ? flightGateAssignments[flight.FlightNumber].GateName
                    : "None";
                string specialReq = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                    ? flightSpecialRequests[flight.FlightNumber]
                    : "None";
                Console.WriteLine($"Flight: {flight.FlightNumber}, Origin: {flight.Origin}, Destination: {flight.Destination}, Expected: {flight.ExpectedTime:yyyy-MM-dd HH:mm}, Status: {flight.Status}, Special Request: {specialReq}, Assigned Gate: {assignedGate}");
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

        // List boarding gates; each gate’s ToString() shows if it has a flight assigned.
        static void ListBoardingGates(Dictionary<string, BoardingGate> gates)
        {
            Console.WriteLine("\n--- List of Boarding Gates ---");
            foreach (var gate in gates.Values)
            {
                // Use the BoardingGate.ToString() method.
                Console.WriteLine(gate);
                Console.WriteLine("---------------------");
            }
        }

        // Assign a boarding gate to a flight.
        // We update the external flightGateAssignments dictionary and also set the gate’s Flight property.
        static void AssignBoardingGate(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> gates)
        {
            Console.Write("Enter Flight Number: ");
            string? flightNumber = Console.ReadLine();
            if (flightNumber != null && flights.TryGetValue(flightNumber, out Flight flight))
            {
                Console.WriteLine($"Selected Flight: {flight.FlightNumber}, {flight.Origin} to {flight.Destination}");
                Console.Write("Enter Boarding Gate: ");
                string? gateName = Console.ReadLine();
                if (gateName != null && gates.TryGetValue(gateName, out BoardingGate gate))
                {
                    Console.Write("Would you like to update flight status? (Y/N): ");
                    if (Console.ReadLine()?.ToUpper() == "Y")
                    {
                        Console.Write("Enter new status: ");
                        string? newStatus = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newStatus))
                        {
                            flight.Status = newStatus;
                        }
                    }
                    // Update external assignment mapping
                    flightGateAssignments[flight.FlightNumber] = gate;
                    // Also update the gate's Flight property so its ToString() shows the assignment.
                    gate.Flight = flight;
                    Console.WriteLine("Boarding gate assigned successfully.");
                }
                else
                {
                    Console.WriteLine("Gate not found.");
                }
            }
            else
            {
                Console.WriteLine("Flight not found.");
            }
        }

        // Create a new flight and add it to the flights dictionary.
        // Any special request code is stored in the external flightSpecialRequests dictionary.
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
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime expectedTime))
            {
                Console.WriteLine("Invalid date/time format.");
                return;
            }
            Console.Write("Enter Special Request Code (if any): ");
            string specialRequest = Console.ReadLine() ?? "";
            // Create the flight using the existing constructor (which does not accept a special request).
            Flight flight = new Flight(flightNumber, origin ?? "", destination ?? "", expectedTime, "On Time");
            flights[flightNumber] = flight;
            // If a special request was entered, store it externally.
            if (!string.IsNullOrEmpty(specialRequest))
            {
                flightSpecialRequests[flightNumber] = specialRequest;
            }
            // Append to CSV file (the CSV file format is assumed to have 6 columns)
            string line = $"\n{flight.FlightNumber},{flight.Origin},{flight.Destination},{flight.ExpectedTime:yyyy-MM-dd HH:mm},{flight.Status},{specialRequest}";
            File.AppendAllText(filePath, line);
            Console.WriteLine("Flight added successfully.");
        }

        // Display flights ordered by expected time.
        static void DisplayScheduledFlights(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\n--- Scheduled Flights (by expected time) ---");
            foreach (var flight in flights.Values.OrderBy(f => f.ExpectedTime))
            {
                string assignedGate = flightGateAssignments.ContainsKey(flight.FlightNumber)
                    ? flightGateAssignments[flight.FlightNumber].GateName
                    : "None";
                string specialReq = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                    ? flightSpecialRequests[flight.FlightNumber]
                    : "None";
                Console.WriteLine($"Flight: {flight.FlightNumber}, Expected: {flight.ExpectedTime:yyyy-MM-dd HH:mm}, Assigned Gate: {assignedGate}, Special Request: {specialReq}");
            }
        }

        // Display flights for a given airline.
        static void DisplayAirlineFlights(Dictionary<string, Airline> airlines)
        {
            Console.Write("Enter Airline Name: ");
            string? airlineName = Console.ReadLine();
            if (airlineName != null && airlines.TryGetValue(airlineName, out Airline airline))
            {
                Console.WriteLine($"\n--- Flights for {airline.Name} ---");
                foreach (var flight in airline.Flights.Values)
                {
                    string assignedGate = flightGateAssignments.ContainsKey(flight.FlightNumber)
                        ? flightGateAssignments[flight.FlightNumber].GateName
                        : "None";
                    string specialReq = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                        ? flightSpecialRequests[flight.FlightNumber]
                        : "None";
                    Console.WriteLine($"Flight: {flight.FlightNumber}, {flight.Origin} to {flight.Destination}, Assigned Gate: {assignedGate}, Special Request: {specialReq}");
                }
            }
            else
            {
                Console.WriteLine("Airline not found.");
            }
        }

        static void ModifyFlightDetails(Dictionary<string, Flight> flights, string filePath)
        {
            Console.Write("Enter Flight Number to modify: ");
            string? flightNumber = Console.ReadLine();
            if (flightNumber != null && flights.TryGetValue(flightNumber, out Flight flight))
            {
                Console.Write("Enter new Origin (or press Enter to keep current): ");
                string? origin = Console.ReadLine();
                if (!string.IsNullOrEmpty(origin))
                {
                    flight.Origin = origin;
                }
                Console.Write("Enter new Destination (or press Enter to keep current): ");
                string? destination = Console.ReadLine();
                if (!string.IsNullOrEmpty(destination))
                {
                    flight.Destination = destination;
                }
                Console.Write("Enter new Expected Time (yyyy-MM-dd HH:mm) (or press Enter to keep current): ");
                string? timeInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(timeInput) && DateTime.TryParse(timeInput, out DateTime newTime))
                {
                    flight.ExpectedTime = newTime;
                }
                Console.Write("Enter new Status (or press Enter to keep current): ");
                string? status = Console.ReadLine();
                if (!string.IsNullOrEmpty(status))
                {
                    flight.Status = status;
                }
                Console.Write("Enter new Special Request Code (or press Enter to keep current): ");
                string? specialRequest = Console.ReadLine();
                if (!string.IsNullOrEmpty(specialRequest))
                {
                    flightSpecialRequests[flight.FlightNumber] = specialRequest;
                }
                Console.WriteLine("Flight details updated.");
            }
            else
            {
                Console.WriteLine("Flight not found.");
            }
        }

        // Process unassigned flights and attempt to assign a boarding gate.
        static void ProcessUnassignedFlights(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> gates)
        {
            Console.WriteLine("\n=== Processing Unassigned Flights ===\n");
            int processedFlights = 0;
            int successfulAssignments = 0;
            // Process each flight that does not have an entry in flightGateAssignments.
            foreach (var flight in flights.Values)
            {
                if (!flightGateAssignments.ContainsKey(flight.FlightNumber))
                {
                    processedFlights++;
                    // Get the special request (if any) from the external dictionary.
                    string specialRequest = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                        ? flightSpecialRequests[flight.FlightNumber]
                        : "";
                    BoardingGate? assignedGate = null;
                    // If there is a special request, try to find a matching gate.
                    if (!string.IsNullOrEmpty(specialRequest))
                    {
                        assignedGate = FindMatchingGate(gates, specialRequest);
                    }
                    // If no matching gate is found, find any available gate.
                    if (assignedGate == null)
                    {
                        assignedGate = FindAnyAvailableGate(gates);
                    }
                    if (assignedGate != null)
                    {
                        // Record the assignment externally and update the gate’s Flight property.
                        flightGateAssignments[flight.FlightNumber] = assignedGate;
                        assignedGate.Flight = flight;
                        successfulAssignments++;
                        DisplayFlightAssignment(flight, assignedGate);
                    }
                    else
                    {
                        Console.WriteLine($"No available gate for flight {flight.FlightNumber}");
                    }
                }
            }
            double percentage = processedFlights > 0 ? (successfulAssignments * 100.0 / processedFlights) : 0;
            Console.WriteLine($"\nProcessed {processedFlights} flights. Successful assignments: {successfulAssignments} ({percentage:F2}%).");
        }

        // Searches for a gate that supports the given special request code.
        static BoardingGate? FindMatchingGate(Dictionary<string, BoardingGate> gates, string specialRequestCode)
        {
            return gates.Values.FirstOrDefault(gate =>
                gate.Flight == null &&
                (
                    (specialRequestCode.Equals("CFFT", StringComparison.OrdinalIgnoreCase) && gate.SupportsCFFT) ||
                    (specialRequestCode.Equals("DDJB", StringComparison.OrdinalIgnoreCase) && gate.SupportsDDJB) ||
                    (specialRequestCode.Equals("LWTT", StringComparison.OrdinalIgnoreCase) && gate.SupportsLWTT)
                )
            );
        }

        // Returns any available gate (one with no flight assigned).
        static BoardingGate? FindAnyAvailableGate(Dictionary<string, BoardingGate> gates)
        {
            return gates.Values.FirstOrDefault(gate => gate.Flight == null);
        }

        // Display details about a flight assignment.
        static void DisplayFlightAssignment(Flight flight, BoardingGate gate)
        {
            string specialReq = flightSpecialRequests.ContainsKey(flight.FlightNumber)
                    ? flightSpecialRequests[flight.FlightNumber]
                    : "None";
            Console.WriteLine("\n--- Flight Assignment Details ---");
            Console.WriteLine($"Flight Number: {flight.FlightNumber}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Time: {flight.ExpectedTime:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Special Request: {specialReq}");
            Console.WriteLine($"Assigned Gate: {gate.GateName}");
        }
    }
}