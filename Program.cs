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
                Console.WriteLine("5. Display full flight details by airline");
                Console.WriteLine("6. Modify flight details");
                Console.WriteLine("7. Display total fee per airline");
                Console.WriteLine("8. Exit");
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
                        DisplayFlightDetailsByAirline(airlines, flights);
                        break;
                    case "6":
                        ModifyFlightDetails(airlines, flights);
                        break;
                    case "7":
                        CalculateAndDisplayAirlineFees(airlines, flights, boardingGates);
                        break;
                    case "8":
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
                    // Store the gate assignment in the dictionary
                    flightGateAssignments[flightNumber] = gateName;  // Add this line

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

        private static Dictionary<string, string> flightSpecialRequests = new Dictionary<string, string>();
        private static Dictionary<string, string> flightGateAssignments = new Dictionary<string, string>();

        static void DisplayFlightDetailsByAirline(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights)
        {
            // Display available airlines
            Console.WriteLine("\n--- Available Airlines ---");
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"Airline Code: {airline.Code} - Name: {airline.Name}");
            }

            // Prompt for airline code
            Console.Write("\nEnter 2-Letter Airline Code: ");
            string? airlineCode = Console.ReadLine()?.ToUpper();

            if (string.IsNullOrEmpty(airlineCode) || !airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine("Invalid airline code.");
                return;
            }

            Airline selectedAirline = airlines[airlineCode];

            // Display flights for the selected airline
            Console.WriteLine($"\n--- Flights for {selectedAirline.Name} ({selectedAirline.Code}) ---");
            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airlineCode));

            if (!airlineFlights.Any())
            {
                Console.WriteLine("No flights found for this airline.");
                return;
            }

            foreach (var flight in airlineFlights)
            {
                Console.WriteLine($"Flight Number: {flight.FlightNumber}");
                Console.WriteLine($"Origin: {flight.Origin}");
                Console.WriteLine($"Destination: {flight.Destination}");
                Console.WriteLine("-------------------");
            }

            // Prompt for flight selection
            Console.Write("\nEnter Flight Number for detailed information: ");
            string? flightNumber = Console.ReadLine()?.ToUpper();

            if (string.IsNullOrEmpty(flightNumber) || !flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Invalid flight number.");
                return;
            }

            Flight selectedFlight = flights[flightNumber];

            // Display detailed flight information
            Console.WriteLine("\n=== Detailed Flight Information ===");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.Name}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:yyyy-MM-dd HH:mm}");

            // Display special request code if it exists
            if (flightSpecialRequests.ContainsKey(selectedFlight.FlightNumber))
            {
                Console.WriteLine($"Special Request Code: {flightSpecialRequests[selectedFlight.FlightNumber]}");
            }

            // Display boarding gate if assigned
            if (flightGateAssignments.ContainsKey(selectedFlight.FlightNumber))
            {
                Console.WriteLine($"Boarding Gate: {flightGateAssignments[selectedFlight.FlightNumber]}");
            }
            else
            {
                Console.WriteLine("Boarding Gate: Not assigned");
            }
        }

        static void ModifyFlightDetails(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights)
        {
            // Display available airlines
            Console.WriteLine("\n--- Available Airlines ---");
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"Airline Code: {airline.Code} - Name: {airline.Name}");
            }

            // Prompt for airline code
            Console.Write("\nEnter 2-Letter Airline Code: ");
            string? airlineCode = Console.ReadLine()?.ToUpper();

            if (string.IsNullOrEmpty(airlineCode) || !airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine("Invalid airline code.");
                return;
            }

            Airline selectedAirline = airlines[airlineCode];

            // Display flights for the selected airline
            Console.WriteLine($"\n--- Flights for {selectedAirline.Name} ({selectedAirline.Code}) ---");
            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airlineCode)).ToList();

            if (!airlineFlights.Any())
            {
                Console.WriteLine("No flights found for this airline.");
                return;
            }

            foreach (var flight in airlineFlights)
            {
                Console.WriteLine($"Flight Number: {flight.FlightNumber}");
                Console.WriteLine($"Origin: {flight.Origin}");
                Console.WriteLine($"Destination: {flight.Destination}");
                Console.WriteLine("-------------------");
            }

            // Prompt for modification choice
            Console.WriteLine("\nChoose an action:");
            Console.WriteLine("[1] Modify existing flight");
            Console.WriteLine("[2] Delete existing flight");
            Console.Write("Enter your choice (1 or 2): ");

            string? actionChoice = Console.ReadLine();

            if (actionChoice == "1") // Modify flight
            {
                Console.Write("\nEnter Flight Number to modify: ");
                string? flightNumber = Console.ReadLine()?.ToUpper();

                if (string.IsNullOrEmpty(flightNumber) || !flights.ContainsKey(flightNumber))
                {
                    Console.WriteLine("Invalid flight number.");
                    return;
                }

                Flight selectedFlight = flights[flightNumber];

                Console.WriteLine("\nWhat would you like to modify?");
                Console.WriteLine("1. Basic Information (Origin, Destination, Expected Time)");
                Console.WriteLine("2. Status");
                Console.WriteLine("3. Special Request Code");
                Console.WriteLine("4. Boarding Gate");
                Console.Write("Enter your choice (1-4): ");

                string? modifyChoice = Console.ReadLine();
                switch (modifyChoice)
                {
                    case "1": // Basic Information
                        Console.Write("Enter new Origin (or press Enter to keep current): ");
                        string? newOrigin = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newOrigin))
                            selectedFlight.Origin = newOrigin;

                        Console.Write("Enter new Destination (or press Enter to keep current): ");
                        string? newDestination = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newDestination))
                            selectedFlight.Destination = newDestination;

                        Console.Write("Enter new Expected Time (yyyy-MM-dd HH:mm) (or press Enter to keep current): ");
                        string? newTimeStr = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newTimeStr) && DateTime.TryParse(newTimeStr, out DateTime newTime))
                            selectedFlight.ExpectedTime = newTime;
                        break;

                    case "2": // Status
                        Console.Write("Enter new Status: ");
                        string? newStatus = Console.ReadLine();
                        if (!string.IsNullOrEmpty(newStatus))
                            selectedFlight.Status = newStatus;
                        break;

                    case "3": // Special Request Code
                        Console.Write("Enter new Special Request Code (or press Enter to remove): ");
                        string? newCode = Console.ReadLine();
                        if (string.IsNullOrEmpty(newCode))
                            flightSpecialRequests.Remove(flightNumber);
                        else
                            flightSpecialRequests[flightNumber] = newCode;
                        break;

                    case "4": // Boarding Gate
                        Console.Write("Enter new Boarding Gate (or press Enter to remove): ");
                        string? newGate = Console.ReadLine();
                        if (string.IsNullOrEmpty(newGate))
                            flightGateAssignments.Remove(flightNumber);
                        else
                            flightGateAssignments[flightNumber] = newGate;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        return;
                }

                Console.WriteLine("Flight updated successfully!");
            }
            else if (actionChoice == "2") // Delete flight
            {
                Console.Write("\nEnter Flight Number to delete: ");
                string? flightNumber = Console.ReadLine()?.ToUpper();

                if (string.IsNullOrEmpty(flightNumber) || !flights.ContainsKey(flightNumber))
                {
                    Console.WriteLine("Invalid flight number.");
                    return;
                }

                Console.Write("Are you sure you want to delete this flight? [Y/N]: ");
                string? confirmation = Console.ReadLine()?.ToUpper();

                if (confirmation == "Y")
                {
                    flights.Remove(flightNumber);
                    // Also remove any special requests or gate assignments
                    flightSpecialRequests.Remove(flightNumber);
                    flightGateAssignments.Remove(flightNumber);
                    Console.WriteLine("Flight deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            // Display updated flight details
            Console.WriteLine("\n=== Updated Flight Details ===");
            foreach (var flight in flights.Values.Where(f => f.FlightNumber.StartsWith(airlineCode)))
            {
                Console.WriteLine($"\nFlight Number: {flight.FlightNumber}");
                Console.WriteLine($"Airline Name: {selectedAirline.Name}");
                Console.WriteLine($"Origin: {flight.Origin}");
                Console.WriteLine($"Destination: {flight.Destination}");
                Console.WriteLine($"Expected Time: {flight.ExpectedTime:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Status: {flight.Status}");

                if (flightSpecialRequests.ContainsKey(flight.FlightNumber))
                    Console.WriteLine($"Special Request Code: {flightSpecialRequests[flight.FlightNumber]}");

                if (flightGateAssignments.ContainsKey(flight.FlightNumber))
                    Console.WriteLine($"Boarding Gate: {flightGateAssignments[flight.FlightNumber]}");

                Console.WriteLine("-------------------");
            }

        }

        static void CalculateAndDisplayAirlineFees(Dictionary<string, Airline> airlines, Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
        {
            // Check for unassigned flights
            var unassignedFlights = flights.Values.Where(f => !flightGateAssignments.ContainsKey(f.FlightNumber)).ToList();
            if (unassignedFlights.Any())
            {
                Console.WriteLine("\nWARNING: The following flights have not been assigned boarding gates:");
                foreach (var flight in unassignedFlights)
                {
                    Console.WriteLine($"- Flight {flight.FlightNumber}");
                }
                Console.WriteLine("\nPlease assign boarding gates to all flights before calculating fees.");
                return;
            }

            double totalFeesAllAirlines = 0;
            double totalDiscountsAllAirlines = 0;

            foreach (var airline in airlines.Values)
            {
                var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airline.Code)).ToList();

                if (!airlineFlights.Any())
                    continue;

                double subtotalFees = 0;
                double subtotalDiscounts = 0;

                Console.WriteLine($"\n=== Fee Calculation for {airline.Name} ({airline.Code}) ===");

                // Calculate base fees
                foreach (var flight in airlineFlights)
                {
                    double flightFee = 0;

                    // Origin/Destination fees
                    if (flight.Origin.Equals("SIN", StringComparison.OrdinalIgnoreCase))
                        flightFee += 800; // Departing flight fee
                    if (flight.Destination.Equals("SIN", StringComparison.OrdinalIgnoreCase))
                        flightFee += 500; // Arriving flight fee

                    // Special request fees
                    if (flightSpecialRequests.TryGetValue(flight.FlightNumber, out string? specialCode))
                    {
                        switch (specialCode)
                        {
                            case "DDJB":
                                flightFee += 300;
                                break;
                            case "CFFT":
                                flightFee += 150;
                                break;
                            case "LWTT":
                                flightFee += 500;
                                break;
                        }
                    }

                    // Boarding gate base fee
                    flightFee += 300;

                    Console.WriteLine($"\nFlight {flight.FlightNumber}:");
                    Console.WriteLine($"Base Fees: ${flightFee:F2}");
                    subtotalFees += flightFee;
                }

                // Calculate discounts
                int flightCount = airlineFlights.Count;

                // Discount for every 5 flights
                int groups = flightCount / 5;
                subtotalDiscounts += groups * 350;

                // Early/Late flight discount
                var earlyLateFlights = airlineFlights.Count(f =>
                    f.ExpectedTime.Hour < 11 || f.ExpectedTime.Hour >= 23);
                subtotalDiscounts += earlyLateFlights * 110;

                // Origin-specific discounts (DXB/BKK/TYO)
                var specialOriginFlights = airlineFlights.Count(f =>
                    f.Origin.Equals("DXB", StringComparison.OrdinalIgnoreCase) ||
                    f.Origin.Equals("BKK", StringComparison.OrdinalIgnoreCase) ||
                    f.Origin.Equals("TYO", StringComparison.OrdinalIgnoreCase));
                subtotalDiscounts += specialOriginFlights * 25;

                // Flights without special requests
                var normalFlights = airlineFlights.Count(f => !flightSpecialRequests.ContainsKey(f.FlightNumber));
                subtotalDiscounts += normalFlights * 50;

                // Volume discount (more than 5 flights)
                if (flightCount > 5)
                {
                    double volumeDiscount = subtotalFees * 0.05; // Removed the 'm' suffix
                    subtotalDiscounts += volumeDiscount;
                }

                double finalFee = subtotalFees - subtotalDiscounts;

                Console.WriteLine($"\nSubtotal Fees: ${subtotalFees:F2}");
                Console.WriteLine($"Subtotal Discounts: ${subtotalDiscounts:F2}");
                Console.WriteLine($"Final Fee: ${finalFee:F2}");

                totalFeesAllAirlines += subtotalFees;
                totalDiscountsAllAirlines += subtotalDiscounts;
            }

            double finalTotalFees = totalFeesAllAirlines - totalDiscountsAllAirlines;
            double discountPercentage = (totalDiscountsAllAirlines / finalTotalFees) * 100;

            Console.WriteLine("\n=== Terminal 5 Summary ===");
            Console.WriteLine($"Total Fees (before discounts): ${totalFeesAllAirlines:F2}");
            Console.WriteLine($"Total Discounts: ${totalDiscountsAllAirlines:F2}");
            Console.WriteLine($"Final Total Fees: ${finalTotalFees:F2}");
            Console.WriteLine($"Discount Percentage: {discountPercentage:F2}%");
        }

    }
}
